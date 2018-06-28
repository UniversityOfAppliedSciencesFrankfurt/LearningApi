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
        /// Index of the label. If there are more than one label (multiclass classifier), this is the index of the first one.
        /// </summary>
        int LabelIndex { get; set; }

        //feature description: feature type, missing-value, 
        DataMappers.Column[] Features { get; set; }
    }
}
