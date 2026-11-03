using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Forms;

internal sealed class UninstallDialog : Form
{
    public UninstallDialog(Entry entry)
    {
        Text=Ui.Text("卸载软件");FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;MinimizeBox=false;ShowInTaskbar=false;
        StartPosition=FormStartPosition.CenterParent;BackColor=Art.Card;ForeColor=Art.Text;Font=new Font(Ui.FontFamily,10);
        AutoScaleDimensions=new SizeF(96,96);AutoScaleMode=AutoScaleMode.Dpi;ClientSize=new Size(560,338);
        Controls.Add(new Label{Text=Ui.Text("卸载软件"),Font=new Font(Ui.FontFamily,17,FontStyle.Bold),ForeColor=Art.Text,Bounds=new Rectangle(24,22,512,38)});
        Controls.Add(new Label{Text=AppManagement.Confirmation(entry),ForeColor=Color.FromArgb(193,205,222),Bounds=new Rectangle(26,76,508,186)});
        Button cancel=new Button{Text=Ui.English?"Cancel":"取消",DialogResult=DialogResult.Cancel,Bounds=new Rectangle(314,280,100,36),FlatStyle=FlatStyle.Flat,BackColor=Color.FromArgb(43,51,64),ForeColor=Art.Text};
        Button remove=new Button{Text=Ui.Text("卸载软件"),DialogResult=DialogResult.OK,Bounds=new Rectangle(426,280,108,36),FlatStyle=FlatStyle.Flat,BackColor=Color.FromArgb(76,45,48),ForeColor=Color.FromArgb(255,201,204)};
        cancel.FlatAppearance.BorderSize=0;remove.FlatAppearance.BorderSize=0;
        Controls.Add(cancel);Controls.Add(remove);CancelButton=cancel;AcceptButton=cancel;
        Shown+=delegate{cancel.Select();};
    }
}

// Read registrations only. Never delete program folders or device drivers ourselves.
internal sealed class RemovalPlan
{
    public string Name, FileName, Arguments, RegistryPath, PackageFamily;
    public bool Elevate;
    public ProcessStartInfo Command()
    {
        return new ProcessStartInfo(FileName,Arguments??"") {UseShellExecute=true,Verb=Elevate?"runas":"",WorkingDirectory=Path.GetDirectoryName(FileName),WindowStyle=String.IsNullOrEmpty(PackageFamily)?ProcessWindowStyle.Normal:ProcessWindowStyle.Hidden};
    }
    public string Identity {get{return FileName+"|"+Arguments;}}
}

