using System;
using System.Net;
using System.Text;
using CloudConvertWrapper.Model;
using Newtonsoft.Json;

namespace CloudConvertWrapper
{
    public class CloudConverter : ICloudConverter
    {
        private readonly string _apiKey;

        public CloudConverter(string apiKey)
        {
            _apiKey = apiKey;
        }

        public CloudConvertResponseResult Convert(string inputFormat, string outputFormat, string fileName, byte[] data)
        {
            var processResponse = CreateProcess(inputFormat, outputFormat);

            if (processResponse.IsError)
            {
                return processResponse;
            }

            var healthyProcessResponseResult = JsonConvert.DeserializeObject<CloudConvertProcessResponse>(processResponse.Result);


            var conversationResponse = StartConversation(healthyProcessResponseResult, outputFormat);

            if (conversationResponse.IsError)
            {
                return conversationResponse;
            }

            var healthyConversationResponseResult = JsonConvert.DeserializeObject<CloudConvertConversationResponse>(conversationResponse.Result);


            var postUploadJsonResult = PostUploadJson("https:" + healthyConversationResponseResult.Upload.Url + "/" + fileName, data);

            if (postUploadJsonResult.IsError)
            {
                return postUploadJsonResult;
            }

            //Status check.
            var getJsonResult = GetJson("https:" + healthyProcessResponseResult.Url + "/?wait");

            if (getJsonResult.IsError)
            {
                return getJsonResult;
            }

            //Everything went well, Return Download URL.
            return new CloudConvertResponseResult()
            {
                IsError = false,
                Result = healthyConversationResponseResult.Output.Url
            };
        }

        private CloudConvertResponseResult CreateProcess(string inputFormat, string outputFormat)
        {
            try
            {

                var request = new
                {
                    apikey = _apiKey,
                    inputformat = inputFormat,
                    outputformat = outputFormat,

                };

                return PostJson("https://api.cloudconvert.com/process", request);
            }
            catch (Exception ee)
            {
                return new CloudConvertResponseResult()
                {
                    IsError = true,
                    Result = ee.Message

                };
            }

        }


        private CloudConvertResponseResult StartConversation(CloudConvertProcessResponse process, string outputFormat)
        {
            try
            {

                var request = new
                {
                    input = "upload",
                    outputformat = outputFormat,
                    save = false
                };

                return PostJson("https:" + process.Url, request);
            }
            catch (Exception ee)
            {
                return new CloudConvertResponseResult()
                {
                    IsError = true,
                    Result = ee.Message

                };
            }

        }


        private static CloudConvertResponseResult PostJson(string url, object data)
        {
            try
            {
                var parameters = JsonConvert.SerializeObject(data);

                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    return new CloudConvertResponseResult()
                    {
                        IsError = false,
                        Result = wc.UploadString(url, "POST", parameters)

                    };

                }
            }
            catch (Exception ee)
            {
                return new CloudConvertResponseResult()
                {
                    IsError = true,
                    Result = ee.Message

                };
            }

        }

        private static CloudConvertResponseResult GetJson(string url)
        {
            try
            {
                using (var wc = new WebClient())
                {
                    return new CloudConvertResponseResult()
                    {
                        IsError = false,
                        Result = wc.DownloadString(url)

                    };
                }
            }
            catch (Exception ee)
            {
                return new CloudConvertResponseResult()
                {
                    IsError = true,
                    Result = ee.Message

                };
            }

        }

        private static CloudConvertResponseResult PostUploadJson(string url, byte[] data)
        {
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "binary/octet-stream";
                    var result = wc.UploadData(url, "PUT", data);
                    return new CloudConvertResponseResult()
                    {
                        IsError = false,
                        Result = Encoding.UTF8.GetString(result, 0, result.Length)

                    };
                }
            }
            catch (Exception ee)
            {
                return new CloudConvertResponseResult()
                {
                    IsError = true,
                    Result = ee.Message

                };
            }

        }

        
    }
}