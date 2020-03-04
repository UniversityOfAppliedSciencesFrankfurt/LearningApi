using System;
using System.Collections.Generic;
using System.IO;
using LearningFoundation;
using System.Collections;

namespace SelfOrganizingMap
{
    /// <summary>Class of Map with initialization of Neurons ,iterations. m_Length, m_Dimensions, and m_Error.
    /// 
    /// </summary>A list of Labels and the corresponding pattern is initialized. 
    ///
    public class Map : IPipelineModule<List<double[]>, ICollection<Neuron>>
    {
        private Neuron[,] m_Outputs;  // Collection of weights.
        private int m_Iteration;      // Current m_Iteration.
        private int m_Length;        // Side m_Length of output grid.
        private int m_Dimensions;    // Number of input m_Dimensions. 
        private Random m_Rnd = new Random();
        private double m_Error;

        private List<string> labels = new List<string>();
        private List<double[]> patterns = new List<double[]>();
       
        /// The data is given in form of a list with a pattern
        /// 
        /// 
        /// <param name="data">list of data</param>
        /// <param name="ctx">context</param>
        /// <returns>returns the map</returns>
        /// 
        public ICollection<Neuron> Run(List<double[]> data, IContext ctx)
        {
            this.patterns = data;

            Initialise();
            NormalisePatterns();
            Train(0.0000001);

            return DumpCoordinates();
        }
        /// <summary> The desired input dimension is given.
        /// 
        /// </summary> The Grid lattice onto which this dimension to be mapped is given.
        /// <param name="dimensions">in this example 3</param>
        /// <param name="length">10x10 is grid</param>
        /// <param name="error">0.000001</param>
        public Map(int dimensions, int length, double error)
        {
            this.m_Length = length;
            this.m_Dimensions = dimensions;
            this.m_Error = error;
        }

        /// <summary>
        /// An array is initizalized with random double value.
        /// A random variable is chosen
        /// </summary>
        private void Initialise()
        {
            m_Outputs = new Neuron[m_Length, m_Length];
            for (int i = 0; i < m_Length; i++)
            {
                for (int j = 0; j < m_Length; j++)
                {
                    m_Outputs[i, j] = new Neuron(i, j, m_Length);
                    m_Outputs[i, j].m_Weights = new double[m_Dimensions];
                    for (int k = 0; k < m_Dimensions; k++)
                    {
                        m_Outputs[i, j].m_Weights[k] = m_Rnd.NextDouble();
                    }
                }
            }
        }

        /// <summary>  The input pattern is normalized 
        /// Every input pattern is normalized with the average of the total inputs
        /// </summary> 
        private void NormalisePatterns()
        {
            for (int j = 0; j < m_Dimensions; j++)
            {
                double sum = 0;
                for (int i = 0; i < patterns.Count; i++)
                {
                    sum += patterns[i][j];
                }
                double average = sum / patterns.Count;
                for (int i = 0; i < patterns.Count; i++)
                {
                    patterns[i][j] = patterns[i][j] / average; //pattern[0][0] = 48, average= 5, then pattern[0][0] = 9.6
                }
            }
        }
        /// <summary> 
        /// A new list of Training list with pattern is
        /// </summary>
        /// <param name="maxError">0.000001</param>
        private void Train(double maxError)
        {
            double currentError = double.MaxValue;
            while (currentError > maxError)
            {
                currentError = 0;
                List<double[]> TrainingSet = new List<double[]>();//List(double[],double[],double[],double[]...)
                foreach (double[] pattern in patterns)
                {
                    TrainingSet.Add(pattern); //Adds the nely created pattern to a new list of 1D double array
                }
                for (int i = 0; i < patterns.Count; i++)
                {
                    double[] pattern = TrainingSet[m_Rnd.Next(patterns.Count - i)];//patterns.Count=5, m_Rnd.Next(5)
                    currentError += TrainPattern(pattern);
                    TrainingSet.Remove(pattern);
                }
                Console.WriteLine(currentError.ToString("0.0000001"));
            }
        }
        /// <summary> 
        /// Total Value of error is calculated and this calculates the current error.
        /// </summary>
        /// <param name="pattern">double</param>
        /// <returns>Absolute value</returns>
        private double TrainPattern(double[] pattern)
        {
            double error = 0;
            Neuron winner = Winner(pattern);
            for (int i = 0; i < m_Length; i++)
            {
                for (int j = 0; j < m_Length; j++)
                {
                    error += m_Outputs[i, j].UpdateWeights(pattern, winner, m_Iteration);
                }
            }
            m_Iteration++;
            return Math.Abs(error / (m_Length * m_Length));
        }
        /// <summary>The result from the class Neuron is stored on Var r.
        /// 
        /// </summary>
        /// <returns>r</returns>
        private ICollection<Neuron> DumpCoordinates()
        {
            List<Neuron> ResultNeuron = new List<Neuron>();
            for (int i = 0; i < patterns.Count; i++)
            {
                Neuron n = Winner(patterns[i]);
                ResultNeuron.Add(n);
            }
            var r = ResultNeuron.ToArray();
            return r;
        }
        /// <summary> 
        /// Calculates the winner neuron.
        /// </summary> 
        /// <param name="pattern">double</param>
        /// <returns>Winner</returns>
        private Neuron Winner(double[] pattern)
        {
            Neuron winner = null;
            double min = double.MaxValue;
            for (int i = 0; i < m_Length; i++)
                for (int j = 0; j < m_Length; j++)
                {
                    double d = Distance(pattern, m_Outputs[i, j].m_Weights);
                    if (d < min)
                    {
                        min = d;
                        winner = m_Outputs[i, j];
                    }
                }
            return winner;
        }
        /// <summary> Eucledian distance is determined with the square root of the respective vectors
        /// 
        /// </summary>
        /// <param name="output_vector1">double</param>
        /// <param name="output_vector2">double</param>
        /// <returns>value</returns>
        private double Distance(double[] output_vector1, double[] output_vector2)
        {
            double value = 0;
            for (int i = 0; i < output_vector1.Length; i++)
            {
                value += Math.Pow((output_vector1[i] - output_vector2[i]), 2); //(x-y)^2
            }
            return Math.Sqrt(value);//sqrt[(x1-y1)^2 +(x2-y2)^2+...]
        }
    }
}
