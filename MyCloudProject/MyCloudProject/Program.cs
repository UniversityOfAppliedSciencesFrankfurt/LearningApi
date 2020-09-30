using MyCloudProject.Common;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading;
using MyExperiment;

namespace MyCloudProject
{
    class Program
    {
        /// <summary>
        /// Your project ID from the last semester.
        /// </summary>
        private static readonly string projectName = "ML19/20-3.31 - Implementation of Image Binarizer";

        
        static void Main(string[] args)
        {
            //// Cancellation token to interrupt the program during runtime ("Crtl" + "C")
            CancellationTokenSource tokeSrc = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                tokeSrc.Cancel();
            };
           
            //// Configuration initialization for the Experiment
            var cfgRoot = Common.InitHelpers.InitConfiguration(args);
            var cfgSec = cfgRoot.GetSection("MyConfig");
            MyConfig cfg = new MyConfig();
            cfgSec.Bind(cfg);

            //// Logging infrastructure initialization
            var logFactory = InitHelpers.InitLogging(cfgRoot);
            var logger = logFactory.CreateLogger("Train.Console");
            logger?.LogInformation($"{DateTime.Now} -  Started experiment: {projectName}");
            Console.WriteLine($"Started experiment:{0}", projectName);

            //// Client initialization for Azure Storage Account
            IStorageProvider storageProvider = new AzureStorageProvider(cfg); 

            //// Run the experiment
            Experiment experiment = new Experiment(cfg, storageProvider, logger);
            experiment.RunQueueListener(tokeSrc.Token).Wait();

            //// Log runtime info
            logger?.LogInformation($"{DateTime.Now} -  Experiment exit: {projectName}");
        }


    }
}
