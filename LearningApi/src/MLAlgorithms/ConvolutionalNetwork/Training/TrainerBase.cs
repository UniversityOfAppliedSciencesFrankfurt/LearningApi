using System;
using System.Diagnostics;
using ConvolutionalNetworks.Tensor;

namespace ConvolutionalNetworks.Training
{
    /// <summary>
    /// Trainer Base Abstract Definition
    /// </summary>
    public abstract class TrainerBase
    {
        /// <summary>
        /// Network Structure object
        /// </summary>
        public INet Net { get; }

        /// <summary>
        /// Trainer type
        /// </summary>
        /// <param name="net">Network Structure Object</param>
        protected TrainerBase(INet net)
        {
            this.Net = net;
        }

        /// <summary>
        /// Backward Pass Time of Execution(ToE)
        /// </summary>
        public double BackwardTimeMs { get; protected set; }

        /// <summary>
        /// Forward Pass ToE
        /// </summary>
        public double ForwardTimeMs { get; protected set; }

        /// <summary>
        /// Delta Updation ToE
        /// </summary>
        public double UpdateWeightsTimeMs { get; private set; }

        /// <summary>
        /// Mean Squared Loss
        /// </summary>
        public virtual double Loss { get; protected set; }

        /// <summary>
        /// Training Batch Size
        /// </summary>
        public int BatchSize { get; set; } = 1;

        /// <summary>
        /// Backward Pass
        /// </summary>
        /// <param name="y">Output Tensor</param>
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

        /// <summary>
        /// Abstract Train IMplementation
        /// </summary>
        /// <param name="x">Input Data</param>
        /// <param name="y">Output Labels</param>
        public virtual void Train(Volume x, Volume y)
        {
            Forward(x);

            Backward(y);

            var batchSize = x.Shape.Dimensions[3];
            var chrono = Stopwatch.StartNew();
            TrainImplem();
            this.UpdateWeightsTimeMs = chrono.Elapsed.TotalMilliseconds/batchSize;
        }

        /// <summary>
        /// Specific Trainer Implementation
        /// </summary>
        protected abstract void TrainImplem();
    }
}
