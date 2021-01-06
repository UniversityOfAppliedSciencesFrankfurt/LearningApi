using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ConvolutionalNetworks.Tensor
{
	/// <summary>
	/// Tensor Topology/Shape object defines the number of classes,count and image dimensions
	/// and the operations that are bound along each dimension of the tensor.
	/// </summary>
    [DebuggerDisplay("Shape {PrettyPrint()}")]
    public class Shape : IEquatable<Shape>
    {
        /// <summary>
        /// Mapping for Auto-guess
        /// </summary>
        public static int None = -1; // Automatically guesses

        /// <summary>
        /// Mapping for dimension retention
        /// </summary>
        public static int Keep = -2; // Keep same dimension

        /// <summary>
        /// Dimension array
        /// </summary>
		public int[] Dimensions { get; set; }

        /// <summary>
        ///     Create shape of [1,1,c,1]
        /// </summary>
        /// <param name="c">Shape dimension</param>
        public Shape(int c)
        {
            if (c == 0)
            {
                throw new ArgumentException("Dimension cannot be 0", nameof(c));
            }

            this.Dimensions = new[] { 1, 1, c, 1 };
            UpdateTotalLength();
        }

        /// <summary>
        ///     Create shape of [dimensionW,dimensionH,1,1]
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public Shape(int w, int h)
        {
            if (w == 0)
            {
                throw new ArgumentException("Dimension cannot be 0", nameof(w));
            }

            if (h == 0)
            {
                throw new ArgumentException("Dimension cannot be 0", nameof(h));
            }

            this.Dimensions = new[] { w, h, 1, 1 };
            UpdateTotalLength();
        }

        /// <summary>
        ///  Create shape of [dimensionW,dimensionH,dimensionC,1]
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="c">Number of classes</param>
        public Shape(int w, int h, int c)
        {
            if (w == 0)
            {
                throw new ArgumentException("Dimension cannot be 0", nameof(w));
            }

            if (h == 0)
            {
                throw new ArgumentException("Dimension cannot be 0", nameof(h));
            }

            if (c == 0)
            {
                throw new ArgumentException("Dimension cannot be 0", nameof(c));
            }

            this.Dimensions = new[] { w, h, c, 1 };
            UpdateTotalLength();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        /// <param name="c">Number of output classes</param>
        /// <param name="batchSize">Training batch size</param>
        public Shape(int w, int h, int c, int batchSize)
        {
            if (w == 0)
            {
                throw new ArgumentException("Dimension cannot be 0", nameof(w));
            }

            if (h == 0)
            {
                throw new ArgumentException("Dimension cannot be 0", nameof(h));
            }

            if (c == 0)
            {
                throw new ArgumentException("Dimension cannot be 0", nameof(c));
            }

            if (batchSize == 0)
            {
                throw new ArgumentException("Dimension cannot be 0", nameof(batchSize));
            }

            this.Dimensions = new[] { w, h, c, batchSize };
            UpdateTotalLength();
        }

        /// <summary>
        /// Shape Constructor
        /// </summary>
        /// <param name="shape">Shape Parameter</param>
        public Shape(Shape shape)
        {
            this.Dimensions = (int[])shape.Dimensions.Clone();
            UpdateTotalLength();
        }
        /// <summary>
        /// Is Scalar Identifier
        /// </summary>
        public bool IsScalar { get; private set; }

        /// <summary>
        /// Total Length Identifier
        /// </summary>
        public long TotalLength { get; private set; }

        /// <summary>
        /// Equality Check
        /// </summary>
        /// <param name="other">Tensor shape</param>
        /// <returns>Equality checks</returns>
        public bool Equals(Shape other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.TotalLength != other.TotalLength)
            {
                return false;
            }

            for (var i = 0; i < 4; i++)
            {
                if (this.Dimensions[i] != other.Dimensions[i])
                {
                    return false;
                }
            }

            return true;
        }

        private string DimensionToString(int d)
        {
            return d == -1 ? "None" : (d == -2 ? "Keep" : d.ToString());
        }

        /// <summary>
        /// Equality Check Overload
        /// </summary>
        /// <param name="obj">Object Comparison</param>
        /// <returns>Equality checks</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Shape)obj);
        }
		/// <summary>
		/// Returns a Shape object from dimension definitions
		/// </summary>
		/// <param name="dimensions">dimension array</param>
		/// <returns>Shape Object</returns>
        public static Shape From(params int[] dimensions)
        {
            switch (dimensions.Length)
            {
                case 1: return new Shape(dimensions[0]);
                case 2: return new Shape(dimensions[0], dimensions[1]);
                case 3: return new Shape(dimensions[0], dimensions[1], dimensions[2]);
                case 4: return new Shape(dimensions[0], dimensions[1], dimensions[2], dimensions[3]);
            }

            throw new ArgumentException($"Invalid number of dimensions {dimensions.Length}. It should be > 0 and <= 4");
        }

		/// <summary>
		/// Create a Shape Object
		/// </summary>
		/// <param name="original">Shape Object</param>
		/// <returns>Shape Object</returns>
        public static Shape From(Shape original)
        {
            return new Shape(original);
        }
		/// <summary>
		/// Generate the hash code for the tensor topology
		/// </summary>
		/// <returns>Hash Code</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((this.Dimensions?.GetHashCode() ?? 0) * 397) ^ this.TotalLength.GetHashCode();
            }
        }

		/// <summary>
		/// Guess the total dimension elements of the stored tensor
		/// </summary>
		/// <param name="totalLength">Total Length of the tensor storage</param>
        public void GuessUnkownDimension(long totalLength)
        {
            long product = 1;
            var unknownIndex = -1;

            if (totalLength <= 0)
            {
                throw new ArgumentException($"{nameof(totalLength)} must be non-negative, not {totalLength}");
            }

            for (var d = 0; d < 4; ++d)
            {
                var size = this.Dimensions[d];
                if (size == -1)
                {
                    if (unknownIndex != -1)
                    {
                        throw new ArgumentException($"Only one input size may be - 1, not both {unknownIndex} and  {d}");
                    }

                    unknownIndex = d;
                }
                else
                {
                    if (size <= 0)
                    {
                        throw new ArgumentException($"Dimension #{d} must be non-negative, not {size}");
                    }

                    product *= size;
                }
            }

            if (unknownIndex != -1)
            {
                if (product <= 0)
                {
                    throw new ArgumentException("Reshape cannot infer the missing input size " +
                                                "for an empty volume unless all specified " +
                                                "input sizes are non-zero");
                }

                var missing = totalLength / product;

                if (missing * product != totalLength)
                {
                    throw new ArgumentException($"Input to reshape is a tensor with totalLength={totalLength}, " +
                                                $"but the requested shape requires totalLength to be a multiple of {product}");
                }

                SetDimension(unknownIndex, (int)missing);
            }
            else
            {
                if (product != totalLength)
                {
                    throw new ArgumentException("incompatible dimensions provided");
                }
            }
        }
		/// <summary>
		/// Print
		/// </summary>
		/// <param name="sep">Separator</param>
		/// <returns>String output</returns>
        public string PrettyPrint(string sep = "x")
        {
            var sb = new StringBuilder();
            for (var i = 0; i < 3; i++)
            {
                sb.Append(DimensionToString(this.Dimensions[i]));
                sb.Append(sep);
            }

            sb.Append(DimensionToString(this.Dimensions[3]));
            return sb.ToString();
        }

		/// <summary>
		/// Set the dimensions
		/// </summary>
		/// <param name="index">Index</param>
		/// <param name="dimension">Dimension Definition</param>
        public void SetDimension(int index, int dimension)
        {
            if (index < 0)
            {
                throw new ArgumentException("index cannot be negative", nameof(index));
            }

            this.Dimensions[index] = dimension;
            UpdateTotalLength();
        }
		/// <summary>
		/// Prints out the to STring
		/// </summary>
		/// <returns>Returns String representation of object</returns>
        public override string ToString()
        {
            return PrettyPrint();
        }

        private void UpdateTotalLength()
        {
            this.TotalLength = this.Dimensions.Aggregate((long)1, (acc, val) => acc * val);
            this.IsScalar = this.Dimensions[0] == this.Dimensions[1] == (this.Dimensions[2] == 1);
        }
    }
}
