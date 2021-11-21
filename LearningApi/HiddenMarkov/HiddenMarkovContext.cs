using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace HiddenMarkov
{
    public class HiddenMarkovContext : IContext
    {   
        public IDataDescriptor DataDescriptor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IScore Score { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsMoreDataAvailable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double tolerance { get; set; }
        public int iterations { get; set; }
    }
}