internal static class AppManagement
{
    internal sealed class Record
    {
        public string Name, Root, Icon, Command, Key, RegistryPath;
        public bool Machine, Msi;
    }
    static string Clean(string value){return Environment.ExpandEnvironmentVariables(value??"").Trim().Trim('"');}
    static string BaseName(string value){return Regex.Replace(value??"",@"\s+v?\d[\d.]*.*$","",RegexOptions.IgnoreCase).Trim();}
    internal static List<Record> ReadRecords()
    {
        List<Record> records=new List<Record>();
        foreach(RegistryHive hive in new[]{RegistryHive.LocalMachine,RegistryHive.CurrentUser})
        foreach(RegistryView view in new[]{RegistryView.Registry32,RegistryView.Registry64})
        try
        {
            using(RegistryKey root=RegistryKey.OpenBaseKey(hive,view))
            using(RegistryKey uninstall=root.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                if(uninstall==null)continue;
                foreach(string id in uninstall.GetSubKeyNames())
                try
                {
                    using(RegistryKey key=uninstall.OpenSubKey(id))
                    {
                        if(key==null||Convert.ToString(key.GetValue("SystemComponent"))=="1"||key.GetValue("ParentKeyName")!=null)continue;
                        string name=Convert.ToString(key.GetValue("DisplayName"));
                        if(String.IsNullOrWhiteSpace(name))continue;
                        string icon=Convert.ToString(key.GetValue("DisplayIcon"));
                        icon=Regex.Replace(icon,@",\s*-?\d+\s*$","");icon=Clean(icon);
                        string location=Clean(Convert.ToString(key.GetValue("InstallLocation")));
                        if(!Directory.Exists(location)&&Path.IsPathRooted(icon)&&File.Exists(icon))location=Path.GetDirectoryName(icon);
                        records.Add(new Record{Name=name,Root=location,Icon=icon,Command=Convert.ToString(key.GetValue("UninstallString")),Key=id,RegistryPath=hive+"/"+view+"/"+id,Machine=hive==RegistryHive.LocalMachine,Msi=Convert.ToString(key.GetValue("WindowsInstaller"))=="1"});
                    }
                }
                catch(System.Security.SecurityException){}catch(UnauthorizedAccessException){}catch(IOException){}catch(ArgumentException){}
            }
        }
        catch(System.Security.SecurityException){}catch(UnauthorizedAccessException){}catch(IOException){}
        return records;
    }
    static string Alias(Entry entry)
    {
        switch(entry.Id)
        {
            case "flydigi":return "Flydigi Space Station";
            case "razer":return "Razer Synapse";
            case "logitech":return "Logitech G HUB";
            case "mchose":return "MCHOSE HUB";
            case "steelseries":return "SteelSeries GG";
            case "nvidia":return "NVIDIA App";
            default:return entry.SourceName??entry.Name;
        }
    }
    static int Score(Entry entry,Record record)
    {
        if(entry.IsSetting||entry.IsDownload||!String.IsNullOrEmpty(entry.AppId))return 0;
        if(!String.IsNullOrEmpty(entry.Target)&&String.Equals(entry.Target,record.Icon,StringComparison.OrdinalIgnoreCase))return 10000;
        if(String.Equals(BaseName(Alias(entry)),BaseName(record.Name),StringComparison.OrdinalIgnoreCase))return 8000;
        if(!String.IsNullOrEmpty(entry.Target)&&SafeRoot(record.Root))
        {
            string root=Path.GetFullPath(record.Root).TrimEnd('\\')+"\\";
            if(Path.GetFullPath(entry.Target).StartsWith(root,StringComparison.OrdinalIgnoreCase))return 100+root.Length;
        }
        return 0;
    }
    static bool SafeRoot(string root)
    {
        if(String.IsNullOrWhiteSpace(root)||!Path.IsPathRooted(root))return false;
        string full=Path.GetFullPath(root).TrimEnd('\\');
        string[] broad={Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),Environment.GetFolderPath(Environment.SpecialFolder.Windows),Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)};
        return full.Length>3&&!broad.Any(x=>full.Equals(x.TrimEnd('\\'),StringComparison.OrdinalIgnoreCase));
    }
    internal static RemovalPlan Resolve(Entry entry,List<Record> records)
    {
        if(entry.IsSetting||entry.IsDownload)return null;
        if(!String.IsNullOrEmpty(entry.AppId))return PackagePlan(entry);
        var matches=records.Select(x=>new{Record=x,Score=Score(entry,x)}).Where(x=>x.Score>0).OrderByDescending(x=>x.Score).ToList();
        if(matches.Count==0)return null;
        var top=matches.Where(x=>x.Score==matches[0].Score).ToList();
        // Identical 32/64-bit registrations are okay; different equally likely removers are not.
        if(top.Select(x=>(x.Record.Command??"")+"|"+x.Record.Key+"|"+x.Record.Machine).Distinct(StringComparer.OrdinalIgnoreCase).Count()!=1)return null;
        Record record=top[0].Record;
        RemovalPlan plan=Parse(record,File.Exists);
        if(plan!=null){plan.Name=record.Name;plan.RegistryPath=record.RegistryPath;}
        return plan;
    }
    public static void Attach(List<Entry> entries)
    {
        List<Record> records=ReadRecords();
        foreach(Entry entry in entries)
        {
            try{entry.Removal=Resolve(entry,records);}catch{entry.Removal=null;}
        }
    }
    public static RemovalPlan ResolveCurrent(Entry entry){return Resolve(entry,ReadRecords());}
    internal static RemovalPlan Parse(Record record,Func<string,bool> exists)
    {
        string raw=Environment.ExpandEnvironmentVariables(record.Command??"").Trim();
        string file=null,args="";
        if(raw.StartsWith("\"",StringComparison.Ordinal))
        {
            int end=raw.IndexOf('"',1);if(end<=1)return null;
            file=raw.Substring(1,end-1);args=raw.Substring(end+1).Trim();
        }
        else
        {
            // Resolve the complete registered EXE path, including spaces; never launch C:\Program.exe.
            Match end=Regex.Match(raw,@"\.exe(?=\s|$)",RegexOptions.IgnoreCase);
            if(end.Success){file=raw.Substring(0,end.Index+4);args=raw.Substring(end.Index+4).Trim();}
        }
        string msi=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),"msiexec.exe");
        Guid product;
        if(record.Msi&&Guid.TryParse(record.Key,out product))
            return new RemovalPlan{FileName=msi,Arguments="/x {"+product.ToString().ToUpperInvariant()+"} /norestart",Elevate=record.Machine};
        if(String.IsNullOrEmpty(file))return null;
        if(Path.GetFileName(file).Equals("msiexec.exe",StringComparison.OrdinalIgnoreCase))
        {
            Match id=Regex.Match(args,@"^/(?:i|x|uninstall)\s*(\{[0-9a-fA-F-]{36}\})\s*$",RegexOptions.IgnoreCase);
            if(!id.Success||!Guid.TryParse(id.Groups[1].Value,out product))return null;
            return new RemovalPlan{FileName=msi,Arguments="/x {"+product.ToString().ToUpperInvariant()+"} /norestart",Elevate=record.Machine};
        }
        if(!Path.IsPathRooted(file)||file.StartsWith(@"\\",StringComparison.Ordinal)||!file.EndsWith(".exe",StringComparison.OrdinalIgnoreCase)||!exists(file))return null;
        if(Regex.IsMatch(Path.GetFileName(file),@"^(cmd|powershell|pwsh|wscript|cscript|regsvr32|mshta)\.exe$",RegexOptions.IgnoreCase))return null;
        // Vendor-provided rundll32 uninstall commands (e.g. NVIDIA) retain their registered arguments.
        if(Path.GetFileName(file).Equals("rundll32.exe",StringComparison.OrdinalIgnoreCase))
        {
            string win=Environment.GetFolderPath(Environment.SpecialFolder.Windows).TrimEnd('\\')+"\\";
            if(!file.StartsWith(win,StringComparison.OrdinalIgnoreCase)||args.IndexOf("uninstall",StringComparison.OrdinalIgnoreCase)<0)return null;
        }
        return new RemovalPlan{FileName=file,Arguments=args,Elevate=record.Machine};
    }
    static RemovalPlan PackagePlan(Entry entry)
    {
        string family=entry.AppId.Split('!')[0];
        if(!Regex.IsMatch(family,@"^[A-Za-z0-9.-]+_[a-z0-9]{13}$"))return null;
        string ps=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),@"WindowsPowerShell\v1.0\powershell.exe");
        if(!File.Exists(ps))return null;
        // Exact package family, current user only. Never remove provisioned packages or dependencies.
        string done=Ui.English?"App removed for this Windows account. Refresh the control center.":"已为当前 Windows 账户卸载此应用，请刷新控制中心。";
        string missing=Ui.English?"No removable app package was found for this account.":"当前账户未找到可卸载的应用包。";
        string title=Ui.English?"Uninstall app":"卸载软件";
        string script="$ErrorActionPreference='Stop'; Add-Type -AssemblyName System.Windows.Forms; try { $p=@(Get-AppxPackage | Where-Object { $_.PackageFamilyName -eq '"+family+"' -and -not $_.IsFramework -and -not $_.IsResourcePackage -and -not $_.NonRemovable }); if($p.Count -ne 1){throw '"+missing+"'}; Remove-AppxPackage -Package $p[0].PackageFullName -ErrorAction Stop; [System.Windows.Forms.MessageBox]::Show('"+done+"','"+title+"') | Out-Null } catch { [System.Windows.Forms.MessageBox]::Show($_.Exception.Message,'"+title+"') | Out-Null; exit 1 }";
        return new RemovalPlan{Name=entry.Name,PackageFamily=family,FileName=ps,Arguments="-NoProfile -NonInteractive -STA -WindowStyle Hidden -EncodedCommand "+Convert.ToBase64String(Encoding.Unicode.GetBytes(script))};
    }
    public static string Confirmation(Entry entry)
    {
        RemovalPlan plan=entry.Removal;
        string name=plan==null?entry.Name:plan.Name;
        return Ui.English?"Uninstall “"+name+"”?\n\nThis removes the software, not just its shortcut. Saved profiles or related functions may be affected.\n\n"+(plan!=null&&!String.IsNullOrEmpty(plan.PackageFamily)?"This app will be removed for the current Windows account.":"The software's own uninstaller will open. It may ask for administrator permission or a restart."):
            "确定卸载“"+name+"”？\n\n这会实际卸载软件，不只是移除入口。已保存的配置或相关功能可能受到影响。\n\n"+(plan!=null&&!String.IsNullOrEmpty(plan.PackageFamily)?"此应用将从当前 Windows 账户卸载。":"接下来将打开软件自身的卸载程序，可能需要管理员授权或重启。");
    }
    public static void Verify()
    {
        Func<string,bool> yes=delegate{return true;};
        Record spaced=new Record{Command=@"C:\Program Files\Vendor\uninstall.exe /remove true",Name="Vendor"};
        RemovalPlan parsed=Parse(spaced,yes);
        if(parsed==null||parsed.FileName!=@"C:\Program Files\Vendor\uninstall.exe"||parsed.Arguments!="/remove true")throw new Exception("Unquoted uninstall path parsing failed.");
        spaced.Command="\"C:\\Program Files\\Vendor\\uninstall.exe\" /allusers";
        if(Parse(spaced,yes).Arguments!="/allusers")throw new Exception("Registered arguments lost.");
        spaced.Command="MsiExec.exe /I{521c89be-637f-4274-a840-baaf7460c2b2}";
        if(!Parse(spaced,yes).Arguments.StartsWith("/x {521C89BE"))throw new Exception("MSI maintenance route not converted to removal.");
        spaced.Command=@"C:\Windows\System32\cmd.exe /c anything";
        if(Parse(spaced,yes)!=null)throw new Exception("Shell wrapper accepted.");
        spaced.Command="uninstall.exe";
        if(Parse(spaced,yes)!=null)throw new Exception("Relative uninstaller accepted.");
        spaced.Command="\"C:\\missing.exe\"";
        if(Parse(spaced,delegate{return false;})!=null)throw new Exception("Missing executable accepted.");
        Entry sample=new Entry{Id="sample",Name="Test App",SourceName="Test App",Target=@"C:\Test\app.exe"};
        List<Record> records=new List<Record>{new Record{Name="Test App",Command="\""+System.Windows.Forms.Application.ExecutablePath+"\"",Key="one"},new Record{Name="Test App",Command="\""+System.Windows.Forms.Application.ExecutablePath+"\" --other",Key="two"}};
        if(Resolve(sample,records)!=null)throw new Exception("Ambiguous uninstall registration accepted.");
        sample.Kind="sound";if(Resolve(sample,records)!=null)throw new Exception("Windows settings received removal action.");
        sample.Kind="peripheral";sample.AppId="NotAValidFamily!App";if(PackagePlan(sample)!=null)throw new Exception("Invalid package family accepted.");
    }
}
