using System;
using System.Collections.Generic;
using System.Text;


namespace LearningFoundation
{
    
    public struct Range : IRange<float>, IEquatable<Range>
    {
        private float min, max;

        public float Min
        {
            get { return min; }
            set { min = value; }
        }

      
        public float Max
        {
            get { return max; }
            set { max = value; }
        }

     
        public float Length
        {
            get { return max - min; }
        }

      
        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

      
        public bool IsInside(float x)
        {
            return ((x >= min) && (x <= max));
        }

  
        public bool IsInside(Range range)
        {
            return ((IsInside(range.min)) && (IsInside(range.max)));
        }

     
        public bool IsOverlapping(Range range)
        {
            return ((IsInside(range.min)) || (IsInside(range.max)) ||
                     (range.IsInside(min)) || (range.IsInside(max)));
        }

      
        public Range Intersection(Range range)
        {
            return new Range(Math.Max(this.Min, range.Min), Math.Min(this.Max, range.Max));
        }

      
        public static bool operator ==(Range range1, Range range2)
        {
            return ((range1.min == range2.min) && (range1.max == range2.max));
        }

       
        public static bool operator !=(Range range1, Range range2)
        {
            return ((range1.min != range2.min) || (range1.max != range2.max));
        }

       
        public bool Equals(Range other)
        {
            return this == other;
        }

       
        public override bool Equals(object obj)
        {
            return (obj is Range) ? (this == (Range)obj) : false;
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


       
        public static implicit operator DoubleRange(Range range)
        {
            return new DoubleRange(range.Min, range.Max);
        }

     
    
        public IntRange ToIntRange(bool provideInnerRange)
        {
            int iMin, iMax;

            if (provideInnerRange)
            {
                iMin = (int)Math.Ceiling(min);
                iMax = (int)Math.Floor(max);
            }
            else
            {
                iMin = (int)Math.Floor(min);
                iMax = (int)Math.Ceiling(max);
            }

            return new IntRange(iMin, iMax);
        }
    }
}
