using System;

namespace LearningFoundation
{
    public interface IRange<T>
    {
        T Min { get; set; }

        T Max { get; set; }
    }
}
