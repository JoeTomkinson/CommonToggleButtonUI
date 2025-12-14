# Update-AppInstaller.ps1
# Updates .appinstaller files with the correct GitHub release URLs

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [string]$GitHubOwner = "JoeTomkinson",
    [string]$GitHubRepo = "CommonToggleButtonUI",
    [string]$PackagesPath = ".\AppPackages"
)

$baseUrl = "https://github.com/$GitHubOwner/$GitHubRepo/releases/download/v$Version"

# Find all .appinstaller files
$appInstallerFiles = Get-ChildItem -Path $PackagesPath -Filter "*.appinstaller" -Recurse

foreach ($file in $appInstallerFiles) {
    Write-Host "Processing: $($file.Name)" -ForegroundColor Cyan
    
    # Read the content
    [xml]$xml = Get-Content $file.FullName
    
    # Determine architecture from filename
    $arch = if ($file.Name -match "_x64") { "x64" } 
            elseif ($file.Name -match "_x86") { "x86" }
            elseif ($file.Name -match "_arm64") { "ARM64" }
            else { "x64" }
    
    # Get the package name (e.g., ToggleNotifier.Package_2.0.0.0_x64.msix)
    $packageName = $xml.AppInstaller.MainPackage.Uri -replace '.*/', ''
    if (-not $packageName) {
        $packageName = "ToggleNotifier.Package_$($Version)_$arch.msix"
    }
    
    # Update the URIs
    $xml.AppInstaller.Uri = "$baseUrl/$($file.Name)"
    $xml.AppInstaller.MainPackage.Uri = "$baseUrl/$packageName"
    
    # Save the updated file
    $xml.Save($file.FullName)
    
    Write-Host "  Updated Uri to: $baseUrl/$($file.Name)" -ForegroundColor Green
    Write-Host "  Updated MainPackage.Uri to: $baseUrl/$packageName" -ForegroundColor Green
}

Write-Host "`nDone! Files to upload to GitHub Release v$Version`:" -ForegroundColor Yellow
Get-ChildItem -Path $PackagesPath -Include "*.msix", "*.appinstaller" -Recurse | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor White
}
