using System;
using NeuralNetworks.Core.Neurons;
namespace NeuralNetworks.Core.Layers
{   
    /// <summary>
    /// Base neural layer class.
    /// </summary>
    /// 
    /// <remarks>This is a base neural layer class, which represents
    /// collection of neurons.</remarks>
    /// 
  
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

        /// <summary>
        /// Layer's output vector.
        /// </summary>
        /// 
        /// <remarks><para>The calculation way of layer's output vector is determined by neurons,
        /// which comprise the layer.</para>
     
        public double[] Output
        {
            get { return output; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Layer"/> class.
        /// </summary>
        /// 
        /// <param name="neuronsCount">Layer's neurons count.</param>
        /// <param name="inputsCount">Layer's inputs count.</param>
       
       
        protected Layer(int neuronsCount, int inputsCount)
        {
            this.inputsCount = Math.Max(1, inputsCount);
            this.neuronsCount = Math.Max(1, neuronsCount);
            // create collection of neurons
            neurons = new Neuron[this.neuronsCount];
        }

        /// <summary>
        /// Compute output vector of the layer.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns layer's output vector.</returns>
        /// 
        /// <remarks><para>The actual layer's output vector is determined by neurons,
        /// which comprise the layer - consists of output values of layer's neurons.
        /// The output vector is also stored in <see cref="Output"/> property.</para>
        /// 
        /// <para><note>The method may be called safely from multiple threads to compute layer's
        /// output value for the specified input values. However, the value of
        /// <see cref="Output"/> property in multi-threaded environment is not predictable,
        /// since it may hold layer's output computed from any of the caller threads. Multi-threaded
        /// access to the method is useful in those cases when it is required to improve performance
        /// by utilizing several threads and the computation is based on the immediate return value
        /// of the method, but not on layer's output property.</note></para>
        /// </remarks>
        /// 
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

        /// <summary>
        /// Randomize neurons of the layer.
        /// </summary>
        /// 
        /// <remarks>Randomizes layer's neurons by calling <see cref="Neuron.Randomize"/> method
        /// of each neuron.</remarks>
        /// 
        public virtual void Randomize()
        {
            foreach (Neuron neuron in neurons)
                neuron.Randomize();
        }
    }
}
