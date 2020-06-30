# WinServiceDemo
C# Windows Service demo app using [Topshelf](http://topshelf-project.com/) and GitHub Actions

## Why Topshelf?
I used to develop Windows Services without the need of any third-party libraries. I created some code used to manage my service, like the processing, but also the entire lifecycle and even install/uninstall features. I then added my code, which was concise and robust in production, on the private NuGet of my company for reusing it on all our Windows Services projects.

Topshelf is widely kwnown an used library among C# developers for creating Windows Services, this time I wanted to give it a try. Top-features I like the most:
- Cross-platform: host services on both Windows and Mono
- Easy to use
- Highly Configurable
- Powerfull command line
- .NET Core/Framework compatibility

## Step 1 - Create the solution:
```bash
md src
cd src
dotnet new sln -n "WinServiceDemo"
```

## Step 2 - Create Console project:
```bash
dotnet new console -n "WinServiceDemo.Console"
```

## Step 3 - Add project to solution:
```bash
dotnet sln add "WinServiceDemo.Console"
```

## Step 4 - Add [Topshelf NuGet package](https://www.nuget.org/packages/topshelf/):
```bash
cd "WinServiceDemo.Console"
dotnet add package Topshelf --version 4.2.1
```
## Step 5 - Add a service manager:
1. Create ServiceManager.cs
2. Add OnStart() method. This method will be fired whenever the service is started.
3. Add OnStop() method. This method will be fired whenever the service is stopped.
4. Add Process(object sender, ElapsedEventArgs eventArgs) method. This method will be fired on our custom time defined tick.
5. Define timer with a custom interval time in OnStart(). Set the timer Elapsed property tou our Process() method. In OnStop() dispose the timer.

## Step 6 - Plug Topshelf to your service manager:
In Program.cs, configure the main entrypoint to initialize Topshelf on top of our service manager

## Step 7 - Debug your code:
Topshelf makes it easy to debug our application, which is basically a simple console app. To test it, just hit "F5" on Visual Studio or execute the `dotnet run` command.

## Step 8 - Deploy your service:
1. Create the artifact
```bash
dotnet build
dotnet publish
```
2. Install
```bash
WinServiceDemo.Console.exe install
Get-Service -Name "WinServiceDemo.Console" | Start-Service
```

<ins>Note:</ins> I used Powershell command for starting the service because otherwise when calling our .exe the Program Main method will log twice, and if you use file logger your file could already be locked because in use by another process.

3. Uninstall
```bash
Get-Service -Name "WinServiceDemo.Console" | Stop-Service
WinServiceDemo.Console.exe uninstall
```

<ins>Note:</ins> When you uninstall it the OnStop() method is correctly fired and your service manager correctly cleaned. However, I noticed that tis way does not correctly stop it, the service status is marked as deactivated and our .exe is still running in task manager. This problem means that we cannot copy our new version of the application because some files are in use and the locked by the operating system. Stopping the service via Powershell command and then uninstalling it fixed this issue.
