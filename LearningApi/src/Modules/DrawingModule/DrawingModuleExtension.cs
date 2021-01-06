using LearningFoundation;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyDrawingModule
{
   public static class DrawingModuleExtension
    {
        public static LearningApi UseDrawingModule(this LearningApi api, string Drawing, List<DrawingModule> drawingPrms)
        {

            DrawingModule impl = new DrawingModule();
            api.AddModule(impl, $"MyPipelineComponent-{Guid.NewGuid()}");
            return api;


        }
    }
}
