# Sequential build script for Sid.DbFieldManager
# Workaround for file lock contention during parallel builds
param(
    [string]$Configuration = "Debug"
)

$projects = @(
    "src/Sid.DbFieldManager.Domain.Shared",
    "src/Sid.DbFieldManager.Domain",
    "src/Sid.DbFieldManager.Application.Contracts",
    "src/Sid.DbFieldManager.Application",
    "src/Sid.DbFieldManager.EntityFrameworkCore",
    "src/Sid.DbFieldManager.HttpApi",
    "src/Sid.DbFieldManager.HttpApi.Client",
    "src/Sid.DbFieldManager.HttpApi.Host",
    "src/Sid.DbFieldManager.DbMigrator"
)

$failed = @()
foreach ($proj in $projects) {
    Write-Host "=== Building $proj ===" -ForegroundColor Cyan
    $result = dotnet build "aspnet-core/$proj" -c $Configuration 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "FAILED: $proj" -ForegroundColor Red
        $failed += $proj
        Write-Host ($result | Select-Object -Last 20)
    } else {
        Write-Host "OK: $proj" -ForegroundColor Green
    }
}

if ($failed.Count -eq 0) {
    Write-Host "`nAll projects built successfully." -ForegroundColor Green
} else {
    Write-Host "`nFailed projects: $($failed -join ', ')" -ForegroundColor Red
    exit 1
}
