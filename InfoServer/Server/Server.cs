using InfoServerDataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    /// <summary>
    /// Contains main server logic
    /// </summary>
    class Server
    {
        public static HttpListener listener;
        public static string url = "http://localhost:80/";
        public static int requestCount = 0;

        public static int curId = 0;

        public static string serverDirectory = "F:\\Server";

        static bool running = true;
        static HttpListenerContext context;
        static HttpListenerRequest req;
        static HttpListenerResponse resp;

        /// <summary>
        /// Handles all upcoming connections
        /// </summary>
        public static async Task HandleConnections()
        {
            while (running)
            {
                Console.WriteLine("Waiting for connection...");
                context = await listener.GetContextAsync();

                req = context.Request;
                resp = context.Response;

                HandleRequest();
            }
        }

        /// <summary>
        /// Handles current request
        /// </summary>
        public static void HandleRequest()
        {
            Console.WriteLine();
            Console.WriteLine("Request #: {0}", ++requestCount);
            Console.WriteLine(req.Url.ToString());            
            Console.WriteLine(req.HttpMethod);

            string request = req.Url.AbsolutePath.Replace(url, "");
            string action = request.Split('/', StringSplitOptions.RemoveEmptyEntries)[0];

            switch (action)
            {
                case "shutdown":
                    ShutDown();
                    break;
                case "add":
                    AddRecord();
                    break;
                case "get":
                    SendRecords();
                    break;
                case "update":
                    UpdateRecord();
                    break;
                case "delete":
                    DeleteRecord();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Shuts server down
        /// </summary>
        public static void ShutDown()
        {
            Console.WriteLine("Shutdown requested");
            running = false;
        }

        /// <summary>
        /// Adds new record
        /// </summary>
        public static void AddRecord()
        {
            Console.WriteLine("create");
            using (System.IO.Stream body = req.InputStream) // here we have data
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(body, req.ContentEncoding))
                {
                    StringBuilder json = new StringBuilder(reader.ReadToEnd());
                    json.Remove(6, 1);
                    json.Insert(6, curId);
                    StringBuilder builder = new StringBuilder();
                    builder.Append(File.ReadAllText($"{serverDirectory}\\data.json"));

                    builder.Insert(builder.Length - 1, json.Insert(0, ','));
                    if (builder[1] == ',')
                        builder.Remove(1, 1);
                    //File.WriteAllText($"{serverDirectory}\\records\\{curId++}.json", json);
                    File.WriteAllText($"{serverDirectory}\\data.json", builder.ToString());

                    string responseString = curId.ToString();
                    byte[] response = Encoding.ASCII.GetBytes(responseString);

                    Respond(response, "text/plain");
                    curId++;
                }
            }
        }

        /// <summary>
        /// Send records to client
        /// </summary>
        public static void SendRecords()
        {
            Console.WriteLine("read");

            string listString = File.ReadAllText($"{serverDirectory}\\data.json");

            byte[] response = Encoding.ASCII.GetBytes(listString.ToString());

            Respond(response, "application/json");
        }

        /// <summary>
        /// Updates existing record
        /// </summary>
        public static void UpdateRecord()
        {
            Console.WriteLine("update");

            using (System.IO.Stream body = req.InputStream) // here we have data
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(body, req.ContentEncoding))
                {
                    string newRecord = reader.ReadToEnd();

                    /*int startIndex = newRecord.IndexOf("\"");
                    int endIndex = newRecord.IndexOf(",");*/

                    //string id = newRecord.Substring(startIndex, endIndex - startIndex);
                    string id = $"\"Id\":{req.Url.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries)[1]},";
                    Console.WriteLine(id);

                    string json = File.ReadAllText($"{serverDirectory}\\data.json");
                    int startIndex = json.IndexOf(id) - 1;
                    int endIndex = json.IndexOf("}", startIndex) + 1;
                    json = json.Remove(startIndex, endIndex - startIndex);

                    json = json.Insert(startIndex, newRecord);

                    File.WriteAllText($"{serverDirectory}\\data.json", json);

                    string responseString = "Record updated";
                    byte[] response = Encoding.ASCII.GetBytes(responseString);

                    Respond(response, "text/plain");
                }
            }
        }

        /// <summary>
        /// Deletes existing record
        /// </summary>
        public static void DeleteRecord()
        {
            Console.WriteLine("delete");

            string id = req.Url.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries)[1];
            string json = File.ReadAllText($"{serverDirectory}\\data.json");
            int startIndex = json.IndexOf($"\"Id\":{id}") - 1;
            int endIndex = json.IndexOf("}", startIndex) + 1;
            if (json[endIndex] == ']')
            {
                if (json[startIndex - 1] != '[')
                    startIndex--;
                endIndex--;
            }

            json = json.Remove(startIndex, 1 + endIndex - startIndex);

            File.WriteAllText($"{serverDirectory}\\data.json", json);

            string responseString = "Record deleted";
            byte[] response = Encoding.ASCII.GetBytes(responseString);

            Respond(response, "text/plain");
        }

        /// <summary>
        /// Sends respond to client
        /// </summary>
        public async static void Respond(byte[] data, string contentType)
        {
            string disableSubmit = !running ? "disabled" : "";
            resp.ContentType = contentType;
            resp.ContentEncoding = Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;

            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
            Console.WriteLine("Connection closed");
        }

        /// <summary>
        /// Starts server
        /// </summary>
        public void Start()
        {
            LoadProperties();
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            Task listenTask = HandleConnections();
            listenTask.GetAwaiter().GetResult();

            listener.Close();
            SaveProperties();
        }

        /// <summary>
        /// Saves server properties to file
        /// </summary>
        public void SaveProperties()
        {
            ServerProperties properties = new ServerProperties();
            properties.nextId = curId;
            properties.ServerFolder = serverDirectory;
            string json = JsonConvert.SerializeObject(properties);
            File.WriteAllText("properties.json", json);
        }

        /// <summary>
        /// Loads server properties from file
        /// </summary>
        public void LoadProperties()
        {
            string json = File.ReadAllText("properties.json");
            try
            {
                ServerProperties properties = JsonConvert.DeserializeObject<ServerProperties>(json);
                curId = properties.nextId;
                serverDirectory = properties.ServerFolder;
                Console.WriteLine(serverDirectory);
                if (!File.Exists($"{serverDirectory}\\data.json"))
                {
                    File.WriteAllText($"{serverDirectory}\\data.json", "[]");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine("An error occurred while loading properties.");
            }
        }

        ~Server()
        {
            Console.WriteLine("Server terminated");
            Thread.Sleep(2000);
            SaveProperties();
        }
    }
}
