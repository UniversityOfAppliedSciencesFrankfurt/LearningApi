using LearningFoundation;
using System;
using System.Collections.Generic;

namespace LearningFoundation.MlAlgorithms.SupportVectorMachineAlgorithm
{
    /// <summary>
    /// SupportVectorMachineAlgorithm is implementation of classification objects by using Support Vector Machince (SVM) Technique.
    /// In this project, SVM will consider a linear classifier for a binary classification problem with labels y (y = 1 or y = -1) and features.
    /// It follows the Simplified Sequential Minimal Optimization algorithm.
    /// </summary>
    public class SVMAlgorithm : IAlgorithm
    {
        #region Properties

        private double bias;
        private double[] alphas;
        private double[] weight; 
        private readonly double c; //regularization parameter
        private readonly double tol; //numerical tolerance
        private double max_passes; //maximum number of times to iterate over alphas without changing

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for SVM. Set value of regularization parameter
        /// </summary>
        /// <param name="c">regularization parameter</param>
        /// <param name="tol">tolerance nummerical is set to 0.01. It can be changed by users</param>
        public SVMAlgorithm(double c, double tol = 0.01)
        {
            this.c = c;
            this.tol = tol;
        }

        /// <summary>
        /// Train is method that will call Run to start training data
        /// </summary>
        /// <param name="data">data for training</param>
        /// <param name="ctx"></param>
        /// <returns>the training results</returns>
        public IScore Train(double[][] data, IContext ctx) => Run(data, ctx);

        /// <summary>
        /// Run is a method that starts training data
        /// </summary>
        /// <param name="rawData">data for training</param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public IScore Run(double[][] rawData, IContext ctx)
        {
            SVMScore score = new SVMScore();

            score = RunCalculation(rawData);
                                 
            return score;
        }

