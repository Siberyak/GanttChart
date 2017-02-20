using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;

namespace DAL
{
    public class EntityInfo
    {
        public static readonly string BaseUrl = "http://localhost:8564/api/PlanGrafik";

        [JsonProperty("@odata.type")]
        public string TypeName { get; set; }

        [JsonProperty("Id")]
        public int Id { get; set; }

        protected static async Task<T> Read<T>(string queryUrl)
        {
            string content = await Content(queryUrl);
            var response = JsonConvert.DeserializeObject<T>(content);
            return response;
        }

        protected static async Task<object> Read(string queryUrl, Type type)
        {
            string content = await Content(queryUrl);
            var response = JsonConvert.DeserializeObject(content, type);
            return response;
        }


        protected static async Task<string> Content(string queryUrl)
        {
            string url = $"{BaseUrl}/{queryUrl}";
            var http = new HttpClient();
            Uri uri = new Uri(url);
            var resp = await http.GetAsync(uri);
            var content = await resp.Content.ReadAsStringAsync();
            return content;
        }



        
    }
}