using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

// Curated manufacturer pages, checked 2026-11-02. No installer hosting or background downloads.
internal static class DriverStore
{
    public const string CheckedDate="2026-11-02";
    public static readonly string[] Hosts={"shops.flydigi.com","www.razer.com","www.logitechg.com","www.mchose.store","steelseries.com","www.asus.com","www.nvidia.com"};
    static Entry Item(string id,string name,string brand,string detail,string url,string terms,string color,List<Entry> installed)
    {
        Entry local=installed.FirstOrDefault(x=>x.Id==id);
        return new Entry{Id="store-"+id,Name=name,SourceName=name,Badge=brand,Detail=detail,Kind="store",Glyph="download",Accent=ColorTranslator.FromHtml(color),Available=true,DownloadUrl=url,SearchTerms=terms,Image=local!=null&&local.Image!=null?new Bitmap(local.Image):null};
    }
    public static List<Entry> Load(List<Entry> installed)
    {
        List<Entry> items=new List<Entry>{
            Item("flydigi",Ui.English?"Flydigi Space Station":"飞智空间站","FLYDIGI",Ui.English?"Gamepad mapping and calibration":"手柄映射 · 摇杆校准 · 固件","https://shops.flydigi.com/pages/game-center","飞智 飞驰 手柄 flydigi controller gamepad space station","#44BBFF",installed),
            Item("razer","Razer Synapse 4","RAZER",Ui.English?"Device settings and Chroma lighting":"雷蛇外设设置 · 按键绑定 · 幻彩灯效","https://www.razer.com/synapse-4","雷蛇 razer synapse mouse keyboard headset","#84DE4D",installed),
            Item("logitech",Ui.English?"Logitech G HUB":"罗技 G HUB","LOGITECH G",Ui.English?"Mouse, keyboard and headset settings":"鼠标 · 键盘 · 耳机配置","https://www.logitechg.com/en-us/software/ghub","罗技 logitech logi g hub 鼠标 键盘 耳机","#4ED5F2",installed),
            Item("mchose","MCHOSE M HUB","MCHOSE",Ui.English?"Software for supported MCHOSE devices":"迈从外设管理 · 按型号选择客户端下载","https://www.mchose.store/pages/mchose-hub","迈从 mchose m hub mouse keyboard headset","#FFA85B",installed),
            Item("steelseries","SteelSeries GG","STEELSERIES",Ui.English?"Engine device settings and Sonar audio":"赛睿外设配置 · Engine · Sonar 音频","https://steelseries.com/gg","赛睿 steelseries gg engine sonar","#F5A477",installed),
            Item("armoury","Armoury Crate","ROG / ASUS",Ui.English?"ASUS / ROG device and lighting software":"华硕 / ROG · 设备管理与灯效","https://www.asus.com/supportonly/armoury_crate/helpdesk_download/","华硕 奥创 rog asus armoury crate","#FF6686",installed),
            Item("aura","Aura Creator","AURA / ASUS",Ui.English?"Custom lighting; shared ASUS installer":"自定义灯效 · 与 Armoury Crate 共用安装页","https://www.asus.com/supportonly/armoury_crate/helpdesk_download/","华硕 rog asus aura creator 灯光 灯效","#BA9BFF",installed),
            Item("nvidia","NVIDIA App","NVIDIA",Ui.English?"Graphics drivers and game settings":"显卡驱动下载 · 游戏与图形设置","https://www.nvidia.com/en-us/software/nvidia-app/","英伟达 nvidia app 显卡 graphics gpu","#A5D751",installed)
        };
        foreach(Entry entry in items)Validate(entry.DownloadUrl);
        return items;
    }
    public static void Validate(string value)
    {
        Uri uri;
        if(!Uri.TryCreate(value,UriKind.Absolute,out uri)||uri.Scheme!=Uri.UriSchemeHttps||!String.IsNullOrEmpty(uri.UserInfo)||!uri.IsDefaultPort||!Hosts.Contains(uri.Host,StringComparer.OrdinalIgnoreCase))throw new InvalidOperationException("Invalid official download page.");
    }
    public static ProcessStartInfo Command(Entry entry)
    {Validate(entry.DownloadUrl);return new ProcessStartInfo(entry.DownloadUrl){UseShellExecute=true};}
    public static void Verify()
    {
        List<Entry> items=Load(new List<Entry>());
        if(items.Count!=8)throw new Exception("Driver store catalog incomplete.");
        foreach(Entry item in items)if(!item.IsDownload||Command(item).FileName!=item.DownloadUrl)throw new Exception("Store command route invalid.");
        foreach(string invalid in new[]{"http://www.razer.com/synapse-4","https://www.razer.com.example.org/","file:///C:/setup.exe","https://example.com/","https://name@www.razer.com/"})
        {
            bool rejected=false;try{Validate(invalid);}catch(InvalidOperationException){rejected=true;}
            if(!rejected)throw new Exception("Unapproved download URL accepted.");
        }
    }
}
