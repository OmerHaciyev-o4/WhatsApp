using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using WhatsAppClient.MVVM.Commands;
using WhatsAppClient.MVVM.Helper;
using WhatsAppClient.MVVM.Models;
using WhatsAppClient.MVVM.Views;

namespace WhatsAppClient.MVVM.ViewModels
{//2
    public class ContactViewModel : BaseViewModel
    {
        #region Private Varibale
        private List<string> regionNameList;
        private List<string> regionCodeList;
        private string accountSid;
        private string authToken;
        private bool changeData;
        private string tempPhoneNumberText;
        private string tempRegionCodeText;

        //Server connect varibales
        private TcpClient client;
        private BinaryWriter writer;
        private BinaryReader reader;
        private DispatcherTimer updateServerState;
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

        private string verificationText;
        public string VerificationText
        {
            get { return verificationText; }
            set { verificationText = value; OnPropertyChanged(); }
        }

        private ObservableCollection<string> numberItems;
        public ObservableCollection<string> NumberItems
        {
            get { return numberItems; }
            set { numberItems = value; OnPropertyChanged(); }
        }

        #endregion

        #region References
        public ContactControl ContactControl { get; set; }
        public AppViewModel ViewModel { get; set; }
        #endregion

        #region Commands
        public ICommand SendCodeCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand BeforeViewCommand { get; set; }
        #endregion

        public ContactViewModel()
        {
            setDataMethod();

            SendCodeCommand = new RelayCommand((sender) =>
            {
                Thread sendThread = new Thread(() =>
                {
                    var customInfos = File.ReadAllLines(File.ReadAllLines("data.log")[0]);
                    if (RegionCodeText + PhoneNumberText == customInfos.Last().ToString())
                    {
                        MessageBox.Show("Please enter friend phone number", "Error", MessageBoxButton.OK);
                        return;
                    }

                    writer.Write($"PhoneNumber|{RegionCodeText}{PhoneNumberText}");
                    var resultInfo = reader.ReadString();

                    if (resultInfo == "Error")
                    {
                        try
                        {
                            //var verification = VerificationResource.Create(
                            //    to: $"{RegionCodeText}{PhoneNumberText}",
                            //    channel: "sms",
                            //    pathServiceSid: "VAd2b650419ce75e49a09e8e692d3175d3");

                            tempPhoneNumberText = PhoneNumberText;
                            tempRegionCodeText = RegionCodeText;

                            App.Current.Dispatcher.Invoke(() => { ContactControl.codeTextBox.IsEnabled = true; });
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Please enter correct phone number or region code or select region", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);

                            return;
                        }
                    }
                    else
                        MessageBox.Show($"{RegionCodeText}{PhoneNumberText} not use WhatsApp.", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                });
                sendThread.Start();
            },
            (pred) =>
            {
                if (!string.IsNullOrEmpty(PhoneNumberText))
                    return true;
                return false;
            });

            AddCommand = new RelayCommand((sender) =>
            {
                Thread addThread = new Thread(() =>
                {
                    try
                    {

                        if (VerificationText != "110418") throw new Exception();

                        //var verificationCheck = VerificationCheckResource.Create(
                        //    to: $"{tempRegionCodeText}{tempPhoneNumberText}",
                        //    code: VerificationText,
                        //    pathServiceSid: "VAe05df72632429af144c249c8b5ffaaa4");


                        try
                        {
                            changeData = true;

                            ChatDataSend data = new ChatDataSend()
                            {
                                CustomID = AppViewModel.CustomID,
                                DataBytes = Encoding.ASCII.GetBytes($"{tempRegionCodeText}{tempPhoneNumberText}")
                            };

                            writer.Write($"GetUser|{JsonConvert.SerializeObject(data)}");
                            var jsonData = reader.ReadString();
                            var user = JsonConvert.DeserializeObject<User>(jsonData);

                            var path = Directory.GetCurrentDirectory() + "\\Data";

                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                                File.Create(path + "\\usersInfo.log");
                            }
                            else
                                if (!File.Exists(path + "\\usersInfo.log"))
                                File.Create(path + "\\usersInfo.log");

                            var infos = File.ReadAllLines(path + "\\usersInfo.log").ToList();
                            path += $"\\{user.ID}";
                            infos.Add(path + "\\info.log");
                            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\Data\\usersInfo.log", infos.ToArray());
                            Directory.CreateDirectory(path);

                            infos = new List<string>();
                            infos.Add(user.ID.ToString());
                            infos.Add(path + $"\\{user.ProfileImagePath.Split('\\').Last()}");
                            infos.Add(user.Name);
                            infos.Add(user.About);
                            infos.Add(user.PhoneNumber);
                            infos.Add("image|0");
                            infos.Add("sound|0");
                            File.WriteAllLines(path + "\\info.log", infos.ToArray());
                            File.WriteAllText(path + "\\chat.log", "");
                            File.WriteAllBytes(path + "\\" + user.ProfileImagePath.Split('\\').Last(), user.ProfileImageByte);

                            if (!File.Exists("contact.log"))
                                File.WriteAllText("contact.log", $"{tempRegionCodeText}{tempPhoneNumberText}\n");
                            else
                                File.AppendAllText("contact.log", $"{tempRegionCodeText}{tempPhoneNumberText}\n");


                            changeData = false;

                            MessageBox.Show("Your friend successfully added.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                            PhoneNumberText = string.Empty;
                            VerificationText = string.Empty;
                            RegionCodeText = "+";
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                ContactControl.codeTextBox.IsEnabled = false;
                                ContactControl.countries.SelectedIndex = 0;
                            });
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Please check internet.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            Process currentProcess = Process.GetCurrentProcess();
                            currentProcess.Kill();
                            return;
                        }
                    }
                    catch (Exception)
                    { MessageBox.Show("Please correct code.", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                });
                addThread.Start();
            },
            (pred) =>
            {
                if (!string.IsNullOrEmpty(VerificationText))
                    return true;
                return false;
            });

            DeleteCommand = new RelayCommand((sender) =>
            {
                Task.Run(() =>
                {
                    changeData = true;

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        NumberItems.RemoveAt(ContactControl.contact.SelectedIndex);
                        File.WriteAllLines("contact.log", NumberItems);
                    });

                    changeData = false;
                });
            },
            (pred) =>
            {
                if (ContactControl.contact.SelectedIndex >= 0)
                    return true;
                return false;
            });

