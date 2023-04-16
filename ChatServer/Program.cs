using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Data.SqlClient;

namespace ChatServer
{
    class Program
    {
        static List<Client> _users;
        static TcpListener _listener;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("26.128.180.248"), 7891);
            _listener.Start();

            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                BroadcastConnection();
            }
        }
        static void BroadcastConnection()
        {
            foreach (var user in _users)
            {
                foreach (var usr in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastMessage(string message, string usrName)
        {
            foreach (var user in _users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(5);
                msgPacket.WriteMessage(message);
                msgPacket.WriteMessage(usrName);
                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
            
        }
        public static void BroadcastImage(byte[] imageData, string usrName)
        {
            foreach (var user in _users)
            {
                var imgPacket = new PacketBuilder();
                imgPacket.WriteOpCode(15);
                imgPacket.WriteImage(imageData);
                imgPacket.WriteMessage(usrName);
                user.ClientSocket.Client.Send(imgPacket.GetPacketBytes());
            }
        }


        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = _users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            _users.Remove(disconnectedUser);

            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }

            BroadcastMessage($"[{disconnectedUser.Username}] Пользователь отключился", disconnectedUser.Username);
        }

        public static void BroadcastDBresult(string uName, string uPassword, TcpClient clientSocket)
        {
            string connectionString = "Data Source=DESKTOP-7OL1NP5;Initial Catalog=ChatAppDB;Integrated Security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT * FROM Users WHERE username='{uName}' AND password='{uPassword}'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            var successPacket = new PacketBuilder();
                            successPacket.WriteOpCode(25);
                            successPacket.WriteMessage("Success");
                            clientSocket.Client.Send(successPacket.GetPacketBytes());
                        }
                        else
                        {
                            var failurePacket = new PacketBuilder();
                            failurePacket.WriteOpCode(25);
                            failurePacket.WriteMessage("Fail");
                            clientSocket.Client.Send(failurePacket.GetPacketBytes());
                        }
                    }
                }
            }
        }

        public static void BroadcastDBregistration(string uName, string uPassword, TcpClient clientSocket)
        {
            string connectionString = "Data Source=DESKTOP-7OL1NP5;Initial Catalog=ChatAppDB;Integrated Security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT * FROM Users WHERE username='{uName}'";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            var failurePacket = new PacketBuilder();
                            failurePacket.WriteOpCode(30);
                            failurePacket.WriteMessage("Fail");
                            clientSocket.Client.Send(failurePacket.GetPacketBytes());
                        }
                        else
                        {
                            reader.Close();
                            
                            query = $"INSERT INTO Users (username, password, role_id) VALUES ('{uName}', '{uPassword}', '{1}')";
                            SqlCommand command2 = new SqlCommand(query, connection);
                            command2.ExecuteNonQuery();

                            var successPacket = new PacketBuilder();
                            successPacket.WriteOpCode(30);
                            successPacket.WriteMessage("Success");
                            clientSocket.Client.Send(successPacket.GetPacketBytes());
                        }
                    }
                }
            }
        }
    }
}
