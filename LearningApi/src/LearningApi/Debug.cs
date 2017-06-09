using System;
using System.Diagnostics;
namespace LearningFoundation
{
    public static class Debug
    {
        /// <summary>
        ///   Throws an exception if a condition is false.
        /// </summary>
        /// 
        [Conditional("DEBUG")]
        public static void Assert(bool condition, string message = "Internal framework error.")
        {
            if (!condition)
                throw new Exception(message);
        }
    }
}
