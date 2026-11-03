using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

#if ENGLISH
[assembly: AssemblyTitle("Peripheral AUTO Control Center - English")]
#else
[assembly: AssemblyTitle("外设AUTO控制中心")]
#endif
[assembly: AssemblyDescription("Peripheral apps, Windows audio shortcuts, direct uninstall and official download pages")]
[assembly: AssemblyVersion("2.2.0.0")]
[assembly: AssemblyCompany("Nonori")]
[assembly: AssemblyProduct("Peripheral AUTO Control Center | 外设AUTO控制中心")]
[assembly: AssemblyCopyright("Copyright © 2026 Nonori")]

internal sealed class Entry
{
    public string Id, Name, SourceName, Detail, Kind, Badge, Glyph, Target, Arguments, AppId, IconPath, WorkingDirectory, DownloadUrl, SearchTerms;
    public RemovalPlan Removal;
    public bool IsDownload { get { return Kind == "store"; } }
    public string[] Candidates, Shortcuts;
    public Color Accent;
    public Bitmap Image;
    public bool Available;
    public bool IsSetting { get { return Kind == "sound"; } }
    public ProcessStartInfo Command()
    {
        if (IsSetting) return new ProcessStartInfo(Target) { UseShellExecute = true };
        if (!String.IsNullOrEmpty(AppId)) return new ProcessStartInfo("explorer.exe", "shell:AppsFolder\\" + AppId) { UseShellExecute = true };
        return new ProcessStartInfo(Target, Arguments ?? "") { UseShellExecute = true, WorkingDirectory = String.IsNullOrEmpty(WorkingDirectory)?Path.GetDirectoryName(Target):WorkingDirectory };
    }
}

