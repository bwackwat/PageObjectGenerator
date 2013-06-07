using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;

namespace ConsoleApplication1
{
    internal class Receiver
    {
        private readonly List<WriterAction> actions = new List<WriterAction>();
        private bool recording;

        public Receiver(int port)
        {
            IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];
            var listener = new TcpListener(ipAddress, port);
            listener.Start();
            while (true)
            {
                Socket s = listener.AcceptSocket();
                var b = new byte[65535];
                int k = s.Receive(b);
                int open = 0;
                bool json = false;
                String jstring = "";
                for (int i = 0; i < k; i++)
                {
                    char chr = Convert.ToChar(b[i]);
                    if (chr == '{')
                    {
                        if (json)
                        {
                            open++;
                        }
                        json = true;
                    }
                    if (json)
                    {
                        jstring += chr;
                    }
                    if (chr == '}')
                    {
                        if (open == 0)
                        {
                            Console.WriteLine(jstring);
                            json = false;
                            ParseJson(jstring);
                        }
                        open--;
                    }
                }
                s.Close();
            }
        }

        public void ParseJson(String jstring)
        {
            if (jstring.Contains("recording"))
            {
                if (jstring.Contains("true"))
                {
                    Console.WriteLine("Starting!");
                    recording = true;
                }
                else
                {
                    Console.WriteLine("Stopping!");
                    List<WriterAction> nactions = new List<WriterAction>(actions.Count);
                    actions.ForEach(nactions.Add);
                    new Thread(GUI.StartGui).Start(nactions);
                    actions.Clear();
                    recording = false;
                }
                return;
            }
            var json = new DataContractJsonSerializer(typeof (UserAction));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jstring));
            UserAction act = json.ReadObject(stream) as UserAction;
            stream.Close();

            if (recording)
            {
                Console.WriteLine(act.ToString());
                actions.Add(act);
            }
        }

        public static void StartRecevicer()
        {
            new Receiver(8055);
        }
    }
}