namespace ConvolutionalNetworks.Tensor
{
	/// <summary>
	/// Tensor Operations Enumerators
	/// </summary>
    public enum TensorReduceOp
    {
        /// <summary>
        /// Add Tensor Operation
        /// </summary>
        Add,
        /// <summary>
        /// Multiply Tensor Operation
        /// </summary>
        Mul,
        /// <summary>
        /// Min Value Search
        /// </summary>
        Min,
        /// <summary>
        /// Max Value Search
        /// </summary>
        Max,
        /// <summary>
        /// AMax Search
        /// </summary>
        AMax,
        /// <summary>
        /// Average Tensor Computation
        /// </summary>
        Avg,
        /// <summary>
        /// L1 Norm
        /// </summary>
        Norm1,
        /// <summary>
        /// L2 Norm
        /// </summary>
        Norm2
    }
}
