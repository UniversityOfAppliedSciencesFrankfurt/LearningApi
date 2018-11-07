using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MLPerceptron.NeuralNetworkCore
{
    /// <summary>
    /// Class ActivationFunctions contains methods which implement the activation functions and their derivatives
    /// </summary>
    public class ActivationFunctions
    {
        /// <summary>
        /// This method returns the sigmoid of the weighted inputs
        /// </summary>
        /// <param name="val">The parameter on which the sigmoid function is applied</param>
        /// <returns>double</returns>
        public static double Sigmoid(double val)
        {
            return 1 / (1 + Math.Exp(-val));
        }

        /// <summary>
        /// This method returns the hyperbolic tangent value of the weighted inputs
        /// </summary>
        /// <param name="val">The parameter on which the hyperbolic tangent function is applied</param>
        /// <returns>double</returns>
        public static double HyperbolicTan(double val)
        {
            return (Math.Exp(2 * val) - 1) / (Math.Exp(2 * val) + 1);
        }

        /// <summary>
        /// This method returns the derivative of the sigmoid function
        /// </summary>
        /// <param name="val">The parameter on which the derivative of the sigmoid function is applied</param>
        /// <returns>double</returns>
        public static double DerivativeSigmoid(double val)
        {
            return (Sigmoid(val) * (1 - Sigmoid(val)));
        }

        /// <summary>
        /// This method returns the derivative of the hyperbolic tangent function
        /// </summary>
        /// <param name="val">The parameter on which the derivative of the hyperbolic tangent function is applied</param>
        /// <returns>double</returns>
        public static double DerivativeHyperbolicTan(double val)
        {
            return (1 - Math.Pow(HyperbolicTan(val),2));
        }

        /// <summary>
        /// This method returns the Leaky ReLU value applied to the weighted inputs
        /// </summary>
        /// <param name="val">The parameter on which the Leaky ReLU function is applied</param>
        /// <returns>double</returns>
        public static double LeakyReLU(double val)
        {
            return (Math.Max(0.01 * val, val));
        }

        /// <summary>
        /// This method returns the derivative of the Leaky ReLU
        /// </summary>
        /// <param name="val">The parameter on which the derivative of the Leaky ReLU function is applied</param>
        /// <returns>double</returns>
        public static double DerivativeLeakyReLU(double val)
        {
            if(val <= 0)
            {
                return 0.01;
            }
            else
            {
                return 1;
            }
        }

        public static double[] SoftMaxClassifier(double[] weightedip)
        {
            double sum = 0.0;

            double[] outputvector = new double[weightedip.Length];

            foreach (var item in weightedip)
            {
                sum += Math.Exp(item);
            }

            for(int i = 0; i < weightedip.Length; i++)
            {
                outputvector[i] = Math.Exp(weightedip[i]) / sum;
            }

            return outputvector;
        }
    }
}
