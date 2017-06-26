using System;
using System.Threading;
using NeuralNetworks.Core.Layers;
using NeuralNet.RestrictedBoltzmannMachine;
using NeuralNetworks.Core.Neurons;
using System.Threading.Tasks;
using NeuralNetworks.Core.ActivationFunctions;
using LearningFoundation;
using LearningFoundation.MathFunction;
using NeuralNetworks.Core;
using NeuralNet.RestrictedBoltmannMachine;
using System.Linq;
using NeuralNetworks.Core.Networks;

namespace NeuralNet.RestrictedBoltzmannMachine
{
    public class RBMAlgorithm : NeuralNetCore
    {
        //RBM algorithm main variables
        private double m_Momentum = 0.9;
        private double m_LearningRate = 0.1;
        private double m_Decay = 0.01;
        private int m_Iterations = 5000;
        private int m_InputCount;
        private int m_HiddenNeurons;
        private int steps = 1;
        //Gradient Variable
        private double[][] m_weightsGradient;
        private double[] m_visibleBiasGradient;
        private double[] m_hiddenBiasGradient;
        //Neuron and layer update variables
        private double[][] m_weightsUpdates;
        private double[] m_visibleBiasUpdates;
        private double[] m_hiddenBiasUpdates;
      //
        private int inputsCount;
        private int hiddenCount;
        private StochasticLayer hidden;
        private StochasticLayer visible;
        [NonSerialized]
        private ParallelOptions parallelOptions;
        private ThreadLocal<ParallelStorage> storage;
        private IStochasticFunction m_StochasticFunction = new BernoulliFunction(alpha: 0.5);
        private Func<double, double> m_ActivationFunction = new BernoulliFunction(alpha: 0.5).Function;

        private int m_Dimensions;
        private double[] m_Weights;
        private double[] m_Errors;
        private double m_Threshold;



        #region Parallel Method implement
        /// <summary>
        ///   Gets or sets parallelization options.
        /// </summary>
        /// 
        public ParallelOptions ParallelOptions
        {
            get
            {
                if (parallelOptions == null)
                    parallelOptions = new ParallelOptions();
                return parallelOptions;
            }
            set { parallelOptions = value; }
        }
        /// <summary>
        /// Get or set parallelization value storage
        /// 
        /// </summary>
        private class ParallelStorage
        {
            public double[][] WeightGradient { get; set; }
            public double[] VisibleBiasGradient { get; set; }
            public double[] HiddenBiasGradient { get; set; }
            public double[] OriginalActivations { get; set; }
            public double[] OriginalProbability { get; set; }
            public double[] ReconstructedInput { get; set; }
            public double[] ReconstructedProbs { get; set; }
            public double ErrorSumOfSquares { get; set; }
            public ParallelStorage(int inputsCount, int hiddenCount)
            {
                WeightGradient = new double[inputsCount][];
                for (int i = 0; i < WeightGradient.Length; i++)
                    WeightGradient[i] = new double[hiddenCount];
                VisibleBiasGradient = new double[inputsCount];
                HiddenBiasGradient = new double[hiddenCount];
                OriginalActivations = new double[hiddenCount];
                OriginalProbability = new double[hiddenCount];
                ReconstructedInput = new double[inputsCount];
                ReconstructedProbs = new double[hiddenCount];
            }
            public ParallelStorage Clear()
            {
                ErrorSumOfSquares = 0;
                for (int i = 0; i < WeightGradient.Length; i++)
                    Array.Clear(WeightGradient[i], 0, WeightGradient[i].Length);
                Array.Clear(VisibleBiasGradient, 0, VisibleBiasGradient.Length);
                Array.Clear(HiddenBiasGradient, 0, HiddenBiasGradient.Length);
                return this;
            }
        }
        #endregion
        ///   Gets the visible layer of the machine.
        public StochasticLayer Visible
        {
            get { return visible; }
        }
        ///   Gets the hidden layer of the machine.
        public StochasticLayer Hidden
        {
            get { return hidden; }
        }


       

