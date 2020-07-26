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
    /// <summary>
    /// Contains main client-server interactions
    /// </summary>
    class Client
    {
        HttpClient client = new HttpClient();

        public Client()
        {
            string site = "http://localhost:80";
            client.BaseAddress = new Uri(site);
        }

        /// <summary>
        /// Get all records from server
        /// </summary>
        public async Task<List<Record>> GetRecords()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/get");
            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            List<Record> records = JsonConvert.DeserializeObject<List<Record>>(responseString);

            foreach (Record record in records)
            {
                record.Image = Compressor.Decompress(record.Image);
            }

            return records;
        }

        /// <summary>
        /// Uploads new record to the server
        /// </summary>
        public async Task<int> UploadRecord(Record record)
        {
            Record uploadingRecord = new Record(0 , record.Name, Compressor.Compress(record.Image));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"/add");

            string json = JsonConvert.SerializeObject(uploadingRecord);

            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            request.Content = byteContent;

            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            return int.Parse(responseString);
        }

        /// <summary>
        /// Updates record on the server
        /// </summary>
        public async void UpdateRecord(Record record)
        {
            Record uploadingRecord = new Record(0, record.Name, Compressor.Compress(record.Image));

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"/update/{uploadingRecord.Id}");

            string json = JsonConvert.SerializeObject(uploadingRecord);

            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            request.Content = byteContent;

            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Deletes record from the server
        /// </summary>
        public async void DeleteRecord(int id)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"/delete/{id}");
            
            var response = await client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();
        }
    }
}
