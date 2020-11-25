using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace HelloLearningApiPipelineModule 
{
    public class HelloLearningApiPipelineModuleExtension : IPipelineModule<double[,,], double[,,]>
    {
   
            public static double[,] XSobel
        {
            get
            {
                return new double[,]
                {
                    { -1, 0, 1 },
                    { -2, 0, 2 },
                    { -1, 0, 1 }
                };
            }
        }

        public static double[,] YSobel
        {
            get
            {
                return new double[,]
                {
                    {  1,  2,  1 },
                    {  0,  0,  0 },
                    { -1, -2, -1 }
                };
            }
        }

        public double[,,] Run(double[,,] data, IContext ctx)
        {
            return ConvolutionFilter(data, XSobel, YSobel);

        }

        private double[,,] ConvolutionFilter(double[,,] data, double[,] xSobel, double[,] ySobel)
        {
            throw new NotImplementedException();
        }
    }
    }
}
