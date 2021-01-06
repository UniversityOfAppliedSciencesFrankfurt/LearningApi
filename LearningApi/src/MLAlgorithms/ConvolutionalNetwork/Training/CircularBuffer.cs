using System;
using System.Collections.Generic;

namespace ConvolutionalNetworks
{
    /// <summary>
    /// A circular buffer for Efficiency parameter computation
    /// </summary>
    public class CircularBuffer
    {
        private readonly double[] _buffer;
        private int _nextFree;

        /// <summary>
        /// Initialize the buffer
        /// </summary>
        /// <param name="capacity">Default capacity</param>
        public CircularBuffer(int capacity)
        {
            this.Capacity = capacity;
            this.Count = 0;
            this._buffer = new double[capacity];
        }

        /// <summary>
        /// Default capacity of the circular buffer
        /// </summary>
        public int Capacity { get; }

        /// <summary>
        /// Length of Buffer
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Enumerable Objects
        /// </summary>
        public IEnumerable<double> Items => this._buffer;

        /// <summary>
        /// Method to add elements to buffer
        /// </summary>
        /// <param name="o">Double Value to be added to the buffer</param>
        public void Add(double o)
        {
            this._buffer[this._nextFree] = o;
            this._nextFree = (this._nextFree + 1) % this._buffer.Length;
            this.Count = Math.Min(this.Count + 1, this.Capacity);
        }
    }
}