            BeforeViewCommand = new RelayCommand((sender) =>
            {
                if (client != null)
                    writer.Write("close");

                ViewModel.ChangeViewControl("persons");
            });

            Thread thread = new Thread(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    ContactControl.regionCode.TextChanged += RegionCode_TextChanged;
                    ContactControl.codeTextBox.IsEnabled = false;

                    ContactControl.addRadioButton.Checked += ChangePanel;
                    ContactControl.deleteRadioButton.Checked += ChangePanel;
                });
            });
            thread.Start();

            Thread updateData = new Thread(() =>
            {
                string[] datas;
                int count = 0;
                while (true)
                {
                    if (!changeData)
                    {
                        try
                        {
                            datas = File.ReadAllLines("contact.log");

                            if (datas.Length != count)
                            {
                                NumberItems = new ObservableCollection<string>();

                                foreach (var data in datas)
                                    NumberItems.Add(data);
                                count = datas.Length;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            });
            updateData.Start();
        }

        private void ChangePanel(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (ContactControl.addRadioButton.IsChecked == true)
                {
                    if (ContactControl.addPanel.Visibility != Visibility.Visible)
                        ContactControl.addPanel.Visibility = Visibility.Visible;

                    ContactControl.deletePanel.Visibility = Visibility.Hidden;
                }
                else if (ContactControl.deleteRadioButton.IsChecked == true)
                {
                    if (ContactControl.deletePanel.Visibility != Visibility.Visible)
                        ContactControl.deletePanel.Visibility = Visibility.Visible;

                    ContactControl.addPanel.Visibility = Visibility.Hidden;
                }
            });
        }

        private void setDataMethod()
        {
            RegionCodeText = "+";


            //accountSid = "ACfd0363cdec733b79d54f6cbc1512492a";
            //authToken = "b07d467ff541cc5d63bc5456eb9bdaa1";

            //TwilioClient.Init(accountSid, authToken);

            updateServerState = new DispatcherTimer();
            updateServerState.Tick += UpdateServerState_Tick;

            Items = new ObservableCollection<ComboBoxItem>();
            NumberItems = new ObservableCollection<string>();

            Task.Run(() =>
            {
                regionNameList = CultureHelper.RegionNameList;
                regionCodeList = CultureHelper.RegionCodeList;

                App.Current.Dispatcher.Invoke(() =>
                {
                    AddItem("Choose a country", "");

                    for (int i = 0; i < regionNameList.Count; i++)
                        AddItem(regionNameList[i], regionCodeList[i]);

                    ContactControl.countries.SelectionChanged += Countries_SelectionChanged;
                });
            });

            Task.Run(() =>
            {
                if (!File.Exists("contact.log"))
                    File.WriteAllText("contact.log", "");
                else
                {
                    var numbers = File.ReadAllLines("contact.log");
                    foreach (var number in numbers)
                        NumberItems.Add(number);
                }
            });

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

                writer.Write(AppViewModel.CustomID.ToString() + "|contact");
            }
            catch (Exception)
            {
                MessageBox.Show("There was a problem joining. Please restart the program ;)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }

            updateServerState.Start();
        }

        private void UpdateServerState_Tick(object sender, EventArgs e)
        {
            if (client == null)
            {
                try
                {
                    client = new TcpClient(IPAddress.Any.ToString(), 27001);
                    writer = new BinaryWriter(client.GetStream());
                    reader = new BinaryReader(client.GetStream());
            
                    writer.Write(AppViewModel.CustomID.ToString() + "|contact");
                }
                catch (Exception)
                {
                    MessageBox.Show("There was a problem joining. Please restart the program ;)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        private void AddItem(string country, string code)
        {
            ComboBoxItem item = new ComboBoxItem()
            {
                Background = new SolidColorBrush(Colors.White),
                Foreground = new SolidColorBrush(Colors.Black),
            };

            Label countryLabel = new Label()
            {
                Foreground = new SolidColorBrush(Colors.Black),
                Content = country,
            };

            if (code == "")
                item.Content = countryLabel;
            else
            {
                Grid grid = new Grid()
                {
                    Width = 280
                };

                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                Label codeLabel = new Label() { Content = "+" + code, HorizontalAlignment = HorizontalAlignment.Center };

                Grid.SetColumn(countryLabel, 0);
                Grid.SetColumn(codeLabel, 1);

                grid.Children.Add(countryLabel);
                grid.Children.Add(codeLabel);

                item.Content = grid;
            }

            Items.Add(item);
        }

        private void Countries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Task.Run(() =>
            {
                ComboBoxItem item = null;
                App.Current.Dispatcher.Invoke(() =>
                {
                    item = (ComboBoxItem)ContactControl.countries.SelectedItem;
                    ContactControl.countries.SelectionChanged -= Countries_SelectionChanged;
                    ContactControl.regionCode.TextChanged -= RegionCode_TextChanged;
                    var grid = item.Content as Grid;

                    if (grid == null)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            ContactControl.regionCode.Focus();
                            ContactControl.countries.SelectionChanged += Countries_SelectionChanged;
                            ContactControl.regionCode.TextChanged += RegionCode_TextChanged;
                        });
                    }
                    else
                    {
                        var label = grid.Children[1] as Label;
                        RegionCodeText = label.Content.ToString();

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            ContactControl.phoneNumber.Focus();
                            ContactControl.countries.SelectionChanged += Countries_SelectionChanged;
                            ContactControl.regionCode.TextChanged += RegionCode_TextChanged;
                        });
                    }
                });
            });
        }

        private void RegionCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ContactControl.regionCode.TextChanged -= RegionCode_TextChanged;
                ContactControl.countries.SelectionChanged -= Countries_SelectionChanged;
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
                            ContactControl.countries.SelectedIndex = i + 1;
                            ContactControl.phoneNumber.Focus();
                        });
                        state = true;
                        break;
                    }
                }

                if (!state)
                    App.Current.Dispatcher.Invoke(() => { ContactControl.countries.SelectedIndex = 0; });
            }
            else if (RegionCodeText.Length == 0)
            {
                RegionCodeText = "+";
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                ContactControl.regionCode.SelectionStart = RegionCodeText.Length;
                ContactControl.regionCode.SelectionLength = 0;
                ContactControl.regionCode.TextChanged += RegionCode_TextChanged;
                ContactControl.countries.SelectionChanged += Countries_SelectionChanged;
            });
        }
    }
}