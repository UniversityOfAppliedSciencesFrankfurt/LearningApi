using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SelfOrganizingMap
{
    /// <summary> 
    /// A class of neuron with weights , x and y , length and learning factor is defined.
    /// </summary>
    public class Neuron
    {
        public double[] m_Weights;
        public int m_X;
        public int m_Y;
        private int m_Length;
        private double m_Nf;

        /// <summary> 
        /// The neuron with input x and y are defined and length is defined.
        /// </summary>
        /// <param name="x">integer</param>
        /// <param name="y">integer</param>
        /// <param name="length">integer</param>
        public Neuron(int x, int y, int length)
        {
            m_X = x;
            m_Y = y;
            this.m_Length = length;
            m_Nf = 1000 / Math.Log(length);
        }
        /// <summary>  
        /// Calculate Gaussian function.
        /// </summary>
        /// <param name="win">Neuron</param>
        /// <param name="it">integer</param>
        /// <returns></returns>
        private double Gauss(Neuron win, int it)
        {
            double distance = Math.Sqrt(Math.Pow(win.m_X - m_X, 2) + Math.Pow(win.m_Y - m_Y, 2));
            return Math.Exp(-Math.Pow(distance, 2) / (Math.Pow(Strength(it), 2)));
        }
        /// <summary> 
        /// Returns Learning rate.
        /// </summary>
        /// <param name="it">integer</param>
        /// <returns>Learning rate</returns>
        private double LearningRate(int it)
        {
            return Math.Exp(-it / 1000) * 0.1;
        }
        /// <summary> 
        /// the value of strength is calculated( sigma)
        /// </summary>
        /// <param name="it">integer</param>
        /// <returns>Double</returns>
        private double Strength(int it)
        {
            return Math.Exp(-it / m_Nf) * m_Length;
        }
        /// <summary> 
        /// each pattern  is updated with the new weight.
        /// value of  delta is added that is the normal Hebbian term 
        /// </summary> 
        /// <param name="pattern">double </param>
        /// <param name="winner">neuron</param>
        /// <param name="it"></param>
        /// <returns>sum</returns>
        public double UpdateWeights(double[] pattern, Neuron winner, int it)
        {
            double sum = 0;
            for (int i = 0; i < m_Weights.Length; i++)
            {
                double delta = LearningRate(it) * Gauss(winner, it) * (pattern[i] - m_Weights[i]);
                m_Weights[i] += delta;
                sum += delta;
            }
            return sum / m_Weights.Length;
        }
    }
}
