using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();

            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();


            Console.WriteLine($"[{DateTime.Now}]: Клиент {Username} присоединён");

            Task.Run(() => Process());

        }

        

        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Сообщение от {Username}: {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: {msg}", Username);
                            break;
                        case 15:
                            var img = _packetReader.ReadImage();
                            Console.WriteLine($"[{DateTime.Now}] {Username} Прикрепил изображение");
                            Program.BroadcastImage(img, Username);
                            break;
                        case 25:
                            var uName = _packetReader.ReadMessage();
                            var uPassword = _packetReader.ReadMessage();
                            Program.BroadcastDBresult(uName, uPassword, ClientSocket);
                            break;
                        case 30:
                            var regUName = _packetReader.ReadMessage();
                            var regUPassword = _packetReader.ReadMessage();
                            Program.BroadcastDBregistration(regUName,regUPassword, ClientSocket);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{UID.ToString()}]: Потеряно соединение");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
