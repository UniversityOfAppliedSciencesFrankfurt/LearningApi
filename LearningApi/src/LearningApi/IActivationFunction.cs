namespace LearningFoundation
{ /// <summary>
  /// Activation function interface.
  /// </summary>
  /// 
  /// <remarks>All activation functions, which are supposed to be used with
  /// neurons, which calculate their output as a function of weighted sum of
  /// their inputs, should implement this interfaces.
  /// </remarks>
  /// 
    public interface IActivationFunction
    {
       

        /// <summary>
        /// Calculates function value.
        /// </summary>
        ///
        /// <param name="x">Function input value.</param>
        /// 
        /// <returns>Function output value, <i>f(x)</i>.</returns>
        ///
        /// <remarks>The method calculates function value at point <paramref name="x"/>.</remarks>
        ///
        double Function( double x );

        //double Derivative(double x);
        //double Derivative2(double y);
    }
}
