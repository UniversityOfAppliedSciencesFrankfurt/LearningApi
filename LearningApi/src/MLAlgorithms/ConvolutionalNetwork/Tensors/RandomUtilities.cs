using System;

namespace ConvolutionalNetworks.Tensor
{
	/// <summary>
	/// Random Value Generator Class
	/// </summary>
    public static class RandomUtilities
    {
        private static readonly Random Random = new Random(Seed);
        private static double val;
        private static bool returnVal;

        /// <summary>
        /// Random Value Seed Generator
        /// </summary>
        public static int Seed => (int) 123;

		/// <summary>
		/// Create a Gaussian Random normalized random number
		/// </summary>
		/// <returns>Random number</returns>
        public static double GaussianRandom()
        {
            if (returnVal)
            {
                returnVal = false;
                return val;
            }

            double r = 0, u = 0, v = 0;
			
            lock (Random)
            {
                while (r == 0 || r > 1)
                {
                    u = 2*Random.NextDouble() - 1;
                    v = 2*Random.NextDouble() - 1;
                    r = u*u + v*v;
                }
            }

            var c = Math.Sqrt(-2 * Math.Log(r) / r);
            val = v * c; 
            returnVal = true;

            return u * c;
        }

		/// <summary>
		/// Returns the next random number in the distribution sequence
		/// </summary>
		/// <returns>Random number</returns>
        public static double NextDouble()
        {
            return Random.NextDouble();
        }

		/// <summary>
		/// Returns a random number under a mean and standard deviation
		/// </summary>
		/// <param name="mu">Mean of the distribution</param>
		/// <param name="std">Standard deviation of the distribution sequence</param>
		/// <returns>Random number generator</returns>
        public static double Randn(double mu, double std)
        {
            return mu + GaussianRandom() * std;
        }

		/// <summary>
		///  Returns a random number under a mean and standard deviation
		/// </summary>
		/// <param name="length">Length of the sequence</param>
		/// <param name="mu">Mean of the distribution</param>
		/// <param name="std">Standard deviation of the distribution sequence</param>
		/// <param name="posisitveOnly">Only positive values of the distribution if this flag is set</param>
		/// <returns>Random number sequence</returns>
		public static double[] RandomDoubleArray(long length, double mu = 0.0, double std = 1.0, bool posisitveOnly = false)
        {
            var values = new double[length];

            for (var i = 0; i < length; i++)
            {
                values[i] = Randn(mu, std);
                if (posisitveOnly)
                {
                    values[i] = Math.Abs(values[i]);
                }
            }

            return values;
        }
		/// <summary>
		///  Returns a random number under a mean and standard deviation
		/// </summary>
		/// <param name="length">Length of the sequence</param>
		/// <param name="mu">Mean of the distribution</param>
		/// <param name="std">Standard deviation of the distribution sequence</param>
		/// <param name="posisitveOnly">Only positive values of the distribution if this flag is set</param>
		/// <returns>Random number sequence</returns>
		public static float[] RandomSingleArray(long length, double mu = 0.0, double std = 1.0, bool posisitveOnly = false)
        {
            var values = new float[length];

            for (var i = 0; i < length; i++)
            {
                values[i] = (float) Randn(mu, std);
                if (posisitveOnly)
                {
                    values[i] = Math.Abs(values[i]);
                }
            }

            return values;
        }
    }
}
