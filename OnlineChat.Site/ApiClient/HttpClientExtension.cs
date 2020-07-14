using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Extensions
{
    public static class HttpClientExtension
    {
        public static string Get(this HttpClient httpClient, string uri)
        {
            return httpClient.GetAsync(uri).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        public static string Send(this HttpClient httpClient, HttpRequestMessage request)
        {
            return httpClient.SendAsync(request).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }

        public static T Get<T>(this HttpClient httpClient, string uri)
        {
            return JsonConvert.DeserializeObject<T>(Get(httpClient, uri));
        }

        public async static Task<T> GetAsync<T>(this HttpClient httpClient, string uri)
        {
            return JsonConvert.DeserializeObject<T>(await (await httpClient.GetAsync(uri)).Content.ReadAsStringAsync(), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
        }

        public async static Task<HttpResponseMessage> PostAsync<T>(this HttpClient httpClient, string uri, T obj)
        {
            return await httpClient.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"));
        }

        public async static Task<HttpResponseMessage> DeleteAsync<T>(this HttpClient httpClient, string uri, T obj)
        {
            return await httpClient.DeleteAsync(uri, new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"));
        }

        public async static Task<HttpResponseMessage> DeleteAsync<T>(this HttpClient httpClient, string uri)
        {
            return await httpClient.DeleteAsync(uri);
        }
    }
}