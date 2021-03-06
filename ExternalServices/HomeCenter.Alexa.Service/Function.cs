using Amazon.Lambda.Core;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HomeCenter.Alexa.Service
{
    public class Function
    {
        public string HandlerUri { get; set; }

        public async Task<object> FunctionHandler(object request, ILambdaContext context)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsync(HandlerUri, new StringContent(request.ToString(), Encoding.UTF8, "application/json"));

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                LambdaLogger.Log(ex.ToString());
            }
            return null;
        }
    }
}