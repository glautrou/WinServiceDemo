$service = Get-Service -Name "$(ApplicationName)" -ErrorAction SilentlyContinue
if ($service.Length -gt 0) {
    Write-Host "Stopping $(ApplicationName) service"
    Get-Service -Name "$(ApplicationName)" | Stop-Service
} else {
    Write-Host "Service $(ApplicationName) not found"
}