# Official download pages | 官网链接记录

Checked / 核查日期: **2026-11-02**. These are manufacturer landing pages, not mirrored installers. Links can change after this date. The app opens them in the default browser and does not run downloads automatically.

以下链接均为品牌官网页面。下载时由用户选择对应型号、系统和软件版本；后续网页地址或内容可能变化。

| Catalog entry | Official source |
| --- | --- |
| Flydigi Space Station / 飞智空间站 | [Flydigi Software Downloads](https://shops.flydigi.com/pages/game-center) |
| Razer Synapse 4 | [Razer Synapse 4](https://www.razer.com/synapse-4) |
| Logitech G HUB / 罗技 G HUB | [Logitech G HUB](https://www.logitechg.com/en-us/software/ghub) |
| MCHOSE M HUB / 迈从 | [MCHOSE HUB](https://www.mchose.store/pages/mchose-hub) |
| SteelSeries GG / 赛睿 | [SteelSeries GG](https://steelseries.com/gg) |
| Armoury Crate | [ASUS Armoury Crate downloads](https://www.asus.com/supportonly/armoury_crate/helpdesk_download/) |
| Aura Creator | [ASUS shared Armoury Crate & Aura Creator installer](https://www.asus.com/supportonly/armoury_crate/helpdesk_download/) |
| NVIDIA App | [NVIDIA App](https://www.nvidia.com/en-us/software/nvidia-app/) |

Flydigi's old `www.flydigi.com/index/down` link redirected to its official store during the check. Its Software navigation leads to the included download page. The catalog therefore uses the observed destination and does not pin a Space Station version; players should select the package supported by their controller.

飞智旧下载地址在本次核查中跳转至官方商店，因此使用其 Software 导航里的软件下载页。目录不固定空间站版本，避免将不兼容版本推荐给不同型号设备。

## Implementation references | 技术参考

- [Microsoft Windows Installer command-line options](https://learn.microsoft.com/en-us/windows/win32/msi/standard-installer-command-line-options): `/x` is the product-uninstall operation; the app uses an interactive uninstall with `/norestart` for MSI products.
- [Microsoft Remove-AppxPackage](https://learn.microsoft.com/en-us/powershell/module/appx/remove-appxpackage): Windows package removal. This app targets one exact package family for the current user, not all accounts.

Local verification captures process commands without executing uninstallers. These references do not establish that every vendor uninstaller completes successfully.
