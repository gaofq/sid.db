param(
    [string]$Action = "up",
    [switch]$Build = $false,
    [switch]$Detached = $false
)

$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

function Print-Step {
    param([string]$Message)
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  $Message" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
}

function Invoke-DockerCompose {
    param([string[]]$Args)
    $cmd = & docker compose 2>&1
    if ($LASTEXITCODE -ne 0) {
        docker-compose @Args
    } else {
        docker compose @Args
    }
}

switch ($Action) {
    "up" {
        Print-Step "Building and starting containers"
        $args = @("up")
        if ($Build) { $args += "--build" }
        if ($Detached) { $args += "-d" }
        $args += "--remove-orphans"
        Invoke-DockerCompose $args
        Write-Host ""
        Write-Host "Frontend : http://localhost" -ForegroundColor Green
        Write-Host "Backend  : http://localhost:8080" -ForegroundColor Green
        Write-Host "Swagger  : http://localhost:8080/swagger" -ForegroundColor Green
    }
    "down" {
        Print-Step "Stopping and removing containers"
        Invoke-DockerCompose down
    }
    "restart" {
        Print-Step "Restarting containers"
        Invoke-DockerCompose restart
    }
    "logs" {
        Invoke-DockerCompose logs -f
    }
    "build" {
        Print-Step "Building images"
        Invoke-DockerCompose build --no-cache
    }
    "status" {
        Invoke-DockerCompose ps
    }
    "clean" {
        Print-Step "Cleaning up Docker resources"
        Invoke-DockerCompose down -v --rmi local
        Write-Host "Cleanup complete" -ForegroundColor Green
    }
    default {
        Write-Host @"
Usage: .\deploy.ps1 [-Action <action>] [-Build] [-Detached]

Actions:
  up        Start containers (default)
  down      Stop and remove containers
  restart   Restart containers
  logs      View logs (follow mode)
  build     Rebuild images (no cache)
  status    Show container status
  clean     Full cleanup (volumes + images)

Options:
  -Build      Force rebuild before starting
  -Detached   Run in background

Examples:
  .\deploy.ps1                          Start services (foreground)
  .\deploy.ps1 -Detached                Start services (background)
  .\deploy.ps1 -Build -Detached         Rebuild and start (background)
  .\deploy.ps1 -Action logs             View real-time logs
  .\deploy.ps1 -Action down             Stop all
"@ -ForegroundColor Yellow
    }
}
