# Peripheral AUTO Control Center | 外设AUTO控制中心

**By Nonori · Windows desktop utility · v2.0.0**

A single place to launch peripheral companion apps and Windows audio settings. Discover newly installed device software, keep its original icon, and open it with one click.

外设软件与声音设置的一站式快捷面板，支持自动扫描和手动添加。

![Peripheral AUTO Control Center](preview.png)

## Download

Download **外设AUTO控制中心.exe** from [Releases](../../releases/latest), then double-click to run. A copy is also available in the repository root.

No installer required. Intended for Windows 11 with the built-in .NET Framework 4.x. Install your device vendor's companion app separately; this utility opens apps and settings already on your PC.

下载 EXE 后直接运行，无需安装本工具。

## Features

- **Automatic discovery:** scans at startup and every 60 seconds while the window is open. Use **刷新应用** to scan immediately.
- **Original app icons:** reads icons from installed applications and shortcuts.
- **One-click launch:** preserves shortcut launch arguments for apps that require them.
- **Broad discovery:** checks Start Menu shortcuts, desktop shortcuts, installed-program records, and the Windows application list.
- **Manual additions:** use **添加软件** to add an EXE or shortcut that was not detected automatically.
- **Audio shortcuts:** default headphones / speakers, microphone, sound devices, app volume mixer, and microphone permissions.
- **Search and categories:** dark card interface with layouts that adapt to the window width.
- **Deduplication:** merges repeated entries and removes apps that can no longer be found.

## Discovery scope

Detection uses brand names, application names, publisher information, and product descriptions. Common peripheral brands and software explicitly described as mouse, keyboard, gamepad, headset, microphone, audio-interface, or similar device utilities can be detected.

An app with no useful device-related metadata may not be recognized. Unregistered portable apps without shortcuts may also be missed. Add these manually. Drivers without a separate management application do not appear as launchable apps.

Scanning stays on your PC; the application does not upload the discovered app list. Custom entries are stored in `%LOCALAPPDATA%\PeripheralHub\custom-apps.xml`. A generic icon is used if the original icon cannot be read.

少数未识别的软件可手动补充；没有独立管理界面的底层驱动不会显示为应用入口。

## Build from source

On Windows, run the following in PowerShell:

```powershell
.\build.ps1
```

The output is `dist\外设AUTO控制中心.exe`. The script uses the Windows .NET Framework C# compiler; no NuGet downloads are required.

## Project files

| File | Purpose |
| --- | --- |
| `PeripheralHub.cs` | Desktop interface, launch actions, audio settings, and baseline entries |
| `PeripheralDiscovery.cs` | Discovery, filtering, deduplication, and custom entries |
| `PeripheralHub.manifest` | Windows application manifest |
| `PeripheralHub.ico` | Application icon |
| `build.ps1` | Local build script |

This project launches companion apps and opens settings pages. It does not install drivers, update firmware, or change device configuration automatically.

Brand names and icons belong to their respective owners. This project is independent and is not affiliated with device manufacturers.

**Creator / 发布者: Nonori**
