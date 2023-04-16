using ChatClient.Net.IO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatClient.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;

        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action imgReceivedEvent;
        public event Action userDisconnectEvent;
        public event Action DbReceivedEvent;
        public event Action DbRegEvent;

        public Server()
        {
            _client = new TcpClient();
            
        }

        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("26.128.180.248", 7891);
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 10:
                            userDisconnectEvent?.Invoke();
                            break;
                        case 15:
                            imgReceivedEvent?.Invoke();
                            break;
                        case 25:
                            DbReceivedEvent?.Invoke();
                            break;
                        case 30:
                            DbRegEvent?.Invoke();
                            break;
                        
                        default:
                            Console.WriteLine("......");
                            break;
                    }
                }
            });
        }


        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBytes());

        }
        public void SendImageToServer()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.png;*.jpeg;*.jpg;*.gif)|*.png;*.jpeg;*.jpg;*.gif"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var imageBytes = File.ReadAllBytes(openFileDialog.FileName);

                var imagePacket = new PacketBuilder();
                imagePacket.WriteOpCode(15);
                imagePacket.WriteImage(imageBytes);
                _client.Client.Send(imagePacket.GetPacketBytes());

            }
        }

        public void CheckCredentials(string username, string password)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(25);
            messagePacket.WriteMessage(username);
            messagePacket.WriteMessage(password);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }
        public void RegisterCredentials(string username, string password)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(30);
            messagePacket.WriteMessage(username);
            messagePacket.WriteMessage(password);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }

    }
}
