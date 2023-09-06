using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace AzureOpenAIDallE
{

    //https://{resourceName}.azurewebsites.net/api/Function1?code={api_key}==&desc=a portrait of a ninja cat painted in Van Gogh style

    public static class FunctionHttpTrigger
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            //log.LogInformation("C# HTTP trigger function processed a request.");

            const string resourceName = ""; // Your azure resource name
            const string api_key = ""; // Your function api key
            const string api_ver = "2023-06-01-preview";
            const string serviceURL = $"https://{resourceName}.openai.azure.com/openai/images/generations:submit?api-version={api_ver}";

            string description = req.Query["desc"];
            string url_img_download = String.Empty;

            HttpClient client = new HttpClient();
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, serviceURL);
            var requestObjectMessage = new ObjectMessage(description, 1, "1024x1024");
            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestObjectMessage), Encoding.UTF8, "application/json");
            requestMessage.Headers.Add("api-key", api_key);
            var responseMessage = await client.SendAsync(requestMessage);

            if (responseMessage.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                var definition = new OperationMessage("", "");
                var jsonResponse = JsonConvert.DeserializeAnonymousType(await responseMessage.Content.ReadAsStringAsync(), definition);
                var operation_id = jsonResponse.Id;

                var ImgURL = $"https://{resourceName}.openai.azure.com/openai/operations/images/{operation_id}?api-version={api_ver}";

                var requestMessageImage = new HttpRequestMessage(HttpMethod.Get, ImgURL);
                requestMessageImage.Headers.Add("api-key", api_key);
                Thread.Sleep(6000);//TODO: Manage service call and wait until image status is completed.
                var responseMessageImage = await client.SendAsync(requestMessageImage);
                var definitionImage = new ImageMessage();
                var messImg = JsonConvert.DeserializeObject<ImageMessage>(await responseMessageImage.Content.ReadAsStringAsync());

                url_img_download = messImg.result.data[0].url;

                return new RedirectResult(url_img_download);
            }

            return new OkObjectResult(url_img_download);
        }
    }
}
