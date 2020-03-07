// Copyright (c) daenet GmbH / Frankfurt University of Applied Sciences. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using LearningFoundation;
using System.Drawing;

namespace EuclideanFilter
{
    /// <summary>
    ///ExtenstionMethod Class
    /// </summary>
    public static class EuclideanFilterModuleExtensions
    {
        /// <summary>
        /// Extension method of Learning Api -> This is needed, to use the "api.Run() as.." - metho
        /// </summary>
        /// <param name="api"></param>
        /// <param name="color"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static LearningApi UseMyPipelineModule(this LearningApi api, Color color, float radius)
        {
            EuclideanFilterModule module = new EuclideanFilterModule(color, radius);
            api.AddModule(module, $"EuclideanFilterModule-{Guid.NewGuid()}");
            return api;
        }
    }
}