        /// <summary>
        ///   Gets or sets the momentum term of the
        ///   learning algorithm. Default is 0.9.
        /// </summary>
        /// 
        public double Momentum
        {
            get { return m_Momentum; }
            set { m_Momentum = value; }
        }
        /// <summary>
        ///   Gets or sets the Weight Decay constant
        ///   of the learning algorithm. Default is 0.01.
        /// </summary>
        /// 
        public double Decay
        {
            get { return m_Decay; }
            set { m_Decay = value; }
        }

        /// <summary>
        ///   Gets or sets the learning rate of the
        ///   learning algorithm.Default is 0.1.
        /// </summary>
        /// 
        public double LearningRate
        {
            get { return m_LearningRate; }
            set { m_LearningRate = value; }
        }
        public int Iteration
        {
            get { return m_Iterations; }
            set { m_Iterations = value; }
        }
        /// <summary>
        ///   Creates a new RBM algorithm.
        /// </summary>
        /// 
        /// <param name="network">The network to be trained.</param>
        /// 
        public RBMAlgorithm(int m_InputCount, int m_HiddenNeurons,
            double[][] samples, Func<double, double> activationfunction = null)
           
        {
            //m_InputCount = m_InputCount;
            //m_m_HiddenNeurons = m_HiddenNeurons;
          

            if (activationfunction != null)
                this.m_ActivationFunction = activationfunction;

              var m_network = new RestrictedBoltzmannMachine(m_InputCount,m_HiddenNeurons);            
                init(m_network.Hidden, m_network.Visible);
        }

