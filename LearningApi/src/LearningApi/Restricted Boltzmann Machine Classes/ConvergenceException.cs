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

        //public ConvergenceException(string message, Exception innerException) :
        //    base(message, innerException)
        //{ }


        //protected ConvergenceException(SerializationInfo info, StreamingContext context) :
        //   base(info, context)
        //{ }

    }
}
