using LearningFoundation;
using Newtonsoft.Json;
using System;
using System.IO;

namespace ConvolutionalNetworksTests
{
	internal class Helpers
	{
		private const String defPath = @"Samples\DataMapper.json";
		static String defaultHelpersPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(),defPath);
		public static DataDescriptor GetDataDescriptors(String mapperPath = null)
		{
			String jsonText;
			if (mapperPath is null)
			{
				jsonText = System.IO.File.ReadAllText(defaultHelpersPath);
			}
			else
			{
				jsonText = System.IO.File.ReadAllText(mapperPath);
			}

			var jsonObj = JsonConvert.DeserializeObject(jsonText, typeof(DataDescriptor));
			return (DataDescriptor)jsonObj;
		}
	}
}
