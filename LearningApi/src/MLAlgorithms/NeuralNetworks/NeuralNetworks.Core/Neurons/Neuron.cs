using System;
using System.Collections.Generic;
using System.Text;
using LearningFoundation.Math;
using LearningFoundation.Statistics;
namespace NeuralNetworks.Core.Neurons
{
[Serializable]
    public abstract class Neuron
    {
      
        protected int inputsCount = 0;

      
        protected double[] weights = null;

      
        protected double output = 0;

     
        protected IRandomNumberGenerator<double> rand =
            new UniformContinuousDistribution();

        public IRandomNumberGenerator<double> RandGenerator
        {
            get { return rand; }
            set { rand = value; }
        }


     
        public int InputsCount
        {
            get { return inputsCount; }
        }

    
        public double Output
        {
            get { return output; }
        }


     
        public double[] Weights
        {
            get { return weights; }
        }

    
        protected Neuron(int inputs)
        {
            // allocate weights
            inputsCount = Math.Max(1, inputs);
            weights = new double[inputsCount];

            // randomize the neuron
            Randomize();
        }

     
        /// 
        public virtual void Randomize()
        {
            rand.Generate(weights.Length, weights);
        }

 
        public abstract double Compute(double[] input);
    }
}
