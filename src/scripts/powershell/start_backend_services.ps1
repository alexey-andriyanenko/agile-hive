# Base paths
$repoRoot = Join-Path $PSScriptRoot "..\.."
$backendPath = Join-Path $repoRoot "backend"
$logsPath = Join-Path $repoRoot "logs"

# Create logs folder if missing
if (-not (Test-Path $logsPath)) {
    New-Item -ItemType Directory -Path $logsPath
}

# Services
$services = @(
    "Web.API",
    "IdentityService\IdentityService.API",
    "OrganizationService\OrganizationService.API",
    "OrganizationUserService\OrganizationUserService.API",
    "ProjectService\ProjectService.API",
    "ProjectUserService\ProjectUserService.API",
    "BoardService\BoardService.API",
    "TaskService\TaskService.API",
    "TaskAggregatorService\TaskAggregatorService.API",
    "TagService\TagService.API"
)

foreach ($service in $services) {
    $servicePath = Join-Path $backendPath $service
    $logFile = Join-Path $logsPath (($service -replace '[\\\/]', '_') + ".log")

    if (-not (Test-Path $servicePath)) {
        Write-Warning "Service path not found: $servicePath. Skipping."
        continue
    }

    Write-Host "Starting $service at $servicePath"
    Write-Host "Logs: $logFile"

    Push-Location $servicePath
    Start-Process "powershell.exe" -ArgumentList "-NoExit", "-Command", "dotnet run | Tee-Object '$logFile'"
    Pop-Location
}
