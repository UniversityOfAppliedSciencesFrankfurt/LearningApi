using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace NeuralNetworks.Core
{
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

        public double[] Output
        {
            get { return output; }
        }


        protected Network(int inputsCount, int layersCount)
        {
            this.inputsCount = Math.Max(1, inputsCount);
            this.layersCount = Math.Max(1, layersCount);
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
        /// <remarks><para>The actual network's output vecor is determined by layers,
        /// which comprise the layer - represents an output vector of the last layer
        /// of the network. The output vector is also stored in <see cref="Output"/> property.</para>
        /// 
        /// <para><note>The method may be called safely from multiple threads to compute network's
        /// output value for the specified input values. However, the value of
        /// <see cref="Output"/> property in multi-threaded environment is not predictable,
        /// since it may hold network's output computed from any of the caller threads. Multi-threaded
        /// access to the method is useful in those cases when it is required to improve performance
        /// by utilizing several threads and the computation is based on the immediate return value
        /// of the method, but not on network's output property.</note></para>
        /// </remarks>
        /// 
        public virtual double[] Compute(double[] input)
        {
            // local variable to avoid mutlithread conflicts
            double[] output = input;

            // compute each layer
            for (int i = 0; i < layers.Length; i++)
            {
                output = layers[i].Compute(output);
            }

            // assign output property as well (works correctly for single threaded usage)
            this.output = output;

            return output;
        }

        /// <summary>
        /// Randomize layers of the network.
        /// </summary>
        /// 
        /// <remarks>Randomizes network's layers by calling <see cref="Layer.Randomize"/> method
        /// of each layer.</remarks>
        /// 
        public virtual void Randomize()
        {
            foreach (Layer layer in layers)
            {
                layer.Randomize();
            }
        }

        /// <summary>
        /// Save network to specified file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to save network into.</param>
        /// 
        /// <remarks><para>The neural network is saved using .NET serialization (binary formatter is used).</para></remarks>
        /// 
        public void Save(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Save(stream);
            stream.Close();
        }

        /// <summary>
        /// Save network to specified file.
        /// </summary>
        /// 
        /// <param name="stream">Stream to save network into.</param>
        /// 
        /// <remarks><para>The neural network is saved using .NET serialization (binary formatter is used).</para></remarks>
        /// 
        public void Save(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
        }

        /// <summary>
        /// Load network from specified file.
        /// </summary>
        /// 
        /// <param name="fileName">File name to load network from.</param>
        /// 
        /// <returns>Returns instance of <see cref="Network"/> class with all properties initialized from file.</returns>
        /// 
        /// <remarks><para>Neural network is loaded from file using .NET serialization (binary formater is used).</para></remarks>
        /// 
        public static Network Load(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Network network = Load(stream);
            stream.Close();

            return network;
        }

        /// <summary>
        /// Load network from specified file.
        /// </summary>
        /// 
        /// <param name="stream">Stream to load network from.</param>
        /// 
        /// <returns>Returns instance of <see cref="Network"/> class with all properties initialized from file.</returns>
        /// 
        /// <remarks><para>Neural network is loaded from file using .NET serialization (binary formater is used).</para></remarks>
        /// 
        public static Network Load(Stream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            Network network = (Network)formatter.Deserialize(stream);
            return network;
        }
    }
}