        /// <summary>
        /// Predict is a method that classify the data based on the training result
        /// </summary>
        /// <param name="data">data for testing</param>
        /// <param name="ctx"></param>
        /// <returns>the testing results</returns>
        public IResult Predict(double[][] data, IContext ctx)
        {
            int Code;
            string Message = "Function <Predict>: ";
            try
            {
                SVMResult result = new SVMResult()
                {
                    PredictedResult = new int[data.Length]
                     
                };

                int predictIndex = 0;

                int n = 0;
                //predict with each item 
                foreach (var item in data)
                {
                    predictIndex = PredictSample(item, this.weight, this.bias);
                    result.PredictedResult[n++] = predictIndex;
                }
                return result;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception: \t" + Ex.ToString();
                throw new SVMException(Code, Message);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// RunCalculation is method that training data in method Train
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private SVMScore RunCalculation(double[][] rawData)
        {
            SVMScore res = new SVMScore();
            int Code;
            string Message = "Function <Train>: ";
            try
            {
                //does some checks on the raw data
                VerifyRawDataConsistency(rawData);

                //convert raw data to data array
                double[,] DataArray = ConvertRawDataToArray(rawData);

                //get input data from data array
                double[,] DataInput = RawDataInput(DataArray);

                //get output data from data array
                double[,] DataOutput2D = RawDataOutput(DataArray);

                //convert output data to single array
                double[] DataOutput = MultiToSingle(DataOutput2D);

                //get the value of bias and alphas
                //this.tol = 0.01;
                this.max_passes = DataInput.Length;
                SimplifiedSMO(DataInput, DataOutput, this.c, this.tol, this.max_passes, out alphas, out bias);
                res.Alphas = this.alphas;
                res.Bias = this.bias;

                //get the value of weight
                this.weight = ComputeWeight(this.alphas, DataInput, DataOutput);
                res.Weight = this.weight;

                //show result via Message
                res.Message = "Training completed!";

                return res;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new SVMException(Code, Message);
            }
        }

        /// <summary>
        /// verifyRawDataConsistency is a method that checks that all the samples have same given number of attributes.
        /// </summary>
        /// <param name="rawData">the samples</param>
        internal static void VerifyRawDataConsistency(double[][] rawData)
        {
            int Code;
            string Message = "Function <verifyRawDataConsistency>: ";
            try
            {
                if (rawData == null)
                {
                    Code = 100;
                    Message += "RawData is null";
                    throw new SVMException(Code, Message);
                }

                if (rawData.Length < 1)
                {
                    Code = 102;
                    Message += "RawData is empty";
                    throw new SVMException(Code, Message);
                }

                int DataLength = rawData.Length;

                for (int i = 0; i < DataLength; i++)
                {
                    if (rawData[i] == null || rawData[i].Length < 2)
                    {
                        Code = 101;
                        Message += "RawData is not the right form. At least one attribute column and one target column.";
                        throw new SVMException(Code, Message);
                    }
                }
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new SVMException(Code, Message);
            }
        }

        /// <summary>
        /// convertDataToArray is a method that convert jagged array raw data to multidimensional array
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        internal static double[,] ConvertRawDataToArray(double[][] rawData)
        {
            int Code;
            string Message = "Function <ConvertRawDataToArray>: ";
            try
            {
                double[,] rawDataArray = new double[rawData.Length, rawData[0].Length];
                for (int i = 0; i < rawData.Length; i++)
                {
                    for (int k = 0; k < rawData[0].Length; k++)
                    {
                        rawDataArray[i, k] = rawData[i][k];
                    }
                }
                return rawDataArray;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new SVMException(Code, Message);
            }
        }

        /// <summary>
        /// rawDataInput is a method that split raw input data from raw array data 
        /// </summary>
        /// <param name="rawDataArray"></param>
        /// <returns></returns>
        internal static double[,] RawDataInput(double[,] rawDataArray)
        {
            int Code;
            string Message = "Function <RawDataInput>: ";
            try
            {
                double[,] rawDataInput = new double[rawDataArray.GetLength(0), rawDataArray.GetLength(1) - 1];

                for (int i = 0; i < rawDataArray.GetLength(0); i++)
                {
                    for (int k = 0; k < rawDataArray.GetLength(1) - 1; k++)
                    {
                        rawDataInput[i, k] = rawDataArray[i, k];
                    }
                }
                return rawDataInput;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new SVMException(Code, Message);
            }
        }

        /// <summary>
        /// rawDataOutput is a method that split raw output data from raw array data
        /// </summary>
        /// <param name="rawDataArray"></param>
        /// <returns></returns>
        internal static double[,] RawDataOutput(double[,] rawDataArray)
        {
            int Code;
            string Message = "Function <RawDataOutput>: ";
            try
            {
                double[,] rawDataOutput = new double[rawDataArray.GetLength(0), 1];

                for (int i = 0; i < rawDataArray.GetLength(0); i++)
                {
                    rawDataOutput[i, 0] = rawDataArray[i, rawDataArray.GetLength(1) - 1];
                }
                return rawDataOutput;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new SVMException(Code, Message);
            }
        }


        /// <summary>
        /// SelectJRandom chooses the order of the number alphaJ
        /// </summary>
        /// <param name="i">the order of alpha i</param>
        /// <param name="m">number of element in array alpha</param>
        /// <returns></returns>
        private static int SelectJRandom(int i, int m)
        {
            int j = i;

            Random random = new Random();
            while (j == i)
            {
                j = random.Next(0, m);
            }
            return j;
        }

        /// <summary>
        /// ClipAlphaJ adjusts the value alphaJ depending on the constraint
        /// </summary>
        /// <param name="alphaJ"></param>
        /// <param name="h"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        private static double ClipAlphaJ(double alphaJ, double h, double l)
        {
            if (alphaJ > h)
            {
                alphaJ = h;
            }
            else if (l > alphaJ)
            {
                alphaJ = l;
            }
            return alphaJ;
        }


        /// <summary>
        /// SimplifiedSMO finds the value of bias and array alpha 
        /// </summary>
        /// <param name="rawDataInput"></param>
        /// <param name="rawDataOutput"></param>
        /// <param name="c">regularization parameter</param>
        /// <param name="tol">numerical tolerance</param>
        /// <param name="max_passes">maximum number of times to iterate over alpha without changing</param>
        /// <param name="alphas_out">the value of alpha after calculation</param>
        /// <param name="bias_out">the value of bias after calculation</param>
        private static void SimplifiedSMO(double[,] rawDataInput, double[] rawDataOutput, double c, double tol, double max_passes, out double[] alphas_out, out double bias_out)
        {
            int Code;
            string Message;
            try
            {
                double[,] X = rawDataInput;
                double[] Y = rawDataOutput;
                int rowX = X.GetLength(0);
                int colX = X.GetLength(1);
                int passes = 0;
                int num_changed_alphas;

                //Initialize bias: threshold for solution
                double b = 0;
                //Initialize alphas: Lagrage multiplier for solution
                double[] alphas = new double[rowX];
                alphas = InitArray(alphas);

                double[] E = new double[rowX];
                E = InitArray(E);

                double[] alphas_old = new double[rowX];
                alphas_old = InitArray(alphas_old);

                while (passes < max_passes)
                {
                    num_changed_alphas = 0;
                    for (int i = 0; i < rowX; i++)
                    {
                        //create Xi
                        double[,] Xi2DArray = new double[1, colX];
                        for (int d = 0; d < colX; d++)
                        {
                            Xi2DArray[0, d] = X[i, d];
                        }
                        double[] Xi = MultiToSingle(Xi2DArray);

                        //Calculate Ei = f(xi) - yi
                        double Fxi = CalculateFx(X, Y, alphas, b, Xi);
                        E[i] = Fxi - Y[i];
                        if (((-Y[i] * E[i] > tol) && (-alphas[i] > -c)) || ((Y[i] * E[i] > tol) && (alphas[i] > 0)))
                        {
                            //select j#i randomly
                            int j = SelectJRandom(i, rowX);

                            //create Xj
                            double[,] Xj2DArray = new double[1, colX];
                            for (int e = 0; e < colX; e++)
                            {
                                Xj2DArray[0, e] = X[j, e];
                            }
                            double[] Xj = MultiToSingle(Xj2DArray);

                            //Calculate Ej = f(xj) - yj
                            double Fxj = CalculateFx(X, Y, alphas, b, Xj);
                            E[j] = Fxj - Y[j];

                            //save old alphas
                            alphas_old[i] = alphas[i];
                            alphas_old[j] = alphas[j];

                            //compute l and h values
                            double l, h;
                            if (Y[i] != Y[j])
                            {
                                l = Math.Max(0, alphas[j] - alphas[i]);
                                h = Math.Min(c, c + alphas[j] - alphas[i]);
                            }
                            else
                            {
                                l = Math.Max(0, alphas[i] + alphas[j] - c);
                                h = Math.Min(c, alphas[i] + alphas[j]);
                            }

                            //if l = h the continue to next i
                            if (l == h)
                            {
                                continue;
                            }

                            //compute eta
                            double eta = 2 * DotProduct(Xi, Xj);
                            eta = eta - DotProduct(Xi, Xi);
                            eta = eta - DotProduct(Xj, Xj);

                            //if eta > 0 the continue to next i
                            if (eta >= 0)
                            {
                                continue;
                            }

                            //compute new value for alpha j
                            alphas[j] = alphas_old[j] - (Y[j] * (E[i] - E[j]) / eta);

                            //clip new value for alpha j
                            alphas[j] = ClipAlphaJ(alphas[j], h, l);

                            //if |alphasJ - alphasJOld| < tol the continue to next i
                            if (Math.Abs(alphas[j] - alphas_old[j]) < 0.00001)
                            {
                                continue;
                            }

                            //determine value for alpha i
                            alphas[i] += Y[i] * Y[j] * (alphas_old[j] - alphas[j]);

                            //compute b1, b2
                            double ii = DotProduct(Xi, Xi);
                            double ij = DotProduct(Xi, Xj);
                            double jj = DotProduct(Xj, Xj);

                            double b1 = b - E[i] - (Y[i] * ii * (alphas[i] - alphas_old[i])) - (Y[j] * ij * (alphas[j] - alphas_old[j]));
                            double b2 = b - E[j] - (Y[i] * ij * (alphas[i] - alphas_old[i])) - (Y[j] * jj * (alphas[j] - alphas_old[j]));

                            //compute b
                            if ((alphas[i] > 0) && (alphas[i] < c))
                            {
                                b = b1;
                            }
                            else if ((alphas[j] > 0) && (alphas[j] < c))
                            {
                                b = b2;
                            }
                            else
                            {
                                b = (b1 + b2) / 2;
                            }
                            num_changed_alphas += 1;
                        }
                    }
                    if (num_changed_alphas == 0)
                    {
                        passes += 1;
                    }
                    else
                    {
                        passes = 0;
                    }
                }
                alphas_out = alphas;
                bias_out = b;

            }
            catch (Exception Ex)
            {
                Code = 400;
                Message = "Unhandled exception:\t" + Ex.ToString();
                throw new SVMException(Code, Message);
            }
            
        }
       
        /// <summary>
        /// CalculateFx calculates the value for a new data point
        /// </summary>
        /// <param name="rawDataInput"></param>
        /// <param name="rawDataOutput"></param>
        /// <param name="alphas"></param>
        /// <param name="bias"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double CalculateFx(double[,] rawDataInput, double[] rawDataOutput, double[] alphas, double bias, double[] x)
        {
            int Code;
            string Message;
            try
            {
                double res = 0;
                for (int i = 0; i < rawDataInput.GetLength(0); i++)
                {
                    //create Xi
                    double[,] Xi2DArray = new double[1, rawDataInput.GetLength(1)];
                    for (int j = 0; j < rawDataInput.GetLength(1); j++)
                    {
                        Xi2DArray[0, j] = rawDataInput[i, j];
                    }
                    //convert Xi to single array
                    double[] Xi = MultiToSingle(Xi2DArray);
                    //res += (alphas[i] * rawDataOutput[i] * RBFKernel(Xi, x, 1));
                    res += alphas[i] * rawDataOutput[i] * DotProduct(Xi, x);
                }
                res += bias;

                return res;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message = "Unhandled exception:\t" + Ex.ToString();
                throw new SVMException(Code, Message);
            }
            
        }
      
        /// <summary>
        /// ComputeWeight finds the value of the weight
        /// </summary>
        /// <param name="alphas"></param>
        /// <param name="rawDataInput"></param>
        /// <param name="rawDataOutput"></param>
        /// <returns></returns>
        private static double[] ComputeWeight(double[] alphas, double[,] rawDataInput, double[] rawDataOutput)
        {
            int Code;
            string Message;
            try
            {
                //Initialize weight
                double[] weight = new double[rawDataInput.GetLength(1)];
                weight = InitArray(weight);
                for (int i = 0; i < rawDataInput.GetLength(0); i++)
                {
                    //create Xi
                    double[,] Xi2DArray = new double[1, rawDataInput.GetLength(1)];
                    for (int j = 0; j < rawDataInput.GetLength(1); j++)
                    {
                        Xi2DArray[0, j] = rawDataInput[i, j];
                    }
                    //convert Xi to single array
                    double[] Xi = MultiToSingle(Xi2DArray);
                    weight = AddArray(weight, MultiplyNumberAndArray(alphas[i] * rawDataOutput[i], Xi));
                }
                return weight;
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message = "Unhandled exception:\t" + Ex.ToString();
                throw new SVMException(Code, Message);
            }
            
        }

        /// <summary>
        /// PredictSample detects to which class the given sample belongs to
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="weight"></param>
        /// <param name="bias"></param>
        /// <returns></returns>
        internal static int PredictSample(double[] sample, double[] weight, double bias)
        {
            int Code;
            string Message = "Function <PredictSample>: ";
            try
            {
                double predictResult = DotProduct(weight, sample) + bias;
                if (predictResult >= 0)
                {
                    return 1; //the sample is belong to class 1
                }
                else
                {
                    return -1; // the sample is belong to class -1
                }
            }
            catch (Exception Ex)
            {
                Code = 400;
                Message += "Unhandled exception:\t" + Ex.ToString();
                throw new SVMException(Code, Message);                
            }
        }

        /// <summary>
        /// InitArray is a method that fill the array with specific value. In this case the value is 0.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static double[] InitArray(double[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = 0;
            }
            return array;
        }

