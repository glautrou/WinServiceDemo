Write-Host "Starting: $(ApplicationName)"
Get-Service -Name "$(ApplicationName)" | Start-Service