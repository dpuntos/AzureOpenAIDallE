using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;

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

            const string resource_name = ""; // Your azure resource name
            const string api_key = ""; // Your function api key
            //TODO: use settigns to get/set resurcename and api key

            const string api_ver = "2023-06-01-preview";
            string url_img_download = String.Empty;

            string description = req.Query["desc"];

            using (var client = new HttpClient())
            {
                var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.BaseAddress = new Uri($"https://{resource_name}.openai.azure.com/");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                client.DefaultRequestHeaders.Add("api-key", api_key);
                var data = new { prompt = description, n = 1, size = "1024x1024" };
                var requestObjectMessage = new ObjectMessage(description, 1, "1024x1024");

                var content = JsonSerializer.Serialize(data);
                var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                var service_response = await client.PostAsync($"openai/images/generations:submit?api-version={api_ver}", httpContent);

                if (service_response.StatusCode == HttpStatusCode.Accepted)
                {
                    var imgURL = service_response.Headers.GetValues("operation-location").FirstOrDefault();
                    var image_response = await client.GetAsync(imgURL);
                    var image_content_response = await image_response.Content.ReadAsStringAsync();

                    string status = JsonSerializer.Deserialize<OperationMessage>(image_content_response).Status;
                    int max = 1;
                    while (status != "succeeded" && max < 20)
                    {
                        Thread.Sleep(500);
                        max++;
                        image_response = await client.GetAsync(imgURL);
                        image_content_response = await image_response.Content.ReadAsStringAsync();
                        status = JsonSerializer.Deserialize<OperationMessage>(image_content_response).Status;
                    }

                    image_content_response = await image_response.Content.ReadAsStringAsync();
                    url_img_download = JsonSerializer.Deserialize<ImageMessage>(image_content_response).result.data[0].url;
                    return new RedirectResult(url_img_download);
                }
            }

            return new OkObjectResult(url_img_download);
        }
    }
}
