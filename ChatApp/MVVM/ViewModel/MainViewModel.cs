using ChatApp;
using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ChatClient.MVVM.View;

namespace ChatClient.MVVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public ObservableCollection<BitmapImage> Images { get; set; }
        public ObservableCollection<MessageAndImageModel> MessagesAndImages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand SendImageCommand { get; set; }
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand RegistrationCommand { get; set; }
        

        public string Username{ get; set; }
        public string Password{ get; set; }
        public string PasswordCheck{ get; set; }
        public string Message { get; set; }

        
        private Server _server;
        
        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            MessagesAndImages = new ObservableCollection<MessageAndImageModel>();
            Messages = new ObservableCollection<string>();
            Images = new ObservableCollection<BitmapImage>();
            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.imgReceivedEvent += ImageReceived;
            _server.userDisconnectEvent += RemoveUser;
            _server.DbReceivedEvent += DbReceived;
            _server.DbRegEvent += DbReg;
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));

            SendImageCommand = new RelayCommand(o => _server.SendImageToServer());

            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));
            LoginCommand = new RelayCommand(o =>
            {
                _server.ConnectToServer(Username);
                _server.CheckCredentials(Username, Password);
                
            });
            RegistrationCommand = new RelayCommand(o =>
            {
                if (Password == PasswordCheck)
                {
                    _server.ConnectToServer(Username);
                    _server.RegisterCredentials(Username, Password);
                    
                }
                else
                    MessageBox.Show("Пароли отличаются");
            });
        }

        private void DbReg()
        {
            var dbResult = _server.PacketReader.ReadMessage();
            if (dbResult == "Success")
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Успешная регистрация");
                    var autorizeWindow = new AutorizeWindow();
                    autorizeWindow.Show();
                });
                
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Пользователь с таким именем уже зарегистрирован");
                });
            }
            
        }

        private void DbReceived()
        {
            var dbResult = _server.PacketReader.ReadMessage();
            if (dbResult == "Success")
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Успешный вход");
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    
                });
                
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Ошибка входа");
                });
            }
        }

        private void RemoveUser()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID ==uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));

        }

        private void MessageReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            var messageAndImage = new MessageAndImageModel { Message = msg, Username = _server.PacketReader.ReadMessage() };
            
            Application.Current.Dispatcher.Invoke(() => MessagesAndImages.Add(messageAndImage));
            
        }


        private void ImageReceived()
        {
            var img = _server.PacketReader.ReadImage();
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(img))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
            }
            bitmapImage.Freeze();
            var messageAndImage = new MessageAndImageModel { Image = bitmapImage, Username =_server.PacketReader.ReadMessage(), Message = $"[{DateTime.Now}]:"};
            if (Message != null)
            {
                messageAndImage.Message += Message;
            }
            
            Application.Current.Dispatcher.Invoke(() => MessagesAndImages.Add(messageAndImage));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage(),
            };

            if (Users.Any(x => x.UID != user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }

        
    }
}
