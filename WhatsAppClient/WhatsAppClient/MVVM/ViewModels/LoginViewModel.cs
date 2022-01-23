using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using WhatsAppClient.MVVM.Commands;
using WhatsAppClient.MVVM.Helper;
using WhatsAppClient.MVVM.Views;

namespace WhatsAppClient.MVVM.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        #region Static Property
        public static string PhoneNumber { get; set; }
        #endregion

        #region Private Varibale
        private List<string> regionNameList;
        private List<string> regionCodeList;
        private string accountSid;
        private string authToken;

        private TcpClient client;
        private BinaryReader reader;
        private BinaryWriter writer;
        private string IP;
        private bool state;
        #endregion

        #region Auto Property
        public ObservableCollection<ComboBoxItem> Items { get; set; }
        #endregion

        #region Full property
        private string regionCodeText;
        public string RegionCodeText
        {
            get { return regionCodeText; }
            set { regionCodeText = value; OnPropertyChanged(); }
        }

        private string phoneNumberText;
        public string PhoneNumberText
        {
            get { return phoneNumberText; }
            set { phoneNumberText = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand NextCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion

        #region References
        public LoginScreen LoginScreen { get; set; }
        #endregion

        // Verification not active

        public LoginViewModel()
        {
            setData();

            RegionCodeText = "+";

            NextCommand = new RelayCommand((sender) =>
            {
                string data = $"PhoneNumber|{RegionCodeText}{PhoneNumberText}";
                try
                {
                    writer.Write(data);
                    writer.Write(data);
                    var info = reader.ReadString();

                    if (info == "Error")
                    {
                        MessageBox.Show("The number is already registered.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        try
                        {
                            //var verification = VerificationResource.Create(
                            //    to: $"{RegionCodeText}{PhoneNumberText}",
                            //    channel: "sms",
                            //    pathServiceSid: "VAe05df72632429af144c249c8b5ffaaa4");

                            PhoneNumber = $"{RegionCodeText}{PhoneNumberText}";

                            VerificationScreen screen = new VerificationScreen() { };
                            writer.Write("close");
                            screen.Show();

                            App.Current.Dispatcher.Invoke(() => { LoginScreen.Hide(); });
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Please enter correct phone number", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Please check internet.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Process currentProcess = Process.GetCurrentProcess();
                    currentProcess.Kill();
                    return;
                }
            });

            MinimizeCommand = new RelayCommand((sender) =>
            {
                if (LoginScreen.WindowState != WindowState.Minimized)
                    App.Current.Dispatcher.Invoke(() => { LoginScreen.WindowState = WindowState.Minimized; });
            });

            CloseCommand = new RelayCommand((sender) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {

                    App.Current.Shutdown();
                });
            });

            Thread thread = new Thread(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    LoginScreen.closeButton.MouseMove += CloseButton_MouseMove;
                    LoginScreen.closeButton.MouseLeave += CloseButton_MouseLeave;
                    LoginScreen.contextBar.MouseDown += ContextBar_MouseDown;
                    LoginScreen.regionCode.TextChanged += RegionCode_TextChanged;
                    LoginScreen.regionCode.SelectionStart = RegionCodeText.Length;
                    LoginScreen.regionCode.SelectionLength = 0;
                    LoginScreen.Countries.SelectionChanged += Countries_SelectionChanged;
                    LoginScreen.Closing += LoginScreen_Closing;
                });
            });
            thread.Start();
        }

        private void LoginScreen_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                writer.Write("close");
            }
            catch (Exception) { }
        }

        private void ContextBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => { LoginScreen.DragMove(); });
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => { LoginScreen.closeButton.Background = new SolidColorBrush(Colors.Transparent); });
        }

        private void CloseButton_MouseMove(object sender, MouseEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => { LoginScreen.closeButton.Background = new SolidColorBrush(Colors.Red); });
        }

        private void Countries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Task.Run(() =>
            {
                ComboBoxItem item = null;
                App.Current.Dispatcher.Invoke(() =>
                {
                    item = (ComboBoxItem)LoginScreen.Countries.SelectedItem;
                    LoginScreen.Countries.SelectionChanged -= Countries_SelectionChanged;
                    LoginScreen.regionCode.TextChanged -= RegionCode_TextChanged;
                    var grid = item.Content as Grid;

                    if (grid == null)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            LoginScreen.regionCode.Focus();
                            LoginScreen.Countries.SelectionChanged += Countries_SelectionChanged;
                            LoginScreen.regionCode.TextChanged += RegionCode_TextChanged;
                        });
                    }
                    else
                    {
                        var label = grid.Children[1] as Label;
                        RegionCodeText = label.Content.ToString();

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            LoginScreen.phonNum.Focus();
                            LoginScreen.Countries.SelectionChanged += Countries_SelectionChanged;
                            LoginScreen.regionCode.TextChanged += RegionCode_TextChanged;
                        });
                    }
                });
            });
        }

        private void RegionCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                LoginScreen.regionCode.TextChanged -= RegionCode_TextChanged;
                LoginScreen.Countries.SelectionChanged -= Countries_SelectionChanged;
            });

            if (RegionCodeText.Length > 1)
            {
                if (!Regex.IsMatch(RegionCodeText[RegionCodeText.Length - 1].ToString(), "^[0-9]"))
                    RegionCodeText = RegionCodeText.Remove(RegionCodeText.Length - 1);

                string tempData = RegionCodeText.Substring(1, RegionCodeText.Length - 1);
                bool state = false;

                for (int i = 0; i < regionCodeList.Count; i++)
                {
                    if (regionCodeList[i] == tempData)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            LoginScreen.Countries.SelectedIndex = i + 1;
                            LoginScreen.phonNum.Focus();
                        });
                        state = true;
                        break;
                    }
                }

                if (!state)
                    App.Current.Dispatcher.Invoke(() => { LoginScreen.Countries.SelectedIndex = 0; });
            }
            else if (RegionCodeText.Length == 0)
            {
                RegionCodeText = "+";
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                LoginScreen.regionCode.SelectionStart = RegionCodeText.Length;
                LoginScreen.regionCode.SelectionLength = 0;
                LoginScreen.regionCode.TextChanged += RegionCode_TextChanged;
                LoginScreen.Countries.SelectionChanged += Countries_SelectionChanged;
            });
        }

        private void setData()
        {
            //accountSid = "ACfd0363cdec733b79d54f6cbc1512492a";
            //authToken = "b07d467ff541cc5d63bc5456eb9bdaa1";



            //TwilioClient.Init(accountSid, authToken);
            Items = new ObservableCollection<ComboBoxItem>();

            //Connect Server
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
                reader = new BinaryReader(client.GetStream());
                writer = new BinaryWriter(client.GetStream());
            }
            catch (Exception)
            {
                MessageBox.Show("There was a problem joining. Please restart the program ;)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }

            //Upload countries infos
            Task.Run(() =>
            {
                CultureHelper.Start();
                regionNameList = CultureHelper.RegionNameList;
                regionCodeList = CultureHelper.RegionCodeList;

                App.Current.Dispatcher.Invoke(() =>
                {
                    AddItem("Choose a country", "");

                    for (int i = 0; i < regionNameList.Count; i++)
                        AddItem(regionNameList[i], regionCodeList[i]);
                });
            });
        }

        private void AddItem(string country, string code)
        {
            ComboBoxItem item = new ComboBoxItem()
            {
                Background = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 15
            };

            Label countryLabel = new Label()
            {
                Foreground = new SolidColorBrush(Colors.Black),
                Content = country,
                FontSize = 15
            };

            if (code == "")
                item.Content = countryLabel;
            else
            {
                Grid grid = new Grid()
                {
                    Width = 390
                };

                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                Label codeLabel = new Label()
                {
                    Foreground = new SolidColorBrush(Colors.Black),
                    Content = "+" + code,
                    FontSize = 15,
                    HorizontalAlignment = HorizontalAlignment.Right
                };

                Grid.SetColumn(countryLabel, 0);
                Grid.SetColumn(codeLabel, 1);

                grid.Children.Add(countryLabel);
                grid.Children.Add(codeLabel);

                item.Content = grid;
            }

            Items.Add(item);
        }
    }
}