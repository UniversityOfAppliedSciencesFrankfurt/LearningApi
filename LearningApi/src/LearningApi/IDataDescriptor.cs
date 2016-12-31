using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    /// <summary>
    /// Holds all required data, which is passed to all pipeline
    /// component.
    /// </summary>
    public interface IDataDescriptor
    {
        /// <summary>
        /// Index of the label.
        /// </summary>
        int LabelIndex { get; set; }

        //feature description: feature type, missingvalue, 
        DataMappers.Column[] Features { get; set; }
    }
}
