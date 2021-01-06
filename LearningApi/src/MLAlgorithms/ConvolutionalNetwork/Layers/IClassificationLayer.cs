namespace ConvolutionalNetworks.Layers
{
    /// <summary>
    /// CLassification Layer Interface
    /// </summary>
    public interface IClassificationLayer
    {
        /// <summary>
        /// Number of output Classes defined in Implementation
        /// </summary>
        int ClassCount { get; set; }
    }
}
