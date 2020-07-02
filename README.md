# WinServiceDemo
C# Windows Service demo app using [Topshelf](http://topshelf-project.com/) and GitHub Actions.

You can find my full blog post [here](https://blog.webnet.fr/creation-dun-service-windows-avec-topshelf-et-ci-cd/) (French).

![Compile](https://github.com/glautrou/WinServiceDemo/workflows/Compile/badge.svg)

## Why Topshelf?
I used to develop Windows Services without the need of any third-party libraries. I created some code used to manage my service (bootstrapper), like the processing, but also the entire lifecycle and even install/uninstall features. That allowed me to easily debug locally. I then added my code, which was concise and robust in production, on the private NuGet of my company for reusing it on all our Windows Services projects.

Topshelf is widely kwnown an used library among C# developers for creating Windows Services, this time I wanted to give it a try. Top-features I like the most:
- Cross-platform: host services on both Windows and Mono
- Easy to use
- Highly Configurable
- Powerfull command line
- .NET Core/Framework compatibility

## Other alternatives than Topshelf?
Yes! As I said in the previous paragraph, you can do the old way by creating a Console application, then managing installation with `sc` commands or better by creating your boostrapper.

Better, if you are using .NET Core 3.0+, you can create a [Worker project](https://devblogs.microsoft.com/aspnet/net-core-workers-as-windows-services/) (`dotnet new worker`). Worker projects fit better with the .NET Core way as they are similar with ASP.NET Core on how they are boostrapped and you can easily reuse code and integrate them together. However with Worker projects you will have to still use `sc` commands.

If you are using Cloud services you may be interested in [Azure Functions](https://azure.microsoft.com/en-us/services/functions/) or [AWS Lambda](https://aws.amazon.com/fr/lambda/).

## Steps from the creation of the project to the CI/CD process:

### Step 1 - Create the solution:
```bash
md src
cd src
dotnet new sln -n "WinServiceDemo"
```

### Step 2 - Create Console project:
```bash
dotnet new console -n "WinServiceDemo.Console"
```

### Step 3 - Add project to solution:
```bash
dotnet sln add "WinServiceDemo.Console"
```

### Step 4 - Add [Topshelf NuGet package](https://www.nuget.org/packages/topshelf/):
```bash
cd "WinServiceDemo.Console"
dotnet add package Topshelf --version 4.2.1
```
### Step 5 - Add a service manager:
1. Create ServiceManager.cs
2. Add OnStart() method. This method will be fired whenever the service is started.
3. Add OnStop() method. This method will be fired whenever the service is stopped.
4. Add Process(object sender, ElapsedEventArgs eventArgs) method. This method will be fired on our custom time defined tick.
5. Define timer with a custom interval time in OnStart(). Set the timer Elapsed property tou our Process() method. In OnStop() dispose the timer.

### Step 6 - Plug Topshelf to your service manager:
In Program.cs, configure the main entrypoint to initialize Topshelf on top of our service manager

### Step 7 - Debug your code:
Topshelf makes it easy to debug our application, which is basically a simple console app. To test it, just hit "F5" on Visual Studio or execute the `dotnet run` command.

### Step 8 - Deploy your service:
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

### Step 9 - Integrate into your CI/CD pipeline:
You can find in /scripts folder the Powershell files that can be used in your CI/CD pipeline:

1. Get your **artifact**
2. Enventualy set environment **variables** or appsettings.json environment values
3. **service-stop.ps1**: Stop the service. If service missing, command is automatically skipped
4. **service-uninstall.ps1**: Uninstall the service. If .exe missing, command is automatically skipped
5. **service-wait-locks.ps1**: In my case there were very long running threads/tasks making service stop/uninstall quite long and that can reach timeout. To avoid operating system file locks during new version copy the script check when file are not locked (e.g. log4net.dll not in use here) to allow a safe upgrade.
6. **Copy artifact** files on server
7. **service-install.ps1**: Install the service
8. **service-start.ps1**: Start the service
9. **Enjoy!**
