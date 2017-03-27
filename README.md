Async and Sync Cloud Convert Wrapper
===================

.NET driver for converting files through [CloudConvert](https://cloudconvert.com/)

## Usage

```
var cc = new CloudConvertWrapper.CloudConverter(Your_API_KEY);
var bytes = Encoding.ASCII.GetBytes(Your_File);
var result = cc.Convert(inputFormat, outputFormat, desiredFileName, bytes);
var wc = new WebClient();
wc.DownloadFile("http:" + result.Result, saveToFilename);
```
