﻿using System;
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
    [Serializable]
    public abstract class Layer
    {
        /// <summary>
        /// Layer's inputs count.
        /// </summary>
        protected int inputsCount = 0;

        /// <summary>
        /// Layer's neurons count.
        /// </summary>
        protected int neuronsCount = 0;

        /// <summary>
        /// Layer's neurons.
        /// </summary>
        protected Neuron[] neurons;

        /// <summary>
        /// Layer's output vector.
        /// </summary>
        protected double[] output;

        /// <summary>
        /// Layer's inputs count.
        /// </summary>
        public int InputsCount
        {
            get { return inputsCount; }
        }

        /// <summary>
        /// Layer's neurons.
        /// </summary>      
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
        /// 
        /// <para><note>The property is not initialized until
        /// <see cref="Compute"/> method is called.</note></para>
        /// </remarks>
        /// 
        public double[] Output
        {
            get { return output; }
        }

        /// <summary>
        /// Initializes a new instance of the Layer class.
        /// </summary>
        /// 
        /// <param name="neuronsCount">Layer's neurons count.</param>
        /// <param name="inputsCount">Layer's inputs count.</param>
        /// 
        /// <remarks>Protected contructor, which initializes <see cref="inputsCount"/>,
        /// <see cref="neuronsCount"/> and <see cref="neurons"/> members.</remarks>
        /// 
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
