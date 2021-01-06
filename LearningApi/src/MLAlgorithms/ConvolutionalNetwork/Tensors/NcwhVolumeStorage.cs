using System;

namespace ConvolutionalNetworks.Tensor
{
	/// <summary>
	/// Create a Tensor Storage element structure for
	/// N : Multiple counts
	/// c:Multiple classes
	/// w:Width elements
	/// h:Height elements
	/// </summary>
    public class NcwhVolumeStorage : VolumeStorage
    {
        private readonly int _dim0, _dim0Dm1, _dim0Dm1Dm2;
        private double[] _storage;

        /// <summary>
        /// Create a tensor object based on topology given by shape object
        /// </summary>
        /// <param name="shape">Topology of the Tensor</param>
        public NcwhVolumeStorage(Shape shape) : base(shape)
        {
            this._storage = new double[shape.TotalLength];

            this._dim0 = this.Shape.Dimensions[0];
            var dim1 = this.Shape.Dimensions[1];
            var dim2 = this.Shape.Dimensions[2];
            this._dim0Dm1 = this._dim0 * dim1;
            this._dim0Dm1Dm2 = this._dim0 * dim1 * dim2;

        }

		/// <summary>
		/// Create a Tensor with array passed as arguement and a shape topology
		/// </summary>
		/// <param name="array">Value elements of the Tensor</param>
		/// <param name="shape">Topology of the Tensor</param>
        public NcwhVolumeStorage(double[] array, Shape shape) : base(shape)
        {
            this._storage = (double[])array.Clone();
            this.Shape.GuessUnkownDimension(this._storage.Length);

            this._dim0 = this.Shape.Dimensions[0];
            var dim1 = this.Shape.Dimensions[1];
            var dim2 = this.Shape.Dimensions[2];
            this._dim0Dm1 = this._dim0 * dim1;
            this._dim0Dm1Dm2 = this._dim0 * dim1 * dim2;
        }
		/// <summary>
		/// Used by dropout layer
		/// </summary>
		public bool[] Dropped { get; set; }

		/// <summary>
		/// Clear the Tensor
		/// </summary>
        public override void Clear()
        {
            Array.Clear(this._storage, 0, this._storage.Length);
        }

		/// <summary>
		/// Copy a Tensor from a VolumeStorage parameter
		/// </summary>
		/// <param name="source">VolumeStorage parameter</param>
        public override void CopyFrom(VolumeStorage source)
        {
            var src = source as NcwhVolumeStorage;

            if (!ReferenceEquals(this, src))
            {
                if (this.Shape.TotalLength != src.Shape.TotalLength)
                {
                    throw new ArgumentException($"origin and destination volume should have the same number of weight ({this.Shape.TotalLength} != {src.Shape}).");
                }

                Array.Copy(src._storage, this._storage, this._storage.Length);
            }
        }

		/// <summary>
		/// Get Element of Tensor at a given NCWH Coordinate
		/// </summary>
		/// <param name="coordinates">NCWH coordinate</param>
		/// <returns>Result at the NCWH coordinate</returns>
        public override double Get(int[] coordinates)
        {
            var length = coordinates.Length;
            return Get(coordinates[0], length > 1 ? coordinates[1] : 0, length > 2 ? coordinates[2] : 0, length > 3 ? coordinates[3] : 0);
        }

		/// <summary>
		/// Get Element of Tensor at a given NCWH Coordinate
		/// </summary>
		/// <param name="w">W coordinate</param>
		/// <param name="c">C coordinate</param>
		/// <param name="h">H coordinate</param>
		/// <param name="n">N coordinate</param>
		/// <returns>result at the coordinate</returns>
		public override double Get(int w, int h, int c, int n)
        {
            return this._storage[w + h * this._dim0 + c * this._dim0Dm1 + n * this._dim0Dm1Dm2];
        }

		/// <summary>
		/// Get Element of Tensor at a given NCWH Coordinate
		/// </summary>
		/// <param name="w">W coordinate</param>
		/// <param name="h">H coordinate</param>
		/// <param name="c">C coordinate</param>
		/// <returns>Result at the given coordinate</returns>
		public override double Get(int w, int h, int c)
        {
            return
                this._storage[w + h * this._dim0 + c * this._dim0Dm1];
        }

		/// <summary>
		/// Get Element of Tensor at a given NCWH Coordinate
		/// </summary>
		/// <param name="w">w coordinate</param>
		/// <param name="h">h coordinate</param>
		/// <returns>Get value at coordinate</returns>
		public override double Get(int w, int h)
        {
            return this._storage[w + h * this._dim0];
        }

		/// <summary>
		/// Get Element of Tensor at a given element index
		/// </summary>
		/// <param name="i">Element Index</param>
		/// <returns>Get value at coordinate</returns>
		public override double Get(int i)
        {
            return this._storage[i];
        }

		/// <summary>
		/// Restructure the Tensor Object
		/// </summary>
		/// <param name="shape">The new Topology of the tensor</param>
		/// <returns>Reshape Volume storage</returns>
        public NcwhVolumeStorage ReShape(Shape shape)
        {
            var storage = new NcwhVolumeStorage(shape) { _storage = this._storage };
            return storage;
        }

		/// <summary>
		/// Set element of Tensor at a particular NCWH coordinate
		/// </summary>
		/// <param name="coordinates">NCWH coordinate</param>
		/// <param name="value">Value to be inserted at the index</param>
        public override void Set(int[] coordinates, double value)
        {
            var length = coordinates.Length;
            Set(coordinates[0], length > 1 ? coordinates[1] : 0, length > 2 ? coordinates[2] : 0, length > 3 ? coordinates[3] : 0, value);
        }

		/// <summary>
		/// Set element of Tensor at a particular NCWH coordinate
		/// </summary>
		/// <param name="w">W coordinate</param>
		/// <param name="h">H coordinate</param>
		/// <param name="c">C coordinate</param>
		/// <param name="n">N coordinate</param>
		/// <param name="value">Value to be inserted at the index</param>
		public override void Set(int w, int h, int c, int n, double value)
        {
            this._storage[w + h * this._dim0 + c * this._dim0Dm1 + n * this._dim0Dm1Dm2] = value;
        }

		/// <summary>
		/// Set element of Tensor at a particular NCWH coordinate
		/// </summary>
		/// <param name="w">W coordinate</param>
		/// <param name="h">H coordinate</param>
		/// <param name="c">C coordinate</param>
		
		/// <param name="value">Value to be inserted at the index</param>
		public override void Set(int w, int h, int c, double value)
        {
            this._storage[w + h * this._dim0 + c * this._dim0Dm1] = value;
        }

		/// <summary>
		/// Set element of Tensor at a particular NCWH coordinate
		/// </summary>
		/// <param name="w">W coordinate</param>
		/// <param name="h">H coordinate</param>
		
		/// <param name="value">Value to be inserted at the index</param>
		public override void Set(int w, int h, double value)
        {
            this._storage[w + h * this._dim0] = value;
        }

		/// <summary>
		/// Set element of Tensor at a particular NCWH coordinate
		/// </summary>
		/// <param name="i">W coordinate</param>
		
		/// <param name="value">Value to be inserted at the index</param>
		public override void Set(int i, double value)
        {
            this._storage[i] = value;
        }

		/// <summary>
		/// Returns the Tensor Object as an array
		/// </summary>
		/// <returns>Tensor object array</returns>
        public override double[] ToArray()
        {
            return (double[])this._storage.Clone();
        }
    }
}
