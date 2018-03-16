using System;
using System.Collections.Generic;
using System.Text;

namespace LearningFoundation
{
    /// <summary>
    /// DEfines interface for lading and saving of pipeline.
    /// </summary>
    public interface IModelPersistenceProvider
    {
        void Save(string name);

        LearningApi Load(string name);
    }
}
