# Update-AppInstaller.ps1
# Updates .appinstaller files with the correct GitHub URLs for auto-updates
#
# IMPORTANT: For Windows auto-updates to work correctly, the .appinstaller file's
# Uri attribute must point to a STABLE URL that always serves the LATEST version.
# 
# This script configures the .appinstaller to use GitHub Pages or the 'releases' 
# branch for the stable update URL, while the MainPackage.Uri points to the 
# specific release download.
#
# Recommended setup:
# 1. Create a 'gh-pages' branch or 'releases' folder in your repo
# 2. After each release, copy the .appinstaller files to that stable location
# 3. Users install from the stable URL, and Windows checks that URL for updates

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [string]$GitHubOwner = "JoeTomkinson",
    [string]$GitHubRepo = "CommonToggleButtonUI",
    [string]$PackagesPath = ".\AppPackages",
    
    # The stable base URL where .appinstaller files are hosted for update checks
    # Option 1: GitHub Pages (recommended)
    # Option 2: Raw GitHub URL from a dedicated branch
    [string]$StableBaseUrl = "https://$GitHubOwner.github.io/$GitHubRepo/releases"
)

$releaseDownloadUrl = "https://github.com/$GitHubOwner/$GitHubRepo/releases/download/v$Version"

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
    
    # Build the package filename
    $packageName = "ToggleNotifier.Package_$($Version)_$arch.msix"
    
    # The Uri attribute is where Windows checks for updates - must be a STABLE URL
    # that always points to the latest .appinstaller file
    $stableAppInstallerUrl = "$StableBaseUrl/$($file.Name)"
    
    # The MainPackage.Uri points to the actual package download for this version
    $packageDownloadUrl = "$releaseDownloadUrl/$packageName"
    
    # Update the URIs
    $xml.AppInstaller.Uri = $stableAppInstallerUrl
    $xml.AppInstaller.MainPackage.Uri = $packageDownloadUrl
    
    # Ensure UpdateSettings are configured for automatic updates
    $updateSettings = $xml.AppInstaller.UpdateSettings
    if ($null -eq $updateSettings) {
        Write-Host "  Warning: No UpdateSettings found in .appinstaller" -ForegroundColor Yellow
    }
    
    # Save the updated file
    $xml.Save($file.FullName)
    
    Write-Host "  AppInstaller Uri (for update checks): $stableAppInstallerUrl" -ForegroundColor Green
    Write-Host "  MainPackage Uri (download): $packageDownloadUrl" -ForegroundColor Green
}

Write-Host ""
Write-Host "============================================" -ForegroundColor Yellow
Write-Host "IMPORTANT: Post-Release Steps" -ForegroundColor Yellow
Write-Host "============================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Upload these files to GitHub Release v$Version`:" -ForegroundColor White
Get-ChildItem -Path $PackagesPath -Include "*.msix", "*.appinstaller" -Recurse | ForEach-Object {
    Write-Host "   - $($_.Name)" -ForegroundColor Cyan
}
Write-Host ""
Write-Host "2. Copy .appinstaller files to your stable hosting location:" -ForegroundColor White
Write-Host "   Target: $StableBaseUrl" -ForegroundColor Cyan
Write-Host ""
Write-Host "   If using GitHub Pages:" -ForegroundColor Gray
Write-Host "   - Commit .appinstaller files to gh-pages branch under /releases/" -ForegroundColor Gray
Write-Host ""
Write-Host "   If using raw GitHub:" -ForegroundColor Gray
Write-Host "   - Commit .appinstaller files to main branch under /releases/" -ForegroundColor Gray
Write-Host "   - Update StableBaseUrl to use raw.githubusercontent.com" -ForegroundColor Gray
Write-Host ""
Write-Host "============================================" -ForegroundColor Yellow
Write-Host "Architecture Note" -ForegroundColor Yellow  
Write-Host "============================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "Each .appinstaller file is architecture-specific (x64, x86, ARM64)." -ForegroundColor White
Write-Host "Users must download the correct .appinstaller for their system." -ForegroundColor White
Write-Host ""
Write-Host "For automatic architecture selection, consider:" -ForegroundColor Gray
Write-Host "- Creating an .msixbundle containing all architectures, OR" -ForegroundColor Gray
Write-Host "- Using a web page that detects architecture and redirects" -ForegroundColor Gray
Write-Host ""
