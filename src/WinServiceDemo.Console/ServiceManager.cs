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
        /// Starts the service
        /// </summary>
        public void Start()
        {
            //Initialize your service related instances here.
            //Do not do it at service level otherwise ressources won't be properly disposed on stop/restart of the service.

            System.Console.WriteLine("Starting service...");

            //Initialize timer:
            //Note: First time you have to wait for interval before first tick.
            //To start quickly we set a short interval that will be overwritten on next tick.
            //You could also call: Process(null, null);
            _timer = new Timer(1_000);
            //Set action associated to each tick
            _timer.Elapsed += Process;
            //Start the timer
            _timer.Start();
        }

        /// <summary>
        /// Stops the service
        /// </summary>
        public void Stop()
        {
            //Clean your ressources here

            System.Console.WriteLine("Stopping service...");

            //Stop timer
            _timer.Stop();
            //Dereference tick action
            _timer.Elapsed -= Process;
            //Dispose timer
            _timer.Dispose();

            //Note: you can create a while(true) here waiting for you pending process to properly end.
            //In that case, you can for example add an IsProcessing property and set while(IsProcessing) { }
            //In your Process() method, set "IsProcessing = true;" at the start and "false" at the end.
            //If your processes can overlap, you neet to improve this code accordingly.
        }

        /// <summary>
        /// Process action for each tick
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void Process(object sender, ElapsedEventArgs eventArgs)
        {
            //Trick to avoid processes to overlap in case process duration longer than timer tick duration
            //If your Process() is slow or you need exact same time between 2 starts you have to do it in another way
            _timer.Enabled = false;

            System.Console.WriteLine("Processing...");
            
            try
            {
                //Make sure there is no error thrown from the process method

                //Your processing code here...
                //...
                System.Threading.Thread.Sleep(1_000); //Simulate a 1 second operation
                //...
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Exception: {ex.Message}");
            }

            System.Console.WriteLine("Processing... DONE!");

            //If interval is different from defined interval we set it to default value.
            //This is the case on Start() method, we set a small interval to tick Process() method asap.
            //You can also adapt interval depending on charge (e.g. 1sec hight charge, 1min avg charge, 10mins low charge)
            if (_timer.Interval != TimerInterval)
                _timer.Interval = TimerInterval;
            //Unpause timer, it can resume
            _timer.Enabled = true;
        }
    }
}
