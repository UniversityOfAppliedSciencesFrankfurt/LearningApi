using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Collections;
namespace LearningFoundation
{
       public struct IntRange : IRange<int>, IEquatable<IntRange>, IEnumerable<int>
    {
        private int min, max;


        public int Min
        {
            get { return min; }
            set { min = value; }
        }


        public int Max
        {
            get { return max; }
            set { max = value; }
        }


        public int Length
        {
            get { return max - min; }
        }


        public IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }


        public bool IsInside(int x)
        {
            return ((x >= min) && (x <= max));
        }


        public IntRange Intersection(IntRange range)
        {
            return new IntRange(Math.Max(this.Min, range.Min), Math.Min(this.Max, range.Max));
        }


        public bool IsInside(IntRange range)
        {
            return ((IsInside(range.min)) && (IsInside(range.max)));
        }


        public bool IsOverlapping(IntRange range)
        {
            return ((IsInside(range.min)) || (IsInside(range.max)) ||
                     (range.IsInside(min)) || (range.IsInside(max)));
        }


        public static bool operator ==(IntRange range1, IntRange range2)
        {
            return ((range1.min == range2.min) && (range1.max == range2.max));
        }


        public static bool operator !=(IntRange range1, IntRange range2)
        {
            return ((range1.min != range2.min) || (range1.max != range2.max));
        }


        public bool Equals(IntRange other)
        {
            return this == other;
        }


        public override bool Equals(object obj)
        {
            return (obj is IntRange) ? (this == (IntRange)obj) : false;
        }


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


        public override string ToString()
        {
            return String.Format("[{0}, {1}]", min, max);
        }


        public string ToString(string format, IFormatProvider formatProvider)
        {
            return String.Format("[{0}, {1}]",
                min.ToString(format, formatProvider),
                max.ToString(format, formatProvider));
        }


        public static implicit operator DoubleRange(IntRange range)
        {
            return new DoubleRange(range.Min, range.Max);
        }


        public static implicit operator Range(IntRange range)
        {
            return new Range(range.Min, range.Max);
        }


        public IEnumerator<int> GetEnumerator()
        {
            for (int i = min; i < max; i++)
                yield return i;
        }


     IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = min; i < max; i++)
                yield return i;
        }
    }
}
