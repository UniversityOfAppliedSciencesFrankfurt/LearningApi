using System;
using System.Diagnostics;
using System.Text;

namespace ConvolutionalNetworks.Tensor
{
    /// <summary>
    ///     A Volume (also called tensor in other librairies) is a data container. It has  a shape and a storage.
    /// </summary>
    [DebuggerDisplay("Volume {Shape.PrettyPrint()}")]
    public 	class Volume : IDisposable
    {
        /// <summary>
        /// Tensor Element count
        /// </summary>
        public static int Count;
        /// <summary>
        /// Tensor Shape
        /// </summary>
		public Shape Shape => this.Storage.Shape;
        
        /// <summary>
        /// Tensor Initialize
        /// </summary>
        /// <param name="storage">Tensor Template</param>
        public Volume(VolumeStorage storage)
        {
            Count++;

            this.Storage = storage;
        }

        /// <summary>
        /// Storage Template
        /// </summary>
        public VolumeStorage Storage { get; set; }

       
        /// <summary>
        /// Tensor Flush
        /// </summary>
        public virtual void Dispose()
        {
            if (this.Storage is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
		/// <summary>
		/// Type of activation function operation to be used on the tensor
		/// </summary>
		/// <param name="type">Activation Function</param>
		/// <param name="result">Tensor object</param>
        public  void Activation(ActivationType type, Volume result)
		{
			switch (type)
			{
				case ActivationType.Sigmoid:
					this.Storage.Map(x => 1.0 / (1.0 + Math.Exp(-x)), result.Storage);
					return;
				case ActivationType.Relu:
					Relu(result);
					break;
			}
		}

		/// <summary>
		/// Gradient of the activation output
		/// </summary>
		/// <param name="input">Input tensor</param>
		/// <param name="outputGradient">Output gradient tensor</param>
		/// <param name="type">Activation Type</param>
		/// <param name="result">Tensor result</param>
        public  void ActivationGradient(Volume input, Volume outputGradient, ActivationType type, Volume result)
		{
			switch (type)
			{
				case ActivationType.Sigmoid:
					this.Storage.Map((output, outGradient) => output * (1.0 - output) * outGradient, outputGradient.Storage,
						result.Storage);
					return;
				case ActivationType.Relu:
					ReluGradient(input, outputGradient, result);
					break;
			}
		}

		/// <summary>
		/// Add 2 tensors
		/// </summary>
		/// <param name="other">Tensor</param>
		/// <param name="result"> Result</param>
        public void Add(Volume other, Volume result)
		{
			this.Storage.MapEx((x, y) => x + y, other.Storage, result.Storage);
		}

        /// <summary>
        /// result = result + this
        /// </summary>
        /// <param name="result">Tensor</param>
        public  void Add(Volume result)
		{
			this.Storage.MapEx((x, y) => x + y, result.Storage, result.Storage);
		}

		/// <summary>
		/// Bias Gradients
		/// </summary>
		/// <param name="result">Tensor Bias gradient</param>
        public  void BiasGradient(Volume result)
		{
			var batchSize = this.Shape.Dimensions[3];

			var outputWidth = this.Shape.Dimensions[0];
			var outputHeight = this.Shape.Dimensions[1];
			var outputDepth = this.Shape.Dimensions[2];

			for (var n = 0; n < batchSize; n++)
			{
				for (var depth = 0; depth < outputDepth; depth++)
				{
					for (var ay = 0; ay < outputHeight; ay++)
					{
						for (var ax = 0; ax < outputWidth; ax++)
						{
							var chainGradient = Get(ax, ay, depth, n);

							result.Storage.Set(0, 0, depth,
								result.Storage.Get(0, 0, depth) + chainGradient);
						}
					}
				}
			}
		}

		/// <summary>
		/// Flush the tensor
		/// </summary>
        public void Clear()
        {
            this.Storage.Clear();
        }

		/// <summary>
		/// Clone the tensor
		/// </summary>
		/// <returns>Cloned tensor object</returns>
        public Volume Clone()
        {
            var data = new double[this.Shape.TotalLength];
            Array.Copy(ToArray(), data, data.Length);

            return BuilderInstance.Volume.From(data, this.Shape);
        }

		/// <summary>
		/// Compute the shape of the output tensor after a pass through a convolutional filter
		/// </summary>
		/// <param name="inputShape">Input Tensor</param>
		/// <param name="filterShape">Convolutional Filter</param>
		/// <param name="pad">Padding of filter passes</param>
		/// <param name="stride">Striding of filter passes</param>
		/// <returns>Shape Object</returns>
        public static Shape ComputeConvolutionShape(Shape inputShape, Shape filterShape, int pad, int stride)
        {
            var outputDepth = filterShape.Dimensions[3];
            var outputWidth = (int)Math.Floor((inputShape.Dimensions[0] + pad * 2 - filterShape.Dimensions[0]) / (double)stride + 1);
            var outputHeight = (int)Math.Floor((inputShape.Dimensions[1] + pad * 2 - filterShape.Dimensions[1]) / (double)stride + 1);

            return new Shape(outputWidth, outputHeight, outputDepth, inputShape.Dimensions[3]);
        }


		/// <summary>
		/// Compute the topology of tensor after pass through a Pool layer
		/// </summary>
		/// <param name="inputShape">Input Tensor</param>
		/// <param name="windowWidth">Window size of pooling</param>
		/// <param name="windowHeight">Window HEIGHT OF POOLING</param>
		/// <param name="horizontalPad">Padding of the filter</param>
		/// <param name="verticalPad">Padding of the filter</param>
		/// <param name="horizontalStride">Striding of the filter</param>
		/// <param name="verticalStride">Striding of the filter</param>
		/// <returns>Shape</returns>
		public static Shape ComputePoolShape(Shape inputShape, int windowWidth, int windowHeight, int horizontalPad, int verticalPad, int horizontalStride, int verticalStride)
        {
            var outputN = inputShape.Dimensions[3];
            var outputDepth = inputShape.Dimensions[2];
            var outputWidth = (int)Math.Floor((inputShape.Dimensions[0] + horizontalPad * 2 - windowWidth) / (double)horizontalStride + 1);
            var outputHeight = (int)Math.Floor((inputShape.Dimensions[1] + verticalPad * 2 - windowHeight) / (double)verticalStride + 1);

            return new Shape(outputWidth, outputHeight, outputDepth, outputN);
        }

        /// <summary>
        /// Compute expected 2D matrix multiplication result shape 
        /// [K, M, 1, BatchSize] x [N, K, 1, BatchSize] => [N, M, 1, BatchSize]
        /// </summary>
        /// <param name="leftShape">left 2D matrix / volume</param>
        /// <param name="rightShape">right 2D matrix / volume</param>
        /// <returns>Shape</returns>
        public static Shape ComputeMatMultiplyShape(Shape leftShape, Shape rightShape)
        {
            var batchSize = Math.Max(leftShape.Dimensions[3], rightShape.Dimensions[3]);

            return new Shape(rightShape.Dimensions[0], leftShape.Dimensions[1], 1, batchSize);
        }

        /// <summary>
        /// Tensr Concatenate
        /// </summary>
        /// <param name="right">Right Hand Tensor</param>
        /// <param name="result">Tensor Result</param>
        public void Concat(Volume right, Volume result)
		{
			var batchSize = Math.Max(this.Shape.Dimensions[3], right.Shape.Dimensions[3]);

			if (this.Shape.TotalLength > 1 && right.Shape.TotalLength > 1)
			{
				var left = ReShape(new Shape(1, 1, -1, batchSize));
				right = right.ReShape(new Shape(1, 1, -1, batchSize));

				var elementPerBatch = result.Shape.TotalLength / batchSize;
				var threshold = left.Shape.Dimensions[2];

				for (var n = 0; n < batchSize; n++)
				{
					for (var i = 0; i < elementPerBatch; i++)
					{
						result.Set(0, 0, i, n, i < threshold ? left.Get(0, 0, i, n) : right.Get(0, 0, i - threshold, n));
					}
				}
			}
			else if (this.Shape.TotalLength == 1 && right.Shape.TotalLength > 1)
			{
				// Left volume is actually a scalar => broadcast its value

				right = right.ReShape(new Shape(1, 1, -1, batchSize));
				var elementPerBatch = result.Shape.TotalLength / batchSize;
				var threshold = 1;

				for (var n = 0; n < batchSize; n++)
				{
					for (var i = 0; i < elementPerBatch; i++)
					{
						result.Set(0, 0, i, n, i < threshold ? Get(0) : right.Get(0, 0, i - threshold, n));
					}
				}
			}
			else
			{
				// Right volume is actually a scalar => broadcast its value

				var left = ReShape(new Shape(1, 1, -1, batchSize));
				var elementPerBatch = result.Shape.TotalLength / batchSize;
				var threshold = left.Shape.Dimensions[2];

				for (var n = 0; n < batchSize; n++)
				{
					for (var i = 0; i < elementPerBatch; i++)
					{
						result.Set(0, 0, i, n, i < threshold ? left.Get(0, 0, i, n) : right.Get(0));
					}
				}
			}
		}

		/// <summary>
		/// Convolution Operation
		/// </summary>
		/// <param name="filters">Convolutional filter tensor</param>
		/// <param name="pad">padding value</param>
		/// <param name="stride">striding value</param>
		/// <param name="result">Result Tensor</param>
        public  void Convolution(Volume filters, int pad, int stride, Volume result)
		{
			var batchSize = this.Shape.Dimensions[3];

			var inputWidth = this.Shape.Dimensions[0];
			var inputHeight = this.Shape.Dimensions[1];

			var outputWidth = result.Shape.Dimensions[0];
			var outputHeight = result.Shape.Dimensions[1];
			var outputDepth = result.Shape.Dimensions[2];

			var filterWidth = filters.Shape.Dimensions[0];
			var filterHeight = filters.Shape.Dimensions[1];
			var filterDepth = filters.Shape.Dimensions[2];

			for (var n = 0; n < batchSize; n++)
			{
				for (var depth = 0; depth < outputDepth; depth++)
				{
					var y = -pad;
					for (var ay = 0; ay < outputHeight; y += stride, ay++)
					{
						var x = -pad;
						for (var ax = 0; ax < outputWidth; x += stride, ax++)
						{
							// convolve centered at this particular location
							var a = 0.0;
							for (var fy = 0; fy < filterHeight; fy++)
							{
								var oy = y + fy; // coordinates in the original input array coordinates
								for (var fx = 0; fx < filterWidth; fx++)
								{
									var ox = x + fx;
									if (oy >= 0 && oy < inputHeight && ox >= 0 && ox < inputWidth)
									{
										for (var fd = 0; fd < filterDepth; fd++)
										{
											a += filters.Storage.Get(fx, fy, fd, depth) *
												 this.Storage.Get(ox, oy, fd, n);
										}
									}
								}
							}

							result.Storage.Set(ax, ay, depth, n, a);
						}
					}
				}
			}
		}

		/// <summary>
		/// Gradient of the convolutional filter
		/// </summary>
		/// <param name="filters">Convolutional filter tensor</param>
		/// <param name="outputGradients">Gradient of the tensor</param>
		/// <param name="filterGradient">Gradient of the filter</param>
		/// <param name="pad">Padding Value</param>
		/// <param name="stride">Stride length of filtering</param>
		/// <param name="inputGradient">Gradient of the downstream layer</param>
        public  void ConvolutionGradient(Volume filters, Volume outputGradients,
            Volume filterGradient, int pad, int stride, Volume inputGradient)
		{
			inputGradient.Clear(); // zero out gradient wrt bottom data, we're about to fill it

			var batchSize = this.Shape.Dimensions[3];

			var inputWidth = this.Shape.Dimensions[0];
			var inputHeight = this.Shape.Dimensions[1];

			var outputWidth = outputGradients.Shape.Dimensions[0];
			var outputHeight = outputGradients.Shape.Dimensions[1];
			var outputDepth = outputGradients.Shape.Dimensions[2];

			var filterWidth = filters.Shape.Dimensions[0];
			var filterHeight = filters.Shape.Dimensions[1];
			var filterDepth = filters.Shape.Dimensions[2];

			for (var n = 0; n < batchSize; n++)
			{
				for (var depth = 0; depth < outputDepth; depth++)
				{
					var y = -pad;
					for (var ay = 0; ay < outputHeight; y += stride, ay++)
					{
						var x = -pad;
						for (var ax = 0; ax < outputWidth; x += stride, ax++)
						{
							// convolve centered at this particular location
							var chainGradient = outputGradients.Get(ax, ay, depth, n);

							// gradient from above, from chain rule
							for (var fy = 0; fy < filterHeight; fy++)
							{
								var oy = y + fy; // coordinates in the original input array coordinates
								for (var fx = 0; fx < filterWidth; fx++)
								{
									var ox = x + fx;
									if (oy >= 0 && oy < inputHeight && ox >= 0 && ox < inputWidth)
									{
										for (var fd = 0; fd < filterDepth; fd++)
										{
											filterGradient.Set(fx, fy, fd, depth,
												filterGradient.Get(fx, fy, fd, depth) +
												Get(ox, oy, fd, n) * chainGradient);
											inputGradient.Set(ox, oy, fd, n,
												inputGradient.Get(ox, oy, fd, n) +
												filters.Get(fx, fy, fd, depth) * chainGradient);
										}
									}
								}
							}
						}
					}
				}
			}
		}
        /// <summary>
        /// Tensor Divide
        /// </summary>
        /// <param name="other">Tensor1 Numerator</param>
        /// <param name="result">Tensor2 Denominator</param>
        public  void Divide(Volume other, Volume result)
		{
			this.Storage.MapEx((left, right) => left / right, other.Storage, result.Storage);
		}

        /// <summary>
        ///     Computes dropout. Result will be scaled up by 1 / (1 - dropProbability).
        /// </summary>
        /// <param name="dropProbability">Probability at which elements will be set to 0</param>
        /// <param name="result">Output volume</param>
        public  void Dropout(double dropProbability, Volume result)
		{
			if (((NcwhVolumeStorage)this.Storage).Dropped == null || ((NcwhVolumeStorage)this.Storage).Dropped.Length != this.Shape.TotalLength)
			{
				((NcwhVolumeStorage)this.Storage).Dropped = new bool[this.Shape.TotalLength];
			}

			if (dropProbability > 0.0)
			{
				// do dropout
				this.Storage.Map((x, i) =>
				{
					var nextDouble = RandomUtilities.NextDouble();
					if (nextDouble < dropProbability)
					{
						((NcwhVolumeStorage)this.Storage).Dropped[i] = true;
						return 0;
					}

					((NcwhVolumeStorage)this.Storage).Dropped[i] = false;
					return x / (1 - dropProbability); // Scale up so that magnitude remains constant accross training and testing
				}, result.Storage);
			}
			else
			{
				this.Storage.Map(x => x, result.Storage);
			}
		}

		/// <summary>
		/// Computes drop probabiity gradient
		/// </summary>
		/// <param name="input">input</param>
		/// <param name="outputGradient">outputGradient</param>
		/// <param name="dropProbability">outputGradient</param>
		/// <param name="inputGradient">inputGradient</param>
		public void DropoutGradient(Volume input, Volume outputGradient, double dropProbability, Volume inputGradient)
		{
			outputGradient.Storage.Map((x, i) =>
			{
				if (((NcwhVolumeStorage)input.Storage).Dropped[i])
				{
					return 0;
				}

				return x / (1.0 - dropProbability);
			}, inputGradient.Storage);
		}

		/// <summary>
		/// Exponent of tensor
		/// </summary>
		/// <param name="result">Result tensor</param>
        public  void Exp(Volume result)
		{
			this.Storage.Map(Math.Exp, result.Storage);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="length">length</param>
		/// <param name="offset">offset</param>
		/// <param name="result">result</param>
		public void Extract(int length, int offset, Volume result)
		{
			var input = ReShape(1, 1, Shape.None, Shape.Keep);

			if (input.Shape.TotalLength == 1)
			{
				var v = input.Get(0);
				this.Storage.Map(x => v, result.Storage);
			}
			else
			{
				var batchSize = this.Shape.Dimensions[3];
				for (var n = 0; n < batchSize; n++)
				{
					for (var i = 0; i < length; i++)
					{
						result.Set(0, 0, i, n, input.Get(0, 0, i + offset, n));
					}
				}
			}
		}

		/// <summary>
		/// Get tensor values at a coordinate
		/// </summary>
		/// <param name="coordinates">coordinate</param>
		/// <returns>result tensor</returns>
        public double Get(int[] coordinates)
        {
            return this.Storage.Get(coordinates);
        }

		/// <summary>
		/// Get tensor values at a coordinate
		/// </summary>
		/// <param name="w">W</param>
		/// <param name="h">H</param>
		/// <param name="c">C</param>
		/// <param name="n">N</param>
		/// <returns>Get Value at the coordinates</returns>
		public double Get(int w, int h, int c, int n)
        {
            return this.Storage.Get(w, h, c, n);
        }

        /// <summary>
        ///  Get tensor values at a coordinate
        /// </summary>
        /// <param name="w">W</param>
        /// <param name="h">H</param>
        /// <param name="c">C</param>
        /// <returns>Get Value at the coordinates</returns>
        public double Get(int w, int h, int c)
        {
            return this.Storage.Get(w, h, c, 0);
        }

        /// <summary>
        ///  Get tensor values at a coordinate
        /// </summary>
        /// <param name="w">W</param>
        /// <param name="h">H</param>
        /// <returns>Get Value at the coordinates</returns>
        public double Get(int w, int h)
        {
            return this.Storage.Get(w, h, 0, 0);
        }

        /// <summary>
        ///  Get tensor values at a coordinate
        /// </summary>
        /// <param name="i">I</param>
        /// <returns>Get Value at the coordinates</returns>
        public double Get(int i)
        {
            return this.Storage.Get(i);
        }

		/// <summary>
		/// Log of the tensor
		/// </summary>
		/// <param name="result">Tensor input</param>
        public void Log(Volume result)
		{
			this.Storage.Map(x => Math.Log(x), result.Storage);
		}

		/// <summary>
		/// Delegate
		/// </summary>
		/// <param name="f">function</param>
        public void MapInplace(Func<double, double> f)
        {
            this.Storage.MapInplace(f);
        }

		/// <summary>
		/// Delegate Map in co-ordinate for a tensor
		/// </summary>
		/// <param name="f">Delegate</param>
		/// <param name="other">Tensor</param>
        public void MapInplace(Func<double, double, double> f, Volume other)
        {
            this.Storage.MapInplace(f, other.Storage);
        }

		/// <summary>
		///     Matrix multiplication
		///     left (this) x right = result
		///     Where left is a 2D volume of shape [K, M, 1, batchsize]
		///     right is a 2D volume of shape [N, K, 1, batchsize]
		///     and result is a 2D volume of shape [N, M, 1, batchsize]
		/// </summary>
		/// <param name="right">2D volume of shape [N, K, 1, batchsize]</param>
		/// <param name="result">2D volume of shape [N, M, 1, batchsize]</param>
		public  void MatMultiply(Volume right, Volume result)
		{
			if (this.Shape.Dimensions[2] != 1 || right.Shape.Dimensions[2] != 1)
			{
				throw new ArgumentException($"Left and right volumes should be [w, h, 1, b]. left = {this.Shape} right = {right.Shape}");
			}

			bool broadCastLeft = this.Shape.Dimensions[3] == 1;
			bool broadCastRight = right.Shape.Dimensions[3] == 1;
			if (this.Shape.Dimensions[3] != right.Shape.Dimensions[3] && !(broadCastLeft || broadCastRight))
			{
				throw new ArgumentException($"Left and right volumes should have the same batch size. left = {this.Shape.Dimensions[3]} right = {right.Shape.Dimensions[3]}");
			}

			var expectedShape = ComputeMatMultiplyShape(this.Shape, right.Shape);

			if (!result.Shape.Equals(expectedShape))
			{
				throw new ArgumentException($"Result shape should be {expectedShape} but is {result.Shape}");
			}

			for (var n = 0; n < this.Shape.Dimensions[3]; n++)
			{
				for (var i = 0; i < expectedShape.Dimensions[0]; i++)
				{
					for (var j = 0; j < expectedShape.Dimensions[1]; j++)
					{
						var cell = 0.0;
						for (var k = 0; k < this.Shape.Dimensions[0]; k++)
						{
							cell = cell + Get(k, j, 0, broadCastLeft ? 0 : n) * right.Get(i, k, 0, broadCastRight ? 0 : n);
						}

						result.Set(i, j, 0, n, cell);
					}
				}
			}
		}

		/// <summary>
		/// Set max value in a tensor
		/// </summary>
		/// <param name="result">Tensor</param>
		public  void Max(Volume result)
		{
			var batchSize = this.Shape.Dimensions[3];
			var reshape = ReShape(-1, batchSize);

			var n = reshape.Shape.Dimensions[0];

			for (var i = 0; i < batchSize; i++)
			{
				var max = double.MinValue;

				for (var j = 0; j < n; j++)
				{
					var d = reshape.Get(j, i);
					if (d > max)
					{
						max = d;
					}
				}

				result.Set(new[] { i }, max);
			}
		}

		/// <summary>
		/// Set Min Value in a Tensor
		/// </summary>
		/// <param name="result">Tensor</param>
		public  void Min(Volume result)
		{
			var batchSize = this.Shape.Dimensions[3];
			var reshape = ReShape(-1, batchSize);

			var n = reshape.Shape.Dimensions[0];

			for (var i = 0; i < batchSize; i++)
			{
				var min = double.MaxValue;

				for (var j = 0; j < n; j++)
				{
					var d = reshape.Get(j, i);
					if (d < min)
					{
						min = d;
					}
				}

				result.Set(new[] { i }, min);
			}
		}

		/// <summary>
		/// Scalar multiplication of a tensor
		/// </summary>
		/// <param name="factor">scalar</param>
		/// <param name="result">Tensor</param>
		public  void Multiply(double factor, Volume result)
		{
			this.Storage.Map(x => x * factor, result.Storage);
		}

		/// <summary>
		/// Tensor Multiplication
		/// </summary>
		/// <param name="right">Tensor 1</param>
		/// <param name="result">Result Tensor</param>
		public  void Multiply(Volume right, Volume result)
		{
			this.Storage.MapEx((x, y) => x * y, right.Storage, result.Storage);
		}

		/// <summary>
		/// Tensor Reflection
		/// </summary>
		/// <param name="volume">Tensor whose storage has to be mirrored</param>
		public  void Negate(Volume volume)
		{
			Multiply(-1.0, volume);
		}

		/// <summary>
		/// Norm of the Tensor
		/// </summary>
		/// <param name="result">Result Tensor</param>
		public void Norm1(Volume result)
		{
			var batchSize = this.Shape.Dimensions[3];
			var reshape = ReShape(-1, batchSize);

			var n = reshape.Shape.Dimensions[0];

			for (var i = 0; i < batchSize; i++)
			{
				var sum = 0.0;

				for (var j = 0; j < n; j++)
				{
					var d = reshape.Get(j, i);
					sum += Math.Abs(d);
				}

				result.Set(new[] { i }, sum);
			}
		}
		/// <summary>
		/// Create a Rank 0 tensor
		/// </summary>
		/// <param name="t">Scalar</param>
		public static implicit operator Volume(double t)
        {
            return BuilderInstance.Volume.From(new[] { t }, new Shape(1));
        }

		/// <summary>
		/// Create a Rank 1 tensor
		/// </summary>
		/// <param name="t">Vector</param>
        public static implicit operator Volume(double[] t)
        {
            return BuilderInstance.Volume.From(t, new Shape(1, 1, t.Length, 1));
        }

		/// <summary>
		/// Create a Rank n Tensor
		/// </summary>
		/// <param name="v">Rank n Object</param>
        public static implicit operator double(Volume v)
        {
            if (v.Shape.TotalLength == 1)
            {
                return v.Get(0);
            }

            throw new ArgumentException($"Volume should have a Shape [1] to be converter to a {typeof(double)}", nameof(v));
        }

		/// <summary>
		/// Pool Tensor Computation
		/// </summary>
		/// <param name="windowWidth">windowWidth</param>
		/// <param name="windowHeight">windowHeight</param>
		/// <param name="horizontalPad">horizontalPad</param>
		/// <param name="verticalPad">verticalPad</param>
		/// <param name="horizontalStride">horizontalStride</param>
		/// <param name="verticalStride">verticalStride</param>
		/// <param name="result">result</param>
		public void Pool(int windowWidth, int windowHeight,
			int horizontalPad, int verticalPad, int horizontalStride, int verticalStride, Volume result)
		{
			var inputWidth = this.Shape.Dimensions[0];
			var inputHeight = this.Shape.Dimensions[1];

			var outputWidth = result.Shape.Dimensions[0];
			var outputHeight = result.Shape.Dimensions[1];
			var outputDepth = result.Shape.Dimensions[2];
			var batchSize = result.Shape.Dimensions[3];

			for (var n = 0; n < batchSize; n++)
			{
				for (var depth = 0; depth < outputDepth; depth++)
				{
					var x = -horizontalPad;
					for (var ax = 0; ax < outputWidth; x += verticalStride, ax++)
					{
						var y = -verticalPad;
						for (var ay = 0; ay < outputHeight; y += horizontalStride, ay++)
						{
							var a = double.MinValue;

							for (var fx = 0; fx < windowWidth; fx++)
							{
								for (var fy = 0; fy < windowHeight; fy++)
								{
									var oy = y + fy;
									var ox = x + fx;
									if (oy >= 0 && oy < inputHeight && ox >= 0 && ox < inputWidth)
									{
										var v = Get(ox, oy, depth, n);
										// perform max pooling and store pointers to where
										// the max came from. This will speed up backprop 
										// and can help make nice visualizations in future
										if (v > a)
										{
											a = v;
										}
									}
								}
							}

							result.Storage.Set(ax, ay, depth, n, a);
						}
					}
				}
			}
		}

		/// <summary>
		/// Compute Gradient of the Pool Computation
		/// </summary>
		/// <param name="input">input</param>
		/// <param name="outputGradient">outputGradient</param>
		/// <param name="windowWidth">windowWidth</param>
		/// <param name="windowHeight">windowHeight</param>
		/// <param name="horizontalPad">horizontalPad</param>
		/// <param name="verticalPad">verticalPad</param>
		/// <param name="horizontalStride">horizontalStride</param>
		/// <param name="verticalStride">verticalStride</param>
		/// <param name="inputGradient">inputGradient</param>
		public void PoolGradient(Volume input, Volume outputGradient,
			int windowWidth, int windowHeight,
			int horizontalPad, int verticalPad, int horizontalStride, int verticalStride,
			Volume inputGradient)
		{
			var inputWidth = input.Shape.Dimensions[0];
			var inputHeight = input.Shape.Dimensions[1];

			var outputWidth = outputGradient.Shape.Dimensions[0];
			var outputHeight = outputGradient.Shape.Dimensions[1];
			var outputDepth = outputGradient.Shape.Dimensions[2];
			var batchSize = outputGradient.Shape.Dimensions[3];

			for (var n = 0; n < batchSize; n++)
			{
				for (var depth = 0; depth < outputDepth; depth++)
				{
					var x = -horizontalPad;
					for (var ax = 0; ax < outputWidth; x += verticalStride, ax++)
					{
						var y = -verticalPad;
						for (var ay = 0; ay < outputHeight; y += horizontalStride, ay++)
						{
							var a = double.MinValue;
							int winx = -1, winy = -1;

							for (var fx = 0; fx < windowWidth; fx++)
							{
								for (var fy = 0; fy < windowHeight; fy++)
								{
									var oy = y + fy;
									var ox = x + fx;
									if (oy >= 0 && oy < inputHeight && ox >= 0 && ox < inputWidth)
									{
										var v = input.Get(ox, oy, depth, n);
										// perform max pooling and store pointers to where
										// the max came from. This will speed up backprop 
										// and can help make nice visualizations in future
										if (v > a)
										{
											a = v;
											winx = ox;
											winy = oy;
										}
									}
								}
							}

							var chainGradient = outputGradient.Get(ax, ay, depth, n);
							inputGradient.Storage.Set(winx, winy, depth, n, chainGradient);
						}
					}
				}
			}
		}

		/// <summary>
		/// Power function on a tensor
		/// </summary>
		/// <param name="power">Exponent</param>
		/// <param name="result">Tensor</param>
		public  void Power(Volume power, Volume result)
		{
			this.Storage.MapEx(Math.Pow, power.Storage, result.Storage);
		}

		/// <summary>
		/// Shape/Topology consistent mathematical Operation
		/// </summary>
		/// <param name="op">Operator</param>
		/// <param name="result">Tensor</param>
		public  void Reduce(TensorReduceOp op, Volume result)
		{
			if (this.Shape.Equals(result.Shape))
			{
				result.Storage.CopyFrom(this.Storage);
				return;
			}

			switch (op)
			{
				case TensorReduceOp.Add:
					Sum(result);
					break;
				case TensorReduceOp.Mul:
					throw new NotImplementedException();
				case TensorReduceOp.Min:
					throw new NotImplementedException();
				case TensorReduceOp.Max:
					Max(result);
					break;
				case TensorReduceOp.AMax:
					throw new NotImplementedException();
				case TensorReduceOp.Avg:
					throw new NotImplementedException();
				case TensorReduceOp.Norm1:
					Norm1(result);
					break;
				case TensorReduceOp.Norm2:
					throw new NotImplementedException();
				default:
					throw new ArgumentOutOfRangeException(nameof(op), op, null);
			}
		}

		/// <summary>
		/// Rectified Linearization Unit
		/// </summary>
		/// <param name="volume">Tensor</param>
		public  void Relu(Volume volume)
		{
			this.Storage.Map(x => x <= 0 ? 0 : x, volume.Storage);
		}


		/// <summary>
		/// ReLU Gradient Tensor
		/// </summary>
		/// <param name="input">Downstream Input</param>
		/// <param name="outputGradient">Output Gradient</param>
		/// <param name="inputGradient">Input Gradient</param>
		public  void ReluGradient(Volume input, Volume outputGradient,
		   Volume inputGradient)
		{
			this.Storage.Map((x, y) => x > 0 ? y : 0, outputGradient.Storage, inputGradient.Storage);
		}


		/// <summary>
		/// Reshaping the Tensor
		/// </summary>
		/// <param name="shape">Shape/Topology</param>
		/// <returns>Tensor</returns>
		public Volume ReShape(Shape shape)
        {
            var guessedShape = new Shape(shape);
            for (var i = 0; i < 4; i++)
            {
                if (shape.Dimensions[i] == Shape.Keep)
                {
                    guessedShape.SetDimension(i, this.Shape.Dimensions[i]);
                }
            }

            guessedShape.GuessUnkownDimension(this.Shape.TotalLength);

            Count--;

            return BuilderInstance.Volume.Build(this.Storage, guessedShape);
        }

		/// <summary>
		/// Reshape the tensor
		/// </summary>
		/// <param name="dimensions">dimesion coordinates</param>
		/// <returns>Tensor</returns>
        public Volume ReShape(params int[] dimensions)
        {
            return ReShape(Shape.From(dimensions));
        }

		/// <summary>
		/// Set a tensor value at a coordinate
		/// </summary>
		/// <param name="coordinates">coordinate array</param>
		/// <param name="value">Value</param>
        public void Set(int[] coordinates, double value)
        {
            this.Storage.Set(coordinates, value);
        }

		/// <summary>
		/// Set a Tensor value at a coordinate
		/// </summary>
		/// <param name="w">W</param>
		/// <param name="h">H</param>
		/// <param name="c">C</param>
		/// <param name="n">N</param>
		/// <param name="value">value</param>
        public void Set(int w, int h, int c, int n, double value)
        {
            this.Storage.Set(w, h, c, n, value);
        }

		/// <summary>
		/// Set a Tensor value at a coordinate
		/// </summary>
		/// <param name="w">W</param>
		/// <param name="h">H</param>
		/// <param name="c">C</param>
		/// <param name="value">N</param>
        public void Set(int w, int h, int c, double value)
        {
            this.Storage.Set(w, h, c, 0, value);
        }

		/// <summary>
		/// Set Tensor value at a coordinate
		/// </summary>
		/// <param name="w">W</param>
		/// <param name="h">H</param>
		/// <param name="value">Value</param>
        public void Set(int w, int h, double value)
        {
            this.Storage.Set(w, h, 0, 0, value);
        }


		/// <summary>
		/// Set Tensor Value at a coordinate
		/// </summary>
		/// <param name="i">I</param>
		/// <param name="value">Value</param>
        public void Set(int i, double value)
        {
            this.Storage.Set(i, value);
        }

		/// <summary>
		/// Sigmoid Activation on Tensor
		/// </summary>
		/// <param name="volume">Tensor</param>
		public  void Sigmoid(Volume volume)
		{
			this.Storage.Map(x => 1.0 / (1.0 + Math.Exp(-x)), volume.Storage);
		}

		/// <summary>
		/// Sigmoid Gradient
		/// </summary>
		/// <param name="input">Input tensor</param>
		/// <param name="outputGradient">gradient</param>
		/// <param name="inputGradient">downstream gradient</param>
		public  void SigmoidGradient(Volume input, Volume outputGradient,
			Volume inputGradient)
		{
			this.Storage.Map((output, outGradient) => output * (1.0 - output) * outGradient, outputGradient.Storage,
				inputGradient.Storage);
		}

		/// <summary>
		/// Sigmoid Normalization
		/// </summary>
		/// <param name="result">Tensor</param>
		public  void Softmax(Volume result)
		{
			var batchSize = this.Shape.Dimensions[3];

			var outputWidth = this.Shape.Dimensions[0];
			var outputHeight = this.Shape.Dimensions[1];
			var outputDepth = this.Shape.Dimensions[2];

			for (var n = 0; n < batchSize; n++)
			{
				// compute max activation
				var amax = double.MinValue;
				for (var depth = 0; depth < outputDepth; depth++)
				{
					for (var ay = 0; ay < outputHeight; ay++)
					{
						for (var ax = 0; ax < outputWidth; ax++)
						{
							var v = Get(ax, ay, depth, n);
							if (v > amax)
							{
								amax = v;
							}
						}
					}
				}

				// compute exponentials (carefully to not blow up)
				var es = new double[outputDepth * outputHeight * outputWidth];
				var esum = 0.0;

				for (var depth = 0; depth < outputDepth; depth++)
				{
					for (var ay = 0; ay < outputHeight; ay++)
					{
						for (var ax = 0; ax < outputWidth; ax++)
						{
							var e = Math.Exp(Get(ax, ay, depth, n) - amax);
							esum += e;
							es[ax + ay * outputWidth + depth * outputWidth * outputHeight] = e;
						}
					}
				}

				// normalize and output to sum to one
				for (var depth = 0; depth < outputDepth; depth++)
				{
					for (var ay = 0; ay < outputHeight; ay++)
					{
						for (var ax = 0; ax < outputWidth; ax++)
						{
							es[ax + ay * outputWidth + depth * outputWidth * outputHeight] /= esum;

							result.Storage.Set(ax, ay, depth, n,
								es[ax + ay * outputWidth + depth * outputWidth * outputHeight]);
						}
					}
				}
			}
		}

		/// <summary>
		/// Softmax Tensor Gradient
		/// </summary>
		/// <param name="outputGradient">upstream gradient</param>
		/// <param name="inputGradient">downstream gradient</param>
		public  void SoftmaxGradient(Volume outputGradient, Volume inputGradient)
		{
			var batchSize = this.Shape.Dimensions[3];

			var outputReshape = ReShape(-1, batchSize);
			var outputGradientReshape = outputGradient.ReShape(-1, batchSize);
			var inputGradientReshape = inputGradient.ReShape(-1, batchSize);

			var firstDim = outputReshape.Shape.Dimensions[0];

			for (var b = 0; b < batchSize; b++)
			{
				var classIndex = -1;

				for (var i = 0; i < firstDim; i++)
				{
					var yi = outputGradientReshape.Get(i, b);

					if (yi == 1.0)
					{
						classIndex = i;
					}
				}

				var pj = outputReshape.Get(classIndex, b);

				// input gradient:
				// pi(1 - pi) if i = class index
				// -pipj if i != class index
				for (var i = 0; i < firstDim; i++)
				{
					var pi = outputReshape.Get(i, b);

					if (i == classIndex)
					{
						inputGradientReshape.Set(i, b, pj * (1.0 - pj));
					}
					else
					{
						inputGradientReshape.Set(i, b, -pj * pi);
					}
				}
			}
		}

		/// <summary>
		/// Square Root
		/// </summary>
		/// <param name="result">Result Tensor</param>
		public  void Sqrt(Volume result)
		{
			this.Storage.Map(Math.Sqrt, result.Storage);
		}

		/// <summary>
		/// Subtract tensors
		/// </summary>
		/// <param name="other">Tensor1</param>
		/// <param name="result">Result Tensor</param>
		public  void SubtractFrom(Volume other, Volume result)
		{
			this.Storage.MapEx((x, y) => y - x, other.Storage, result.Storage);
		}

		/// <summary>
		/// Add tensor
		/// </summary>
		/// <param name="result">Result Tensor</param>
		public  void Sum(Volume result)
		{
			var batchsize = this.Shape.Dimensions[3];
			var channel = this.Shape.Dimensions[2];
			var height = this.Shape.Dimensions[1];
			var width = this.Shape.Dimensions[0];

			var resultWIsOne = result.Shape.Dimensions[0] == 1;
			var resultHIsOne = result.Shape.Dimensions[1] == 1;
			var resultCIsOne = result.Shape.Dimensions[2] == 1;
			var resultNIsOne = result.Shape.Dimensions[3] == 1;

			for (var n = 0; n < batchsize; n++)
			{
				for (var c = 0; c < channel; c++)
				{
					for (var h = 0; h < height; h++)
					{
						for (var w = 0; w < width; w++)
						{
							var val = Get(w, h, c, n);

							var resultW = resultWIsOne ? 0 : w;
							var resultH = resultHIsOne ? 0 : h;
							var resultC = resultCIsOne ? 0 : c;
							var resultN = resultNIsOne ? 0 : n;

							var current = result.Get(resultW, resultH, resultC, resultN);
							result.Set(resultW, resultH, resultC, resultN, current + val);
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Tensor to array
		/// </summary>
		/// <returns>Array</returns>
		public double[] ToArray()
        {
            return this.Storage.ToArray();
        }

        /// <summary>
        /// Flips a 2D volume over its diagonal
        /// [i, j, 0, batch] => [j, i, 0, batch]
        /// </summary>
        /// <param name="result">Tensor</param>
        public void Transpose(Volume result)
		{ var expectedShape = new Shape(this.Shape.Dimensions[1], this.Shape.Dimensions[0], 1, this.Shape.Dimensions[3]);

            if (!result.Shape.Equals(expectedShape))
            {
                throw new ArgumentException($"Result shape should be {expectedShape}");
	}

            for (var n = 0; n< this.Shape.Dimensions[3]; n++)
            {
                for (var j = 0; j< this.Shape.Dimensions[1]; j++)
                {
                    for (var i = 0; i< this.Shape.Dimensions[0]; i++)
                    {
                        result.Set(j, i, 0, n, Get(i, j, 0, n));
                    }
                }
            }
        }
    }
}
