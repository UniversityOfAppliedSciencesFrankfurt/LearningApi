using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Collections;
namespace LearningFoundation
{
    /// <summary>
    ///   Represents an integer range with minimum and maximum values.
    /// </summary>
    /// 
    /// <remarks>
    ///   The class represents an integer range with inclusive limits, where
    ///   both minimum and maximum values of the range are included into it.
    ///   Mathematical notation of such range is <b>[min, max]</b>.
    /// </remarks>
  
    [Serializable]
    public struct IntRange 
    {
        private int min, max;
        /// <summary>
        ///   Minimum value of the range.
        /// </summary>
        /// 
        /// <remarks>
        ///   Represents minimum value (left side limit) of the range [<b>min</b>, max].
        /// </remarks>
        /// 
        public int Min
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
        public int Max
        {
            get { return max; }
            set { max = value; }
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
                hash = hash * 31 + min.GetHashCode( );
                hash = hash * 31 + max.GetHashCode( );
                return hash;
            }
        }
        /// <summary>
        ///   Performs an implicit conversion from IntRange to DoubleRange
        /// </summary>
        /// 
        /// <param name="range">The range.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator DoubleRange( IntRange range )
        {
            return new DoubleRange( range.Min, range.Max );
        }
        /// <summary>
        ///   Performs an implicit conversion from IntRange to Range
        /// </summary>
        /// 
        /// <param name="range">The range.</param>
        /// 
        /// <returns>
        ///   The result of the conversion.
        /// </returns>
        /// 
        public static implicit operator Range( IntRange range )
        {
            return new Range( range.Min, range.Max );
        }


    }
}
