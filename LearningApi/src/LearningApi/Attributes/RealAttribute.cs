using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LearningFoundation.Attributes
{  

    
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class PositiveAttribute : RealAttribute
    {
        
        public PositiveAttribute()
            : base(double.Epsilon, double.MaxValue) { }

       
        public PositiveAttribute(double minimum)
            : base(minimum, double.MaxValue) { }
    }

    
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NegativeAttribute : RealAttribute
    {
        
        public NegativeAttribute()
            : base(double.MinValue, -double.Epsilon) { }

       
        public NegativeAttribute(double maximum)
            : base(double.MinValue, maximum) { }
    }

    
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NonpositiveAttribute : RealAttribute
    {
        
        public NonpositiveAttribute()
            : base(double.MinValue, 0) { }
    }

    
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NonnegativeAttribute : RangeAttribute
    {
       
        public NonnegativeAttribute()
            : base(0, double.MaxValue) { }
    }

   
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class RealAttribute : RangeAttribute
    {
        
        public RealAttribute()
            : base(double.MinValue, double.MaxValue) { }

       
        public RealAttribute(double minimum, double maximum)
            : base(minimum, maximum) { }

        
        public new double Minimum { get { return (double)base.Minimum; } }

        
        public new double Maximum { get { return (double)base.Maximum; } }
    }

    
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class UnitAttribute : RangeAttribute
    {
     
        public UnitAttribute()
            : base(0, 1) { }
    }

}