        /// <summary>
        ///   Runs learning algorithm.
        /// </summary>
        /// 
        /// feature value :Array of input vectors
        /// 
        /// <returns>
        ///   Returns sum of learning errors.
        /// </returns>
        /// 
        public override IScore Run(double[][] featureValues, IContext ctx)
        {
            ///
            // var network = new RestrictedBoltzmannMachine(m_StochasticFunction, m_InputCount, m_m_HiddenNeurons);

            m_Dimensions = ctx.DataDescriptor.Features.Count();

            int numOfInputVectors = featureValues.Length;           
            m_Errors = new double[numOfInputVectors];
            // initialize the neuron weight
            m_Weights = new double[m_Dimensions];
            initializeWeights();

            double totalError = 0;
            var score = new RBMScore();
            ///
            for (int j = 0; j < m_Iterations; j++)
            {
                for (int i = 0; i < m_weightsGradient.Length; i++)
                    Array.Clear(m_weightsGradient[i], 0, m_weightsGradient[i].Length);
                Array.Clear(m_hiddenBiasGradient, 0, m_hiddenBiasGradient.Length);
                Array.Clear(m_visibleBiasGradient, 0, m_visibleBiasGradient.Length);


                /// calculate gradient and model error
                double error = ComputeGradient(featureValues);
                /// calculate weights updates
                CalculateUpdates(featureValues);
                /// update the network
                UpdateNetwork();
                 //return error;



                //for (int inputVectIndx = 0; inputVectIndx < numOfInputVectors; inputVectIndx++)
                //{
                //    // Calculate the output value with current weights.
                //    double calculatedOutput = calculateResult(featureValues[inputVectIndx], m_Dimensions);
                //    // Get expected output.
                //    double expectedOutput = featureValues[inputVectIndx][ctx.DataDescriptor.LabelIndex];
                //    // Error is difference between calculated output and expected output.
                //    double error = expectedOutput - calculatedOutput;
                //    this.m_Errors[inputVectIndx] = error;
                //    // Total error for all input vectors.
                //    totalError += error;
                //    if (error != 0)
                //    {
                //        // Y = W * X
                //        // error = expectedOutput - calculatedOutput
                //        // W = Y/X
                //        //
                //        // Updating of weights
                //        for (int dimensionIndx = 0; dimensionIndx < m_Dimensions; dimensionIndx++)
                //        {
                //            double delta = m_LearningRate * featureValues[inputVectIndx][dimensionIndx] * error;
                //            m_Weights[dimensionIndx] += delta;
                //        }
                //    }
                //    //
                //    // Updating of threshold
                //    this.m_Threshold += this.m_LearningRate * error;
                //}
                //// Debug.WriteLine($"{m_Weights[0]}, {m_Threshold}");
                //if (totalError == 0)
                //{
                //    score.Iterations = i;
                //    break;
                //}

            }
            score.Weights = this.m_Weights;
            score.Errors = this.m_Errors;
            score.TotolEpochError = totalError;
            ctx.Score = score;
            return ctx.Score;

        }
        private void init(StochasticLayer hidden, StochasticLayer visible)
        {
            this.hidden = hidden;
            this.visible = visible;
            inputsCount = hidden.InputsCount;
            hiddenCount = hidden.Neurons.Length;
            m_weightsGradient = new double[inputsCount][];
            for (int i = 0; i < m_weightsGradient.Length; i++)
                m_weightsGradient[i] = new double[hiddenCount];
            m_visibleBiasGradient = new double[inputsCount];
            m_hiddenBiasGradient = new double[hiddenCount];
            m_weightsUpdates = new double[inputsCount][];
            for (int i = 0; i < m_weightsUpdates.Length; i++)
                m_weightsUpdates[i] = new double[hiddenCount];
            m_visibleBiasUpdates = new double[inputsCount];
            m_hiddenBiasUpdates = new double[hiddenCount];
            storage = new ThreadLocal<ParallelStorage>(() =>
                new ParallelStorage(inputsCount, hiddenCount));
            this.ParallelOptions = new ParallelOptions();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private double ComputeGradient(double[][] input)
        {
            double errors = 0;
            Object lockObj = new Object();
            // For each training instance
            Parallel.For(0, input.Length,
                ParallelOptions,
                // Initialize
                () => storage.Value.Clear(),
                // Map
                (observationIndex, loopState, partial) =>
                {
                    var observation = input[observationIndex];
                    var probability = partial.OriginalProbability;
                    var activations = partial.OriginalActivations;
                    var reconstruction = partial.ReconstructedInput;
                    var reprobability = partial.ReconstructedProbs;
                    var weightGradient = partial.WeightGradient;
                    var hiddenGradient = partial.HiddenBiasGradient;
                    var visibleGradient = partial.VisibleBiasGradient;
                    // 1. Compute a forward pass. The network is being
                    //    driven by data, so we will gather activations
                    for (int j = 0; j < hidden.Neurons.Length; j++)
                    {
                        probability[j] = hidden.Neurons[j].Compute(observation);  // output probabilities
                        activations[j] = hidden.Neurons[j].Generate(probability[j]); // state activations
                    }
                    // 2. Reconstruct inputs from previous outputs
                    for (int j = 0; j < visible.Neurons.Length; j++)
                        reconstruction[j] = visible.Neurons[j].Compute(activations);
                    if (steps > 1)
                    {
                        // Perform Gibbs sampling
                        double[] current = probability;
                        for (int k = 0; k < steps - 1; k++)
                        {
                            for (int j = 0; j < probability.Length; j++)
                                probability[j] = hidden.Neurons[j].Compute(current);
                            for (int j = 0; j < reconstruction.Length; j++)
                                reconstruction[j] = visible.Neurons[j].Compute(probability);
                            current = reconstruction;
                        }
                    }
                    // 3. Compute outputs for the reconstruction. The network
                    //    is now being driven by reconstructions, so we should
                    //    gather the output probabilities without sampling
                    for (int j = 0; j < hidden.Neurons.Length; j++)
                        reprobability[j] = hidden.Neurons[j].Compute(reconstruction);
                    // 4.1. Compute positive associations
                    for (int k = 0; k < observation.Length; k++)
                        for (int j = 0; j < probability.Length; j++)
                            weightGradient[k][j] += observation[k] * probability[j];
                    for (int j = 0; j < hiddenGradient.Length; j++)
                        hiddenGradient[j] += probability[j];
                    for (int j = 0; j < visibleGradient.Length; j++)
                        visibleGradient[j] += observation[j];
                    // 4.2. Compute negative associations
                    for (int k = 0; k < reconstruction.Length; k++)
                        for (int j = 0; j < reprobability.Length; j++)
                            weightGradient[k][j] -= reconstruction[k] * reprobability[j];
                    for (int j = 0; j < reprobability.Length; j++)
                        hiddenGradient[j] -= reprobability[j];
                    for (int j = 0; j < reconstruction.Length; j++)
                        visibleGradient[j] -= reconstruction[j];
                    // Compute current error
                    for (int j = 0; j < observation.Length; j++)
                    {
                        double e = observation[j] - reconstruction[j];
                        partial.ErrorSumOfSquares += e * e;
                    }
                    return partial; // Report partial solution
                },
                // Reduce
                (partial) =>
                {
                    lock (lockObj)
                    {
                        // Accumulate partial solutions
                        for (int i = 0; i < m_weightsGradient.Length; i++)
                            for (int j = 0; j < m_weightsGradient[i].Length; j++)
                                m_weightsGradient[i][j] += partial.WeightGradient[i][j];
                        for (int i = 0; i < m_hiddenBiasGradient.Length; i++)
                            m_hiddenBiasGradient[i] += partial.HiddenBiasGradient[i];
                        for (int i = 0; i < m_visibleBiasGradient.Length; i++)
                            m_visibleBiasGradient[i] += partial.VisibleBiasGradient[i];
                        errors += partial.ErrorSumOfSquares;
                    }
                });
            return errors;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        private void CalculateUpdates(double[][] input)
        {
            double rate = m_LearningRate;
            // Assume all neurons in the layer have the same act function
            if (visible.Neurons[0].ActivationFunction is GaussianFunction)
                rate = m_LearningRate / (100 * input.Length);
            else rate = m_LearningRate / (input.Length);
            // 5. Compute gradient descent updates
            for (int i = 0; i < m_weightsGradient.Length; i++)
                for (int j = 0; j < m_weightsGradient[i].Length; j++)
                    m_weightsUpdates[i][j] = m_Momentum * m_weightsUpdates[i][j]
                        + (rate * m_weightsGradient[i][j]);
            for (int i = 0; i < m_hiddenBiasUpdates.Length; i++)
                m_hiddenBiasUpdates[i] = m_Momentum * m_hiddenBiasUpdates[i]
                        + (rate * m_hiddenBiasGradient[i]);
            for (int i = 0; i < m_visibleBiasUpdates.Length; i++)
                m_visibleBiasUpdates[i] = m_Momentum * m_visibleBiasUpdates[i]
                        + (rate * m_visibleBiasGradient[i]);
            Debug.Assert(!m_weightsGradient.HasNaN());
            Debug.Assert(!m_visibleBiasUpdates.HasNaN());
            Debug.Assert(!m_hiddenBiasUpdates.HasNaN());
        }
       
        private void UpdateNetwork()
        {
            // 6.1 Update hidden layer weights
            for (int i = 0; i < hidden.Neurons.Length; i++)
            {
                StochasticNeuron neuron = hidden.Neurons[i];
                for (int j = 0; j < neuron.Weights.Length; j++)
                    neuron.Weights[j] += m_weightsUpdates[j][i] - m_LearningRate * m_Decay * neuron.Weights[j];
                neuron.Threshold += m_hiddenBiasUpdates[i];
            }
            // 6.2 Update visible layer with reverse weights
            for (int i = 0; i < visible.Neurons.Length; i++)
                visible.Neurons[i].Threshold += m_visibleBiasUpdates[i];
            visible.CopyReversedWeightsFrom(hidden);
        }
        public override double[] Predict(double[][] data, IContext ctx)
        {
            double[] results = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                results[i] = calculateResult(data[i], ctx.DataDescriptor.Features.Length);
            }
            return results;


            //neuronscount - data.length - neuron.length

            //for (int i = 0; i < neurons.Length; i++)
            //    output[i] = neurons[i].Compute(input);

            //this.output = output;

            //return output;

        }
        private double calculateResult(double[] input, int numOfFeatures) //compute output value of neuron
        {
            double result = 0.0;
            for (int i = 0; i < numOfFeatures; i++)
            {
                result += m_Weights[i] * input[i];
            }
            result += this.m_Threshold;
            return m_ActivationFunction(result);


            // weights.Length (RBM) - numOfFeatures - ctx.datadescriptor.feature.length



        }
        private void initializeWeights()
        {
            //Random rnd = new Random();
            //for (int i = 0; i < m_Dimensions; i++)
            //{
            //    m_Weights[i] = rnd.Next();
            //}
        }

    }
}
