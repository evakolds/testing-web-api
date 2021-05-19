using NUnit.Framework;
using RestSharp;
using System.IO;

namespace WebApi
{
    public class Tests
    {
        private string _picJpg = "../../../pic.jpg";
        private string _bearer = "Bearer w36p9SOuJNMAAAAAAAAAAaKDfZnGnWSJncHEOGaixzsiYgFTRKyixmI4g_4aj43E";

        [Test]
        public void FileUploadTest()
        {
            var client = new RestClient("https://content.dropboxapi.com/2/files/upload");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", _bearer);
            request.AddHeader("Dropbox-API-Arg", 
                "{\"mode\":\"add\"," +
                "\"autorename\":false," +
                "\"mute\":false," +
                "\"path\":\"/uploadpic.jpg\"}");
            request.AddHeader("Content-Type", "application/octet-stream");
            byte[] data = File.ReadAllBytes(_picJpg);
            request.AddParameter("application/octet-stream", data, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Assert.AreEqual(200, (int) response.StatusCode);
        }

        [Test]
        public void GetFileMetadataTest()
        {
            var client = new RestClient("https://api.dropboxapi.com/2/files/get_metadata");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", _bearer);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", 
                "{\r\n\"path\":\"/Homework/math\",\r\n" +
                "\"include_media_info\": false,\r\n" +
                "\"include_deleted\": false,\r\n" +
                "\"include_has_explicit_shared_members\": false\r\n}",
                ParameterType.RequestBody);
            
            IRestResponse response = client.Execute(request);
            Assert.AreEqual(200, (int) response.StatusCode);
        }

        [Test]
        public void DeleteFileTest()
        {
            var uploadClient = new RestClient("https://content.dropboxapi.com/2/files/upload");
            uploadClient.Timeout = -1;
            var uploadRequest = new RestRequest(Method.POST);
            uploadRequest.AddHeader("Authorization", _bearer);
            uploadRequest.AddHeader("Dropbox-API-Arg", 
                "{\"mode\":\"add\"," +
                "\"autorename\":false," +
                "\"mute\":false," +
                "\"path\":\"/deletepic.jpg\"}");
            
            uploadRequest.AddHeader("Content-Type", "application/octet-stream");
            byte[] data = File.ReadAllBytes(_picJpg);
            uploadRequest.AddParameter("application/octet-stream", data, ParameterType.RequestBody);
            uploadClient.Execute(uploadRequest);

            var client = new RestClient("https://api.dropboxapi.com/2/files/delete_v2");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", _bearer);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\r\n\"path\":\"/deletepic.jpg\"\r\n}",
                ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Assert.AreEqual(200, (int) response.StatusCode);
        }
    }
}