internal static class Catalog
{
    static Entry App(string id, string name, string detail, string kind, string badge, string color, string[] candidates, string[] shortcuts, string appId, string iconPath)
    {
        return new Entry { Id=id, Name=name, Detail=detail, Kind=kind, Badge=badge, Accent=ColorTranslator.FromHtml(color), Candidates=candidates, Shortcuts=shortcuts, AppId=appId, IconPath=iconPath, Glyph="app" };
    }
    static Entry Setting(string id, string name, string detail, string target, string glyph, string color)
    {
        return new Entry { Id=id, Name=name, Detail=detail, Kind="sound", Badge="WINDOWS", Accent=ColorTranslator.FromHtml(color), Target=target, Glyph=glyph, Available=true };
    }
    public static List<Entry> Load()
    {
        Discovery.Trace("catalog start");
        List<Entry> list = new List<Entry>();
        list.Add(App("flydigi", "飞智空间站 4.0", "手柄映射 · 摇杆校准 · 固件", "peripheral", "FLYDIGI", "#44BBFF", new string[] {@"C:\Program Files\Flydigi Space Station\Flydigi Space Station.exe"}, new string[] {"飞智空间站 4.0"}, null, null));
        list.Add(App("razer", "雷蛇 Synapse", "按键绑定 · 灵敏度 · 幻彩灯效", "peripheral", "RAZER", "#84DE4D", new string[] {@"C:\Program Files\Razer\RazerAppEngine\RazerAppEngine.exe"}, new string[] {"Razer Synapse"}, null, @"C:\ProgramData\Razer\Razer Synapse\synapse.ico"));
        list.Add(App("logitech", "罗技 G HUB", "鼠标 · 键盘 · 耳机配置", "peripheral", "LOGITECH G", "#4ED5F2", new string[] {@"C:\Program Files\LGHUB\system_tray\lghub_system_tray.exe", @"C:\Program Files\LGHUB\lghub.exe"}, new string[] {"Logitech G HUB"}, null, @"C:\Program Files\LGHUB\lghub.exe"));
        list.Add(App("mchose", "迈从 MCHOSE HUB", "鼠标与键盘 · 性能 · 灯效", "peripheral", "MCHOSE", "#FFA85B", new string[] {@"C:\Program Files\MCHOSE HUB\MCHOSE HUB.exe"}, new string[] {"MCHOSE HUB"}, null, null));
        list.Add(App("steelseries", "赛睿 SteelSeries GG", "外设配置 · Sonar 音频", "peripheral", "STEELSERIES", "#F5A477", new string[] {@"C:\Program Files\SteelSeries\GG\SteelSeriesGGEZ.exe"}, new string[] {"SteelSeries GG"}, null, @"C:\Program Files\SteelSeries\GG\SteelSeriesGGClient.exe"));
        list.Add(App("armoury", "Armoury Crate", "华硕 / ROG · 设备与灯效", "peripheral", "ROG / ASUS", "#FF6686", null, null, "B9ECED6F.ArmouryCrate_qmba6cd70vzyy!App", null));
        list.Add(App("aura", "Aura Creator", "华硕 / ROG · 自定义灯光效果", "peripheral", "AURA", "#BA9BFF", null, null, "B9ECED6F.AURACreator_qmba6cd70vzyy!App", null));
        list.Add(Setting("headphones", "耳机 / 扬声器", "默认输出 · 音量 · 音效", "ms-settings:sound-defaultoutputproperties", "headphone", "#5DD9D0"));
        list.Add(Setting("microphone", "麦克风设置", "默认输入 · 音量 · 麦克风测试", "ms-settings:sound-defaultinputproperties", "mic", "#C3A1FF"));
        list.Add(Setting("sound", "声音与设备", "选择输出 / 输入 · 管理音频设备", "ms-settings:sound", "speaker", "#75ADFF"));
        list.Add(Setting("mixer", "应用音量混合器", "分别调整每个应用的音量", "ms-settings:apps-volume", "mixer", "#FFB879"));
        list.Add(Setting("privacy", "麦克风权限", "管理应用对麦克风的访问", "ms-settings:privacy-microphone", "shield", "#F593BE"));
        list.Add(App("realtek", "Realtek 音频控制台", "声卡 · 接口 · 音频效果", "tools", "REALTEK", "#78BFFF", null, null, "RealtekSemiconductorCorp.RealtekAudioControl_dt26b99r8h8gj!App", null));
        list.Add(App("dolby", "Dolby Access", "杜比全景声 · 耳机空间音效", "tools", "DOLBY", "#D4D9E7", null, null, "DolbyLaboratories.DolbyAccess_rz1tebttyb220!App", null));
        list.Add(App("nvidia", "NVIDIA App", "显卡设置 · 图形选项 · 游戏优化", "tools", "NVIDIA", "#A5D751", new string[] {@"C:\Program Files\NVIDIA Corporation\NVIDIA App\CEF\NVIDIA App.exe"}, new string[] {"NVIDIA App"}, null, null));
        list.Add(App("nvcpl", "NVIDIA 控制面板", "显示器 · 分辨率 · 3D 设置", "tools", "NVIDIA", "#A5D751", null, null, "NVIDIACorp.NVIDIAControlPanel_56jybvy8sckqj!NVIDIACorp.NVIDIAControlPanel", null));
        Dictionary<string, Link> links = ReadLinks();
        Discovery.Trace("shortcuts read: "+links.Count);
        Discovery.Expand(list,links.Values.Select(x=>new Discovery.Candidate {Name=x.Name,Target=x.Target,Arguments=x.Arguments,Icon=x.Icon,WorkingDirectory=x.WorkingDirectory}));
        Discovery.Trace("registered desktop apps read: "+list.Count);
        HashSet<string> installedApps = ReadStoreApps(list);
        Discovery.Trace("store apps read: "+list.Count);
        foreach (Entry e in list)
        {
            if (e.IsSetting) { Ui.Localize(e); continue; }
            if (e.Shortcuts != null)
                foreach (string name in e.Shortcuts)
                {
                    Link link;
                    if (links.TryGetValue(name, out link) && File.Exists(link.Target))
                    {
                        e.Target=link.Target; e.Arguments=link.Arguments;
                        if (String.IsNullOrEmpty(e.IconPath) && File.Exists(link.Icon)) e.IconPath=link.Icon;
                        break;
                    }
                }
            if (String.IsNullOrEmpty(e.Target) && e.Candidates != null)
                e.Target=e.Candidates.FirstOrDefault(File.Exists);
            if (e.Id=="razer" && String.IsNullOrEmpty(e.Arguments)) e.Arguments="--url-params=apps=synapse";
            if (e.Id=="steelseries" && String.IsNullOrEmpty(e.Arguments)) e.Arguments="-dataPath=\"C:\\ProgramData\\SteelSeries\\GG\" -dbEnv=production";
            e.Available = !String.IsNullOrEmpty(e.AppId) ? installedApps.Contains(e.AppId) : !String.IsNullOrEmpty(e.Target) && File.Exists(e.Target);
            Ui.Localize(e);
            if (e.Image==null && !String.IsNullOrEmpty(e.AppId)) e.Image = ShellImages.Load("::{4234d49b-0245-4df3-b780-3893943456e1}\\"+e.AppId);
            if (e.Image==null && !String.IsNullOrEmpty(e.IconPath)) e.Image=ShellImages.LoadFile(e.IconPath);
            if (e.Image==null && !String.IsNullOrEmpty(e.Target)) e.Image=ShellImages.LoadFile(e.Target);
        }
        List<Entry> available=list.Where(x=>x.Available).ToList();
        foreach(Entry e in list.Except(available))if(e.Image!=null)e.Image.Dispose();
        List<Entry> deduped=Discovery.Deduplicate(available);AppManagement.Attach(deduped);Discovery.Trace("catalog done: "+deduped.Count);return deduped;
    }
    sealed class Link { public string Name, Target, Arguments, Icon, WorkingDirectory; }
    static Dictionary<string, Link> ReadLinks()
    {
        Dictionary<string, Link> links=new Dictionary<string, Link>(StringComparer.OrdinalIgnoreCase);
        Type shellType=Type.GetTypeFromProgID("WScript.Shell");
        if (shellType==null) return links;
        object shellObject=Activator.CreateInstance(shellType);
        dynamic shell=shellObject;
        try
        {
            IEnumerable<string> roots=new Environment.SpecialFolder[] {Environment.SpecialFolder.CommonStartMenu, Environment.SpecialFolder.StartMenu, Environment.SpecialFolder.CommonDesktopDirectory, Environment.SpecialFolder.DesktopDirectory}.Select(Environment.GetFolderPath).Concat(Discovery.ExtraShortcutRoots);
            foreach (string root in roots)
            {
                if (!Directory.Exists(root)) continue;
                string[] paths;
                try { paths=Discovery.Files(root,"*.lnk",8).ToArray(); } catch { continue; }
                foreach (string path in paths)
                {
                    string name=Path.GetFileNameWithoutExtension(path);
                    if(Discovery.IsMaintenance(name))continue;
                    object shortcutObject=null;
                    try
                    {
                        shortcutObject=shell.CreateShortcut(path);
                        dynamic shortcut=shortcutObject;
                        string icon=(string)shortcut.IconLocation;
                        int comma=icon.LastIndexOf(',');
                        if (comma>=0) icon=icon.Substring(0,comma);
                        string target=Environment.ExpandEnvironmentVariables((string)shortcut.TargetPath).Trim('"');
                        links[name+"\n"+target+"\n"+(string)shortcut.Arguments]=new Link {Name=name,Target=target,Arguments=(string)shortcut.Arguments,Icon=icon.Trim('"'),WorkingDirectory=(string)shortcut.WorkingDirectory};
                        if(!links.ContainsKey(name))links[name]=links[name+"\n"+target+"\n"+(string)shortcut.Arguments];
                    }
                    catch { }
                    finally { if (shortcutObject!=null) Marshal.FinalReleaseComObject(shortcutObject); }
                }
            }
        }
        finally { Marshal.FinalReleaseComObject(shellObject); }
        return links;
    }
    static HashSet<string> ReadStoreApps(List<Entry> entries)
    {
        HashSet<string> result=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        object shellObject=null, folderObject=null, itemsObject=null;
        try
        {
            shellObject=Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));
            dynamic shell=shellObject;
            folderObject=shell.NameSpace("shell:AppsFolder");
            dynamic folder=folderObject;
            itemsObject=folder.Items();
            dynamic items=itemsObject;
            int count=(int)items.Count;
            for (int i=0;i<count;i++)
            {
                object itemObject=items.Item(i);
                try
                {
                    dynamic item=itemObject; string id=(string)item.ExtendedProperty("System.AppUserModel.ID");
                    if (!String.IsNullOrEmpty(id)) result.Add(id);
                    Entry entry=entries.FirstOrDefault(x=>x.AppId==id);
                    string packageRoot=(string)item.ExtendedProperty("System.AppUserModel.PackageInstallPath");
                    if(entry==null&&!String.IsNullOrEmpty(packageRoot)&&Discovery.IsRelevant((string)item.Name,id))
                    {
                        entry=Discovery.Create(new Discovery.Candidate {Name=(string)item.Name,AppId=id});
                        entries.Add(entry);
                    }
                    if(entry!=null)
                    {
                        string root=packageRoot;
                        string logo=(string)item.ExtendedProperty("System.Tile.SmallLogoPath");
                        if(!String.IsNullOrEmpty(root)&&!String.IsNullOrEmpty(logo))
                        {
                            string exact=Path.Combine(root,logo);
                            string asset=File.Exists(exact)?exact:Directory.GetFiles(Path.GetDirectoryName(exact),Path.GetFileNameWithoutExtension(exact)+"*.png").OrderByDescending(AssetScore).FirstOrDefault();
                            if(asset!=null)using(Image image=Image.FromFile(asset))entry.Image=new Bitmap(image);
                        }
                    }
                }
                catch { }
                finally { Marshal.FinalReleaseComObject(itemObject); }
            }
        }
        catch { }
        finally
        {
            if(itemsObject!=null) Marshal.FinalReleaseComObject(itemsObject);
            if(folderObject!=null) Marshal.FinalReleaseComObject(folderObject);
            if(shellObject!=null) Marshal.FinalReleaseComObject(shellObject);
        }
        return result;
    }
    static int AssetScore(string path)
    {
        string name=Path.GetFileName(path).ToLowerInvariant();
        int score=name.Contains("targetsize-256")?1000:name.Contains("targetsize-128")?950:name.Contains("targetsize-96")?900:name.Contains("targetsize-64")?850:name.Contains("scale-200")?800:name.Contains("scale-400")?750:name.Contains("targetsize-48")?700:name.Contains("scale-150")?650:name.Contains("scale-100")?600:100;
        return score+(name.Contains("unplated")?20:0);
    }
}

