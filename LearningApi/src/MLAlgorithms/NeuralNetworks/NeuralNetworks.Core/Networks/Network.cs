using System;
using NeuralNetworks.Core.Layers;

namespace NeuralNetworks.Core.Networks
{
    /// <summary>
    /// Base neural network class.
    /// </summary>
    /// 
    /// <remarks>This is a base neural netwok class, which represents
    /// collection of neuron's layers.</remarks>
    /// 
    [Serializable]
    public abstract class Network
    {
        /// <summary>
        /// Network's inputs count.
        /// </summary>
        protected int inputsCount;

        /// <summary>
        /// Network's layers count.
        /// </summary>
        protected int layersCount;

        /// <summary>
        /// Network's layers.
        /// </summary>
        protected Layer[] layers;

        /// <summary>
        /// Network's output vector.
        /// </summary>
        protected double[] output;

        /// <summary>
        /// Network's inputs count.
        /// </summary>
        public int InputsCount
        {
            get { return inputsCount; }
        }

        /// <summary>
        /// Network's layers.
        /// </summary>
        public Layer[] Layers
        {
            get { return layers; }
        }

        /// <summary>
        /// Network's output vector.
        /// </summary>
        /// 
        /// <remarks><para>The calculation way of network's output vector is determined by
        /// layers, which comprise the network.</para>
        /// 
        /// <para><note>The property is not initialized until Compute
        /// method is called.</note></para>
        /// </remarks>
        /// 
        public double[] Output
        {
            get { return output; }
        }

        /// <summary>
        /// Initializes a new instance of the Network class.
        /// </summary>
        /// 
        /// <param name="inputsCount">Network's inputs count.</param>
        /// <param name="layersCount">Network's layers count.</param>
        /// 
        /// <remarks>Protected constructor, which initializes inputcount,
        /// layercount and layers members.</remarks>
        /// 
        protected Network( int inputsCount, int layersCount )
        {
            this.inputsCount = Math.Max( 1, inputsCount );
            this.layersCount = Math.Max( 1, layersCount );
            // create collection of layers
            this.layers = new Layer[this.layersCount];
        }

        /// <summary>
        /// Compute output vector of the network.
        /// </summary>
        /// 
        /// <param name="input">Input vector.</param>
        /// 
        /// <returns>Returns network's output vector.</returns>
        /// 
        public virtual double[] Compute( double[] input )
        {
            // local variable to avoid mutlithread conflicts
            double[] output = input;

            // compute each layer
            for (int i = 0; i < layers.Length; i++)
            {
                output = layers[i].Compute( output );
            }

            // assign output property as well (works correctly for single threaded usage)
            this.output = output;

            return output;
        }

        /// <summary>
        /// Randomize layers of the network.
        /// </summary>
        /// 
        /// <remarks>Randomizes network's layers by calling Randomize method
        /// of each layer.</remarks>
        /// 
        public virtual void Randomize()
        {
            foreach (Layer layer in layers)
            {
                layer.Randomize( );
            }
        }
    }
}