using System;
using NeuralNetworks.Core.Layers;



namespace NeuralNetworks.Core.Networks
{
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

    
        public virtual void Randomize()
        {
            foreach (Layer layer in layers)
            {
                layer.Randomize();
            }
        }

        #region :Stream management
        //public void Save(string fileName)
        //{
        //    FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
        //    Save(stream);
        //    // stream.Close(); for Net standard 2
        //    stream.Dispose();
        //}


        //public void Save(Stream stream)
        //{
        //    IFormatter formatter = new BinaryFormatter();
        //    formatter.Serialize(stream, this);
        //}


        //public static Network Load(string fileName)
        //{
        //    FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        //    Network network = Load(stream);
        //    // stream.Close(); for Net standard 2
        //    stream.Dispose();
        //    return network;
        //}


        //public static Network Load(Stream stream)
        //{
        //    IFormatter formatter = new BinaryFormatter();
        //    Network network = (Network)formatter.Deserialize(stream);
        //    return network;
        //}
        #endregion
    }
}