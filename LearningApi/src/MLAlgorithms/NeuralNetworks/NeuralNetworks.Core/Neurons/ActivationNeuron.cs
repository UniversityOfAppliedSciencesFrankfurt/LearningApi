using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation;

namespace NeuralNetworks.Core.Neurons
{
    [Serializable]
    public class ActivationNeuron : Neuron
    {
       
        protected double threshold = 0.0;

        
        protected IActivationFunction function = null;

       
        public double Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }

      
        public IActivationFunction ActivationFunction
        {
            get { return function; }
            set { function = value; }
        }

        
        public ActivationNeuron(int inputs, IActivationFunction function)
            : base(inputs)
        {
            this.function = function;
        }

        
        public override void Randomize()
        {
            // randomize weights
            base.Randomize();

            // randomize threshold
            threshold = rand.Generate();
        }

       
        public override double Compute(double[] input)
        {
            // check for corrent input vector
            if (input.Length != inputsCount)
                throw new ArgumentException("Wrong length of the input vector.");

            // initial sum value
            double sum = 0.0;

            // compute weighted sum of inputs
            for (int i = 0; i < weights.Length; i++)
                sum += weights[i] * input[i];
            sum += threshold;

            // local variable to avoid mutlithreaded conflicts
            double output = function.Function(sum);

            // assign output property as well (works correctly for single threaded usage)
            this.output = output;

            return output;
        }
    }
}
