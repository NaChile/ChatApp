using ChatApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChatClient.MVVM.ViewModel;

namespace ChatClient.MVVM.View
{
    /// <summary>
    /// Логика взаимодействия для AutorizeWindow.xaml
    /// </summary>
    public partial class AutorizeWindow : Window
    {
        
        public AutorizeWindow()
        {
            InitializeComponent();
            
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var regWindow = new RegistrationWindow();
            regWindow.Show();
            this.Close();
        }

        private void LoginBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //if (Application.Current.MainWindow.IsActive == true)
            //{
            //    this.Close();
            //}
        }
    }
}
