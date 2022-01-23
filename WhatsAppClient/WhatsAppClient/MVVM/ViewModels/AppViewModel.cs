using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WhatsAppClient.MVVM.Commands;
using WhatsAppClient.MVVM.Helper;
using WhatsAppClient.MVVM.Models;
using WhatsAppClient.MVVM.Views;

namespace WhatsAppClient.MVVM.ViewModels
{
    public class AppViewModel : BaseViewModel
    {
        #region Static Proprerty
        public static UserLow user { get; set; }
        public static long CustomID { get; set; }
        #endregion

        #region Private Varivable
        private DispatcherTimer _time;
        private Thread requestThread;

        public TcpClient client;
        public BinaryWriter writer;
        public BinaryReader reader;
        public string IP;
        internal static bool IsStarted;
        #endregion

        #region Commands
        public ICommand MinimizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion

        #region References
        public AppScreen AppScreen { get; set; }
        #endregion

        public AppViewModel()
        {
            setDataMethod();

            MinimizeCommand = new RelayCommand((sender) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    if (AppScreen.WindowState != WindowState.Minimized)
                        AppScreen.WindowState = WindowState.Minimized;
                });
            });

            CloseCommand = new RelayCommand((sender) =>
            {
                var process = Process.GetCurrentProcess();
                process.Kill();
            });

            Thread setData = new Thread(() =>
            {
                CultureHelper.Start();
                App.Current.Dispatcher.Invoke(() =>
                {
                    AppScreen.contextBar.MouseDown += ContextBarOnMouseDown;
                    AppScreen.closeButton.MouseLeave += CloseButton_MouseLeave;
                    AppScreen.closeButton.MouseMove += CloseButton_MouseMove;
                    AppScreen.Closing += AppScreen_Closing;
                    ChangeViewControl("persons");
                });
            });
            setData.Start();

            requestThread = new Thread(() =>
            {
                while (true)
                {
                    ChatDataSend data = null;
                    User user = null;

                    string jsonData = string.Empty;

                    try
                    {
                        jsonData = reader.ReadString();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Please check internet.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Process currentProcess = Process.GetCurrentProcess();
                        currentProcess.Kill();
                        return;
                    }

                    try
                    {

                        if (jsonData.Split('|')[0].Contains("NewFriend"))
                            user = JsonConvert.DeserializeObject<User>(jsonData.Split('|')[1]);
                        else
                            data = JsonConvert.DeserializeObject<ChatDataSend>(jsonData.Split('|')[1]);
                    }
                    catch (Exception) { }

                    IsStarted = false;
                    if (data != null)
                    {
                        var path = Directory.GetCurrentDirectory() + $"\\Data\\{data.CustomID}\\";

                        var time = DateTime.Now;
                        if (data.SMSType == "sms")
                        {
                            path += "chat.log";
                            File.AppendAllText(path, $"you: {Encoding.ASCII.GetString(data.DataBytes)}| |{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                        }
                        else if (data.SMSType == "voice")
                        {
                            var tempPath = path + "chat.log";
                            while (true)
                            {
                                try
                                {
                                    File.WriteAllBytes(path + data.FileName, data.DataBytes);
                                    break;
                                }
                                catch (Exception) { }
                            }

                            try
                            {
                                var infoDatas = File.ReadAllLines(path + "info.log").ToList();
                                var number = Convert.ToInt32(data.FileName.Split('.')[0].Remove(data.FileName.Split('.')[0].IndexOf("Audio"), 5));
                                infoDatas[infoDatas.Count - 1] = $"audio|{number}";
                                File.WriteAllLines(path + "info.log", infoDatas.ToArray());
                            }
                            catch (Exception) { }

                            while (true)
                            {
                                try
                                {
                                    File.AppendAllText(tempPath, $"you: voice|{path + data.FileName}|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                                    break;
                                }
                                catch (Exception) { }
                            }
                        }
                        else if (data.SMSType == "image")
                        {
                            var tempPath = path + "chat.log";
                            while (true)
                            {
                                try
                                {
                                    File.WriteAllBytes(path + data.FileName, data.DataBytes);
                                    break;
                                }
                                catch (Exception) { }
                            }

                            try
                            {
                                var infoDatas = File.ReadAllLines(path + "info.log").ToList();
                                var number = Convert.ToInt32(data.FileName.Split('.')[0].Remove(data.FileName.Split('.')[0].IndexOf("Image"), 5));
                                infoDatas[infoDatas.Count - 2] = $"image|{number}";
                                File.WriteAllLines(path + "info.log", infoDatas.ToArray());
                            }
                            catch (Exception) { }

                            while (true)
                            {
                                try
                                {
                                    File.AppendAllText(tempPath, $"you: image|{path + data.FileName}|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                                    break;
                                }
                                catch (Exception) { }
                            }

                        }
                        else if (data.SMSType == "file")
                        {
                            var tempPath = path + "chat.log";
                            File.WriteAllBytes(path + data.FileName, data.DataBytes);

                            File.AppendAllText(tempPath, $"you: file|{path + data.FileName}|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                        }
                    }
                    else if (user != null)
                    {
                        var path = Directory.GetCurrentDirectory() + "\\Data";

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                            File.Create(path + "\\usersInfo.log");
                        }
                        else
                            if (!File.Exists(path + "\\usersInfo.log"))
                            File.Create(path + "\\usersInfo.log");

                        var usersInfoFilePath = path;
                        var infos = File.ReadAllLines(usersInfoFilePath + "\\usersInfo.log").ToList();
                        infos.Add(usersInfoFilePath + $"\\{user.ID}\\info.log");
                        path += $"\\{user.ID}";
                        Directory.CreateDirectory(path);

                        var tempInfos = new List<string>();
                        tempInfos.Add(user.ID.ToString());
                        tempInfos.Add(path + $"\\{user.ProfileImagePath.Split('\\').Last()}");
                        tempInfos.Add(user.Name);
                        tempInfos.Add(user.About);
                        tempInfos.Add(user.PhoneNumber);
                        tempInfos.Add("image|0");
                        tempInfos.Add("sound|0");
                        File.WriteAllLines(path + "\\info.log", tempInfos.ToArray());
                        File.WriteAllText(path + "\\chat.log", "");
                        File.WriteAllBytes(path + "\\" + user.ProfileImagePath.Split('\\').Last(), user.ProfileImageByte);

                        if (!File.Exists("contact.log"))
                            File.WriteAllText("contact.log", user.PhoneNumber + '\n');
                        else
                            File.AppendAllText("contact.log", user.PhoneNumber + '\n');

                        Thread.Sleep(500);
                        File.WriteAllLines(usersInfoFilePath + "\\usersInfo.log", infos.ToArray());
                    }
                    IsStarted = true;
                }
            });
            requestThread.Start();
        }

        private void AppScreen_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                writer.Write("close");
            }
            catch (Exception) { }
        }

        private void setDataMethod()
        {
            var customInfoPath = File.ReadAllText("data.log");
            var customInfo = File.ReadAllLines(customInfoPath);
            CustomID = Convert.ToInt64(customInfo[0]);

            _time = new DispatcherTimer();
            _time.Interval = new TimeSpan(0, 0, 1);
            _time.Tick += _time_Tick;

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

                writer.Write(CustomID + "|App");
            }
            catch(Exception)
            {
                MessageBox.Show("There was a problem joining. Please restart the program ;)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }

            IsStarted = true;
        }

        private void _time_Tick(object sender, EventArgs e)
        {
            if (ChatViewModel.IsSetPhoto == -1 || ChatViewModel.IsSetPhoto == 1)
            {
                ChatControl chat = new ChatControl(this);
                AppScreen.Dispatcher.Invoke(() => { AppScreen.MainGrid.Children.Clear(); AppScreen.MainGrid.Children.Add(chat); });
                _time.Stop();
            }
        }

        public void ChangeViewControl(string name)
        {
            App.Current.Dispatcher.Invoke(() => { AppScreen.MainGrid.Children.Clear(); });

            string[] infos = null;
            try { infos = name.Split('|'); }
            catch (Exception) { }

            if (infos.Length == 2)
            {
                if (infos[0] == "camera" && infos[1] == "persons")
                {
                    TakeImageControl control = new TakeImageControl(this);
                    App.Current.Dispatcher.Invoke(() => { AppScreen.MainGrid.Children.Add(control); });
                    _time.Start();
                }
            }
            else
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    switch (name)
                    {
                        case "persons":
                            PersonsControl persons = new PersonsControl(this);
                            AppScreen.MainGrid.Children.Add(persons);
                            break;
                        case "contact":
                            ContactControl contact = new ContactControl(this);
                            AppScreen.MainGrid.Children.Add(contact);
                            break;
                        case "chat":
                            ChatControl chat = new ChatControl(this);
                            AppScreen.MainGrid.Children.Add(chat);
                            break;
                    }
                });
            }
        }

        private void CloseButton_MouseMove(object sender, MouseEventArgs e)
        {
            AppScreen.closeButton.Background = new SolidColorBrush(Colors.Red);
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            AppScreen.closeButton.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void ContextBarOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => { AppScreen.DragMove(); });
        }
    }
}