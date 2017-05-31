using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace LearningFoundation.Attributes
{  

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be greater than zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class PositiveAttribute : RealAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="PositiveAttribute"/> class.
        /// </summary>
        /// 
        public PositiveAttribute()
            : base(double.Epsilon, double.MaxValue) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="PositiveAttribute"/> class.
        /// </summary>
        /// 
        public PositiveAttribute(double minimum)
            : base(minimum, double.MaxValue) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be lesser than zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NegativeAttribute : RealAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NegativeAttribute"/> class.
        /// </summary>
        /// 
        public NegativeAttribute()
            : base(double.MinValue, -double.Epsilon) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NegativeAttribute"/> class.
        /// </summary>
        /// 
        public NegativeAttribute(double maximum)
            : base(double.MinValue, maximum) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be lesser than or equal to zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NonpositiveAttribute : RealAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NonpositiveAttribute"/> class.
        /// </summary>
        /// 
        public NonpositiveAttribute()
            : base(double.MinValue, 0) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be greater than or equal to zero.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NonnegativeAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NonnegativeAttribute"/> class.
        /// </summary>
        /// 
        public NonnegativeAttribute()
            : base(0, double.MaxValue) { }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be real (double).
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class RealAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="RealAttribute"/> class.
        /// </summary>
        /// 
        public RealAttribute()
            : base(double.MinValue, double.MaxValue) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RealAttribute"/> class.
        /// </summary>
        /// 
        public RealAttribute(double minimum, double maximum)
            : base(minimum, maximum) { }

        /// <summary>
        ///   Gets the minimum allowed field value.
        /// </summary>
        /// 
        public new double Minimum { get { return (double)base.Minimum; } }

        /// <summary>
        ///   Gets the maximum allowed field value.
        /// </summary>
        /// 
        public new double Maximum { get { return (double)base.Maximum; } }
    }

    /// <summary>
    ///   Specifies that an argument, in a method or function,
    ///   must be real between 0 and 1.
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class UnitAttribute : RangeAttribute
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="UnitAttribute"/> class.
        /// </summary>
        /// 
        public UnitAttribute()
            : base(0, 1) { }
    }

}
