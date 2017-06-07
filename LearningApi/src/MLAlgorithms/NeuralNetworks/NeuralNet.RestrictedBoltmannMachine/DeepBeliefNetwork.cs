using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NeuralNetworks.Core.ActivationFunctions;
using NeuralNetworks.Core.Layers;
using NeuralNetworks.Core.Neurons;
using NeuralNetworks.Core.Networks;
using LearningFoundation;


namespace NeuralNet.RestrictedBoltmannMachine
{
    [Serializable]
    public class DeepBeliefNetwork : ActivationNetwork
    {

        private List<RestrictedBoltzmannMachine> machines;

      
        public int OutputCount
        {
            get
            {
                if (machines.Count == 0) return 0;
                return machines[machines.Count - 1].Hidden.Neurons.Length;
            }
        }

       
        public IList<RestrictedBoltzmannMachine> Machines
        {
            get { return machines; }
        }

    
        public DeepBeliefNetwork(int inputsCount, params int[] hiddenNeurons)
            : this(new BernoulliFunction(alpha: 1), inputsCount, hiddenNeurons) { }


     
        public DeepBeliefNetwork(IStochasticFunction function, int inputsCount, params int[] hiddenNeurons)
            : base(function, inputsCount, hiddenNeurons)
        {
            machines = new List<RestrictedBoltzmannMachine>();

            // Create first layer
            machines.Add(new RestrictedBoltzmannMachine(
                hidden: new StochasticLayer(function, hiddenNeurons[0], inputsCount),
                visible: new StochasticLayer(function, inputsCount, hiddenNeurons[0]))
            );

            // Create other layers
            for (int i = 1; i < hiddenNeurons.Length; i++)
            {
                machines.Add(new RestrictedBoltzmannMachine(
                    hidden: new StochasticLayer(function, hiddenNeurons[i], hiddenNeurons[i - 1]),
                    visible: new StochasticLayer(function, hiddenNeurons[i - 1], hiddenNeurons[i])));
            }

            // Override AForge layers
            layers = new Layer[machines.Count];
            for (int i = 0; i < layers.Length; i++)
                layers[i] = machines[i].Hidden;
        }

        
        public DeepBeliefNetwork(int inputsCount, params RestrictedBoltzmannMachine[] layers)
            : base(null, inputsCount, new int[layers.Length])
        {
            machines = new List<RestrictedBoltzmannMachine>(layers);

            // Override AForge layers
            base.layers = new Layer[machines.Count];
            for (int i = 0; i < layers.Length; i++)
                base.layers[i] = machines[i].Hidden;
        }

       
        public override double[] Compute(double[] input)
        {
            double[] output = input;

            foreach (RestrictedBoltzmannMachine layer in machines)
                output = layer.Hidden.Compute(output);

            return output;
        }

       
        public double[] Compute(double[] input, int layerIndex)
        {
            double[] output = input;

            for (int i = 0; i <= layerIndex; i++)
                output = machines[i].Hidden.Compute(output);

            return output;
        }

      
        public double[] Reconstruct(double[] output)
        {
            double[] input = output;

            for (int i = machines.Count - 1; i >= 0; i--)
                input = machines[i].Visible.Compute(input);

            return input;
        }

       
        public double[] Reconstruct(double[] output, int layerIndex)
        {
            double[] input = output;

            for (int i = layerIndex; i >= 0; i--)
                input = machines[i].Visible.Compute(input);

            return input;
        }

     
        public double[] GenerateOutput(double[] input)
        {
            double[] output = input;

            foreach (RestrictedBoltzmannMachine layer in machines)
                output = layer.Hidden.Generate(output);

            return output;
        }

      
        public double[] GenerateOutput(double[] input, int layerIndex)
        {
            double[] output = input;

            for (int i = 0; i <= layerIndex; i++)
                output = machines[i].Hidden.Generate(output);

            return output;
        }

        
        public double[] GenerateInput(double[] output)
        {
            double[] input = output;

            for (int i = layers.Length - 1; i >= 0; i--)
                input = machines[i].Visible.Generate(input);

            return input;
        }


      
        public void Push(int neurons)
        {
            Push(neurons, new BernoulliFunction(alpha: 1));
        }

       
        public void Push(int neurons, IStochasticFunction function)
        {
            Push(neurons, function, function);
        }

        public void Push(int neurons, IStochasticFunction visibleFunction, IStochasticFunction hiddenFunction)
        {
            int lastLayerNeurons;

            if (machines.Count > 0)
                lastLayerNeurons = machines[machines.Count - 1].Hidden.Neurons.Length;
            else lastLayerNeurons = inputsCount;

            machines.Add(new RestrictedBoltzmannMachine(
                hidden: new StochasticLayer(hiddenFunction, neurons, lastLayerNeurons),
                visible: new StochasticLayer(visibleFunction, lastLayerNeurons, neurons)));

            // Override AForge layers
            layers = new Layer[machines.Count];
            for (int i = 0; i < layers.Length; i++)
                layers[i] = machines[i].Hidden;
        }

       
        public void Push(RestrictedBoltzmannMachine network)
        {
            int lastLayerNeurons;

            if (machines.Count > 0)
                lastLayerNeurons = machines[machines.Count - 1].Hidden.Neurons.Length;
            else lastLayerNeurons = inputsCount;

            machines.Add(network);

            // Override AForge layers
            layers = new Layer[machines.Count];
            for (int i = 0; i < layers.Length; i++)
                layers[i] = machines[i].Hidden;
        }

       
        public void Pop()
        {
            if (machines.Count == 0)
                return;

            machines.RemoveAt(machines.Count - 1);

            // Override AForge layers
            layers = new Layer[machines.Count];
            for (int i = 0; i < layers.Length; i++)
                layers[i] = machines[i].Hidden;
        }

       
        public void UpdateVisibleWeights()
        {
            foreach (var machine in machines)
                machine.UpdateVisibleWeights();
        }

        
        public static DeepBeliefNetwork CreateGaussianBernoulli(int inputsCount, params int[] hiddenNeurons)
        {
            DeepBeliefNetwork network = new DeepBeliefNetwork(inputsCount, hiddenNeurons);

            GaussianFunction gaussian = new GaussianFunction();
            foreach (StochasticNeuron neuron in network.machines[0].Visible.Neurons)
                neuron.ActivationFunction = gaussian;

            return network;
        }

       
        public static DeepBeliefNetwork CreateMixedNetwork(IStochasticFunction visible,
            IStochasticFunction hidden, int inputsCount, params int[] hiddenNeurons)
        {
            DeepBeliefNetwork network = new DeepBeliefNetwork(hidden, inputsCount, hiddenNeurons);

            foreach (StochasticNeuron neuron in network.machines[0].Visible.Neurons)
                neuron.ActivationFunction = visible;

            return network;
        }

       
        //public new void Save(Stream stream)
        //{
        //    BinaryFormatter b = new BinaryFormatter();
        //    b.Serialize(stream, this);
        //}

       
        //public new void Save(string path)
        //{
        //    using (FileStream fs = new FileStream(path, FileMode.Create))
        //    {
        //        Save(fs);
        //    }
        //}

       
        //public static new DeepBeliefNetwork Load(Stream stream)
        //{
        //    BinaryFormatter b = new BinaryFormatter();
        //    return (DeepBeliefNetwork)b.Deserialize(stream);
        //}

        
        //public static new DeepBeliefNetwork Load(string path)
        //{
        //    using (FileStream fs = new FileStream(path, FileMode.Open))
        //    {
        //        return Load(fs);
        //    }
        //}

    }
}
