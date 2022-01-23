using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WhatsAppClient.MVVM.Commands;
using WhatsAppClient.MVVM.Helper;
using WhatsAppClient.MVVM.Models;
using WhatsAppClient.MVVM.Views;

namespace WhatsAppClient.MVVM.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        #region Static property
        public static Bitmap Source { get; set; }
        public static int IsSetPhoto { get; set; } = 0;
        #endregion

        #region Private Varibale
        private bool _isRecord;
        private bool _writeMessage;
        private PackIcon packIcon;
        private Thread changeIconThread;
        private DispatcherTimer dispatcher;

        //Connect server variable
        private TcpClient client;
        private BinaryWriter writer;
        private BinaryReader reader;

        //sound listing variables
        private Grid _grid;
        private System.Windows.Controls.Image _image;
        private Button _button;
        private Slider _slider;
        private Label _label;
        private MediaElement _media;
        private DispatcherTimer _mediaStateTimer;
        private string _tempPath;
        private int _voiceIndex;

        #endregion

        #region Full Property
        private string messageBox;
        public string MessageBoxText
        {
            get { return messageBox; }
            set { messageBox = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ListBoxItem> items;
        public ObservableCollection<ListBoxItem> Items
        {
            get { return items; }
            set { items = value; OnPropertyChanged(); }
        }

        #endregion

        #region References
        public ChatControl ChatControl { get; set; }
        public AppViewModel ViewModel { get; set; }
        #endregion

        #region Commands
        public ICommand ExitCommand { get; set; }
        public ICommand PhotoCommand { get; set; }
        #endregion

        #region AutoProperty
        public UserLow User { get; set; }
        #endregion

        public ChatViewModel()
        {
            setDataMethod();

            Thread setData = new Thread(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    ChatControl.RecordButton.PreviewMouseLeftButtonDown += RecordButton_MouseDown;
                    ChatControl.RecordButton.PreviewMouseLeftButtonUp += RecordButton_MouseUp;
                    ChatControl.RecordButton.Click += RecordButton_Click;
                    ChatControl.MessageBox.PreviewKeyUp += MessageBox_PreviewKeyUp;
                    ChatControl.MessageBox.TextChanged += MessageBox_TextChanged;
                    ChatControl.ChatBox.Drop += ChatBox_Drop;
                });
            });
            setData.Start();

            Thread th = new Thread(() =>
            {
                while (client == null)
                {
                    try
                    {
                        client = new TcpClient(IPAddress.Loopback.ToString(), 27001);
                        writer = new BinaryWriter(client.GetStream());
                        reader = new BinaryReader(client.GetStream());
                        writer.Write(AppViewModel.CustomID.ToString());
                    }
                    catch (Exception e) 
                    {
                        MessageBox.Show(e.Message);
                        client = null;
                    }
                }
            });
            th.Start();

            ExitCommand = new RelayCommand((sender) =>
            {
                ViewModel.ChangeViewControl("persons");
            });

            PhotoCommand = new RelayCommand((sender) =>
            {
                ViewModel.ChangeViewControl("camera|persons");
            },
            (pred) =>
            {
                if (ViewModel.client.Connected)
                    return true;
                return false;
            });

            changeIconThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(100);
                    if (MessageBoxText.Length > 0)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            packIcon.Kind = PackIconKind.Send;
                            ChatControl.RecordButton.Content = packIcon;
                        });
                        _writeMessage = true;
                    }
                    else
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            packIcon.Kind = PackIconKind.Microphone;
                            ChatControl.RecordButton.Content = packIcon;
                        });
                        _writeMessage = false;
                    }

                }
            });
            changeIconThread.Start();

            Thread updateDataState = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        if (AppViewModel.IsStarted)
                            dispatcher.Start();
                        else
                            dispatcher.Stop();
                    }
                    catch (Exception) { }
                }
            });
            updateDataState.Start();
        }

        private void CreateItemAndAddItem(string[] info, string type)
        {
            ListBoxItem item = new ListBoxItem();
            Grid grid = new Grid();
            Label time = new Label();

            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            time.Margin = new Thickness(0, -5, 5, 5);
            time.Padding = new Thickness(0);
            time.HorizontalAlignment = HorizontalAlignment.Right;
            time.FontSize = 10;
            time.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 161, 163));

            RectangleGeometry rec = new RectangleGeometry();
            rec.RadiusX = 10;
            rec.RadiusY = 10;

            var tempinfo = info[0].Split(' ');

            if (type == "sms")
            {
                RichTextBox rich = new RichTextBox();
                rich.Padding = new Thickness(5, 5, 5, 0);
                rich.Foreground = new SolidColorBrush(Colors.White);
                rich.BorderBrush = new SolidColorBrush(Colors.Transparent);
                rich.BorderThickness = new Thickness(0);
                rich.IsReadOnly = true;
                rich.Cursor = Cursors.Arrow;

                var index = info[0].IndexOf(' ');
                var sms = info[0].Remove(0, index + 1);
                rich.Height = 25;
                if (sms.Length <= 10)
                {
                    rich.Width = 100;
                    rec.Rect = new Rect(0, 0, 100, 35);
                }
                else if (sms.Length <= 45)
                {
                    rich.Width = sms.Length * 8;
                    rec.Rect = new Rect(0, 0, sms.Length * 8, 35);
                }
                else if (sms.Length > 45)
                {
                    var count = sms.Length / 45.00;
                    index = (int)count;
                    if (count > index)
                        index++;

                    rich.Height = index * 20;
                    rich.Width = 300;
                    rec.Rect = new Rect(0, 0, 300, (index * 20) + 10);
                }
                rich.AppendText(sms);

                Grid.SetRow(rich, 0);
                grid.Children.Add(rich);
            }
            else if (type == "voice")
            {
                _media = new MediaElement();
                _media.Source = new Uri(info[1]);
                _media.LoadedBehavior = MediaState.Manual;
                _media.UnloadedBehavior = MediaState.Manual;
                _media.Volume = 0.5;
                _media.Play();
                string time1 = string.Empty;
                while (true)
                {
                    if (_media.NaturalDuration.HasTimeSpan)
                    {
                        time1 = $"{_media.NaturalDuration.TimeSpan.Minutes}:{_media.NaturalDuration.TimeSpan.Seconds}";
                        break;
                    }
                }
                _media.Visibility = Visibility.Hidden;
                _media.Stop();
                _media.Volume = 1;

                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(220, GridUnitType.Pixel) });

                System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                {
                    Width = 50,
                    Height = 50,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 15, 5),
                    Clip = new EllipseGeometry(new System.Windows.Point(25, 25), 25, 25),
                    Source = new BitmapImage(new Uri(User.ProfileImagePath))
                };

                Button button = new Button()
                {
                    Width = 30,
                    Height = 30,
                    Padding = new Thickness(0),
                    Margin = new Thickness(0),
                    Background = new SolidColorBrush(Colors.Transparent),
                    BorderThickness = new Thickness(0),
                    Content = new PackIcon()
                    {
                        Kind = PackIconKind.Play,
                        Width = 30,
                        Height = 30,
                        Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 161, 163))
                    },
                    Uid = info[1]
                };

                Slider slider = new Slider()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5, 0, 0, 0),
                    BorderThickness = new Thickness(0),
                    Foreground = new SolidColorBrush(Colors.DodgerBlue),
                    Width = 200
                };

                Label playTime = new Label()
                {
                    Content = time1,
                    Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 161, 163)),
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(10, 5, 0, -5)
                };

                rec.Rect = new Rect(0, 0, 320, 70);

                Grid.SetColumn(image, 0);
                Grid.SetRow(image, 0);
                Grid.SetColumn(button, 1);
                Grid.SetRow(button, 0);
                Grid.SetColumn(slider, 2);
                Grid.SetRow(slider, 0);
                Grid.SetColumn(playTime, 2);
                Grid.SetRow(playTime, 0);

                grid.Children.Add(image);
                grid.Children.Add(button);
                grid.Children.Add(slider);
                grid.Children.Add(playTime);
                grid.Children.Add(_media);

                button.Click += Button_Click;


                if (tempinfo[0].Contains("me"))
                {
                    var path = Directory.GetCurrentDirectory() + $"\\{AppViewModel.CustomID}\\info.log";
                    tempinfo = File.ReadAllLines(path);
                    image.Source = new BitmapImage(new Uri(tempinfo[1]));
                }

            }
            else if (type == "image" || type == "file")
            {
                System.Windows.Controls.Image image = new System.Windows.Controls.Image()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Width = 160,
                    Height = 200
                };

                if (type == "file")
                {
                    var path = Directory.GetCurrentDirectory();
                    path = path.Remove(path.IndexOf("bin\\"));
                    path += "MVVM\\Resource\\fileImage.png";
                    image.Source = new BitmapImage(new Uri(path));

                    Label fileName = new Label()
                    {
                        Content = info[1].Split('\\').Last(),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        FontSize = 10,
                        Padding = new Thickness(0),
                        Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 161, 163)),
                        Margin = new Thickness(5, -5, 0, 5),
                        Width = 50
                    };

                    Grid.SetRow(fileName, 1);
                    grid.Children.Add(fileName);
                }
                else if (type == "image")
                {
                    image.Source = new BitmapImage(new Uri(info[1]));
                }

                rec.Rect = new Rect(0, 0, 160, 210);
                Grid.SetRow(image, 0);
                Grid.SetColumn(image, 0);

                grid.Children.Add(image);
            }

            Grid.SetRow(time, 1);
            time.Content = info[2];

            if (grid.ColumnDefinitions.Count > 0)
                Grid.SetColumn(time, 2);

            grid.Children.Add(time);

            grid.Clip = rec;

            tempinfo = info[0].Split(' ');
            if (tempinfo[0].Contains("you"))
            {
                grid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(16, 29, 36));
                item.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else if (tempinfo[0].Contains("me"))
            {
                grid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 93, 75));
                item.HorizontalAlignment = HorizontalAlignment.Right;
            }

            item.Content = grid;
            Items.Add(item);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //_mediaStateTimer.Stop();
            //
            //Button senderButton = (Button)sender;
            //
            //if (senderButton.Uid != "Working")
            //{
            //    var workId = -1;
            //    var normalId = -1;
            //    string uid = string.Empty;
            //    Grid grid = null;
            //    System.Windows.Controls.Image image = null;
            //    Button button = null;
            //    Slider slider = null;
            //    Label label = null;
            //    MediaElement tempMedia = null;
            //    bool state1 = false;
            //
            //    for (int i = 0; i < Items.Count; i++)
            //    {
            //        try
            //        {
            //            grid = Items[i].Content as Grid;
            //            button = grid.Children[1] as Button;
            //            if (button.Uid == senderButton.Uid)
            //            {
            //                _grid = grid;
            //                _image = grid.Children[0] as System.Windows.Controls.Image;
            //
            //                if (_button != null)
            //                    uid = _button.Uid;
            //
            //                _button = button;
            //                _slider = grid.Children[2] as Slider;
            //                _label = grid.Children[3] as Label;
            //                tempMedia = _media;
            //                _media = grid.Children[4] as MediaElement;
            //                
            //                normalId = i;
            //            }
            //            else if (button.Uid == "Working")
            //            {
            //                image = grid.Children[0] as System.Windows.Controls.Image;
            //                slider = grid.Children[2] as Slider;
            //                label = grid.Children[3] as Label;
            //                
            //                workId = i;
            //                state1 = true;
            //            }
            //        }
            //        catch (Exception) { }
            //    }
            //
            //    if (state1)
            //    {
            //        slider.Value = 0;
            //        button.Content = new PackIcon()
            //        {
            //            Kind = PackIconKind.Play,
            //            Width = 30,
            //            Height = 30,
            //            Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 161, 163))
            //        };
            //        button.Uid = uid;
            //        tempMedia.Play();
            //        label.Content = $"{tempMedia.NaturalDuration.TimeSpan.Minutes}:{tempMedia.NaturalDuration.TimeSpan.Seconds}";
            //        tempMedia.Stop();
            //
            //        grid.Children.Clear();
            //        grid.Children.Add(image);
            //        grid.Children.Add(button);
            //        grid.Children.Add(slider);
            //        grid.Children.Add(label);
            //    }
            //
            //    _voiceIndex = normalId;
            //    _media.Play();
            //    _media.LoadedBehavior = MediaState.Play;
            //    _button.Content = new PackIcon() { Kind = PackIconKind.Pause, Width = 30, Height = 30, Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(155, 161, 163)) }
            //    _mediaStateTimer.Start();
            //}
        }

        private void ChatBox_Drop(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            fileList = Helpers.ReturnNoVideo(fileList);

            Task.Run(() =>
            {
                List<ChatDataSend> datas = new List<ChatDataSend>();
                for (int i = 0; i < fileList.Length; i++)
                {
                    datas.Add(new ChatDataSend()
                    {
                        CustomID = AppViewModel.CustomID,
                        DataBytes = File.ReadAllBytes(fileList[i]),
                        FileName = fileList[i].Split('\\').Last(),
                        FriendID = User.ID
                    });
                }

                for (int i = 0; i < datas.Count; i++)
                {
                    string ext = Path.GetExtension(datas[i].FileName).ToLower();
                    if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".bmp" || ext == ".ico" || ext == ".jfif")
                    {
                        datas[i].SMSType = "image";
                        writer.Write("image|" + JsonConvert.SerializeObject(datas[i]));
                    }
                    else if (ext == ".mp3" || ext == ".m4a" || ext == ".wav" || ext == ".aac")
                    {
                        datas[i].SMSType = "voice";
                        writer.Write("voice|" + JsonConvert.SerializeObject(datas[i]));
                    }
                    else
                    {
                        datas[i].SMSType = "file";
                        writer.Write("file|" + JsonConvert.SerializeObject(datas[i]));
                    }

                    var path = Directory.GetCurrentDirectory() + $"\\Data\\{User.ID}\\chat.log";
                    var dataList = File.ReadAllLines(path).ToList();
                    var time = DateTime.Now;
                    dataList.Add($"me: {datas[i].SMSType}|{path.Remove(path.IndexOf("chat.log")) + datas[i].FileName}|{time.Day}.{time.Month}:{time.Year} {time.Hour}:{time.Minute}\n");

                    while (true)
                    {
                        try
                        {
                            File.WriteAllLines(path, dataList.ToArray());
                            break;
                        }
                        catch (Exception) { }
                    }
                }
            });
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRecord)
                _isRecord = false;
            else
                messageSend();
        }

        private void MessageBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => { ChatControl.MessageBox.TextChanged -= MessageBox_TextChanged; });

            if (MessageBoxText[MessageBoxText.Length - 1] == '|')
                MessageBoxText = MessageBoxText.Remove(MessageBoxText.Length - 1);

            App.Current.Dispatcher.Invoke(() =>
            {
                ChatControl.MessageBox.SelectionStart = MessageBoxText.Length;
                ChatControl.MessageBox.SelectionLength = 0;
                ChatControl.MessageBox.TextChanged -= MessageBox_TextChanged;
            });
        }

        private void MessageBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                messageSend();
            }
        }

        private void messageSend()
        {
            if (client == null)
            {
                MessageBox.Show("Please check internet", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var path = string.Empty;
            var pathInfos = User.ProfileImagePath.Split('\\');
            for (int i = 0; i < pathInfos.Length - 1; i++)
                path += pathInfos[i] + "\\";
            path += "chat.log";
            var time = DateTime.Now;
            File.AppendAllText(path, $"me: {MessageBoxText}| |{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
            CreateItemAndAddItem($"me: {MessageBoxText}| |{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n".Split('|'), "sms");

            ObservableCollection<ListBoxItem> _item = Items;
            Items = new ObservableCollection<ListBoxItem>();
            for (int i = 0; i < _item.Count; i++)
                Items.Add(_item[i]);
            App.Current.Dispatcher.Invoke(() =>
            {
                ChatControl.ChatBox.SelectedIndex = Items.Count - 1;
            });

            ChatDataSend tempData = new ChatDataSend()
            {
                CustomID = AppViewModel.CustomID,
                DataBytes = Encoding.ASCII.GetBytes(MessageBoxText),
                SMSType = "sms",
                FriendID = User.ID
            };
            var data = JsonConvert.SerializeObject(tempData);
            writer.Write($"sms|" + data);
            MessageBoxText = string.Empty;
        }

        private void setDataMethod()
        {
            User = AppViewModel.user;

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
                client = new TcpClient(IPAddress.Any.ToString(), 27001);
                writer = new BinaryWriter(client.GetStream());
                reader = new BinaryReader(client.GetStream());
                writer.Write(AppViewModel.CustomID.ToString());
            }
            catch (Exception) { }

            GC.Collect(0);
            GC.Collect(1);
            GC.Collect(2);

            MessageBoxText = "";

            packIcon = new PackIcon()
            {
                Foreground = new SolidColorBrush(Colors.White),
                Width = 25,
                Height = 25
            };

            Items = new ObservableCollection<ListBoxItem>();

            dispatcher = new DispatcherTimer();
            dispatcher.Tick += Dispatcher_Tick;
            dispatcher.Start();

            //_mediaStateTimer = new DispatcherTimer();
            //_mediaStateTimer.Tick += _mediaStateTimer_Tick;
        }

        private void _mediaStateTimer_Tick(object sender, EventArgs e)
        {
            _label.Content = $"{_media.Position.Minutes}:{_media.Position.Seconds}";

            _grid.Children.Clear();
            _grid.Children.Add(_image);
            _grid.Children.Add(_button);
            _grid.Children.Add(_slider);
            _grid.Children.Add(_image);
        }

        private void Dispatcher_Tick(object sender, EventArgs e)
        {
            List<string> temp = null;

            while (true)
            {
                try
                {
                    temp = File.ReadAllLines(Directory.GetCurrentDirectory() + $"\\Data\\{User.ID}\\chat.log").ToList();
                    break;
                }
                catch (Exception) { }
            }

            if (temp.Count > 0)
            {
                if (temp[temp.Count - 1] == "")
                    temp.RemoveAt(temp.Count - 1);
            }

            if (Items.Count != temp.Count)
            {
                Items = new ObservableCollection<ListBoxItem>();
                for (int i = 0; i < temp.Count; i++)
                {
                    try
                    {
                        var tempInfo = temp[i].Split('|');
                        if (tempInfo[1] == " ")
                            CreateItemAndAddItem(tempInfo, "sms");
                        else if (tempInfo[0].Contains("file") && tempInfo[1] != "")
                            CreateItemAndAddItem(tempInfo, "file");
                        else if (tempInfo[0].Contains("voice") && tempInfo[1] != "")
                            CreateItemAndAddItem(tempInfo, "voice");
                        else if (tempInfo[0].Contains("image") && tempInfo[1] != "")
                            CreateItemAndAddItem(tempInfo, "image");
                    }
                    catch (Exception) { }
                }
                if (Source != null && IsSetPhoto == 1)
                {
                    System.Windows.Forms.PictureBox box = new System.Windows.Forms.PictureBox();
                    box.Image = Source;

                    var infos = File.ReadAllLines(Directory.GetCurrentDirectory() + $"\\Data\\{User.ID}\\info.log");
                    var imageLineSplited = infos[infos.Length - 2].Split('|');
                    var count = Convert.ToInt64(imageLineSplited[1]) + 1;
                    infos[infos.Length - 2] = imageLineSplited[0] + "|" + count;
                    File.WriteAllLines(Directory.GetCurrentDirectory() + $"\\Data\\{User.ID}\\info.log", infos);

                   
                    Source.Save(Directory.GetCurrentDirectory() + $"\\Data\\{User.ID}\\Image{count}.jpeg", ImageFormat.Jpeg);

                    var time = DateTime.Now;
                    File.AppendAllText(Directory.GetCurrentDirectory() + $"\\Data\\{User.ID}\\chat.log", "me: image|" + Directory.GetCurrentDirectory() + $"\\Data\\{User.ID}\\Image{count}.jpeg|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");
                    //CreateItemAndAddItem($"me: image|{Directory.GetCurrentDirectory()}\\Data\\{User.ID}\\Image{count}.jpeg|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n".Split('|'), "image");

                    ChatDataSend data = new ChatDataSend()
                    {
                        CustomID = AppViewModel.CustomID,
                        DataBytes = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}\\Data\\{User.ID}\\Image{count}.jpeg"),
                        FileName = $"Image{count}.jpeg",
                        SMSType = "image",
                        FriendID = User.ID
                    };

                    writer.Write($"image|{JsonConvert.SerializeObject(data)}");

                    Source = null;
                    IsSetPhoto = 0;
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    if (Items.Count > 0)
                        ChatControl.ChatBox.SelectedItem = Items[Items.Count - 1];
                });
            }
        }

        [DllImport("winmm.dll")]
        private static extern int mciSendString(string MciComando, string MciRetron, int MciRetronLenght, int Callback);

        private void RecordButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_writeMessage)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!_isRecord)
                    {
                        try
                        {
                            Ping myPing = new Ping();
                            String host = "google.com";
                            byte[] buffer = new byte[32];
                            int timeout = 1000;
                            PingOptions pingOptions = new PingOptions();
                            PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                            if (reply.Status != IPStatus.Success) throw new Exception();
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Please check network.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        recordMethod();
                        _isRecord = true;
                        changeIconThread.Suspend();
                        PackIcon icon = new PackIcon()
                        {
                            Kind = PackIconKind.Stop,
                            Foreground = new SolidColorBrush(Colors.White),
                            Width = 25,
                            Height = 25
                        };

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            ChatControl.RecordButton.Content = icon;
                        });
                    }
                }
            }
        }

        private void RecordButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isRecord)
            {
                saveRecord();
                changeIconThread.Resume();

                ObservableCollection<ListBoxItem> _item = Items;
                Items = new ObservableCollection<ListBoxItem>();
                Items = _item;
                App.Current.Dispatcher.Invoke(() =>
                {
                    ChatControl.ChatBox.SelectedIndex = Items.Count - 1;
                });
            }
        }

        private void saveRecord()
        {
            mciSendString("pause Som", null, 0, 0);

            var spliedPath = User.ProfileImagePath.Split('\\');
            string path = string.Empty;

            for (int i = 0; i < spliedPath.Length - 1; i++)
                path += spliedPath[i] + "\\";

            var tempPath = path + "info.log";
            spliedPath = File.ReadAllLines(tempPath);
            var lastLineSplit = spliedPath.Last().Split('|');
            int number = Convert.ToInt32(lastLineSplit.Last());
            number++;
            lastLineSplit[1] = number.ToString();
            spliedPath[6] = lastLineSplit[0] + "|" + lastLineSplit[1];
            File.WriteAllLines(tempPath, spliedPath);
            tempPath = path + "chat.log";
            path += $"audio{number}.wav";
            var time = DateTime.Now;
            File.AppendAllText(tempPath, $"me: voice|{path}|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n");

            mciSendString($"save Som {path}", null, 0, 0);
            mciSendString("close Som", null, 0, 0);
            //CreateItemAndAddItem($"me: voice|{path}|{time.Day}.{time.Month}.{time.Year} {time.Hour}:{time.Minute}\n".Split('|'), "voice");

            ChatDataSend data = new ChatDataSend()
            {
                CustomID = AppViewModel.CustomID,
                DataBytes = File.ReadAllBytes(path),
                FileName = path.Split('\\').Last(),
                SMSType = "voice",
                FriendID = User.ID
            };

            var serializeData = JsonConvert.SerializeObject(data);

            writer.Write("voice|" + serializeData);

            PackIcon icon = new PackIcon()
            {
                Kind = PackIconKind.Microphone,
                Foreground = new SolidColorBrush(Colors.White),
                Width = 25,
                Height = 25
            };

            App.Current.Dispatcher.Invoke(() =>
            {
                ChatControl.RecordButton.Content = icon;
            });
        }

        private void recordMethod()
        {
            mciSendString("open new type waveaudio alias Som", null, 0, 0);
            mciSendString("record Som", null, 0, 0);
        }
    }
}