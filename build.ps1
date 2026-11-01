param(
    [ValidateSet('all','en-US','zh-CN')]
    [string]$Language = 'all'
)
$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) {
    $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
}
if (-not (Test-Path -LiteralPath $compiler)) { throw '.NET Framework C# compiler not found.' }
$destination = Join-Path $PSScriptRoot 'dist'
New-Item -ItemType Directory -Path $destination -Force | Out-Null
$editions = if ($Language -eq 'all') { @('en-US','zh-CN') } else { @($Language) }
foreach ($edition in $editions) {
    $fileName = if ($edition -eq 'en-US') { 'Peripheral-AUTO-Control-Center-EN.exe' } else { 'Peripheral-AUTO-Control-Center-ZH-CN.exe' }
    $compilerArguments = @(
        '/nologo', '/target:winexe', '/platform:anycpu', '/optimize+',
        '/reference:System.Windows.Forms.dll', '/reference:System.Drawing.dll',
        '/reference:System.Core.dll', '/reference:System.Xml.Linq.dll', '/reference:Microsoft.CSharp.dll',
        ('/win32manifest:' + (Join-Path $PSScriptRoot 'PeripheralHub.manifest')),
        ('/win32icon:' + (Join-Path $PSScriptRoot 'PeripheralHub.ico')),
        ('/resource:' + (Join-Path $PSScriptRoot 'Nonori.Signature.png') + ',Nonori.Signature.png'),
        ('/out:' + (Join-Path $destination $fileName)),
        (Join-Path $PSScriptRoot 'PeripheralHub.cs'),
        (Join-Path $PSScriptRoot 'PeripheralDiscovery.cs'),
        (Join-Path $PSScriptRoot 'UiLocalization.cs')
    )
    if ($edition -eq 'en-US') { $compilerArguments += '/define:ENGLISH' }
    & $compiler @compilerArguments
    if ($LASTEXITCODE -ne 0) { throw "Build failed: $edition" }
    Write-Output (Join-Path $destination $fileName)
}
