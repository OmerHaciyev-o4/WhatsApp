using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsAppClient.MVVM.Commands;
using WhatsAppClient.MVVM.Models;
using WhatsAppClient.MVVM.Views;

namespace WhatsAppClient.MVVM.ViewModels
{
    public class InfoViewModel : BaseViewModel
    {
        #region Private Varibale
        private string _path;
        private string IP = string.Empty;
        private bool state;
        private TcpClient client;
        private BinaryWriter writer;
        private BinaryReader reader;
        #endregion

        #region Full Property
        private string nameText;
        public string NameText
        {
            get { return nameText; }
            set { nameText = value; OnPropertyChanged(); }
        }

        private string aboutText;
        public string AboutText
        {
            get { return aboutText; }
            set { aboutText = value; OnPropertyChanged(); }
        }

        private string phoneNumber;
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; OnPropertyChanged(); }
        }
        #endregion

        #region References
        public InfoScreen InfoScreen { get; set; }
        #endregion

        #region Commands
        public ICommand FinishCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion


        public InfoViewModel()
        {
            setDataMethod();

            FinishCommand = new RelayCommand((sender) =>
            {
                User user = new User()
                {
                    Name = NameText,
                    About = AboutText,
                    PhoneNumber = PhoneNumber,
                    ProfileImageByte = File.ReadAllBytes(_path),
                    ProfileImagePath = _path.Split('\\').Last()
                };
                var data = JsonConvert.SerializeObject(user);

                try
                {
                    writer.Write("NewUser|" + data);
                    writer.Write(data);
                    string strId = string.Empty;

                    strId = reader.ReadString();
                    var id = Convert.ToInt64(strId);

                    string path = Directory.GetCurrentDirectory() + $"\\{id}";
                    Directory.CreateDirectory(path);

                    var imageinfo = _path.Split('\\').Last();
                    imageinfo = $"{path}\\{imageinfo}";
                    File.WriteAllBytes(imageinfo, user.ProfileImageByte);

                    List<string> infos = new List<string>();
                    infos.Add(id.ToString());
                    infos.Add(imageinfo);
                    infos.Add(NameText);
                    infos.Add(AboutText);
                    infos.Add(PhoneNumber);
                    path += "\\info.log";
                    File.WriteAllLines(path, infos);
                    File.WriteAllText("data.log", path);
                    File.AppendAllText("state.log", "\n\"info\":\"true\"");

                    Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
                catch (Exception)
                {
                    MessageBox.Show("Please check internet.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Process.GetCurrentProcess().Kill();
                    return;
                }
            },
            (pred) =>
            {
                if (!string.IsNullOrEmpty(NameText))
                    return true;
                return false;
            });

            MinimizeCommand = new RelayCommand((sender) =>
            {
                if (InfoScreen.WindowState != WindowState.Minimized)
                    App.Current.Dispatcher.Invoke(() => { InfoScreen.WindowState = WindowState.Minimized; });
            });

            CloseCommand = new RelayCommand((sender) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    App.Current.Shutdown();
                });
            });

            Thread setData = new Thread(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    InfoScreen.dragDrop.Drop += DragDrop_DragEnter;
                    InfoScreen.closeButton.MouseMove += CloseButtonOnMouseMove;
                    InfoScreen.closeButton.MouseLeave += CloseButtonOnMouseLeave;
                    InfoScreen.contextBar.MouseDown += ContextBarOnMouseDown;
                    InfoScreen.Closing += InfoScreen_Closing;
                });

            });
            setData.Start();
        }

        private void InfoScreen_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                writer.Write("close");
            }
            catch (Exception) { }
        }

        private void ContextBarOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                App.Current.Dispatcher.Invoke(() => { InfoScreen.DragMove(); });
        }

        private void CloseButtonOnMouseLeave(object sender, MouseEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
                InfoScreen.closeButton.Background = new SolidColorBrush(Colors.Transparent));
        }

        private void CloseButtonOnMouseMove(object sender, MouseEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
                InfoScreen.closeButton.Background = new SolidColorBrush(Colors.Red));
        }

        private void DragDrop_DragEnter(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            var path = fileList[0];
            fileList = fileList[0].Split('.');

            if (fileList[1].ToLower() == "png" || fileList[1].ToLower() == "jpg" || fileList[1].ToLower() == "jpeg")
            {
                _path = path;

                App.Current.Dispatcher.Invoke(() =>
                {
                    InfoScreen.profileImage.Stretch = Stretch.UniformToFill;
                    InfoScreen.profileImage.Source = new BitmapImage(new Uri(_path));
                });
            }
            else
                MessageBox.Show("Please enter correct image and rectangle(is absolute)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void setDataMethod()
        {
            PhoneNumber = File.ReadAllText("data.log");
            if (PhoneNumber.Contains("\n"))
                PhoneNumber = PhoneNumber.Remove(PhoneNumber.Length - 1);

            var tempPath = Directory.GetCurrentDirectory().Split('\\');
            _path = string.Empty;
            for (int i = 0; i < tempPath.Length - 2; i++)
                _path += tempPath[i] + "\\";
            _path += "MVVM\\Resource\\defaultAvatar.png";

            //Server connect part
            //if (!File.Exists("LocalAddress.txt"))
            //{
            //    File.WriteAllText("LocalAddress.txt", "");
            //    MessageBox.Show("\"" + Directory.GetCurrentDirectory() + "\\LocalAddress.txt\" Please show file set Server IP");
            //    Process pro = Process.GetCurrentProcess();
            //    pro.Kill();
            //}
            //else
            //    IP = File.ReadAllText("LocalAddress.txt");
            try
            {
                client = new TcpClient(IPAddress.Loopback.ToString(), 27001);
                writer = new BinaryWriter(client.GetStream());
                reader = new BinaryReader(client.GetStream());
            }
            catch (Exception)
            {
                MessageBox.Show("There was a problem joining. Please restart the program ;)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}