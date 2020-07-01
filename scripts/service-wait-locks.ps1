$file = "$(ServicePath)\log4net.dll"
$isLocked = $true;
try {
    # [IO.File]::OpenWrite($file).close();
    # If the file cannot be renamed that means it is in use and locked
    Rename-Item -Path "$file" -NewName "log4net.dll.old"
    $isLocked = $false;
}
catch {
    $isLocked = $true;
    Write-Warning $Error[0]
}

while($isLocked)
{
    if (Test-Path $file) {
        #File exists: neet to check if locked
    } else {
        # File missing, thus nothing should be locked
        $isLocked = $false;
    }

    Write-Host "File $(ServicePath)\log4net.dll is locked. Waiting for the lock to be released. Next try in 10 seconds..."
    Start-Sleep -s 10
    try {
        # [IO.File]::OpenWrite($file).close();
        Rename-Item -Path "$file" -NewName "log4net.dll.old"
        $isLocked = $false;
    }
    catch {
        $isLocked = $true;
        Write-Warning $Error[0]
    }
}
