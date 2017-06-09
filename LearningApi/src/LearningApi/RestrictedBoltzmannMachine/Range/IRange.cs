using System;

namespace LearningFoundation
{
    public interface IRange<T> : IFormattable
    {
        T Min { get; set; }

        T Max { get; set; }
    }
}
