using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using InfoServerDataModels;

namespace ClientGUI
{
    class Client
    {
        HttpClient client = new HttpClient();

        public Client()
        {
            string site = "http://localhost:80";
            client.BaseAddress = new Uri(site);
        }

        public async Task<List<Record>> GetRecords()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/get");
            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            List<Record> records = JsonConvert.DeserializeObject<List<Record>>(responseString);

            return records;
        }

        public async Task<int> SendRecord(Record record)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"/add");

            string json = JsonConvert.SerializeObject(record);

            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"/shutdown");
            request.Content = byteContent;

            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            return int.Parse(responseString);
        }
        public async void UpdateRecord(Record record)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"/update");

            string json = JsonConvert.SerializeObject(record);

            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"/shutdown");
            request.Content = byteContent;

            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
        }

        public async void DeleteRecord(int id)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"/delete");
            
            var buffer = System.Text.Encoding.UTF8.GetBytes(id.ToString());
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"/shutdown");
            request.Content = byteContent;

            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
        }
    }
}
