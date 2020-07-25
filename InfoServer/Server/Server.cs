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

        public static async Task HandleConnections()
        {
            while (running)
            {
                Console.WriteLine("Waiting for connection...");
                // Will wait here until we hear from a connection
                context = await listener.GetContextAsync();

                // Peel out the requests and response objects
                req = context.Request;
                resp = context.Response;

                // Print out some info about the request
                Console.WriteLine();
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    Thread.Sleep(5000);
                    running = false;
                }
                
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/add"))
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

                            builder.Insert(builder.Length-1, json.Insert(0,','));
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

                if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/get"))
                {
                    Console.WriteLine("read");
                                                         
                    string listString = File.ReadAllText($"{serverDirectory}\\data.json");

                    byte[] response = Encoding.ASCII.GetBytes(listString.ToString());

                    Respond(response, "application/json");
                }

                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/update"))
                {
                    Console.WriteLine("update");

                    using (System.IO.Stream body = req.InputStream) // here we have data
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(body, req.ContentEncoding))
                        {
                            string newRecord = reader.ReadToEnd();

                            int startIndex = newRecord.IndexOf("\"");
                            int endIndex = newRecord.IndexOf(",");

                            string id = newRecord.Substring(startIndex, endIndex - startIndex);
                            Console.WriteLine(id);

                            string json = File.ReadAllText($"{serverDirectory}\\data.json");
                            startIndex = json.IndexOf(id) - 1;
                            endIndex = json.IndexOf("}", startIndex) + 1;
                            json = json.Remove(startIndex, endIndex - startIndex);

                            /*if (json[1] == ',')
                                json = json.Remove(1, 1);*/

                            json = json.Insert(startIndex, newRecord);

                            File.WriteAllText($"{serverDirectory}\\data.json", json);

                            string responseString = "Record updated";
                            byte[] response = Encoding.ASCII.GetBytes(responseString);

                            Respond(response, "text/plain");
                        }
                    }
                }

                if ((req.HttpMethod == "DELETE") && (req.Url.AbsolutePath == "/delete"))
                {
                    Console.WriteLine("delete");

                    using (System.IO.Stream body = req.InputStream) // here we have data
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(body, req.ContentEncoding))
                        {
                            string id = reader.ReadToEnd();
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
                            
                            /*if (json[1] == ',')
                                json = json.Remove(1, 1);*/

                            File.WriteAllText($"{serverDirectory}\\data.json", json);

                            string responseString = "Record deleted";
                            byte[] response = Encoding.ASCII.GetBytes(responseString);

                            Respond(response, "text/plain");
                        }
                    }
                }
            }
        }

        public async static void Respond(byte[] data, string contentType)
        {
            // Write the response info
            string disableSubmit = !running ? "disabled" : "";
            //byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
            resp.ContentType = contentType;
            resp.ContentEncoding = Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;

            // Write out to the response stream (asynchronously), then close it
            await resp.OutputStream.WriteAsync(data, 0, data.Length);
            resp.Close();
            Console.WriteLine("Connection closed");
        }

        public void Start()
        {
            LoadProperties();
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
            SaveProperties();
        }

        public void SaveProperties()
        {
            ServerProperties properties = new ServerProperties();
            properties.nextId = curId;
            properties.ServerFolder = serverDirectory;
            string json = JsonConvert.SerializeObject(properties);
            File.WriteAllText("properties.json", json);
        }

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
