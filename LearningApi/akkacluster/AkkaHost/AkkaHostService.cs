using Akka.Actor;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaHost
{
    public class AkkaHostService
    {

        protected ActorSystem AkkaClusterSystem;

        public Task WhenTerminated => AkkaClusterSystem.WhenTerminated;


        public AkkaHostService()
        {

        }

        public void Start(string hoconConfig, string systemName, string[] seedHosts, int port)
        {
            var strCfg = File.ReadAllText(hoconConfig);

            strCfg = strCfg.Replace("@PORT", port.ToString());

            bool isFirst = true;

            StringBuilder sb = new StringBuilder();

            foreach (var item in seedHosts)
            {
                if (isFirst == false)
                    sb.Append(", ");

                sb.Append($"\"akka.tcp://{systemName}@{item}\"");
                //example: seed - nodes = ["akka.tcp://ClusterSystem@localhost:8081"]

                isFirst = false;
            }

            strCfg = strCfg.Replace("@SEEDHOSTS", sb.ToString());

            var config = ConfigurationFactory.ParseString(strCfg);

            AkkaClusterSystem = ActorSystem.Create(systemName, config);

        }

        public Task Stop()
        {
            return CoordinatedShutdown.Get(AkkaClusterSystem).Run(reason: CoordinatedShutdown.ClrExitReason.Instance);
        }

        //public static Akka.Configuration.Config ParseConfig(string hoconPath)
        //{
        //    return ConfigurationFactory.ParseString(File.ReadAllText(hoconPath));
        //}
    }
}