internal static class ShellImages
{
    [StructLayout(LayoutKind.Sequential)] internal struct Size { public int X,Y; public Size(int x,int y){X=x;Y=y;} }
    [ComImport, Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IImageFactory { [PreserveSig] int GetImage(Size size, int flags, out IntPtr bitmap); }
    [DllImport("shell32.dll",CharSet=CharSet.Unicode,PreserveSig=false)]
    static extern void SHCreateItemFromParsingName(string name, IntPtr bind, ref Guid iid, [MarshalAs(UnmanagedType.Interface)] out IImageFactory factory);
    [DllImport("gdi32.dll")] static extern bool DeleteObject(IntPtr handle);
    public static Bitmap Load(string name)
    {
        IImageFactory factory=null; IntPtr handle=IntPtr.Zero;
        try
        {
            Guid id=typeof(IImageFactory).GUID;
            SHCreateItemFromParsingName(name,IntPtr.Zero,ref id,out factory);
            if(factory.GetImage(new Size(128,128),0x100|0x4,out handle)!=0 || handle==IntPtr.Zero) return null;
            using(Bitmap image=Image.FromHbitmap(handle)) return new Bitmap(image);
        }
        catch { return null; }
        finally { if(handle!=IntPtr.Zero) DeleteObject(handle); if(factory!=null) Marshal.FinalReleaseComObject(factory); }
    }
    public static Bitmap LoadFile(string path)
    {
        try
        {
            if (!File.Exists(path)) return null;
            if (Path.GetExtension(path).Equals(".ico",StringComparison.OrdinalIgnoreCase))
                using (Icon icon=new Icon(path,128,128)) return icon.ToBitmap();
            Bitmap shell=Load(path);
            if(shell!=null) return shell;
            using (Icon icon=Icon.ExtractAssociatedIcon(path)) return icon.ToBitmap();
        }
        catch { return null; }
    }
}

internal static class Art
{
    static readonly Bitmap SignatureImage=LoadSignature();
    static Bitmap LoadSignature()
    {
        using(Stream stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("Nonori.Signature.png"))
        {
            if(stream==null)return null;
            using(Image image=Image.FromStream(stream))return new Bitmap(image);
        }
    }
    static readonly string SignatureFont=new string[] {"Edwardian Script ITC","Kunstler Script","French Script MT","Vivaldi","Monotype Corsiva","Segoe Script"}.FirstOrDefault(name=>FontFamily.Families.Any(family=>family.Name.Equals(name,StringComparison.OrdinalIgnoreCase)))??"Segoe UI";
    public static void Signature(Graphics g,float x,float y)
    {
        if(SignatureImage!=null)
        {
            GraphicsState state=g.Save();g.InterpolationMode=InterpolationMode.HighQualityBicubic;
            g.DrawImage(SignatureImage,new RectangleF(x,y,150,28));g.Restore(state);return;
        }
        using(Font font=new Font(SignatureFont,22,FontStyle.Regular,GraphicsUnit.Pixel))
        using(SolidBrush brush=new SolidBrush(Color.FromArgb(126,148,166)))
            g.DrawString("designed by nonori",font,brush,x,y,StringFormat.GenericTypographic);
    }
    public static Color Background=Color.FromArgb(12,17,25), Card=Color.FromArgb(23,31,44), Muted=Color.FromArgb(138,153,175), Text=Color.FromArgb(239,244,251), Cyan=Color.FromArgb(101,231,206);
    public static GraphicsPath Round(RectangleF r,float radius)
    {
        GraphicsPath p=new GraphicsPath(); float d=radius*2;
        p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();return p;
    }
    public static void Fill(Graphics g,RectangleF r,float radius,Color color)
    { using(GraphicsPath p=Round(r,radius)) using(SolidBrush b=new SolidBrush(color)) g.FillPath(b,p); }
    public static void Stroke(Graphics g,RectangleF r,float radius,Color color)
    { using(GraphicsPath p=Round(r,radius)) using(Pen pen=new Pen(color)) g.DrawPath(pen,p); }
    public static void Label(Graphics g,string text,float x,float y,float size,Color color,bool bold)
    { using(Font font=new Font(Ui.FontFamily,size,bold?FontStyle.Bold:FontStyle.Regular,GraphicsUnit.Pixel)) using(SolidBrush brush=new SolidBrush(color)) g.DrawString(Ui.Text(text),font,brush,x,y,StringFormat.GenericTypographic); }
    public static void Glyph(Graphics g,string type,RectangleF r,Color color)
    {
        GraphicsState state=g.Save();g.TranslateTransform(r.X,r.Y);g.ScaleTransform(r.Width/32,r.Height/32);
        using(Pen p=new Pen(color,2.1f))
        {
            p.StartCap=LineCap.Round;p.EndCap=LineCap.Round;p.LineJoin=LineJoin.Round;
            if(type=="headphone") {g.DrawArc(p,5,4,22,24,180,180);g.DrawLine(p,5,16,5,25);g.DrawLine(p,27,16,27,25);Fill(g,new RectangleF(4,17,5,11),2,color);Fill(g,new RectangleF(23,17,5,11),2,color);}
            else if(type=="mic") {using(GraphicsPath path=Round(new RectangleF(11,3,10,18),5))g.DrawPath(p,path);g.DrawArc(p,7,10,18,15,0,180);g.DrawLine(p,7,15,7,18);g.DrawLine(p,25,15,25,18);g.DrawLine(p,16,25,16,30);g.DrawLine(p,11,30,21,30);}
            else if(type=="speaker") {g.DrawLines(p,new PointF[]{new PointF(4,12),new PointF(10,12),new PointF(17,6),new PointF(17,27),new PointF(10,21),new PointF(4,21),new PointF(4,12)});g.DrawArc(p,13,7,14,19,-60,120);g.DrawArc(p,15,2,16,29,-60,120);}
            else if(type=="mixer") {for(int i=0;i<3;i++){float x=7+i*9;g.DrawLine(p,x,4,x,28);Fill(g,new RectangleF(x-3,8+i*5,6,5),1.5f,color);}}
            else if(type=="shield") {g.DrawLines(p,new PointF[]{new PointF(16,3),new PointF(27,7),new PointF(25,21),new PointF(16,29),new PointF(7,21),new PointF(5,7),new PointF(16,3)});g.DrawLines(p,new PointF[]{new PointF(11,15),new PointF(15,19),new PointF(22,12)});}
            else if(type=="search") {g.DrawEllipse(p,5,4,16,16);g.DrawLine(p,19,18,27,27);}
            else if(type=="refresh") {g.DrawArc(p,6,6,20,20,30,290);g.DrawLines(p,new PointF[]{new PointF(20,4),new PointF(26,6),new PointF(25,12)});}
            else if(type=="grid") {for(int y=0;y<2;y++)for(int x=0;x<2;x++)Stroke(g,new RectangleF(4+x*14,4+y*14,10,10),2,color);}
            else if(type=="gamepad") {g.DrawArc(p,3,9,26,19,180,180);g.DrawLines(p,new PointF[]{new PointF(3,18),new PointF(4,27),new PointF(10,25),new PointF(12,22),new PointF(20,22),new PointF(23,26),new PointF(28,26),new PointF(29,18)});g.DrawLine(p,8,16,15,16);g.DrawLine(p,11.5f,12.5f,11.5f,19.5f);g.DrawEllipse(p,21,13,1,1);g.DrawEllipse(p,25,17,1,1);}
            else if(type=="arrow") {g.DrawLine(p,6,16,26,16);g.DrawLine(p,20,10,26,16);g.DrawLine(p,20,22,26,16);}
            else if(type=="download") {g.DrawLine(p,16,3,16,22);g.DrawLines(p,new PointF[]{new PointF(9,15),new PointF(16,22),new PointF(23,15)});g.DrawLines(p,new PointF[]{new PointF(5,23),new PointF(5,29),new PointF(27,29),new PointF(27,23)});}
            else {Stroke(g,new RectangleF(5,5,22,22),5,color);g.DrawLine(p,11,16,21,16);g.DrawLine(p,16,11,16,21);}
        }
        g.Restore(state);
    }
}

internal sealed class CardButton : Button
{
    public Entry Entry;
    public float ScaleFactor;
    public Button UninstallButton;
    readonly ToolTip tip=new ToolTip {ShowAlways=true};
    public event EventHandler UninstallRequested;
    bool hover;
    public CardButton(Entry entry,float scale)
    {
        Entry=entry;ScaleFactor=scale;Text=entry.Name;AccessibleName=entry.Name;AccessibleDescription=entry.Detail;Cursor=entry.Available?Cursors.Hand:Cursors.Default;
        SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);
        FlatStyle=FlatStyle.Flat;TabStop=true;
        MouseEnter+=delegate{hover=true;Invalidate();};MouseLeave+=delegate{hover=false;Invalidate();};
        GotFocus+=delegate{Invalidate();};LostFocus+=delegate{Invalidate();};
        if(!entry.IsSetting&&!entry.IsDownload)
        {
            UninstallButton=new Button{Text=Ui.Text("卸载"),AccessibleName=Ui.Text("卸载")+" "+entry.Name,AccessibleDescription=entry.Removal==null?Ui.Text("未找到唯一且可用的卸载程序，暂不支持直接卸载。"):entry.Removal.Name,Enabled=entry.Removal!=null,FlatStyle=FlatStyle.Flat,BackColor=Color.FromArgb(43,51,64),ForeColor=Color.FromArgb(190,199,212),Font=new Font(Ui.FontFamily,11*scale,FontStyle.Regular,GraphicsUnit.Pixel),Cursor=Cursors.Hand,TabStop=true};
            UninstallButton.FlatAppearance.BorderSize=0;
            tip.SetToolTip(UninstallButton,entry.Removal==null?Ui.Text("未找到唯一且可用的卸载程序，暂不支持直接卸载。"):Ui.Text(String.IsNullOrEmpty(entry.Removal.PackageFamily)?"启动软件自身的卸载程序。":"为当前 Windows 账户卸载此应用。"));
            UninstallButton.Click+=delegate{if(UninstallRequested!=null)UninstallRequested(this,EventArgs.Empty);};
            Controls.Add(UninstallButton);
        }
    }
    protected override void OnResize(EventArgs e)
    {base.OnResize(e);if(UninstallButton!=null)UninstallButton.SetBounds(Width-(int)(104*ScaleFactor),(int)(122*ScaleFactor),(int)(84*ScaleFactor),(int)(25*ScaleFactor));}
    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g=e.Graphics;g.Clear(Art.Background);g.ScaleTransform(ScaleFactor,ScaleFactor);g.SmoothingMode=SmoothingMode.AntiAlias;
        float w=Width/ScaleFactor-1,h=Height/ScaleFactor-1;
        bool active=hover||Focused;
        Art.Fill(g,new RectangleF(0.5f,0.5f,w,h),16,active?Color.FromArgb(30,42,57):Art.Card);
        Art.Stroke(g,new RectangleF(0.5f,0.5f,w,h),16,active?Color.FromArgb(150,Entry.Accent):Color.FromArgb(40,52,69));
        Art.Fill(g,new RectangleF(18,18,54,54),13,Color.FromArgb(36,Entry.Accent));
        if(Entry.Image!=null){g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.DrawImage(Entry.Image,new RectangleF(26,26,38,38));}
        else Art.Glyph(g,Entry.Glyph,new RectangleF(30,30,30,30),Entry.Accent);
        Art.Label(g,Entry.Badge,84,22,10,Entry.Accent,true);
        using(Font font=new Font(Ui.FontFamily,16,FontStyle.Bold,GraphicsUnit.Pixel))
        using(SolidBrush b=new SolidBrush(Art.Text))
        using(StringFormat format=new StringFormat { Trimming=StringTrimming.EllipsisCharacter, FormatFlags=Ui.English?StringFormatFlags.LineLimit:StringFormatFlags.NoWrap })
            g.DrawString(Entry.Name,font,b,new RectangleF(83,41,w-99,Ui.English?43:25),format);
        using(Font font=new Font(Ui.FontFamily,12,FontStyle.Regular,GraphicsUnit.Pixel))
        using(SolidBrush brush=new SolidBrush(Art.Muted))
        using(StringFormat format=new StringFormat {Trimming=StringTrimming.EllipsisCharacter,FormatFlags=StringFormatFlags.LineLimit})
            g.DrawString(Entry.Detail,font,brush,new RectangleF(18,89,w-36,30),format);
        if(Entry.IsDownload)
        {
            Art.Label(g,new Uri(Entry.DownloadUrl).Host,18,127,10,Art.Muted,false);
            using(SolidBrush dot=new SolidBrush(Art.Cyan))g.FillEllipse(dot,20,h-29,5,5);
            Art.Label(g,"官网来源",32,h-35,10,Art.Muted,false);
            Art.Label(g,"官网下载",w-(Ui.English?140:81),h-36,11,active?Entry.Accent:Art.Text,false);
            Art.Glyph(g,"arrow",new RectangleF(w-32,h-35,16,16),Entry.Accent);
            return;
        }
        using(SolidBrush dot=new SolidBrush(Entry.Available?Color.FromArgb(107,207,158):Color.FromArgb(104,112,126)))g.FillEllipse(dot,20,h-29,5,5);
        Art.Label(g,Entry.Available?(Entry.IsSetting?"系统设置":"已安装"):"未检测到",32,h-35,10,Art.Muted,false);
        Art.Label(g,Entry.Available?"打开":"检查",w-62,h-36,11,active?Entry.Accent:Color.FromArgb(187,201,219),false);
        Art.Glyph(g,"arrow",new RectangleF(w-32,h-35,16,16),active?Entry.Accent:Art.Muted);
    }
    protected override void Dispose(bool disposing){if(disposing)tip.Dispose();base.Dispose(disposing);}
}

