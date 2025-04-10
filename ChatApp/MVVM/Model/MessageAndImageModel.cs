﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ChatClient.MVVM.Model
{
    public class MessageAndImageModel
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public BitmapImage Image { get; set; }
    }
}
