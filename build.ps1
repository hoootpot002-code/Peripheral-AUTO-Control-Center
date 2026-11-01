$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) {
    $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
}
if (-not (Test-Path -LiteralPath $compiler)) { throw '.NET Framework C# compiler not found.' }
$destination = Join-Path $PSScriptRoot 'dist'
New-Item -ItemType Directory -Path $destination -Force | Out-Null
$compilerArguments = @(
    '/nologo', '/target:winexe', '/platform:anycpu', '/optimize+',
    '/reference:System.Windows.Forms.dll', '/reference:System.Drawing.dll',
    '/reference:System.Core.dll', '/reference:System.Xml.Linq.dll', '/reference:Microsoft.CSharp.dll',
    ('/win32manifest:' + (Join-Path $PSScriptRoot 'PeripheralHub.manifest')),
    ('/win32icon:' + (Join-Path $PSScriptRoot 'PeripheralHub.ico')),
    ('/out:' + (Join-Path $destination '外设AUTO控制中心.exe')),
    (Join-Path $PSScriptRoot 'PeripheralHub.cs'),
    (Join-Path $PSScriptRoot 'PeripheralDiscovery.cs')
)
& $compiler @compilerArguments
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
Write-Output (Join-Path $destination '外设AUTO控制中心.exe')
