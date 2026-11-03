using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

internal static class Ui
{
#if ENGLISH
    public static readonly bool English=true;
    public const string FontFamily="Segoe UI";
#else
    public static readonly bool English=false;
    public const string FontFamily="Microsoft YaHei UI";
#endif
    static readonly Dictionary<string,string> Strings=new Dictionary<string,string>
    {
        {"官网驱动商店","Driver Store"},
        {"官网下载，清楚直达。","Official downloads. One place."},
        {"搜索已收录的软件，前往品牌官网选择下载。","Search the catalog and visit the manufacturer's download page."},
        {"只打开官网页面，由你选择下载和安装。","Opens official pages. You choose what to download and install."},
        {"搜索品牌或驱动软件…","Search brands or driver software..."},
        {"官网下载","Official website"},
        {"官网来源","Official source"},
        {"卸载","Uninstall"},
        {"卸载软件","Uninstall app"},
        {"卸载未启动","Could not start uninstall"},
        {"未找到唯一且可用的卸载程序，暂不支持直接卸载。","No unique, usable uninstaller was found. Direct uninstall is unavailable."},
        {"启动软件自身的卸载程序。","Start this application's own uninstaller."},
        {"为当前 Windows 账户卸载此应用。","Remove this app for the current Windows account."},
        {"卸载流程已启动，请按提示完成；随后可刷新列表。","Uninstall started. Follow its prompts, then refresh the list."},
        {"已取消卸载授权，软件未由本工具卸载。","Administrator approval was cancelled. Uninstall was not started."},
        {"已打开官网，请按设备型号选择下载。","Official page opened. Choose the download for your device."},
        {"返回上次分类时保留搜索内容。","Your search is retained when switching categories."},
        {"外设AUTO控制中心","Peripheral AUTO Control Center"},
        {"外设控制中心","by Nonori"},
        {"飞智空间站 4.0","Flydigi Space Station 4.0"},
        {"雷蛇 Synapse","Razer Synapse"},
        {"罗技 G HUB","Logitech G HUB"},
        {"迈从 MCHOSE HUB","MCHOSE HUB"},
        {"赛睿 SteelSeries GG","SteelSeries GG"},
        {"Realtek 音频控制台","Realtek Audio Console"},
        {"NVIDIA 控制面板","NVIDIA Control Panel"},
        {"手柄映射 · 摇杆校准 · 固件","Mapping · Calibration · Firmware"},
        {"按键绑定 · 灵敏度 · 幻彩灯效","Bindings · Sensitivity · Chroma"},
        {"鼠标 · 键盘 · 耳机配置","Mouse · Keyboard · Headset"},
        {"鼠标与键盘 · 性能 · 灯效","Mouse & keyboard · Performance"},
        {"外设配置 · Sonar 音频","Device settings · Sonar audio"},
        {"华硕 / ROG · 设备与灯效","ASUS / ROG · Devices · Lighting"},
        {"华硕 / ROG · 自定义灯光效果","ASUS / ROG · Custom lighting"},
        {"声卡 · 接口 · 音频效果","Sound card · Ports · Effects"},
        {"杜比全景声 · 耳机空间音效","Dolby Atmos · Spatial audio"},
        {"显卡设置 · 图形选项 · 游戏优化","Graphics · Game optimization"},
        {"显示器 · 分辨率 · 3D 设置","Display · Resolution · 3D"},
        {"耳机 / 扬声器","Headphones / Speakers"},
        {"默认输出 · 音量 · 音效","Default output · Volume · Effects"},
        {"麦克风设置","Microphone"},
        {"默认输入 · 音量 · 麦克风测试","Default input · Volume · Test"},
        {"声音与设备","Sound & Devices"},
        {"选择输出 / 输入 · 管理音频设备","Output / Input · Audio devices"},
        {"应用音量混合器","Volume Mixer"},
        {"分别调整每个应用的音量","Adjust volume for each app"},
        {"麦克风权限","Microphone Privacy"},
        {"管理应用对麦克风的访问","Manage microphone access"},
        {"全部入口","All Shortcuts"},
        {"外设驱动","Peripheral Apps"},
        {"Windows 声音","Windows Audio"},
        {"更多工具","More Tools"},
        {"刷新应用","Refresh Apps"},
        {"添加软件","Add App"},
        {"系统设置","Windows settings"},
        {"已安装","Installed"},
        {"未检测到","Not found"},
        {"打开","Open"},
        {"检查","Check"},
        {"工作台","WORKSPACE"},
        {"你的设备，一处直达。","Your devices. One place."},
        {"自动发现外设软件，新安装也能一键直达。","Discover device apps automatically. Open them in one click."},
        {"启动自动扫描 · 每 60 秒检查新软件","Auto-scan on startup and every 60 seconds"},
        {"搜索软件或设置","Search apps or settings"},
        {"搜索品牌、软件或设置…","Search brands, apps, or settings..."},
        {"快捷入口列表","App and settings shortcuts"},
        {"选择外设管理软件或快捷方式","Select a device app or shortcut"},
        {"应用或快捷方式 (*.exe;*.lnk)|*.exe;*.lnk","Apps or shortcuts (*.exe;*.lnk)|*.exe;*.lnk"},
        {"添加失败","Could not add app"},
        {"正在扫描新软件与图标…","Scanning for new apps and icons..."},
        {"扫描未完成，可稍后重试","Scan incomplete. Please try again."},
        {"扫描完成，入口和图标已更新","Scan complete. Apps and icons are up to date."},
        {"没有找到此应用。请确认它仍已安装，然后点击左下角“刷新应用”。","This app could not be found. Make sure it is still installed, then click Refresh Apps."},
        {"打开失败","Could not open app"},
        {"没有找到匹配的入口","No matching shortcuts"},
        {"试试品牌名称，或清空搜索。","Try a brand name, or clear your search."},
        {"本机快捷面板","LOCAL DEVICE HUB"},
        {"手动添加","CUSTOM APP"},
        {"自动发现","DISCOVERED"},
        {"设备管理软件 · 一键打开","Device companion app · Click to open"},
        {"请选择可运行的管理软件 EXE，或指向该软件的快捷方式。","Select a runnable companion app EXE or a shortcut to that app."}
    };
    public static string Text(string value)
    {
        if(!English||String.IsNullOrEmpty(value))return value;
        string result;if(Strings.TryGetValue(value,out result))return result;
        Match count=Regex.Match(value,@"^(\d+) 个本机应用 · 5 个系统入口$");
        if(count.Success)return count.Groups[1].Value+" apps · 5 Windows shortcuts";
        count=Regex.Match(value,@"^(\d+) 项$");if(count.Success)return count.Groups[1].Value+" items";
        count=Regex.Match(value,@"^发现 (\d+) 个新软件，已自动添加入口与图标$");
        if(count.Success)return "Found "+count.Groups[1].Value+" new apps. Shortcuts and icons added.";
        const string launch="已发送打开请求：";
        if(value.StartsWith(launch,StringComparison.Ordinal))return "Opening: "+Text(value.Substring(launch.Length));
        const string error="无法打开“";
        if(value.StartsWith(error,StringComparison.Ordinal))return "Could not open "+value.Substring(error.Length).Replace("”。\n\n",".\n\n");
        return value;
    }
    public static void Localize(Entry entry)
    {
        if(entry.SourceName==null)entry.SourceName=entry.Name;
        entry.Name=Text(entry.Name);entry.Detail=Text(entry.Detail);entry.Badge=Text(entry.Badge);
    }
}

