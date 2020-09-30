using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCloudProject.Common
{
    public interface IStorageProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">The name of the local file where the input is downloaded.</param>
        /// <returns></returns>
        Task<string> DownloadInputFile(string fileName);

        Task<byte[]> UploadResultFile(string fileName, byte[] data);

        Task UploadExperimentResult(ExperimentResult result);
    }
}
