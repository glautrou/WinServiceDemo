$exePath = "$(ServicePath)\WinServiceDemo.Console.exe"

$command = "$exePath install"
Write-Host "Installing: $command"
iex $command