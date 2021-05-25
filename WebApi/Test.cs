using NUnit.Framework;
using RestSharp;
using System.IO;

namespace WebApi
{
    public class RequestBuilder
    {
        private RestRequest request;
        public RequestBuilder(Method method)
        {
            request = new RestRequest(method);
        }
        public RequestBuilder AddHeaderToRequest(string name, string value)
        {
            request.AddHeader(name, value);
            return this;
        }
        public RequestBuilder AddByteParameterToRequest(string name, byte[] data, ParameterType type)
        {
            request.AddParameter(name, data, type);
            return this;
        }
        public RequestBuilder AddStringParameterToRequest(string name, string data, ParameterType type)
        {
            request.AddParameter(name, data, type);
            return this;
        }
        public RestRequest GetRequest()
        {
            return request;
        }
    }

    public class Tests
    {
        private string _picJpg = "../../../pic.jpg";
        private string _bearer = "Bearer w36p9SOuJNMAAAAAAAAAAaKDfZnGnWSJncHEOGaixzsiYgFTRKyixmI4g_4aj43E";

        [Test]
        public void FileUploadTest()
        {
            var client = new RestClient("https://content.dropboxapi.com/2/files/upload");
            client.Timeout = -1;
            byte[] data = File.ReadAllBytes(_picJpg);
            var request = new RequestBuilder(Method.POST)
                .AddHeaderToRequest("Authorization", _bearer)
                .AddHeaderToRequest("Dropbox-API-Arg", "{\"mode\":\"add\"," +
                                                      "\"autorename\":false," +
                                                      "\"mute\":false," +
                                                      "\"path\":\"/uploadpic.jpg\"}")
                .AddHeaderToRequest("Content-Type", "application/octet-stream")
                .AddByteParameterToRequest("application/octet-stream", data, 
                    ParameterType.RequestBody)
                .GetRequest();
            IRestResponse response = client.Execute(request);
            Assert.AreEqual(200, (int) response.StatusCode);
        }

        [Test]
        public void GetFileMetadataTest()
        {
            var client = new RestClient("https://api.dropboxapi.com/2/files/get_metadata");
            client.Timeout = -1;
            var request = new RequestBuilder(Method.POST)
                .AddHeaderToRequest("Authorization", _bearer)
                .AddHeaderToRequest("Content-Type", "application/json")
                .AddStringParameterToRequest("application/json",
                    "{\r\n\"path\":\"/Homework/math\",\r\n" +
                    "\"include_media_info\": false,\r\n" +
                    "\"include_deleted\": false,\r\n" +
                    "\"include_has_explicit_shared_members\": false\r\n}",
                    ParameterType.RequestBody)
                .GetRequest();
            IRestResponse response = client.Execute(request);
            Assert.AreEqual(200, (int) response.StatusCode);
        }

        [Test]
        public void DeleteFileTest()
        {
            var uploadClient = new RestClient("https://content.dropboxapi.com/2/files/upload");
            uploadClient.Timeout = -1;
            byte[] data = File.ReadAllBytes(_picJpg);
            var uploadRequest = new RequestBuilder(Method.POST)
                .AddHeaderToRequest("Authorization", _bearer)
                .AddHeaderToRequest("Dropbox-API-Arg", 
                    "{\"mode\":\"add\"," +
                    "\"autorename\":false," +
                    "\"mute\":false," +
                    "\"path\":\"/deletepic.jpg\"}")
                .AddHeaderToRequest("Content-Type", "application/octet-stream")
                .AddByteParameterToRequest("application/octet-stream", data, 
                    ParameterType.RequestBody)
                .GetRequest();
            uploadClient.Execute(uploadRequest);

            var client = new RestClient("https://api.dropboxapi.com/2/files/delete_v2");
            client.Timeout = -1;
            var request = new RequestBuilder(Method.POST)
                .AddHeaderToRequest("Authorization", _bearer)
                .AddHeaderToRequest("Content-Type", "application/json")
                .AddStringParameterToRequest("application/json", "{\r\n\"path\":\"/deletepic.jpg\"\r\n}", 
                    ParameterType.RequestBody)
                .GetRequest();
            IRestResponse response = client.Execute(request);
            Assert.AreEqual(200, (int) response.StatusCode);
        }
    }
}