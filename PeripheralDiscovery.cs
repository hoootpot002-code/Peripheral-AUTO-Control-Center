using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Win32;

// Read-only discovery. Never run installers, services, firmware tools or uninstallers.
internal static class Discovery
{
    internal sealed class Candidate
    {
        public string Name, Target, Arguments, Icon, WorkingDirectory, AppId;
    }
    sealed class Installed
    {
        public string Name, Publisher, Root, Icon;
    }
    static readonly Regex Brands=new Regex(@"\b(logitech|logi|razer|flydigi|steelseries|corsair|icue|hyperx|ngenuity|roccat|turtle\s*beach|mchose|rapoo|wacom|huion|xp[- ]?pen|xppen|ugee|xencelabs|8bitdo|gamesir|gulikit|bigbig\s*won|vgn|vxe|atk|aula|keychron|lemokey|wooting|glorious|darmoshark|dareu|ajazz|akko|monsgeek|ducky|varmilo|epomaker|redragon|fantech|pulsar|lamzu|ninjutso|endgame\s*gear|finalmouse|pwnage|waizowl|delux|fuhlen|bloody|a4tech|speedlink|thrustmaster|fanatec|moza|simagic|simucube|pxn|betop|fifine|maono|elgato|stream\s*deck|loupedeck|tobii|trackir|creative\s*technology|sound\s*blaster|sennheiser|epos|audeze|beyerdynamic|focusrite|audient|motu|presonus|yamaha\s*steinberg|realtek|dolby|nahimic|sonarworks)\b|罗技|雷蛇|飞智|飞驰空间站|赛睿|海盗船|美商海盗船|金士顿|迈从|雷柏|绘王|友基|高漫|八位堂|盖世小鸡|谷粒|北通|墨将|小派|机械师|狼蛛|渴创|珂芝|阿米洛|达尔优|达摩鲨|黑爵|艾石头|前行者|黑峡谷|腹灵|雷神鼠标|钛度|铝厂|摩豹|山业|漫步者|得胜|飞傲|艾肯|纽曼声卡|ROG\s*Armoury|Armoury\s*Crate|Aura\s*Creator",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant);
    static readonly Regex PeripheralTerms=new Regex(@"\b(mouse|mice|keyboard|keypad|headset|headphone|microphone|gamepad|joystick|peripheral|soundcard|drawing\s*tablet|pen\s*tablet|racing\s*wheel|audio\s*(console|control|interface)|rgb\s*(control|fusion)|gaming\s*(hub|mouse|keyboard|gear)|device\s*(configuration|configurator))\b|鼠标|键盘|手柄|外设|耳机|麦克风|声卡|音频控制|数位板|数位屏|绘图板|方向盘|灯效控制|灯光控制|驱动精灵外设",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant);
    static readonly Regex Maintenance=new Regex(@"uninstall|unins\d*|卸载|安装程序|安装向导|固件升级工具|固件烧录|setup|installer|updater?|auto[-_ ]?update|crash|reporter|telemetry|notification|notifier|watcher|daemon|service|服务|(?:^|[ ._-])sdk(?:$|[ ._-])|\bhal\b|framework|组件|component|redistribut|repair|修复|firmware|bootloader|devcon|burntool|驱动包|驱动程序包|软件包|driver\s*package|图形驱动程序|HD\s*音频驱动程序|physx|messagebus|backend|container|vcredist|vc_redist|virtual\s*assistant|virtual\s*pet|virtual\s*audio",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant);
    public static string DataDirectoryOverride;
    public static string[] ExtraShortcutRoots=new string[0];
    public static string TraceFile;
    public static void Trace(string message){if(TraceFile!=null)File.AppendAllText(TraceFile,DateTime.Now.ToString("HH:mm:ss.fff")+" "+message+Environment.NewLine);}
    static string ManualPath { get { return Path.Combine(DataDirectoryOverride??Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"PeripheralHub"),"custom-apps.xml"); } }
    public static bool IsMaintenance(string name) { return !String.IsNullOrEmpty(name)&&Maintenance.IsMatch(name); }
    public static bool IsRelevant(string name,string metadata)
    {
        if(String.IsNullOrWhiteSpace(name)||IsMaintenance(name))return false;
        string all=name+" "+metadata;
        return Brands.IsMatch(all)||PeripheralTerms.IsMatch(all)||Regex.IsMatch(name,@"\bNVIDIA\s*(App|Control\s*Panel)\b|NVIDIA\s*控制面板",RegexOptions.IgnoreCase);
    }
    static string Clean(string value) {return Environment.ExpandEnvironmentVariables(value??"").Trim().Trim('"');}
    public static IEnumerable<string> Files(string root,string pattern,int depth)
    {
        List<string> result=new List<string>();
        if(depth<0||String.IsNullOrEmpty(root))return result;
        try
        {
            result.AddRange(Directory.GetFiles(root,pattern));
            if(depth>0)foreach(string dir in Directory.GetDirectories(root))
            {
                try
                {
                    if((File.GetAttributes(dir)&FileAttributes.ReparsePoint)!=0)continue;
                    if(pattern=="*.exe"&&Regex.IsMatch(Path.GetFileName(dir),@"^(driver|drivers|firmware|cache|node_modules|locales|resources|redist|update|installer|logs)$",RegexOptions.IgnoreCase))continue;
                    result.AddRange(Files(dir,pattern,depth-1));
                }
                catch(IOException){}catch(UnauthorizedAccessException){}
            }
        }
        catch(IOException){}catch(UnauthorizedAccessException){}
        return result;
    }
    static List<Installed> InstalledApps()
    {
        List<Installed> result=new List<Installed>();
        foreach(RegistryHive hive in new RegistryHive[]{RegistryHive.LocalMachine,RegistryHive.CurrentUser})
        foreach(RegistryView view in new RegistryView[]{RegistryView.Registry32,RegistryView.Registry64})
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
                        string publisher=Convert.ToString(key.GetValue("Publisher"));
                        if(!IsRelevant(name,publisher))continue;
                        string icon=IconFile(Convert.ToString(key.GetValue("DisplayIcon")));
                        string location=Clean(Convert.ToString(key.GetValue("InstallLocation")));
                        if(!Directory.Exists(location)&&File.Exists(icon))location=Path.GetDirectoryName(icon);
                        result.Add(new Installed{Name=name,Publisher=publisher,Root=location,Icon=icon});
                    }
                }
                catch(System.Security.SecurityException){}catch(UnauthorizedAccessException){}catch(IOException){}
            }
        }
        catch(System.Security.SecurityException){}catch(UnauthorizedAccessException){}catch(IOException){}
        return result;
    }
    static string IconFile(string value)
    {
        value=value??"";int comma=value.LastIndexOf(',');
        if(comma>=0){int index;if(Int32.TryParse(value.Substring(comma+1),out index))value=value.Substring(0,comma);}
        return Clean(value);
    }
    static bool Usable(string path)
    {
        return !String.IsNullOrEmpty(path)&&File.Exists(path)&&Path.GetExtension(path).Equals(".exe",StringComparison.OrdinalIgnoreCase)&&!IsMaintenance(Path.GetFileNameWithoutExtension(path))&&!Regex.IsMatch(Path.GetFileNameWithoutExtension(path),@"helper|elevat|agent|(^|[._-])(rundll32|cmd|powershell|pwsh|msiexec|regsvr32|wscript|cscript|conhost)([._-]|$)",RegexOptions.IgnoreCase);
    }
    static string Product(string path)
    {
        try{FileVersionInfo info=FileVersionInfo.GetVersionInfo(path);return info.ProductName+" "+info.FileDescription+" "+info.CompanyName;}catch{return "";}
    }
    public static void Expand(List<Entry> entries,IEnumerable<Candidate> shortcuts)
    {
        List<Installed> records=InstalledApps();
        List<Candidate> links=shortcuts.GroupBy(Key,StringComparer.OrdinalIgnoreCase).Select(x=>x.First()).ToList();
        foreach(Candidate c in links)
        {
            if(!Usable(c.Target)||IsMaintenance(c.Name))continue;
            if(c.Target.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.Windows).TrimEnd('\\')+"\\",StringComparison.OrdinalIgnoreCase))continue;
            Installed record=records.FirstOrDefault(x=>String.Equals(x.Name,c.Name,StringComparison.OrdinalIgnoreCase)||(!String.IsNullOrEmpty(x.Root)&&c.Target.StartsWith(x.Root.TrimEnd('\\')+"\\",StringComparison.OrdinalIgnoreCase))||String.Equals(x.Icon,c.Target,StringComparison.OrdinalIgnoreCase));
            string metadata=Product(c.Target)+(record==null?"":" "+record.Name+" "+record.Publisher);
            if(IsRelevant(c.Name,metadata))entries.Add(Create(c));
        }
        foreach(Installed record in records)
        {
            // Prefer the registered shortcut, which preserves required launch arguments.
            if(links.Any(x=>Usable(x.Target)&&(String.Equals(record.Name,x.Name,StringComparison.OrdinalIgnoreCase)||(!String.IsNullOrEmpty(record.Root)&&x.Target.StartsWith(record.Root.TrimEnd('\\')+"\\",StringComparison.OrdinalIgnoreCase)))))continue;
            string target=null;
            if(Usable(record.Icon))target=record.Icon;
            else if(SafeInstallRoot(record.Root))
                target=Files(record.Root,"*.exe",2).Where(Usable).Take(150).OrderByDescending(x=>ExecutableScore(x,record.Name)).FirstOrDefault();
            if(target!=null)entries.Add(Create(new Candidate{Name=record.Name,Target=target,Icon=record.Icon}));
        }
        foreach(Candidate manual in ReadManual())if(Usable(manual.Target)){Entry entry=Create(manual);entry.Badge="手动添加";entries.Add(entry);}
    }
    static bool SafeInstallRoot(string root)
    {
        if(String.IsNullOrEmpty(root)||!Directory.Exists(root))return false;
        string full=Path.GetFullPath(root).TrimEnd('\\');
        if(full.Length<=3||full.Equals(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),StringComparison.OrdinalIgnoreCase)||full.Equals(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),StringComparison.OrdinalIgnoreCase))return false;
        return full.IndexOf("Installer",StringComparison.OrdinalIgnoreCase)<0&&full.IndexOf("Package Cache",StringComparison.OrdinalIgnoreCase)<0&&!full.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.Windows),StringComparison.OrdinalIgnoreCase);
    }
    static int ExecutableScore(string path,string product)
    {
        string file=Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
        string name=Regex.Replace(product.ToLowerInvariant(),@"[\s\d._-]+","");
        string simple=Regex.Replace(file,@"[\s\d._-]+","");
        int result=file.Contains("launcher")?30:0;
        if(name.Contains(simple)||simple.Contains(name))result+=100;
        if(IsRelevant(file,Product(path)))result+=40;
        if(file.Contains("client")||file.Contains("control")||file.Contains("hub"))result+=15;
        return result-path.Count(x=>x=='\\');
    }
    static string StableId(string key)
    {
        using(SHA256 hash=SHA256.Create())return "scan-"+BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(key.ToLowerInvariant()))).Replace("-","").Substring(0,16);
    }
    static string Key(Candidate c){return !String.IsNullOrEmpty(c.AppId)?"app:"+c.AppId:Clean(c.Target)+"|"+(c.Arguments??"").Trim();}
    public static string Key(Entry entry){return entry.IsSetting?"uri:"+entry.Target:Key(new Candidate{Target=entry.Target,Arguments=entry.Arguments,AppId=entry.AppId});}
    public static Entry Create(Candidate c)
    {
        return new Entry{Id=StableId(Key(c)),Name=c.Name,Target=c.Target,Arguments=c.Arguments,WorkingDirectory=c.WorkingDirectory,IconPath=c.Icon,AppId=c.AppId,Kind="peripheral",Badge="自动发现",Detail="设备管理软件 · 一键打开",Glyph="app",Accent=Color.FromArgb(108,200,235)};
    }
    public static List<Entry> Deduplicate(List<Entry> entries)
    {
        List<Entry> result=new List<Entry>();
        HashSet<string> seen=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach(Entry e in entries)
        {
            string key=Key(e);
            if(seen.Add(key))result.Add(e);
            else if(e.Image!=null)e.Image.Dispose();
        }
        return result;
    }
    public static Candidate FromFile(string path)
    {
        if(Path.GetExtension(path).Equals(".lnk",StringComparison.OrdinalIgnoreCase))
        {
            object shellObject=Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));object shortcutObject=null;
            try
            {
                dynamic shell=shellObject;shortcutObject=shell.CreateShortcut(path);dynamic link=shortcutObject;
                return new Candidate{Name=Path.GetFileNameWithoutExtension(path),Target=Clean((string)link.TargetPath),Arguments=(string)link.Arguments,WorkingDirectory=(string)link.WorkingDirectory,Icon=IconFile((string)link.IconLocation)};
            }
            finally{if(shortcutObject!=null)Marshal.FinalReleaseComObject(shortcutObject);Marshal.FinalReleaseComObject(shellObject);}
        }
        return new Candidate{Name=Path.GetFileNameWithoutExtension(path),Target=path,Icon=path};
    }
    static List<Candidate> ReadManual()
    {
        try
        {
            if(!File.Exists(ManualPath))return new List<Candidate>();
            return XDocument.Load(ManualPath).Root.Elements("app").Select(x=>new Candidate{Name=(string)x.Attribute("name"),Target=(string)x.Attribute("target"),Arguments=(string)x.Attribute("arguments"),Icon=(string)x.Attribute("icon"),WorkingDirectory=(string)x.Attribute("workingDirectory")}).ToList();
        }
        catch{return new List<Candidate>();}
    }
    public static void AddManual(Candidate candidate)
    {
        if(!Usable(candidate.Target))throw new ArgumentException(Ui.Text("请选择可运行的管理软件 EXE，或指向该软件的快捷方式。"));
        List<Candidate> list=ReadManual();
        if(!list.Any(x=>String.Equals(Key(x),Key(candidate),StringComparison.OrdinalIgnoreCase)))list.Add(candidate);
        XElement root=new XElement("apps",list.Select(x=>new XElement("app",new XAttribute("name",x.Name??""),new XAttribute("target",x.Target),new XAttribute("arguments",x.Arguments??""),new XAttribute("icon",x.Icon??""),new XAttribute("workingDirectory",x.WorkingDirectory??""))));
        Directory.CreateDirectory(Path.GetDirectoryName(ManualPath));
        string temp=ManualPath+".tmp";new XDocument(root).Save(temp);
        if(File.Exists(ManualPath))File.Replace(temp,ManualPath,null);else File.Move(temp,ManualPath);
    }
    public static void Verify(string folder)
    {
        if(!IsRelevant("Nebula 无线鼠标控制台","Nebula Labs"))throw new Exception("Unknown Chinese peripheral not recognized.");
        if(!IsRelevant("Nebula Device Center","Nebula Gaming Keyboard Configuration"))throw new Exception("Unknown product metadata not recognized.");
        if(!IsRelevant("Corsair iCUE",""))throw new Exception("Known brand not recognized.");
        if(IsRelevant("Photo Editor","Example Labs")||IsRelevant("Adobe Creative Cloud","Adobe")||IsRelevant("Firefox","Mozilla")||IsRelevant("Razer Updater","")||IsRelevant("Armoury Crate Service","")||IsRelevant("NVIDIA Container",""))throw new Exception("Non-UI software incorrectly classified.");
        string old=DataDirectoryOverride;DataDirectoryOverride=Path.Combine(folder,"manual-state");
        try
        {
            Candidate manual=new Candidate{Name="未知品牌控制台",Target=System.Windows.Forms.Application.ExecutablePath,Arguments="--sample-value=\"a b\"",Icon=System.Windows.Forms.Application.ExecutablePath};
            AddManual(manual);AddManual(manual);
            List<Candidate> stored=ReadManual();
            if(stored.Count!=1||stored[0].Arguments!=manual.Arguments)throw new Exception("Manual entry persistence or deduplication failed.");
            List<Entry> entries=new List<Entry>{Create(manual),Create(manual)};
            if(Deduplicate(entries).Count!=1)throw new Exception("Entry deduplication failed.");
            // Integration fixture is a real .lnk pointing to a local executable with custom arguments.
            string fixture=Path.Combine(folder,"Nebula 无线鼠标控制台.lnk");
            object shellObject=Activator.CreateInstance(Type.GetTypeFromProgID("WScript.Shell"));object linkObject=null;
            try{dynamic shell=shellObject;linkObject=shell.CreateShortcut(fixture);dynamic link=linkObject;link.TargetPath=manual.Target;link.Arguments=manual.Arguments;link.IconLocation=manual.Icon+",0";link.Save();}
            finally{if(linkObject!=null)Marshal.FinalReleaseComObject(linkObject);Marshal.FinalReleaseComObject(shellObject);}
            Candidate resolved=FromFile(fixture);
            if(resolved.Target!=manual.Target||resolved.Arguments!=manual.Arguments)throw new Exception("Shortcut routing failed.");
            List<Entry> discovered=new List<Entry>();Expand(discovered,new Candidate[]{resolved,resolved});
            Entry found=Deduplicate(discovered).SingleOrDefault(x=>x.Name==resolved.Name);
            if(found==null||found.Command().Arguments!=manual.Arguments)throw new Exception("Automatic discovery of a previously unknown peripheral failed.");
        }
        finally{DataDirectoryOverride=old;}
    }
}
