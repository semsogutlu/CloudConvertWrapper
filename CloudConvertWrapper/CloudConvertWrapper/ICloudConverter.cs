using CloudConvertWrapper.Model;

namespace CloudConvertWrapper
{
    public interface ICloudConverter
    {
        CloudConvertResponseResult Convert(string inputFormat, string outputFormat, string fileName, byte[] data);
    }
}