internal sealed class NavButton : Button
{
    public string Caption,IconType;
    public bool Selected;
    public float ScaleFactor;
    bool hover;
    public NavButton(string caption,string icon,float scale)
    {
        Caption=caption;IconType=icon;ScaleFactor=scale;Text=Ui.Text(caption);Cursor=Cursors.Hand;
        SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);
        MouseEnter+=delegate{hover=true;Invalidate();};MouseLeave+=delegate{hover=false;Invalidate();};
        GotFocus+=delegate{Invalidate();};LostFocus+=delegate{Invalidate();};
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g=e.Graphics;g.Clear(Color.FromArgb(16,23,33));g.ScaleTransform(ScaleFactor,ScaleFactor);g.SmoothingMode=SmoothingMode.AntiAlias;
        float w=Width/ScaleFactor,h=Height/ScaleFactor;
        if(Selected||hover||Focused)Art.Fill(g,new RectangleF(0,0,w-1,h-1),10,Selected?Color.FromArgb(27,55,57):Color.FromArgb(27,36,49));
        Color color=Selected?Art.Cyan:Art.Muted;
        Art.Glyph(g,IconType,new RectangleF(16,(h-21)/2,21,21),color);
        Art.Label(g,Caption,49,(h-17)/2,13,color,Selected);
    }
}

