using System;
using System.Diagnostics;
using LearningAPIFramework.Tensor;

namespace LearningAPIFramework.ConvolutionalNetze.Training
{
    public abstract class TrainerBase
    {
        public INet Net { get; }

        protected TrainerBase(INet net)
        {
            this.Net = net;
        }

        public double BackwardTimeMs { get; protected set; }

        public double ForwardTimeMs { get; protected set; }

        public double UpdateWeightsTimeMs { get; private set; }

        public virtual double Loss { get; protected set; }

        public int BatchSize { get; set; } = 1;

        protected virtual void Backward(Volume y)
        {
            var chrono = Stopwatch.StartNew();

            var batchSize = y.Shape.Dimensions[3];
            this.Loss = Ops<double>.Divide(this.Net.Backward(y), Ops<double>.Cast(batchSize));
            this.BackwardTimeMs = chrono.Elapsed.TotalMilliseconds/batchSize;
        }

        private void Forward(Volume x)
        {
            var chrono = Stopwatch.StartNew();
            var batchSize = x.Shape.Dimensions[3];
            this.Net.Forward(x, true); // also set the flag that lets the net know we're just training
            this.ForwardTimeMs = chrono.Elapsed.TotalMilliseconds/batchSize;
        }

        public virtual void Train(Volume x, Volume y)
        {
            Forward(x);

            Backward(y);

            var batchSize = x.Shape.Dimensions[3];
            var chrono = Stopwatch.StartNew();
            TrainImplem();
            this.UpdateWeightsTimeMs = chrono.Elapsed.TotalMilliseconds/batchSize;
        }

        protected abstract void TrainImplem();
    }
}