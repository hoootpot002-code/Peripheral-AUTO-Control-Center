# Changelog | 更新记录

## v2.2.0

- Added a small gray Uninstall action above Open, with a named confirmation dialog and cancellation by default.
- Match desktop apps to registered uninstallers; preserve vendor arguments and use Windows Installer removal for MSI products.
- Remove supported Windows app packages for the current account through Windows package management; do not remove provisioned apps or dependency packages.
- Disable uncertain or missing uninstall actions; leave Windows settings cards unchanged.
- Added the Driver Store sidebar with eight curated manufacturer download pages, displayed domains, and Chinese/English keyword search.
- Preserved both editions, automatic discovery, audio shortcuts, original icons, and the gold signature.
- Added bilingual usage notes, source-link records, preview images, and local validation records.

新增实际卸载入口和官网驱动商店；卸载前确认具体软件，官网页面由浏览器打开。中英文 EXE 与更新后的源码、说明一并提供。

## v2.1.0

- Added a full English interface alongside the Chinese edition.
- Adjusted long English card titles to wrap cleanly.
- Added a small calligraphic "designed by nonori" signature beneath the headline in both editions.
- Added clear language-specific downloads, bilingual quick-start information, FAQs, and a dedicated Chinese guide.
- Updated the build script to build either edition or both from the same source.

新增英文版、分语言下载和中英文使用指南，两种界面共用相同功能及手动入口记录。

## v2.0.0

- Initial public release by Nonori.
- Automatic peripheral-app discovery, original application icons, and manual custom entries.
- Windows audio settings shortcuts, search, categories, and periodic scans while open.

首次公开发布：外设自动扫描、原版图标、手动添加、声音设置快捷入口和分类搜索。