        /// <summary>
        /// multiplyNumberAndArray is a method that multiply number and array
        /// </summary>
        /// <param name="number"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static double[] MultiplyNumberAndArray(double number, double[] array)
        {
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = number * array[i];
            }
            return result;
        }

        /// <summary>
        /// addArray is a method that add two arrays
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        internal static double[] AddArray(double[] array1, double[] array2)
        {
            double[] result = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
            {
                result[i] = array1[i] + array2[i];
            }
            return result;
        }

        /// <summary>
        /// DotProduct is a method that calculate dot product of two vectors
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        internal static double DotProduct(double[] vec1, double[] vec2)
        {
            if (vec1 == null)
            {
                return 0;
            }
            if (vec2 == null)
            {
                return 0;
            }
            if (vec1.Length != vec2.Length)
            {
                return 0;
            }
            double res = 0;
            for (int i = 0; i < vec1.Length; i++)
            {
                res += vec1[i] * vec2[i];
            }
            return res;
        }

        /// <summary>
        /// MultiToSingle is a method that convert multidimensional array to single array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static double[] MultiToSingle(double[,] array)
        {
            int index = 0;
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            double[] single = new double[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    single[index] = array[x, y];
                    index++;
                }
            }
            return single;
        }               

        #endregion

    }
}
