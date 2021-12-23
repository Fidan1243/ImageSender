using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public class MainViewModel:BaseViewModel
    {
        private ObservableCollection<PhotoUser> allPhotos;

        public ObservableCollection<PhotoUser> AllPhotos
        {
            get { return allPhotos; }
            set { allPhotos = value;OnPropertyChanged(); }
        }

        public List<PhotoUser> Users { get; set; }
        public MainViewModel()
        {
            Task.Run(() => { Reciever(); });
            Users = new List<PhotoUser>();
            AllPhotos = new ObservableCollection<PhotoUser>();
        }


        public void Reciever()
        {
            var ipAddress = IPAddress.Parse("10.1.18.52");
            var port = 27002;
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                var ep = new IPEndPoint(ipAddress, port);
                socket.Bind(ep);

                socket.Listen(10);
                Console.WriteLine($"Listening on {socket.LocalEndPoint}");


                while (true)
                {
                    var client = socket.Accept();
                    Task.Run(() =>
                    {

                        MessageBox.Show($"{client.RemoteEndPoint}  connected  . . . ");
                        var length = 0;
                        var bytes = new byte[1024];
                        
                        do
                        {
                            length = client.Receive(bytes);
                            var msg = Encoding.UTF8.GetString(bytes, 0, length);
                            Users.Add(new PhotoUser
                            {
                                UserName = client.RemoteEndPoint.ToString(),
                                ImagePath = msg
                            });
                            AllPhotos = new ObservableCollection<PhotoUser>(Users);
                            if (msg == "ok")
                            {
                                client.Shutdown(SocketShutdown.Both);
                                client.Dispose();
                                break;
                            }


                        } while (true);
                    });
                }

            }
        }
    }
}
