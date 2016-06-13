using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CloudConvertWrapper.Model;

namespace CloudConvertWrapper
{
    public interface ICloudConverterAsync
    {
        Task<CloudConvertResponseResult> ConvertAsync(string inputFormat, string outputFormat, string fileName, byte[] data);
    }
}