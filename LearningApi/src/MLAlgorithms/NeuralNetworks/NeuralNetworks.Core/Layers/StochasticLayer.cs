using System;
using NeuralNetworks.Core;
using NeuralNetworks.Core.Neurons;
using NeuralNetworks.Core.ActivationFunctions;
using LearningFoundation;

namespace NeuralNetworks.Core.Layers
{
    [Serializable]
    public class StochasticLayer : ActivationLayer
    {

        private new StochasticNeuron[] neurons;
        private double[] sample;

        public new StochasticNeuron[] Neurons
        {
            get { return neurons; }
        }

      
        public double[] Sample
        {
            get { return sample; }
        }
        
        public StochasticLayer(int neuronsCount, int inputsCount)
            : this(new BernoulliFunction(alpha: 1), neuronsCount, inputsCount) { }
        
        public StochasticLayer(IStochasticFunction function, int neuronsCount, int inputsCount)
            : base(neuronsCount, inputsCount, function)
        {
            neurons = new StochasticNeuron[neuronsCount];
            for (int i = 0; i < neurons.Length; i++)
                base.neurons[i] = this.neurons[i] =
                    new StochasticNeuron(inputsCount, function);
        }

        
        public override double[] Compute(double[] input)
        {
            double[] output = new double[neuronsCount];

            for (int i = 0; i < neurons.Length; i++)
                output[i] = neurons[i].Compute(input);

            this.output = output;

            return output;
        }
        
        public double[] Generate(double[] input)
        {
            double[] sample = new double[neuronsCount];
            double[] output = new double[neuronsCount];

            for (int i = 0; i < neurons.Length; i++)
            {
                sample[i] = neurons[i].Generate(input);
                output[i] = neurons[i].Output;
            }

            this.sample = sample;
            this.output = output;

            return sample;
        }
        
        public void CopyReversedWeightsFrom(StochasticLayer layer)
        {
            for (int i = 0; i < Neurons.Length; i++)
                for (int j = 0; j < inputsCount; j++)
                    this.Neurons[i].Weights[j] = layer.Neurons[j].Weights[i];
        }
    }
}
