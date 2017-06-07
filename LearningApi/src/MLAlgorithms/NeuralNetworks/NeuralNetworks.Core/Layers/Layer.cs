using System;
using NeuralNetworks.Core.Neurons;

namespace NeuralNetworks.Core.Layers
{
    [Serializable]

    public abstract class Layer
    {
      
        /// Layer's inputs count.
      
        protected int inputsCount = 0;

       
        /// Layer's neurons count.
       
        protected int neuronsCount = 0;

       
        /// Layer's neurons.
        
        protected Neuron[] neurons;

       
        /// Layer's output vector.
        
        protected double[] output;

      
        /// Layer's inputs count.
        
        public int InputsCount
        {
            get { return inputsCount; }
        }

       
        /// Layer's neurons.
       
        public Neuron[] Neurons
        {
            get { return neurons; }
        }

       
        /// Layer's output vector.
     
        public double[] Output
        {
            get { return output; }
        }

        
        /// Initializes a new instance of the <see cref="Layer"/> class.
        
       
       
        protected Layer(int neuronsCount, int inputsCount)
        {
            this.inputsCount = Math.Max(1, inputsCount);
            this.neuronsCount = Math.Max(1, neuronsCount);
            // create collection of neurons
            neurons = new Neuron[this.neuronsCount];
        }

       
        public virtual double[] Compute(double[] input)
        {
            // local variable to avoid mutlithread conflicts
            double[] output = new double[neuronsCount];

            // compute each neuron
            for (int i = 0; i < neurons.Length; i++)
                output[i] = neurons[i].Compute(input);

            // assign output property as well (works correctly for single threaded usage)
            this.output = output;

            return output;
        }

      
        public virtual void Randomize()
        {
            foreach (Neuron neuron in neurons)
                neuron.Randomize();
        }
    }
}
