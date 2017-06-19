using System;
using System.ComponentModel.DataAnnotations;

namespace LearningFoundation.Attributes
{


    [AttributeUsage( AttributeTargets.Parameter, AllowMultiple = false )]
    public class RealAttribute : RangeAttribute
    {
        /// <summary>
        ///   Specifies that an argument, in a method or function,
        ///   must be real (double).
        /// </summary>
        /// 
        public RealAttribute()
            : base( double.MinValue, double.MaxValue ) { }


        public RealAttribute( double minimum, double maximum )
            : base( minimum, maximum ) { }

        public new double Minimum { get { return (double)base.Minimum; } }

        public new double Maximum { get { return (double)base.Maximum; } }
    }




}