internal sealed class HubForm : Form
{
    [DllImport("user32.dll",CharSet=CharSet.Unicode)] static extern IntPtr SendMessage(IntPtr handle,int message,IntPtr wParam,string lParam);
    [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr handle,int attribute,ref int value,int size);
    public List<Entry> Entries;
    public List<Entry> StoreEntries;
    public List<CardButton> Cards=new List<CardButton>();
    public Func<Entry,bool> ConfirmRemoval;
    public Func<Entry,RemovalPlan> ResolveRemoval=AppManagement.ResolveCurrent;
    readonly float scale;
    readonly TextBox search;
    readonly Panel viewport;
    readonly List<NavButton> navs=new List<NavButton>();
    readonly Action<ProcessStartInfo> dispatch;
    static readonly string DefaultNotice=Ui.Text("启动自动扫描 · 每 60 秒检查新软件");
    string category="all", notice=DefaultNotice;
    readonly Timer noticeTimer=new Timer();
    readonly Timer scanTimer=new Timer();
    bool scanning;
    NavButton refresh,addSoftware;
    int sidebar=190;
    public HubForm(Action<ProcessStartInfo> launch)
    {
        dispatch=launch;
        ConfirmRemoval=delegate(Entry entry){using(UninstallDialog dialog=new UninstallDialog(entry))return dialog.ShowDialog(this)==DialogResult.OK;};
        using(Graphics g=CreateGraphics())scale=g.DpiX/96f;
        AutoScaleMode=AutoScaleMode.None;
        BackColor=Art.Background;ForeColor=Art.Text;
        Text=Ui.Text("外设AUTO控制中心");StartPosition=FormStartPosition.CenterScreen;
        ClientSize=new Size(S(1120),Math.Min(S(820),Screen.PrimaryScreen.WorkingArea.Height-S(60)));
        MinimumSize=new Size(S(1020),S(650));
        Font=new Font(Ui.FontFamily,10);
        Icon=Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        DoubleBuffered=true;
        search=new TextBox { BorderStyle=BorderStyle.None, BackColor=Color.FromArgb(23,31,44), ForeColor=Art.Text, Font=new Font(Ui.FontFamily,10.5f), AccessibleName=Ui.Text("搜索软件或设置") };
        search.TextChanged+=delegate{RenderCards();Invalidate();};
        search.HandleCreated+=delegate{SendMessage(search.Handle,0x1501,new IntPtr(1),Ui.Text("搜索品牌、软件或设置…"));};
        Controls.Add(search);
        viewport=new Panel { BackColor=Art.Background, AutoScroll=true, AccessibleName=Ui.Text("快捷入口列表") };
        Controls.Add(viewport);
        string[] captions={"全部入口","外设驱动","Windows 声音","更多工具","官网驱动商店"};
        string[] icons={"grid","gamepad","headphone","app","download"};
        string[] categories={"all","peripheral","sound","tools","store"};
        for(int i=0;i<captions.Length;i++)
        {
            NavButton nav=new NavButton(captions[i],icons[i],scale);nav.Selected=i==0;nav.Tag=categories[i];
            nav.Click+=delegate(object sender,EventArgs args){Filter(search.Text,(string)((Control)sender).Tag);};
            navs.Add(nav);Controls.Add(nav);
        }
        refresh=new NavButton("刷新应用","refresh",scale);
        refresh.Click+=delegate{Scan(true);};
        Controls.Add(refresh);
        addSoftware=new NavButton("添加软件","app",scale);
        addSoftware.Click+=delegate
        {
            using(OpenFileDialog dialog=new OpenFileDialog {Title=Ui.Text("选择外设管理软件或快捷方式"),Filter=Ui.Text("应用或快捷方式 (*.exe;*.lnk)|*.exe;*.lnk"),CheckFileExists=true})
            {
                if(dialog.ShowDialog(this)!=DialogResult.OK)return;
                try{Discovery.AddManual(Discovery.FromFile(dialog.FileName));Scan(true);}
                catch(Exception ex){MessageBox.Show(this,ex.Message,Ui.Text("添加失败"),MessageBoxButtons.OK,MessageBoxIcon.Information);}
            }
        };
        Controls.Add(addSoftware);
        noticeTimer.Interval=4500;noticeTimer.Tick+=delegate{noticeTimer.Stop();notice=DefaultNotice;Invalidate();};
        scanTimer.Interval=60000;scanTimer.Tick+=delegate{Scan(false);};scanTimer.Start();
        Entries=Catalog.Load();
        StoreEntries=DriverStore.Load(Entries);
        Resize+=delegate{Arrange();};
        Arrange();
    }
    int S(float value){return (int)Math.Round(value*scale);}
    void Arrange()
    {
        if(search==null||viewport==null||refresh==null||addSoftware==null)return;
        float w=ClientSize.Width/scale,h=ClientSize.Height/scale;
        search.SetBounds(S(sidebar+62),S(190),S(w-sidebar-110),S(27));
        viewport.SetBounds(S(sidebar+28),S(268),S(w-sidebar-52),S(h-320));
        for(int i=0;i<navs.Count;i++)navs[i].SetBounds(S(14),S(190+i*52),S(sidebar-28),S(44));
        refresh.SetBounds(S(14),S(h-122),S(sidebar-28),S(44));
        addSoftware.SetBounds(S(14),S(h-174),S(sidebar-28),S(44));
        RenderCards();Invalidate();
    }
    public void Filter(string query,string kind)
    {category=kind;foreach(NavButton nav in navs){nav.Selected=(string)nav.Tag==kind;nav.Invalidate();}search.Text=query;if(search.IsHandleCreated)SendMessage(search.Handle,0x1501,new IntPtr(1),Ui.Text(kind=="store"?"搜索品牌或驱动软件…":"搜索品牌、软件或设置…"));RenderCards();}
    public void ResetNotice(){noticeTimer.Stop();notice=DefaultNotice;Invalidate();}
    static string Signature(List<Entry> entries)
    {return String.Join("\n",entries.Select(x=>Discovery.Key(x)+"|"+x.Name+"|"+x.IconPath+"|"+(x.Removal==null?"":x.Removal.Identity)).OrderBy(x=>x));}
    public bool IsScanning {get{return scanning;}}
    public string ScanError;
    public void Scan(bool requested)
    {
        if(scanning||IsDisposed)return;
        scanning=true;refresh.Enabled=false;
        if(requested){noticeTimer.Stop();notice="正在扫描新软件与图标…";Invalidate();}
        // Windows shortcut and AppsFolder providers are apartment-bound shell COM objects.
        // Keep their scan on the existing UI STA, scheduled after the button repaints.
        BeginInvoke((Action)delegate
        {
            List<Entry> fresh=null;Exception error=null;
            try{fresh=Catalog.Load();}catch(Exception ex){error=ex;}
            if(IsDisposed||Disposing){DisposeEntries(fresh);return;}
            try
            {
                ((Action)delegate
                {
                    scanning=false;refresh.Enabled=true;
                    if(error!=null){ScanError=error.ToString();if(requested){notice="扫描未完成，可稍后重试";noticeTimer.Start();Invalidate();}return;}
                    List<Entry> old=Entries;
                    int added=fresh.Count(x=>!x.IsSetting&&!old.Any(y=>String.Equals(Discovery.Key(x),Discovery.Key(y),StringComparison.OrdinalIgnoreCase)));
                    bool changed=Signature(old)!=Signature(fresh);
                    if(changed||requested)
                    {
                        Point scroll=viewport.AutoScrollPosition;
                        DisposeEntries(StoreEntries);StoreEntries=DriverStore.Load(fresh);
                        Entries=fresh;RenderCards();
                        viewport.AutoScrollPosition=new Point(-scroll.X,-scroll.Y);
                        DisposeEntries(old);
                    }
                    else DisposeEntries(fresh);
                    if(requested||added>0){notice=added>0?"发现 "+added+" 个新软件，已自动添加入口与图标":"扫描完成，入口和图标已更新";noticeTimer.Start();Invalidate();}
                })();
            }
            catch(InvalidOperationException){DisposeEntries(fresh);}
        });
    }
    protected override void OnHandleCreated(EventArgs e)
    {base.OnHandleCreated(e);try{int enabled=1;DwmSetWindowAttribute(Handle,20,ref enabled,4);}catch{}}
    public void RenderCards()
    {
        if(Entries==null||viewport==null)return;
        viewport.SuspendLayout();
        foreach(CardButton card in Cards){viewport.Controls.Remove(card);card.Dispose();}Cards.Clear();
        string query=search.Text.Trim();
        List<Entry> source=category=="store"?StoreEntries:Entries;
        List<Entry> visible=source.Where(delegate(Entry e){return(category=="all"||e.Kind==category)&&(query.Length==0||(e.Name+" "+e.Detail+" "+e.Badge+" "+e.SearchTerms).IndexOf(query,StringComparison.OrdinalIgnoreCase)>=0);}).ToList();
        viewport.AutoScrollPosition=Point.Empty;
        float width=viewport.ClientSize.Width/scale-20;
        int cols=width>=800?3:2;
        float gap=14,cardWidth=(width-gap*(cols-1))/cols;
        for(int i=0;i<visible.Count;i++)
        {
            CardButton card=new CardButton(visible[i],scale);
            card.SetBounds(S((i%cols)*(cardWidth+gap)),S((i/cols)*208),S(cardWidth),S(194));
            card.Click+=delegate(object sender,EventArgs args){Open(((CardButton)sender).Entry);};
            card.UninstallRequested+=delegate(object sender,EventArgs args){Uninstall(((CardButton)sender).Entry);};
            Cards.Add(card);viewport.Controls.Add(card);
        }
        viewport.AutoScrollMinSize=new Size(0,S(((visible.Count+cols-1)/cols)*208));
        viewport.ResumeLayout();Invalidate();
    }
    void Open(Entry entry)
    {
        if(entry.IsDownload)
        {
            try{dispatch(DriverStore.Command(entry));notice="已打开官网，请按设备型号选择下载。";noticeTimer.Start();Invalidate();}
            catch(Exception ex){MessageBox.Show(this,ex.Message,Ui.Text("打开失败"),MessageBoxButtons.OK,MessageBoxIcon.Warning);}
            return;
        }
        if(!entry.Available){MessageBox.Show(this,Ui.Text("没有找到此应用。请确认它仍已安装，然后点击左下角“刷新应用”。"),entry.Name,MessageBoxButtons.OK,MessageBoxIcon.Information);return;}
        try {dispatch(entry.Command());notice="已发送打开请求："+entry.Name;noticeTimer.Start();Invalidate();}
        catch(Exception ex){MessageBox.Show(this,Ui.Text("无法打开“"+entry.Name+"”。\n\n"+ex.Message),Ui.Text("打开失败"),MessageBoxButtons.OK,MessageBoxIcon.Warning);}
    }
    public void Uninstall(Entry entry)
    {
        if(entry.IsSetting||entry.IsDownload)return;
        try
        {
            RemovalPlan current=ResolveRemoval(entry);
            if(current==null){MessageBox.Show(this,Ui.Text("未找到唯一且可用的卸载程序，暂不支持直接卸载。"),entry.Name,MessageBoxButtons.OK,MessageBoxIcon.Information);return;}
            entry.Removal=current;
            if(!ConfirmRemoval(entry))return;
            dispatch(current.Command());notice="卸载流程已启动，请按提示完成；随后可刷新列表。";noticeTimer.Start();Invalidate();
        }
        catch(System.ComponentModel.Win32Exception ex)
        {
            if(ex.NativeErrorCode==1223){notice="已取消卸载授权，软件未由本工具卸载。";noticeTimer.Start();Invalidate();}
            else MessageBox.Show(this,ex.Message,Ui.Text("卸载未启动"),MessageBoxButtons.OK,MessageBoxIcon.Warning);
        }
        catch(Exception ex){MessageBox.Show(this,ex.Message,Ui.Text("卸载未启动"),MessageBoxButtons.OK,MessageBoxIcon.Warning);}
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);Graphics g=e.Graphics;g.ScaleTransform(scale,scale);g.SmoothingMode=SmoothingMode.AntiAlias;
        float w=ClientSize.Width/scale,h=ClientSize.Height/scale;
        using(SolidBrush b=new SolidBrush(Color.FromArgb(16,23,33)))g.FillRectangle(b,0,0,sidebar,h);
        using(Pen line=new Pen(Color.FromArgb(30,41,57)))g.DrawLine(line,sidebar,0,sidebar,h);
        Art.Fill(g,new RectangleF(24,31,40,40),12,Color.FromArgb(34,70,70));
        Art.Glyph(g,"grid",new RectangleF(33,40,22,22),Art.Cyan);
        Art.Label(g,"DEVICE HUB",76,34,12,Art.Text,true);Art.Label(g,"外设控制中心",76,54,11,Art.Muted,false);
        Art.Label(g,"工作台",26,146,10,Color.FromArgb(99,118,143),false);
        Art.Label(g,category=="store"?"官网下载，清楚直达。":"你的设备，一处直达。",sidebar+30,35,28,Art.Text,true);
        Art.Signature(g,sidebar+34,73);
        Art.Label(g,category=="store"?"搜索已收录的软件，前往品牌官网选择下载。":"自动发现外设软件，新安装也能一键直达。",sidebar+31,102,13,Art.Muted,false);
        Art.Fill(g,new RectangleF(sidebar+30,130,200,30),15,Color.FromArgb(23,46,43));
        using(SolidBrush dot=new SolidBrush(Art.Cyan))g.FillEllipse(dot,sidebar+44,142,6,6);
        int count=Entries==null?0:Entries.Count(x=>!x.IsSetting&&x.Available);
        Art.Label(g,category=="store"?(Ui.English?StoreEntries.Count+" official download pages":StoreEntries.Count+" 个官网下载入口"):count+" 个本机应用 · 5 个系统入口",sidebar+58,136,11,Color.FromArgb(146,215,195),false);
        Art.Fill(g,new RectangleF(sidebar+30,178,w-sidebar-60,48),12,Color.FromArgb(23,31,44));
        Art.Stroke(g,new RectangleF(sidebar+30,178,w-sidebar-60,48),12,Color.FromArgb(43,55,72));
        Art.Glyph(g,"search",new RectangleF(sidebar+43,192,19,19),Art.Muted);
        string heading=category=="store"?"官网驱动商店":category=="peripheral"?"外设驱动":category=="sound"?"Windows 声音":category=="tools"?"更多工具":"全部入口";
        Art.Label(g,heading,sidebar+30,242,13,Art.Text,true);
        Art.Label(g,Cards.Count+" 项",w-72,243,11,Art.Muted,false);
        if(Cards.Count==0){Art.Label(g,"没有找到匹配的入口",sidebar+80,335,20,Art.Text,true);Art.Label(g,"试试品牌名称，或清空搜索。",sidebar+80,372,13,Art.Muted,false);}
        using(Pen line=new Pen(Color.FromArgb(30,41,57)))g.DrawLine(line,sidebar+30,h-41,w-30,h-41);
        Art.Label(g,category=="store"&&notice==DefaultNotice?"只打开官网页面，由你选择下载和安装。":notice,sidebar+31,h-28,11,Art.Muted,false);
        Art.Label(g,"本机快捷面板",24,h-52,11,Art.Text,false);Art.Label(g,"PERIPHERALS & AUDIO",24,h-31,8,Color.FromArgb(89,109,135),false);
    }
    static void DisposeEntries(List<Entry> entries){if(entries!=null)foreach(Entry e in entries)if(e.Image!=null)e.Image.Dispose();}
    protected override void Dispose(bool disposing){if(disposing){scanTimer.Dispose();noticeTimer.Dispose();DisposeEntries(StoreEntries);DisposeEntries(Entries);}base.Dispose(disposing);}
}

