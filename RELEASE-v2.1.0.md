# Peripheral AUTO Control Center | 外设AUTO控制中心 v2.1.0

**By Nonori · English & 简体中文**

Two interface languages, the same one-click access to peripheral apps and Windows audio settings.

中英文两个独立 EXE，功能一致。选择习惯的语言，下载后双击运行。

## Downloads | 下载选择

| Asset | What to choose |
| --- | --- |
| **Peripheral-AUTO-Control-Center-EN.exe** | English interface |
| **Peripheral-AUTO-Control-Center-ZH-CN.exe** | 简体中文界面 |
| **Peripheral-AUTO-Control-Center-v2.1.0-bundle.zip** | Both EXEs, source, build script, guides, and screenshots / 完整包 |
| Source code (zip / tar.gz) | Repository snapshot for developers / 源码快照 |

Download **one of the EXEs** for normal use. No installer required. Intended for Windows 11 with .NET Framework 4.x. Vendor companion apps must already be installed.

普通玩家只需下载一个 EXE，无需下载源码或完整包。

## What's new | 本次更新

- Full English interface: navigation, card descriptions, search prompts, and application messages.
- Separate Chinese and English downloads with the same functionality.
- Longer English app titles wrap to fit the cards.
- A discreet calligraphic **designed by nonori** signature beneath the main headline in both editions.
- Expanded English overview, dedicated Chinese guide, quick-start steps, download choices, FAQ, and community feedback guidance.
- Updated source and build script that can produce both editions.

新增英文版、清晰的语言下载入口和更完整的中英文说明，并在标语下方加入低调的花体签名。

## Quick start | 快速上手

1. Run the EXE and let the initial scan finish.
2. Click an app or audio settings card.
3. After installing new software, use **Refresh Apps / 刷新应用**, or wait for the next automatic check while the launcher is open. Use **Add App / 添加软件** for an app that is missed.

## Before you use it | 使用说明

Automatic checks run at startup and every 60 seconds **while the window is open**. Detection depends on product metadata; not every application can be recognized. Both editions share the same saved custom-app list on the same Windows account.

Windows audio pages and vendor software keep their own language. This launcher does not install or update drivers, flash firmware, or automatically change your device settings.

自动扫描只在面板运行时进行；无法识别的软件可手动添加。耳机、麦克风入口跟随 Windows 默认设备，其他设备请从“声音与设备”选择。

## Verification | 验证情况

Both builds passed discovery of a simulated new app through a real shortcut, original-icon loading, launch-argument preservation, repeated-scan deduplication, missing-entry removal, custom-entry persistence, search, filters, and compact-layout checks. English labels and both interface layouts were also checked.

Automated checks did not launch external vendor apps and are not a guarantee of compatibility with every device.

For feedback, include the app name/version, Windows version, interface language, and the expected versus actual result in GitHub Issues.
