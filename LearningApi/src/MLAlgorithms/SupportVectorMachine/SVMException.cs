using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation.MlAlgorithms.SupportVectorMachineAlgorithm
{
    /// <summary>
    /// SVMException is an exception class that is used to throw the encountered errors. Note that the error message is preceded by the function name where the error was encountered
    /// Code: 100    "RawData is null"
    /// Code: 101	  "RawData is not the right form. At least one attribute column and one target column.
    /// Code: 102    "RawData is empty"
    /// Code: 400     "Unhnadled exception: " + Exception
    /// </summary>
    public class SVMException : Exception
        {
            /// <summary>
            /// Function error code
            /// </summary>
            public int Code { get; internal set; }

            /// <summary>
            /// Function error message
            /// </summary>
            public string ErrorMessage { get; internal set; }

            /// <summary>
            /// Constructor to create the KMeans Exception
            /// </summary>
            /// <param name="code"> Function error code </param>
            /// <param name="message"> Function error message </param>
            public SVMException(int code, string message)
            {
                this.Code = code;
                this.ErrorMessage = message;
            }
        }
}