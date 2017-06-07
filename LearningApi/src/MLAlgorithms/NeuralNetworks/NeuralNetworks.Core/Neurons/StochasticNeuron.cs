using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;

namespace NeuralNetworks.Core.Neurons
{
    [Serializable]
    public class StochasticNeuron : ActivationNeuron
    {

        private double sample;
        private new IStochasticFunction function;


        public double Sample
        {
            get { return sample; }
        }


        public new IStochasticFunction ActivationFunction
        {
            get { return function; }
            set { base.function = this.function = value; }
        }



        public StochasticNeuron(int inputs, IStochasticFunction function)
            : base(inputs, function)
        {
            this.ActivationFunction = function;
            this.Threshold = 0; // Ruslan Salakhutdinov and Geoff Hinton
                                // also start with zero thresholds
        }


        public override double Compute(double[] input)
        {
            double sum = threshold;
            for (int i = 0; i < weights.Length; i++)
                sum += weights[i] * input[i];

            double output = function.Function(sum);

            this.output = output;

            return output;
        }



        public double Generate(double[] input)
        {
            double sum = threshold;
            for (int i = 0; i < weights.Length; i++)
                sum += weights[i] * input[i];

            double output = function.Function(sum);
            double sample = function.Generate2(output);

            this.output = output;
            this.sample = sample;

            return sample;
        }


        public double Generate(double output)
        {
            double sample = function.Generate2(output);

            this.output = output;
            this.sample = sample;

            return sample;
        }

    }
}
