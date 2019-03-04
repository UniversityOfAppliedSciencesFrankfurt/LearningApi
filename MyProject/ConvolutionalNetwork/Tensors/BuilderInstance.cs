using System;

namespace LearningAPIFramework.Tensor
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
		/// <returns></returns>
        public static VolumeBuilder Create()
        {
                return new VolumeBuilder();
        }
    }
}