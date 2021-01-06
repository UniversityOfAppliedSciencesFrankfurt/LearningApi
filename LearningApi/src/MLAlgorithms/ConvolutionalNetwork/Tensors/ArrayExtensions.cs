namespace ConvolutionalNetworks.Tensor
{
	/// <summary>
	/// Extension Class for Tensor Population
	/// </summary>
    public static class ArrayExtensions
    {
		/// <summary>
		/// Extension Method for populating the tensor object.
		/// </summary>
		/// <param name="arr">Double array with which tensor is to be populated</param>
		/// <param name="value">Value which has to be passed.</param>
		/// <returns>Populate array tensor form</returns>
        public static double[] Populate(this double[] arr, double value)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }

            return arr;
        }
    }
}
