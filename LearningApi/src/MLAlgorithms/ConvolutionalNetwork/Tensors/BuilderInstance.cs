using System;

namespace ConvolutionalNetworks.Tensor
{
	/// <summary>
	/// Tensor builder instance.
	/// </summary>
    public class BuilderInstance
    {
        private static readonly Lazy<VolumeBuilder> Singleton = new Lazy<VolumeBuilder>(Create);

		/// <summary>
		/// Singleton VolumeBuilder object
		/// </summary>
        public static VolumeBuilder Volume { get; set; } = Singleton.Value;

		/// <summary>
		/// Create a Tensor object
		/// </summary>
		/// <returns>Volume builder result</returns>
        public static VolumeBuilder Create()
        {
                return new VolumeBuilder();
        }
    }
}
