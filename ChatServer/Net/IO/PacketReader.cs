using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Net.IO
{
    class PacketReader : BinaryReader
    {

        private NetworkStream _ns;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            _ns = ns;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = new byte[length];
            _ns.Read(msgBuffer, 0, length);
            var msg = Encoding.UTF8.GetString(msgBuffer);
            return msg;
        }

        public byte[] ReadImage()
        {
            byte[] imgBytes;
            var length = ReadInt32();
            imgBytes = new byte[length];
            _ns.Read(imgBytes, 0, length);
            return imgBytes;
        }

    }
}
