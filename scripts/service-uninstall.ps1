Write-Host "Create path if needed: $(ServicePath)"
New-Item -ItemType Directory -Force -Path $(ServicePath)

$exePath = "$(ServicePath)\WinServiceDemo.Console.exe"
Write-Host "Executable path: $exePath"
if (Test-Path $exePath) {
  $command = "$exePath uninstall"
  Write-Host "Executable found, uninstalling: $command"
  iex $command
}