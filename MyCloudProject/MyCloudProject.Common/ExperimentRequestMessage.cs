﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyCloudProject.Common
{
    public class ExperimentRequestMessage
    {
        public string ExperimentId { get; set; }

        public string InputFile { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

    }
}
