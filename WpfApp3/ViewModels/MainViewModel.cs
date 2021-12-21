using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WpfApp3.Commands;

namespace WpfApp3.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public RelayCommand SelectICommand { get; set; }
        public RelayCommand SendCommand { get; set; }
        public bool SendClickChecker { get; set; } = false;
        public string ImagePath { get; set; } = "";

        public MainViewModel()
        {
            SelectICommand = new RelayCommand((e) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    ImagePath = openFileDialog.FileName;

                }
            });
            SendCommand = new RelayCommand((e) =>
            {
                SendClickChecker = true;
                Send();
            });
            
        }

        public void Send()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var ipAddress = IPAddress.Parse("10.1.18.52");
            var port = 27001;
            var ep = new IPEndPoint(ipAddress, port);

            try
            {
                socket.Connect(ep);
                if (socket.Connected)
                {
                    Console.WriteLine("Connected to the server . . .");
                    while (true)
                    {
                        if (SendClickChecker)
                        {
                            if (ImagePath != "")
                            {

                                var bytes = Encoding.UTF8.GetBytes(ImagePath);
                                socket.Send(bytes);
                                SendClickChecker = false;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Can not connect to the server . . .");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can not connect to the server . . .");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
