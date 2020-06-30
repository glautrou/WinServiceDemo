using System;
using Topshelf;

namespace WinServiceDemo.Console
{
    /// <summary>
    /// Program
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// It is used to set up the Windows Service.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            //Configure service host
            var rc = HostFactory.Run(configure =>
            {
                //Service actions
                configure.Service<ServiceManager>(s =>
                {
                    s.ConstructUsing(() => new ServiceManager());
                    s.WhenStarted(i => i.Start());
                    s.WhenStopped(i => i.Stop());
                    s.WhenShutdown(i => i.Stop());
                });

                //Service settings
                configure.RunAsLocalSystem();
                configure.StartAutomatically();
                configure.SetStopTimeout(TimeSpan.FromSeconds(10));
                configure.OnException(ex => { System.Console.WriteLine($"Windows Service level exception: {ex.Message}"); });

                //Service description
                configure.SetServiceName("WinServiceDemo.Console");
                configure.SetDisplayName("WinServiceDemo.Console");
                configure.SetDescription("C# Windows Service demo using Topshelf");
            });

            //Exit code
            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            System.Console.WriteLine($"Exit code: {exitCode}");
            Environment.ExitCode = exitCode;
        }
    }
}
