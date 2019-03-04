using System;
using System.Collections.Generic;

namespace LearningAPIFramework.ConvolutionalNetze
{
    public class CircularBuffer
    {
        private readonly double[] _buffer;
        private int _nextFree;

        public CircularBuffer(int capacity)
        {
            this.Capacity = capacity;
            this.Count = 0;
            this._buffer = new double[capacity];
        }

        public int Capacity { get; }

        public int Count { get; private set; }

        public IEnumerable<double> Items => this._buffer;

        public void Add(double o)
        {
            this._buffer[this._nextFree] = o;
            this._nextFree = (this._nextFree + 1) % this._buffer.Length;
            this.Count = Math.Min(this.Count + 1, this.Capacity);
        }
    }
}