internal static class Program
{
    [STAThread]
    static int Main(string[] args)
    {
        Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);
        if(args.Length==2&&args[0]=="--verify")return Verify(args[1]);
        Application.Run(new HubForm(delegate(ProcessStartInfo info){using(Process process=Process.Start(info)){} }));
        return 0;
    }
    static int Verify(string folder)
    {
        try
        {
            Directory.CreateDirectory(folder);
            Discovery.TraceFile=Path.Combine(folder,"scan-trace.txt");
            Discovery.Verify(folder);
            AppManagement.Verify();DriverStore.Verify();
            List<ProcessStartInfo> commands=new List<ProcessStartInfo>();
            bool simulateUacCancel=false;
            StringBuilder report=new StringBuilder();
            using(HubForm form=new HubForm(delegate(ProcessStartInfo info){if(simulateUacCancel)throw new System.ComponentModel.Win32Exception(1223);commands.Add(info);}))
            {
                form.Show();Application.DoEvents();
                if(Ui.English)
                {
                    if(form.Text!="Peripheral AUTO Control Center")throw new Exception("English window title missing.");
                    foreach(Entry entry in form.Entries)
                        if(System.Text.RegularExpressions.Regex.IsMatch(entry.Name+entry.Detail+entry.Badge,@"\p{IsCJKUnifiedIdeographs}"))throw new Exception("Untranslated catalog entry: "+entry.Name);
                    if(form.Controls.OfType<TextBox>().Single().AccessibleName!="Search apps or settings")throw new Exception("English search accessibility label missing.");
                    if(!form.Controls.OfType<NavButton>().Any(x=>x.Text=="Add App"))throw new Exception("English navigation missing.");
                }
                if(form.Entries.Count<16)throw new Exception("Previously installed applications were lost.");
                foreach(CardButton card in form.Cards)
                {
                    Entry entry=card.Entry;
                    report.AppendLine(entry.Name+" | available="+entry.Available+" | icon="+(entry.Image!=null)+" | "+entry.Command().FileName+" "+entry.Command().Arguments);
                    if(!entry.Available)throw new Exception("Missing installed app: "+entry.Name);
                    if(!entry.IsSetting&&entry.Image==null)throw new Exception("Missing original app icon: "+entry.Name);
                    int before=commands.Count;card.PerformClick();
                    if(commands.Count!=before+1)throw new Exception("Click did not dispatch: "+entry.Name);
                    ProcessStartInfo expected=entry.Command(),actual=commands[commands.Count-1];
                    if(actual.FileName!=expected.FileName||actual.Arguments!=expected.Arguments)throw new Exception("Launch command mismatch.");
                }
                if(commands.Single(x=>x.FileName=="ms-settings:sound-defaultinputproperties")==null)throw new Exception("Microphone route missing.");
                if(commands.Single(x=>x.FileName=="ms-settings:sound-defaultoutputproperties")==null)throw new Exception("Headphone route missing.");
                report.AppendLine("\nUNINSTALL REGISTRATIONS (read only; no uninstaller executed)");
                foreach(Entry entry in form.Entries.Where(x=>!x.IsSetting))
                    report.AppendLine(entry.Name+" | "+(entry.Removal==null?"unavailable":entry.Removal.Name+" | "+(String.IsNullOrEmpty(entry.Removal.PackageFamily)?entry.Removal.FileName+" "+entry.Removal.Arguments:"current-user package: "+entry.Removal.PackageFamily)));
                foreach(CardButton card in form.Cards)
                {
                    if(card.Entry.IsSetting&&card.UninstallButton!=null)throw new Exception("Settings card has uninstall button.");
                    if(!card.Entry.IsSetting&&(card.UninstallButton==null||card.UninstallButton.Enabled!=(card.Entry.Removal!=null)))throw new Exception("Uninstall enabled state incorrect.");
                }
                CardButton removable=form.Cards.First(x=>x.Entry.Removal!=null&&String.IsNullOrEmpty(x.Entry.AppId));
                int uninstallCalls=commands.Count,confirmCalls=0;
                form.ConfirmRemoval=delegate{confirmCalls++;return false;};
                removable.UninstallButton.PerformClick();
                if(commands.Count!=uninstallCalls||confirmCalls!=1)throw new Exception("Cancelled uninstall dispatched a process.");
                form.ConfirmRemoval=delegate{return true;};
                removable.UninstallButton.PerformClick();
                if(commands.Count!=uninstallCalls+1||commands.Last().FileName!=removable.Entry.Removal.FileName||commands.Last().Arguments!=removable.Entry.Removal.Arguments)throw new Exception("Uninstall button did not route the registered remover.");
                if(commands.Last().Verb!="runas")throw new Exception("Machine-wide uninstaller did not request elevation.");
                int beforeUacCancel=commands.Count;simulateUacCancel=true;removable.UninstallButton.PerformClick();simulateUacCancel=false;
                if(commands.Count!=beforeUacCancel)throw new Exception("Cancelled UAC launched removal.");
                if(form.Cards.All(x=>x.Entry.Id!=removable.Entry.Id))throw new Exception("App card removed before uninstall completed.");
                Entry packaged=form.Entries.First(x=>!String.IsNullOrEmpty(x.AppId));
                int packageCalls=commands.Count;form.Uninstall(packaged);
                if(commands.Count!=packageCalls+1||!commands.Last().Arguments.Contains("-EncodedCommand"))throw new Exception("Store app current-user removal route missing.");
                if(commands.Last().Verb=="runas")throw new Exception("Current-user package removal requested system-wide elevation.");
                string encoded=commands.Last().Arguments.Split(new[]{"-EncodedCommand "},StringSplitOptions.None)[1];
                string packageScript=Encoding.Unicode.GetString(Convert.FromBase64String(encoded));
                if(packageScript.Contains("-AllUsers")||packageScript.Contains("Remove-AppxProvisionedPackage")||!packageScript.Contains("PackageFamilyName -eq '"+packaged.Removal.PackageFamily+"'"))throw new Exception("Package removal scope is incorrect.");
                File.WriteAllText(Path.Combine(folder,"package-removal-script.txt"),packageScript,Encoding.UTF8);
                int forbiddenCalls=commands.Count;form.Uninstall(form.Entries.First(x=>x.IsSetting));
                if(commands.Count!=forbiddenCalls)throw new Exception("Settings page could be uninstalled.");
                form.Filter("","store");form.ResetNotice();Application.DoEvents();
                if(form.Cards.Count!=8||form.Cards.Any(x=>!x.Entry.IsDownload||x.UninstallButton!=null))throw new Exception("Driver store category incorrect.");
                foreach(CardButton card in form.Cards)
                {
                    int before=commands.Count;card.PerformClick();
                    if(commands.Count!=before+1||commands.Last().FileName!=card.Entry.DownloadUrl||!commands.Last().UseShellExecute)throw new Exception("Official page click did not dispatch URL.");
                }
                form.ResetNotice();Save(form,Path.Combine(folder,"PeripheralHub-store.png"));
                form.Filter("罗技","store");if(form.Cards.Count!=1||form.Cards[0].Entry.Id!="store-logitech")throw new Exception("Bilingual store keyword search failed.");
                form.Filter("unlikely-product-zz","store");if(form.Cards.Count!=0)throw new Exception("Store empty search failed.");
                form.Filter("","all");
                form.Filter(Ui.English?"Razer":"雷蛇","all");if(!form.Cards.Any(x=>x.Entry.Id=="razer"))throw new Exception("Search failed.");
                form.Filter("","sound");if(form.Cards.Count!=5)throw new Exception("Sound filter failed.");
                form.Filter("没有这个品牌","all");if(form.Cards.Count!=0)throw new Exception("Empty search failed.");
                int beforeRefresh=form.Entries.Count;
                File.WriteAllText(Path.Combine(folder,"before-refresh.txt"),report.ToString(),Encoding.UTF8);
                RefreshAndWait(form,true);
                if(form.Entries.Count!=beforeRefresh)throw new Exception("Refresh duplicated entries.");
                // Simulate installation through an isolated shortcut root; no real Start Menu changes.
                Discovery.ExtraShortcutRoots=new string[]{folder};
                RefreshAndWait(form,false);
                Entry added=form.Entries.SingleOrDefault(x=>x.Name=="Nebula 无线鼠标控制台");
                if(form.Entries.Count!=beforeRefresh+1||added==null||added.Image==null||added.Command().Arguments!="--sample-value=\"a b\"")throw new Exception("New installation was not automatically added with its icon and original arguments.");
                form.Filter("Nebula","all");if(form.Cards.Count!=1)throw new Exception("New entry is not searchable.");
                int routed=commands.Count;form.Cards[0].PerformClick();
                if(commands.Count!=routed+1||commands.Last().Arguments!="--sample-value=\"a b\"")throw new Exception("Automatically added entry click failed.");
                Save(form,Path.Combine(folder,"PeripheralHub-new-install.png"));
                RefreshAndWait(form,false);if(form.Entries.Count!=beforeRefresh+1)throw new Exception("Periodic scan duplicated the new app.");
                Discovery.ExtraShortcutRoots=new string[0];RefreshAndWait(form,false);
                if(form.Entries.Count!=beforeRefresh||form.Entries.Any(x=>x.Name=="Nebula 无线鼠标控制台"))throw new Exception("Removed app entry was not removed.");
                form.Filter("","all");form.ResetNotice();Application.DoEvents();
                Save(form,Path.Combine(folder,"PeripheralHub-preview.png"));
                form.Filter("","sound");Application.DoEvents();Save(form,Path.Combine(folder,"PeripheralHub-sound.png"));
                form.Filter("","tools");Application.DoEvents();Save(form,Path.Combine(folder,"PeripheralHub-tools.png"));
                form.ClientSize=new Size(1020,650);form.Filter("","all");Application.DoEvents();
                if(form.Cards.Count<3||form.Cards[2].Top<=form.Cards[0].Top)throw new Exception("Compact window should use two columns.");
                if(form.Cards[1].Right>form.Cards[1].Parent.ClientSize.Width)throw new Exception("Card exceeds viewport.");
                Save(form,Path.Combine(folder,"PeripheralHub-compact.png"));
                form.Filter("","store");Application.DoEvents();Save(form,Path.Combine(folder,"PeripheralHub-store-compact.png"));
                NavButton storeNav=form.Controls.OfType<NavButton>().Single(x=>(string)x.Tag=="store");
                NavButton addNav=form.Controls.OfType<NavButton>().Single(x=>x.Text==Ui.Text("添加软件"));
                if(storeNav.Bottom>addNav.Top)throw new Exception("Sidebar buttons overlap in compact mode.");
                form.ClientSize=new Size(1120,820);form.Filter("","all");Application.DoEvents();
                Entry confirmEntry=form.Entries.First(x=>x.Id=="logitech");
                using(UninstallDialog dialog=new UninstallDialog(confirmEntry))
                {
                    dialog.Show(form);Application.DoEvents();Save(dialog,Path.Combine(folder,"PeripheralHub-uninstall-confirm.png"));dialog.Close();
                }
                form.Close();
            }
            report.AppendLine("PASS: installation of an unknown-brand app was simulated with an isolated real shortcut; periodic scan added its entry and icon; click preserved arguments; repeated scan did not duplicate; removing the source removed the entry. Custom entry persistence, service/installer filtering, existing apps, search, filters and compact layout also passed. No external apps were launched.");
            report.AppendLine("PASS: uninstall parsing, exact/ambiguous mapping, confirmation cancellation and registered dispatch, current-user package routing, no premature card removal; eight official HTTPS pages, bilingual store search and compact layouts. All commands captured by fake dispatcher; no real uninstall or download executed.");
            File.WriteAllText(Path.Combine(folder,"verification.txt"),report.ToString(),Encoding.UTF8);
            return 0;
        }
        catch(Exception ex){File.WriteAllText(Path.Combine(folder,"error.txt"),ex.ToString(),Encoding.UTF8);return 1;}
    }
    static void Save(Form form,string path)
    {using(Bitmap image=new Bitmap(form.Width,form.Height)){form.DrawToBitmap(image,new Rectangle(Point.Empty,image.Size));image.Save(path,System.Drawing.Imaging.ImageFormat.Png);}}
    static void RefreshAndWait(HubForm form,bool requested)
    {
        form.Scan(requested);Stopwatch wait=Stopwatch.StartNew();
        while(form.IsScanning&&wait.ElapsedMilliseconds<20000){Application.DoEvents();System.Threading.Thread.Sleep(20);}
        if(form.IsScanning||form.ScanError!=null)throw new Exception("Scan failed: "+form.ScanError);
    }
}

