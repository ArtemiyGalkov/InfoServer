﻿using Newtonsoft.Json;
using System;
using System.IO;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
        }
    }
}
