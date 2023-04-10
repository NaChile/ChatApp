using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net.IO
{
    class PacketBuilder
    {
        MemoryStream _ms;
        public PacketBuilder()
        {
            _ms = new MemoryStream();
        }

        public void WriteOpCode(byte opcode)
        {
            _ms.WriteByte(opcode);
        }

        public void WriteMessage(string msg)
        {
            byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
            int msgLength = msgBytes.Length;

            _ms.Write(BitConverter.GetBytes(msgLength));
            _ms.Write(msgBytes);
        }
        public void WriteImage(byte[] imageData)
        {
            int imageLength = imageData.Length;
            _ms.Write(BitConverter.GetBytes(imageLength));
            _ms.Write(imageData);
        }

        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
    }

}
