# Clean Architecture Migration - Cleanup Script
# This script removes the old project structure files after confirming the new structure works

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  RentSystem - Clean Architecture Cleanup" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "This script will remove old project files that have been migrated to Clean Architecture." -ForegroundColor Yellow
Write-Host ""
Write-Host "Files/Folders to be removed:" -ForegroundColor Yellow
Write-Host "  - Controllers/ (old controllers)" -ForegroundColor Gray
Write-Host "  - Models/ (old models)" -ForegroundColor Gray
Write-Host "  - Dtos/ (old DTOs)" -ForegroundColor Gray
Write-Host "  - Helpers/ (old helpers)" -ForegroundColor Gray
Write-Host "  - Migrations/ (old migrations - will need new ones)" -ForegroundColor Gray
Write-Host "  - bin/ (build outputs)" -ForegroundColor Gray
Write-Host "  - obj/ (build cache)" -ForegroundColor Gray
Write-Host "  - Old Program.cs" -ForegroundColor Gray
Write-Host "  - Old appsettings files" -ForegroundColor Gray
Write-Host "  - RentApi.csproj (old project file)" -ForegroundColor Gray
Write-Host ""

$confirmation = Read-Host "Are you sure you want to proceed? Type 'YES' to confirm"

if ($confirmation -ne 'YES') {
    Write-Host "Cleanup cancelled." -ForegroundColor Red
    exit
}

Write-Host ""
Write-Host "Starting cleanup..." -ForegroundColor Green

$rootPath = "c:\Users\ka\Desktop\CS\RentSystem\RentApi"

# Remove old folders
$foldersToRemove = @(
    "Controllers",
    "Models", 
    "Dtos",
    "Helpers",
    "Migrations",
    "bin",
    "obj",
    "Properties"
)

foreach ($folder in $foldersToRemove) {
    $path = Join-Path $rootPath $folder
    if (Test-Path $path) {
        Write-Host "Removing folder: $folder" -ForegroundColor Yellow
        Remove-Item -Path $path -Recurse -Force
        Write-Host "  ✓ Removed" -ForegroundColor Green
    } else {
        Write-Host "  - Folder not found: $folder" -ForegroundColor Gray
    }
}

# Remove old files
$filesToRemove = @(
    "Program.cs",
    "appsettings.json",
    "appsettings.Development.json",
    "RentApi.csproj",
    "RentApi.csproj.user",
    "RentApi.http"
)

foreach ($file in $filesToRemove) {
    $path = Join-Path $rootPath $file
    if (Test-Path $path) {
        Write-Host "Removing file: $file" -ForegroundColor Yellow
        Remove-Item -Path $path -Force
        Write-Host "  ✓ Removed" -ForegroundColor Green
    } else {
        Write-Host "  - File not found: $file" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  Cleanup Complete!" -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Your project now has a clean Clean Architecture structure:" -ForegroundColor Green
Write-Host ""
Write-Host "RentSystem/" -ForegroundColor White
Write-Host "├── src/" -ForegroundColor White
Write-Host "│   ├── RentSystem.Domain/" -ForegroundColor Cyan
Write-Host "│   ├── RentSystem.Application/" -ForegroundColor Cyan
Write-Host "│   ├── RentSystem.Infrastructure/" -ForegroundColor Cyan
Write-Host "│   └── RentSystem.API/" -ForegroundColor Cyan
Write-Host "└── RentSystem.sln" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Navigate to src/RentSystem.API" -ForegroundColor Gray
Write-Host "2. Run: dotnet ef migrations add InitialMigration --project ../RentSystem.Infrastructure" -ForegroundColor Gray
Write-Host "3. Run: dotnet ef database update --project ../RentSystem.Infrastructure" -ForegroundColor Gray
Write-Host "4. Run: dotnet run" -ForegroundColor Gray
Write-Host ""
