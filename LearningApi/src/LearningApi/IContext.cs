using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningFoundation
{
    public interface IContext 
    {
        IDataDescriptor DataDescriptor { get; set; }
    }
}
