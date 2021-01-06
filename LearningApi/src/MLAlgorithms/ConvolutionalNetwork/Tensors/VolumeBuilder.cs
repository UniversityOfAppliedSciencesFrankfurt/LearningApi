using System;

namespace ConvolutionalNetworks.Tensor
{
	/// <summary>
	/// Volumae Builder Class
	/// </summary>
	public class VolumeBuilder
	{
		/// <summary>
		/// Returns a Tensor object based on a storage value set and tensor topology shape.
		/// </summary>
		/// <param name="storage">Values to be stored in tensor</param>
		/// <param name="shape">Tensor topology</param>
		/// <returns>Tensor Object Volume</returns>
		public  Volume Build(VolumeStorage storage, Shape shape)
		{
			if (storage is NcwhVolumeStorage ncwh)
			{
				return new Volume(ncwh.ReShape(shape));
			}

			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates a new Tensor Object
		/// </summary>
		/// <param name="value">Value array to be stored in tensor</param>
		/// <param name="shape">Tensor Topology</param>
		/// <returns>Tensor Object Volume</returns>
		public Volume From(double[] value, Shape shape)
		{
			shape.GuessUnkownDimension(value.Length);

			if (shape.TotalLength != value.Length)
			{
				throw new ArgumentException($"Array size ({value.Length}) and shape ({shape}) are incompatible");
			}

			return new Volume(new NcwhVolumeStorage(value, shape));
		}

		/// <summary>
		/// Initialize a randomized value tensor
		/// </summary>
		/// <param name="shape">Tensor Topology</param>
		/// <param name="mu">Mean of the Gaussian Distribution</param>
		/// <param name="std">Standard deviation of the distribution</param>
		/// <returns>Random Tensor</returns>
		public Volume Random(Shape shape, double mu = 0, double std = 1.0)
		{
			var vol = new Volume(new NcwhVolumeStorage(shape));

			for (var n = 0; n < shape.Dimensions[3]; n++)
			{
				for (var c = 0; c < shape.Dimensions[2]; c++)
				{
					for (var y = 0; y < shape.Dimensions[1]; y++)
					{
						for (var x = 0; x < shape.Dimensions[0]; x++)
						{
							vol.Set(x, y, c, n, RandomUtilities.Randn(mu, std));
						}
					}
				}
			}

			return vol;
		}

		/// <summary>
		/// Creates a new Volume tensor identical to the parameter passed 
		/// </summary>
		/// <param name="example">Volume Tensor</param>
		/// <param name="shape">Tensor topology</param>
		/// <returns>Cloned Tensor</returns>
		public  Volume SameAs(VolumeStorage example, Shape shape)
		{
			if (example is NcwhVolumeStorage)
			{
				return new Volume(new NcwhVolumeStorage(shape));
			}

			throw new NotImplementedException();
		}

		/// <summary>
		/// Set a new tensor with a value and topology
		/// </summary>
		/// <param name="example">Image Tensor</param>
		/// <param name="value">Value parameter</param>
		/// <param name="shape">Tensor topology</param>
		/// <returns>Tensor Object Volume</returns>
		public  Volume SameAs(VolumeStorage example, double value, Shape shape)
		{
			if (example is NcwhVolumeStorage)
			{
				return new Volume(new NcwhVolumeStorage(new double[shape.TotalLength].Populate(value), shape));
			}

			throw new NotImplementedException();
		}

        /// <summary>
        /// Volume Building based on a template Shape
        /// </summary>
        /// <param name="shape">Template Shape</param>
        /// <returns>CLoned Tensor</returns>
		public  Volume SameAs(Shape shape)
		{
			return new Volume(new NcwhVolumeStorage(shape));
		}
	}
}
