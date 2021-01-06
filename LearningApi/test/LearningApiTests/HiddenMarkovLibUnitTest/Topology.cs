using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HiddenMarkov;

namespace HiddenMarkovLibUnitTest
{
    [TestClass]
    public class Topology
    {
      
        [TestMethod]
        public void ErgodicTest()
        {
            // Create a new Ergodic hidden Markov model with three
            //   fully-connected states and four sequence symbols.
            var hiddenMarkovAlgorithm = new HiddenMarkovAlgorithm
                (4, 3, HiddenMarkov.HiddenMarkovModel.HiddenMarkovModelType.Ergodic);
            var expectedA = new double[,]
            {
                { 0.33, 0.33, 0.33 },
                { 0.33, 0.33, 0.33 },
                { 0.33, 0.33, 0.33 },
            };
            //todo
            //Assert.IsTrue(hiddenMarkovAlgorithm.m_MyModel.Transitions.Equals(expectedA, 0.01));
            Assert.AreEqual(hiddenMarkovAlgorithm.m_MyModel.States, 3);
            Assert.AreEqual(hiddenMarkovAlgorithm.m_MyModel.Symbols, 4);
        }

        [TestMethod]
        public void ForwardTest()
        {
            // Create a new Forward-only hidden Markov model with
            // three forward-only states and four sequence symbols.
            var hiddenMarkovAlgorithm = new HiddenMarkovAlgorithm
                (4, 3, HiddenMarkov.HiddenMarkovModel.HiddenMarkovModelType.Forward);

            Assert.AreEqual(hiddenMarkovAlgorithm.m_MyModel.States, 3);
            var actual = hiddenMarkovAlgorithm.m_MyModel.A;
            var expected = new double[,]
            {
                { 0.33, 0.33, 0.33 },
                { 0.00, 0.50, 0.50 },
                { 0.00, 0.00, 1.00 },
            };

            Assert.IsTrue(actual == expected);//, 0.01));
        }


    }
}
    

