using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnomalyDetection.Interfaces
{
    /// <summary>
    /// AnomalyDetectionResponse is a class that is used to determine whether the function succeeded or encountered an error. Note that the error message is preceeded by the function name <br />
    /// <br />
    /// Succes/Error Codes and Messages: <br />
    /// - success: 0-99 <br />
    /// - user input errors: 100-199
    /// <ul style="list-style-type:none">
    /// <li> - 100    "RawData is null" </li>
    /// <li> - 101    "At least one input is null" </li>
    /// <li> - 102    "RawData is empty" </li>
    /// <li> - 103    "Means is empty" </li>
    /// <li> - 104    "Maximum number of clusters must be at least 2" </li>
    /// <li> - 105	  "Unacceptable number of clusters. Clusters more than samples" </li>
    /// <li> - 106	  "Unacceptable number of clusters. Must be at least 2" </li>
    /// <li> - 107	  "Unacceptable number of attributes. Must be at least 1" </li>
    /// <li> - 108	  "Unacceptable number of maximum iterations" </li>
    /// <li> - 109	  "Unacceptable save obect" </li>
    /// <li> - 110	  "Unacceptable tolerance value" </li>
    /// <li> - 111	  "Data sample and number of attributes are inconsistent. First encountered inconsistency in data sample: " + Sample_Index + "." </li>
    /// <li> - 112	  "Mismatch between old and new cluster numbers" </li>
    /// <li> - 113	  "Mismatch between old and new number of atributes" </li>
    /// <li> - 114	  "Mismatch in number of attributes" </li>
    /// <li> - 115	  "Inputs have different dimensions" </li>
    /// <li> - 116	  "Path provided : " + Path + " has no root" </li>
    /// <li> - 117	  "Path provided : " + Path + " contains invalid chars. First invalid char encountered is: " + Invalid_Char + "." </li>
    /// <li> - 118	  "Path provided : " + Path + " has no project name specified." </li>
    /// <li> - 119	  "Path provided : " + Path + " has a project name containing invalid chars. First invalid char encountered is: " + Invalid_Char + "." </li>
    /// <li> - 120	  "Path provided : " + Path + " has wrong extension." </li>
    /// <li> - 121	  "Requested load path does not exist" </li>
    /// <li> - 122	  "Method must be either 0,1 or 2" </li>
    /// <li> - 123	  "Parameter StdDev is needed" </li>
    /// <li> - 124	  "Unacceptable input for K-means Algorithm" </li>
    /// <li> - 125	  "Settings to save can't be null" </li>
    /// <li> - 126	  "Settings to load can't be null" </li>
    /// <li> - 127	  "Undefined Method to save or load" </li>
    /// </ul>
    /// - file related errors: 200-299
    /// <ul style="list-style-type:none">
    /// <li> - 200	  "File not found" </li>
    /// <li> - 201	  "File already exists" </li>
    /// <li> - 202	  "File cannot be loaded" </li>
    /// <li> - 203	  "File content is corrupted" </li>
    /// <li> - 204	  "Unauthorized access to file. File is readonly." </li>
    /// <li> - 205	  "Unauthorized access to file. File is a system file." </li>
    /// <li> - 206	  "Can't deserialize file" </li>
    /// </ul>
    /// - calculation errors: 300-399
    /// <ul style="list-style-type:none">
    /// <li> - 300	  "Division by zero" </li>
    /// </ul>
    /// - unhandled errors: 400
    /// <ul style="list-style-type:none">
    /// <li> - 400     "Unhnadled exception: " + Exception </li>
    /// </ul>
    /// </summary>
    public class AnomalyDetectionResponse
    {
        /// <summary>
        /// Function state code, Succes/Error Codes
        /// </summary>
        public int Code { get; internal set; }

        /// <summary>
        /// Function state message, Succes/Error Message
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        /// Constructor to create the AnomalyDetectionResponse instance
        /// </summary>
        /// <param name="Code"> Function state code, Succes/Error Codes </param>
        /// <param name="Message"> Function state message, Succes/Error Message </param>
        public AnomalyDetectionResponse(int Code, string Message)
        {
            this.Code = Code;
            this.Message = Message;
        }
    }
}
