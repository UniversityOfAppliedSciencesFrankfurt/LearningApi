using System;

namespace LearningFoundation
{
    public struct DoubleRange : IRange<double>, IEquatable<DoubleRange>
    {
        private double min, max;

      
        public double Min
        {
            get { return min; }
            set { min = value; }
        }

        public double Max
        {
            get { return max; }
            set { max = value; }
        }

    
        public double Length
        {
            get { return max - min; }
        }

      
        public DoubleRange(double min, double max)
        {
            this.min = min;
            this.max = max;
        }

       
        public bool IsInside(double x)
        {
            return ((x >= min) && (x <= max));
        }

      
        public bool IsInside(DoubleRange range)
        {
            return ((IsInside(range.min)) && (IsInside(range.max)));
        }

     
        public bool IsOverlapping(DoubleRange range)
        {
            return ((IsInside(range.min)) || (IsInside(range.max)) ||
                     (range.IsInside(min)) || (range.IsInside(max)));
        }

    
        public DoubleRange Intersection(DoubleRange range)
        {
            return new DoubleRange(Math.Max(this.Min, range.Min), Math.Min(this.Max, range.Max));
        }

       
        public static bool operator ==(DoubleRange range1, DoubleRange range2)
        {
            return ((range1.min == range2.min) && (range1.max == range2.max));
        }

      
        public static bool operator !=(DoubleRange range1, DoubleRange range2)
        {
            return ((range1.min != range2.min) || (range1.max != range2.max));
        }

       
        public bool Equals(DoubleRange other)
        {
            return this == other;
        }

        
        public override bool Equals(object obj)
        {
            return (obj is DoubleRange) ? (this == (DoubleRange)obj) : false;
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
