using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace JH.Applications
{
    public class Launchpad : IGenerator
    {
        GeneratorAbstraction abstraction;
        BinaryReader reader;

        public Launchpad()
        {
        }

        public GeneratorAbstraction BaseClass
        {
            set { abstraction = value; }
        }

        public void GenerateNextBuffer()
        {

            for (int i = 0; i < abstraction.buffer.Length; i++)
            {
                short l0 = reader.ReadInt16();
                short l1 = reader.ReadInt16();
                abstraction.buffer[i] = l0 *10.85;
            }
        }

        public void Init()
        {
            TcpClient client = new TcpClient();
            client.Connect("192.168.1.1", 7913);
            NetworkStream stream = client.GetStream();
            reader = new BinaryReader(stream);
         }

    }
}
