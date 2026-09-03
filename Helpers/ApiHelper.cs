using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public static class ApiHelper
{
    private static readonly string baseUrl =
        ConfigurationManager.AppSettings["ApiBaseUrl"];

    public static async Task<T> GetAsync<T>(string endpoint)
    {
        using(HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);

            HttpResponseMessage response =
                await client.GetAsync(endpoint);

            if(response.IsSuccessStatusCode)
            {
                string json =
                    await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(json);
            }

            return default(T);
        }
    }

    public static async Task<bool> PostAsync<T>(string endpoint, T model)
    {
        using(HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);

            string json =
                JsonConvert.SerializeObject(model);

            StringContent content =
                new StringContent(json,
                                  Encoding.UTF8,
                                  "application/json");

            HttpResponseMessage response =
                await client.PostAsync(endpoint, content);

            return response.IsSuccessStatusCode;
        }
    }
}