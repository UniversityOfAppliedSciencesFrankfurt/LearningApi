using System;
using System.Collections.Generic;
using System.Text;


namespace LearningFoundation
{

    public struct Range 
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

        
        public Range( float min, float max )
        {
            this.min = min;
            this.max = max;
        }
        
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

        public override string ToString()
        {
            return String.Format( "[{0}, {1}]", min, max );
        }

        public static implicit operator DoubleRange( Range range )
        {
            return new DoubleRange( range.Min, range.Max );
        }

    }
}
