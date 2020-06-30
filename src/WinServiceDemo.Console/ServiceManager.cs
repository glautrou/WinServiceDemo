using System;
using System.Timers;

namespace WinServiceDemo.Console
{
    /// <summary>
    /// Service manager containing service actions and managing processing.
    /// </summary>
    public class ServiceManager
    {

        /// <summary>
        /// The timer interval
        /// </summary>
        private static readonly double TimerInterval = 10_000; //10 seconds
        /// <summary>
        /// The service timer
        /// </summary>
        private Timer _timer;

        /// <summary>
        /// Starts the servive
        /// </summary>
        public void Start()
        {
            //Initialize your service related instances here.
            //Do not do it at service level otherwise ressources won't be properly disposed on stop/restart of the service.

            System.Console.WriteLine("Starting service...");

            _timer = new Timer(1_000);
            _timer.Elapsed += Process;
            _timer.Start();
        }

        /// <summary>
        /// Stops the service
        /// </summary>
        public void Stop()
        {
            //Clean your ressources here

            System.Console.WriteLine("Stopping service...");

            _timer.Stop();
            _timer.Elapsed -= Process;
            _timer.Dispose();
        }


        /// <summary>
        /// Process action for each tick
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void Process(object sender, ElapsedEventArgs eventArgs)
        {
            _timer.Enabled = false; //Trick to avoid processes to overlap in case process duration longer than timer tick duration

            System.Console.WriteLine("Processing...");

            try
            {
                //Make sure there is no error thrown from the process method

                //Simulate 1 second operation
                System.Threading.Thread.Sleep(1_000);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Exception: {ex.Message}");
            }

            System.Console.WriteLine("Processing... DONE!");

            _timer.Interval = TimerInterval;
            _timer.Enabled = true; //Unpause timer
        }
    }
}