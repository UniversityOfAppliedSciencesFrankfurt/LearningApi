using System;

namespace ConvolutionalNetworks.Tensor
{
	/// <summary>
	/// Tensor Storage for a given Shape/Topology
	/// </summary>
    public abstract class VolumeStorage
    {
        /// <summary>
        /// Volume Storage Initialize
        /// </summary>
        public VolumeStorage()
        {

        }

        /// <summary>
        /// Volume Storage Initialize with shape Template
        /// </summary>
        /// <param name="shape">Shape Template</param>
        protected VolumeStorage(Shape shape)
        {
            this.Shape = new Shape(shape);
        }

        /// <summary>
        /// Shape feature of Storage
        /// </summary>
        public Shape Shape { get; set; }

        /// <summary>
        /// Clear Storage
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// Copy from a Storage Source
        /// </summary>
        /// <param name="source">Storage source</param>
        public abstract void CopyFrom(VolumeStorage source);

        /// <summary>
        /// Get Co-ordinates of Storage 
        /// </summary>
        /// <param name="coordinates">Co-ordinate array</param>
        /// <returns>Double Value at that coordinate</returns>
        public abstract double Get(int[] coordinates);

        /// <summary>
        /// Get explicit storage co-ordinates
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="c">Class number</param>
        /// <param name="n">Number(Count)</param>
        /// <returns>Value at the given Coordinate</returns>
        public abstract double Get(int w, int h, int c, int n);

        /// <summary>
        /// Get explicit storage co-ordinates
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="c">Class number</param>
        /// <returns>Value at the given Coordinate</returns>
        public abstract double Get(int w, int h, int c);

        /// <summary>
        /// Get explicit storage co-ordinates
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <returns>Value at the given Coordinate</returns>
        public abstract double Get(int w, int h);

        /// <summary>
        ///  Get explicit storage co-ordinates
        /// </summary>
        /// <param name="i">Storage coordinates</param>
        /// <returns>Value at given coordinate</returns>
        public abstract double Get(int i);

		/// <summary>
		/// Map a function(operation) f on a input tensor and store result in an output tensor
		/// </summary>
		/// <param name="f">Delegate</param>
		/// <param name="other">Storage element</param>
		/// <param name="result">Result Storage Element</param>
        public void Map(Func<double, double, double> f, VolumeStorage other, VolumeStorage result)
        {
            for (var i = 0; i < this.Shape.TotalLength; i++)
            {
                result.Set(i, f(Get(i), other.Get(i)));
            }
        }
		/// <summary>
		/// Map a function(operation) f on a input tensor and store result in an output tensor
		/// </summary>
		/// <param name="f">Delegate</param>
		/// <param name="result">Result Storage Element</param>
		public void Map(Func<double, double> f, VolumeStorage result)
        {
            for (var i = 0; i < this.Shape.TotalLength; i++)
            {
                result.Set(i, f(Get(i)));
            }
        }
		/// <summary>
		/// Map a function(operation) f on a input tensor and store result in an output tensor
		/// </summary>
		/// <param name="f">Delegate</param>
		/// <param name="result">Result Storage Element</param>
		public void Map(Func<double,int, double> f, VolumeStorage result)
        {
            for (var i = 0; i < this.Shape.TotalLength; i++)
            {
                result.Set(i, f(Get(i), i));
            }
        }

        /// <summary>
        /// Implement Broadcast
        /// </summary>
        /// <param name="f">Delegate function</param>
        /// <param name="other">Tensor</param>
        /// <param name="result">Result tensor</param>
        public void MapEx(Func<double, double, double> f, VolumeStorage other, VolumeStorage result)
        {
            var big = this;
            var small = other;

            if (small.Shape.TotalLength > big.Shape.TotalLength)
            {
                big = other;
                small = this;
            }
            else if (small.Shape.TotalLength == big.Shape.TotalLength)
            {
                if (!small.Shape.Equals(big.Shape))
                {
                    throw new ArgumentException("Volumes have the same total number of dimensions but have different shapes");
                }
				
                this.Map(f, other, result);
                return;
            }

            var w = big.Shape.Dimensions[0];
            var h = big.Shape.Dimensions[1];
            var C = big.Shape.Dimensions[2];
            var N = big.Shape.Dimensions[3];

            var otherWIsOne = small.Shape.Dimensions[0] == 1;
            var otherHIsOne = small.Shape.Dimensions[1] == 1;
            var otherCIsOne = small.Shape.Dimensions[2] == 1;
            var otherNIsOne = small.Shape.Dimensions[3] == 1;

            for (var n = 0; n < N; n++)
            {
                for (var c = 0; c < C; c++)
                {
                    for (var j = 0; j < h; j++)
                    {
                        for (var i = 0; i < w; i++)
                        {
                            result.Set(i, j, c, n,
                                f(big.Get(i, j, c, n),
                                    small.Get(otherWIsOne ? 0 : i, otherHIsOne ? 0 : j, otherCIsOne ? 0 : c,
                                        otherNIsOne ? 0 : n)));
                        }
                    }
                }
            }
        }
		/// <summary>
		/// Map a function(operation) f on a input tensor and store result in an output tensor
		/// </summary>
		/// <param name="f">Delegate</param>
		public void MapInplace(Func<double, double> f)
        {
            for (var i = 0; i < this.Shape.TotalLength; i++)
            {
                Set(i, f(Get(i)));
            }
        }
		/// <summary>
		/// Map a function(operation) f on a input tensor and store result in an output tensor
		/// </summary>
		/// <param name="f">Delegate</param>
		/// <param name="other">Storage element</param>
		public void MapInplace(Func<double, double, double> f, VolumeStorage other)
        {
            for (var i = 0; i < this.Shape.TotalLength; i++)
            {
                Set(i, f(Get(i), other.Get(i)));
            }
        }

        /// <summary>
        /// Set Value at a coordinate
        /// </summary>
        /// <param name="coordinates">Coordinate array</param>
        /// <param name="value">Double value to be set</param>
        public abstract void Set(int[] coordinates, double value);

        /// <summary>
        /// Set value at explicit WHCN coordinate
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="c">Number of Classes</param>
        /// <param name="n">Count</param>
        /// <param name="value">Value to be set</param>
        public abstract void Set(int w, int h, int c, int n, double value);

        /// <summary>
        /// Set value at explicit WHC coordinate
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="c">Number of Classes</param>
        /// <param name="value">Value to be set</param>
        public abstract void Set(int w, int h, int c, double value);

        /// <summary>
        /// Set value at explicit WHCN coordinate
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="value">Value to be set</param>
        public abstract void Set(int w, int h, double value);

        /// <summary>
        /// Set value at explicit WHCN coordinate
        /// </summary>
        /// <param name="i">Index</param>
        /// <param name="value">Value to be set</param>
        public abstract void Set(int i, double value);

        /// <summary>
        /// Converts to array
        /// </summary>
        /// <returns>Double array result</returns>
        public abstract double[] ToArray();
    }
}
