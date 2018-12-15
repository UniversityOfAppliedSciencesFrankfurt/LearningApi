using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace AkkaMLPerceptron
{

    public class BackPropagationActor : ReceiveActor
    {
        /// <summary>
        /// Performs BackPropagation calculation of a single layer for specified calculationWidth.
        /// </summary>
        /// <param name="biases"></param>
        /// <param name="hiddenlayerneurons"></param>
        /// <param name="outputlayerneurons"></param>
        /// <param name="numOfInputNeurons"></param>
        /// <param name="calculationWidth">Number of features to be calculated by this instance. Calculation
        /// of back-propagation is shared across multiple nodes. This value specifies how many will be calculated by thi sinstance.</param>
        public BackPropagationActor(double[][] biases, 
            int[] hiddenlayerneurons, int outputlayerneurons, int numOfInputNeurons,
            int calculationWidth)
        {
            Receive<BackPropActorIn>(new Action<BackPropActorIn>(CalculateLayer));
        }

        protected override void PreStart()
        {
            Console.WriteLine($"Started Actor: {Context.Self.Path}");

            base.PreStart();
        }

        protected void CalculateLayer(BackPropActorIn msg)
        {
            Console.WriteLine($"Entered calculation: {Context.Self.Path}");

            Thread.Sleep(2000);
            
            Sender.Tell(new BackPropActorOut() { Status = "Success" });
        }
    }
}
