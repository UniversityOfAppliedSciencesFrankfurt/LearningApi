using System;

namespace LearningFoundation
{

    /// <summary>
    ///   Represents a double range with minimum and maximum values.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class represents a double range with inclusive limits, where
    ///   both minimum and maximum values of the range are included into it.
    ///   Mathematical notation of such range is <b>[min, max]</b>.
    /// </remarks>
    /// 
    [Serializable]
    public struct DoubleRange : IEquatable<DoubleRange>
    {
        private double min, max;

        /// <summary>
        ///   Minimum value of the range.
        /// </summary>
        /// 
        /// <remarks>
        ///   Represents minimum value (left side limit) of the range [<b>min</b>, max].
        /// </remarks>
        /// 
        public double Min
        {
            get { return min; }
            set { min = value; }
        }
        /// <summary>
        ///   Maximum value of the range.
        /// </summary>
        /// 
        /// <remarks>
        ///   Represents maximum value (right side limit) of the range [min, <b>max</b>].
        /// </remarks>
        /// 
        public double Max
        {
            get { return max; }
            set { max = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the DoubleRange class.
        /// </summary>
        /// 
        /// <param name="min">Minimum value of the range.</param>
        /// <param name="max">Maximum value of the range.</param>
        /// 
        public DoubleRange(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        ///   Determines whether two instances are equal.
        /// </summary>
        /// 
        public static bool operator ==(DoubleRange range1, DoubleRange range2)
        {
            return ((range1.min == range2.min) && (range1.max == range2.max));
        }

        /// <summary>
        ///   Determines whether two instances are not equal.
        /// </summary>
        /// 

        public static bool operator !=(DoubleRange range1, DoubleRange range2)
        {
            return ((range1.min != range2.min) || (range1.max != range2.max));
        }

        /// <summary>
        ///   Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// 
        /// <param name="other">An object to compare with this object.</param>
        /// 
        /// <returns>
        ///   true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// 
        public bool Equals(DoubleRange other)
        {
            return this == other;
        }

        /// <summary>
        ///   Determines whether the specified object is equal to this instance.
        /// </summary>
        /// 
        /// <param name="obj">The object to compare with this instance.</param>
        /// 
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// 

        public override bool Equals(object obj)
        {
            return (obj is DoubleRange) ? (this == (DoubleRange)obj) : false;
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// 
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + min.GetHashCode();
                hash = hash * 31 + max.GetHashCode();
                return hash;
            }
        }
        
    }
}
