#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Publishes EFCore.BulkExtensions.Dotnet10 packages to NuGet.org

.DESCRIPTION
    This script builds the solution in Release mode and publishes all
    EFCore.BulkExtensions.Dotnet10.* packages to NuGet.org using the
    API key from nuget-key.txt

.PARAMETER SkipBuild
    Skip the build step and publish existing packages from Nugets folder

.EXAMPLE
    .\publish.ps1
    Builds and publishes all packages

.EXAMPLE
    .\publish.ps1 -SkipBuild
    Publishes existing packages without rebuilding
#>

param(
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

# Package IDs
$packages = @(
    "EFCore.BulkExtensions.Dotnet10",
    "EFCore.BulkExtensions.Dotnet10.Core",
    "EFCore.BulkExtensions.Dotnet10.SqlServer",
    "EFCore.BulkExtensions.Dotnet10.PostgreSql",
    "EFCore.BulkExtensions.Dotnet10.Sqlite"
)

# Get NuGet API key
$keyFile = Join-Path $PSScriptRoot "nuget-key.txt"
if (-not (Test-Path $keyFile)) {
    Write-Error "NuGet API key file not found: $keyFile"
    Write-Host "Please create nuget-key.txt with your NuGet API key" -ForegroundColor Red
    exit 1
}

# Read all non-comment, non-blank lines
$apiKey = (Get-Content $keyFile | Where-Object { $_ -notmatch '^\s*#' -and $_ -notmatch '^\s*$' } | Select-Object -First 1)
if ([string]::IsNullOrWhiteSpace($apiKey)) {
    Write-Error "NuGet API key not found in $keyFile"
    Write-Host "Please add your NuGet API key to nuget-key.txt" -ForegroundColor Red
    exit 1
}

Write-Host "=== EFCore.BulkExtensions.Dotnet10 NuGet Publisher ===" -ForegroundColor Cyan
Write-Host ""

# Build solution
if (-not $SkipBuild) {
    Write-Host "Building solution in Release mode..." -ForegroundColor Yellow
    dotnet build EFCore.BulkExtensions.sln --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed!"
        exit 1
    }
    Write-Host "Build successful!" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "Skipping build (using existing packages)..." -ForegroundColor Yellow
    Write-Host ""
}

# Find package files
$nugetFolder = Join-Path $PSScriptRoot "Nugets\net10"
if (-not (Test-Path $nugetFolder)) {
    Write-Error "NuGet packages folder not found: $nugetFolder"
    exit 1
}

Write-Host "Looking for packages in: $nugetFolder" -ForegroundColor Yellow
$packageFiles = Get-ChildItem -Path $nugetFolder -Filter "*.nupkg" | Where-Object { $_.Name -notlike "*.symbols.nupkg" }

if ($packageFiles.Count -eq 0) {
    Write-Error "No NuGet packages found in $nugetFolder"
    Write-Host "Run without -SkipBuild to build packages first" -ForegroundColor Red
    exit 1
}

Write-Host "Found $($packageFiles.Count) package(s):" -ForegroundColor Green
foreach ($pkg in $packageFiles) {
    Write-Host "  - $($pkg.Name)" -ForegroundColor Gray
}
Write-Host ""

# Confirm publication
Write-Host "WARNING: This will publish packages to NuGet.org" -ForegroundColor Yellow
Write-Host "Press any key to continue or Ctrl+C to cancel..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
Write-Host ""

# Publish packages
$published = 0
$failed = 0

foreach ($packageFile in $packageFiles) {
    $packagePath = $packageFile.FullName
    Write-Host "Publishing: $($packageFile.Name)" -ForegroundColor Cyan
    
    try {
        dotnet nuget push $packagePath --api-key $apiKey --source https://api.nuget.org/v3/index.json --skip-duplicate
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "  ✓ Published successfully" -ForegroundColor Green
            $published++
        } else {
            Write-Host "  ✗ Failed to publish" -ForegroundColor Red
            $failed++
        }
    }
    catch {
        Write-Host "  ✗ Error: $_" -ForegroundColor Red
        $failed++
    }
    
    Write-Host ""
}

# Summary
Write-Host "=== Publication Summary ===" -ForegroundColor Cyan
Write-Host "Published: $published" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor $(if ($failed -gt 0) { "Red" } else { "Gray" })
Write-Host ""

if ($published -gt 0) {
    Write-Host "Packages published successfully!" -ForegroundColor Green
    Write-Host "View your packages at: https://www.nuget.org/packages?q=EFCore.BulkExtensions.Dotnet10" -ForegroundColor Cyan
}

if ($failed -gt 0) {
    Write-Host "Some packages failed to publish. Check the errors above." -ForegroundColor Red
    exit 1
}

exit 0
