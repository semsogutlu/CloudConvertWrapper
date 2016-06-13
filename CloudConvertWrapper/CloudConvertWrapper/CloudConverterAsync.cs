using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CloudConvertWrapper.Model;
using Newtonsoft.Json;

namespace CloudConvertWrapper
{
    public class CloudConverterAsync : ICloudConverterAsync
    {
        private readonly string _apiKey;

        public CloudConverterAsync(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<CloudConvertResponseResult> ConvertAsync(string inputFormat, string outputFormat, string fileName, byte[] data)
        {
            var processResponse = await CreateProcessAsync(inputFormat, outputFormat);

            if (processResponse.IsError)
            {
                return processResponse;
            }

            var healthyProcessResponseResult = JsonConvert.DeserializeObject<CloudConvertProcessResponse>(processResponse.Result);


            var conversationResponse = await StartConversationAsync(healthyProcessResponseResult, outputFormat);

            if (conversationResponse.IsError)
            {
                return conversationResponse;
            }

            var healthyConversationResponseResult = JsonConvert.DeserializeObject<CloudConvertConversationResponse>(conversationResponse.Result);


            var postUploadJsonResult = await PostUploadJsonAsync("https:" + healthyConversationResponseResult.Upload.Url + "/" + fileName, data);

            if (postUploadJsonResult.IsError)
            {
                return postUploadJsonResult;
            }

            //Status check.
            var getJsonResult = await GetJsonAsync("https:" + healthyProcessResponseResult.Url + "/?wait");

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

        private async Task<CloudConvertResponseResult> CreateProcessAsync(string inputFormat, string outputFormat)
        {
            try
            {

                var request = new
                {
                    apikey = _apiKey,
                    inputformat = inputFormat,
                    outputformat = outputFormat,

                };

                return await PostJsonAsync("https://api.cloudconvert.com/process", request);
            }
            catch (WebException webEx)
            {
                return await new Task<CloudConvertResponseResult>(() => new CloudConvertResponseResult()
                {
                    IsError = true,
                    Result = webEx.Message
                });
            }

        }


        private async Task<CloudConvertResponseResult> StartConversationAsync(CloudConvertProcessResponse process, string outputFormat)
        {
            try
            {

                var request = new
                {
                    input = "upload",
                    outputformat = outputFormat,
                    save = false
                };

                return await PostJsonAsync("https:" + process.Url, request);
            }
            catch (WebException webEx)
            {
                return await new Task<CloudConvertResponseResult>(() => new CloudConvertResponseResult()
                {
                    IsError = true,
                    Result = webEx.Message
                });
            }

        }


        private static async Task<CloudConvertResponseResult> PostJsonAsync(string url, object data)
        {
            try
            {
                var parameters = JsonConvert.SerializeObject(data);
                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    var result = await wc.UploadStringTaskAsync(url, "POST", parameters)
                        .ContinueWith(task => new CloudConvertResponseResult()
                        {
                            IsError = false,
                            Result = task.Result
                        });
                    return result;
                }

            }
            catch (WebException webEx)
            {
                return await new Task<CloudConvertResponseResult>(() => new CloudConvertResponseResult()
                {
                    IsError = true,
                    Result = webEx.Message
                });
            }

        }

        private static async Task<CloudConvertResponseResult> GetJsonAsync(string url)
        {
            try
            {
                using (var wc = new WebClient())
                {
                    var result = wc.DownloadStringTaskAsync(url)
                        .ContinueWith(task => new CloudConvertResponseResult()
                        {
                            IsError = false,
                            Result = task.Result
                        });
                    return await result;
                }
            }
            catch (WebException webEx)
            {
                return await new Task<CloudConvertResponseResult>(() => new CloudConvertResponseResult()
                {
                    IsError = true,
                    Result = webEx.Message
                });
            }

        }


        private static async Task<CloudConvertResponseResult> PostUploadJsonAsync(string url, byte[] data)
        {

            try
            {


                using (var wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "binary/octet-stream";
                    var result = wc.UploadDataTaskAsync(url, "PUT", data)
                        .ContinueWith(task => new CloudConvertResponseResult()
                        {
                            IsError = false,
                            Result = Encoding.UTF8.GetString(task.Result, 0, task.Result.Length)
                        });
                    return await result;
                }
            }
            catch (WebException webEx)
            {
                return await new Task<CloudConvertResponseResult>(() => new CloudConvertResponseResult()
                {
                    IsError = true,
                    Result = webEx.Message
                });
            }


        }
    }
}