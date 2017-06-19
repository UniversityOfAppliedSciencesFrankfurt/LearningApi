using System;
using System.Runtime.Serialization;

namespace LearningFoundation
{
    [Serializable]
    public class ConvergenceException : Exception
    {

        public ConvergenceException() { }


        public ConvergenceException(string message) :
            base(message)
        { }
        
    }
}
