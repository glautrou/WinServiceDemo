$file = "$(ServicePath)\log4net.dll"
$isLocked = $true;
try {
    [IO.File]::OpenWrite($file).close();
    $isLocked = $false;
}
catch {
    $isLocked = $true;
}

while($isLocked)
{
    Write-Host "File $(ServicePath)\log4net.dll is locked. Waiting for the lock to be released. Next try in 10 seconds..."
    Start-Sleep -s 10
    try {
        [IO.File]::OpenWrite($file).close();
        $isLocked = $false;
    }
    catch {
        $isLocked = $true;
    }
}