using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;


namespace AkkaHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            builder.AddEnvironmentVariables();
            IConfigurationRoot cfg = builder.Build();

            var strSeedHosts = cfg["seedhosts"];
            var seedHosts = strSeedHosts.Split(',');

            var strPort = cfg["port"];
            int port = -1;
            int.TryParse(strPort, out port);

            var sysName = cfg["sysName"];
            if (String.IsNullOrEmpty(sysName))
                sysName = "LearningAPISystem";

            var pubHostName = cfg["publichostname"];

            var hostName = cfg["hostname"];

            Console.WriteLine("Started initialization of AKKA.NET Node");

            AkkaHostService svc = new AkkaHostService();

            svc.Start("akkahost.hocon", pubHostName, hostName, sysName, seedHosts.Select(h=>h.TrimStart(' ').TrimEnd(' ')).ToArray(), port);

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                svc.Stop();
                eventArgs.Cancel = true;
            };

            Console.WriteLine("Press any key to stop AKKA.NET Node");
            svc.WhenTerminated.Wait();
        }
    }
}
