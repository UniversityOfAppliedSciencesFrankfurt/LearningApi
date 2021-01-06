using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HiddenMarkov;

namespace HiddenMarkovLibUnitTest
{
    /// <summary>
    /// Summary description for MaximumLikelihood
    /// </summary>
    [TestClass]
    public class HiddenMarkovModel
    {
        [TestMethod]
        public void ErgodicTest()
        {
            // Create a Ergodic HMM with 2 states and 4 symbols
            var hiddenMarkovAlgorithm = new HiddenMarkovAlgorithm
                (4, 2, HiddenMarkov.HiddenMarkovModel.HiddenMarkovModelType.Ergodic);

            double[,] A, B;
            double[] pi;

            A = new double[,]
           {
                { 0.5, 0.5 },
                { 0.5, 0.5 }
           };

            B = new double[,]
            {
                { 0.25, 0.25, 0.25, 0.25 },
                { 0.25, 0.25, 0.25, 0.25 },
            };

            pi = new double[] { 1, 0 };

            Assert.AreEqual(2, hiddenMarkovAlgorithm.m_MyModel.States);
            Assert.AreEqual(4, hiddenMarkovAlgorithm.m_MyModel.Symbols);
            Assert.IsTrue(A.Equals(hiddenMarkovAlgorithm.m_MyModel.Transitions));
            Assert.IsTrue(B.Equals(hiddenMarkovAlgorithm.m_MyModel.Emissions));
            Assert.IsTrue(pi.Equals(hiddenMarkovAlgorithm.m_MyModel.Probabilities));
        }

        [TestMethod]
        public void ForwardTest()
        {
            // Create a Forward HMM with 2 states and 4 symbols
            var hiddenMarkovAlgorithm = new HiddenMarkovAlgorithm
                (4, 2, HiddenMarkov.HiddenMarkovModel.HiddenMarkovModelType.Forward);

            double[,] A, B;
            double[] pi;

            A = new double[,]
          {
                { 0.5, 0.5 },
                { 0.0, 1.0 }
          };

            B = new double[,]
            {
                { 0.25, 0.25, 0.25, 0.25 },
                { 0.25, 0.25, 0.25, 0.25 },
            };

            pi = new double[] { 1, 0 };

            Assert.AreEqual(2, hiddenMarkovAlgorithm.m_MyModel.States);
            Assert.AreEqual(4, hiddenMarkovAlgorithm.m_MyModel.Symbols);
            Assert.IsTrue(A.Equals(hiddenMarkovAlgorithm.m_MyModel.Transitions));
            Assert.IsTrue(B.Equals(hiddenMarkovAlgorithm.m_MyModel.Emissions));
            Assert.IsTrue(pi.Equals(hiddenMarkovAlgorithm.m_MyModel.Probabilities));
        }

        [TestMethod]
        public void LearnTest()
        {
            // We will try to create a Hidden Markov Model which
            //  can detect if a given sequence starts with a zero
            //  and has any number of ones after that.
            double[][] sequences = new double[][]
            {
                new double[] { 0,1,1,1,1,0,1,1,1,1 },
                new double[] { 0,1,1,1,0,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1         },
                new double[] { 0,1,1,1,1,1,1       },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
            };

            // Creates a new Hidden Markov Model with 3 states for
            //  an output alphabet of two characters (zero and one)
            var hmm = new HiddenMarkovAlgorithm
                (3, 2, HiddenMarkov.HiddenMarkovModel.HiddenMarkovModelType.Ergodic);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            HiddenMarkovContext myContext = new HiddenMarkovContext();
            myContext.tolerance = 0.0001;
            myContext.iterations = 0;
            hmm.Train(sequences, myContext);

            double[][] seq = new double[][]
            {

                new double[] { 0, 1       }, //0.999973
                new double[] { 0, 1, 1, 1 }, //0.916672
                new double[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }, //0.027687
                new double[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }, //0.027687
                new double[] { 1, 1       }, //0.000026
                new double[] { 1, 0, 0, 0 }, //0.000000
            };

            // Calculate the probability that the given
            //  sequences originated from the model
            HiddenMarkovResult myResult = (HiddenMarkovResult)hmm.Predict(seq, myContext);

            // Sequences starting with zero have higher probability.
            Assert.AreEqual(0.999973, myResult.Probability[0], 1e-6);
            Assert.AreEqual(0.916672, myResult.Probability[1], 1e-6);

            // Sequences which do not start with zero have much lesser probability.
            Assert.AreEqual(0.000026, myResult.Probability[2], 1e-6);
            Assert.AreEqual(0.000000, myResult.Probability[3], 1e-6);
        }

        [TestMethod]
        public void LearnTest2()
        {
            // We will try to create a Hidden Markov Model which
            //  can detect if a given sequence starts with a zero
            //  and has any number of ones after that.
            double[][] sequences = new double[][]
            {
                new double[] { 0,1,1,1,1,0,1,1,1,1 },
                new double[] { 0,1,1,1,0,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1         },
                new double[] { 0,1,1,1,1,1,1       },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
            };

            // Creates a new Hidden Markov Model with 3 states for
            //  an output alphabet of two characters (zero and one)
            var hmm = new HiddenMarkovAlgorithm
                (3, 2, HiddenMarkov.HiddenMarkovModel.HiddenMarkovModelType.Forward);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            HiddenMarkovContext myContext = new HiddenMarkovContext();
            myContext.tolerance = 0.0001;
            myContext.iterations = 0;
            hmm.Train(sequences, myContext);

            double[][] seq = new double[][]
            {
                new double[] { 0, 1       },                //0.964285
                new double[] { 0, 1, 1, 1 },                //0.896638
                new double[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }, //0.027687
                new double[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }, //0.027687
                new double[] { 1, 1       },                //0.000000
                new double[] { 1, 0, 0, 0 },                //0.000000
            };

            // Calculate the probability that the given
            //  sequences originated from the model
            HiddenMarkovResult myResult = (HiddenMarkovResult)hmm.Predict(seq, myContext);

            // Sequences starting with zero have higher probability.
            Assert.AreEqual(0.964285, myResult.Probability[0], 1e-6);
            Assert.AreEqual(0.896638, myResult.Probability[1], 1e-6);

            // Sequences which contains few errors have higher probability
            //  than the ones which do not start with zero. This shows some
            //  of the temporal elasticity and error tolerance of the HMMs.
            Assert.AreEqual(0.027687, myResult.Probability[2], 1e-6);
            Assert.AreEqual(0.027687, myResult.Probability[3], 1e-6);

            // Sequences which do not start with zero have much lesser probability.
            Assert.AreEqual(0.000000, myResult.Probability[4], 1e-6);
            Assert.AreEqual(0.000000, myResult.Probability[5], 1e-6);
        }


    }

}
