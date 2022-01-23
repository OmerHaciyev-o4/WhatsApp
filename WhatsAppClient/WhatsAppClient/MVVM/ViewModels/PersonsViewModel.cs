using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
using WhatsAppClient.MVVM.Models;
using WhatsAppClient.MVVM.Views;

namespace WhatsAppClient.MVVM.ViewModels
{
    public class PersonsViewModel : BaseViewModel
    {
        #region Private Varibale

        private string _currentFilePath;
        private DispatcherTimer timer;
        #endregion

        #region Full Property
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ListBoxItem> items;
        public ObservableCollection<ListBoxItem> Items
        {
            get { return items; }
            set { items = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand SearchCommand { get; set; }
        public ICommand DefaultCommand { get; set; }
        public ICommand OpenContactViewCommand { get; set; }
        #endregion

        #region References
        public PersonsControl PersonsControl { get; set; }
        public AppViewModel ViewModel { get; set; }
        #endregion

        public PersonsViewModel()
        {
            setDataMethod();

            SearchCommand = new RelayCommand((sender) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    PersonsControl.firstPanel.Visibility = Visibility.Hidden;
                    PersonsControl.lastPanel.Visibility = Visibility.Visible;
                    PersonsControl.searchBox.Focus();
                });
            });

            DefaultCommand = new RelayCommand((sender) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    PersonsControl.lastPanel.Visibility = Visibility.Hidden;
                    PersonsControl.firstPanel.Visibility = Visibility.Visible;
                    SearchText = "";
                });
            });

            OpenContactViewCommand = new RelayCommand((sender) =>
            {
                timer.Stop();
                ViewModel.ChangeViewControl("contact");
            });

            Thread setData = new Thread(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    PersonsControl.chatsPanel.Checked += Panel_Checked;
                    PersonsControl.statusPanel.Checked += Panel_Checked;
                    PersonsControl.callsPanel.Checked += Panel_Checked;
                });
            });
            setData.Start();
        }

        private void setDataMethod()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;

            Items = new ObservableCollection<ListBoxItem>();

            _currentFilePath = Directory.GetCurrentDirectory() + "\\Data\\usersInfo.log";
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\Data"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\Data");
                File.WriteAllText(_currentFilePath, "");
            }
            else if (!File.Exists(_currentFilePath))
                File.WriteAllText(_currentFilePath, "");


            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string[] usersInfo = null;
            while (true)
            {
                try
                {
                    usersInfo = File.ReadAllLines(_currentFilePath);
                    break;
                }
                catch (Exception) { }
            }

            if (usersInfo != null && usersInfo.Length != Items.Count)
            {
                Items = new ObservableCollection<ListBoxItem>();
                int count = 0;
                for (int i = 0; i < usersInfo.Length; i++)
                {
                    addItem(usersInfo[i], count);
                    count++;
                }
            }
        }

        private void addItem(string path, int count)
        {
            var userInfo = File.ReadAllLines(path);

            if (userInfo != null && userInfo.Length > 0)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    UserLow user = new UserLow();
                    user.ID = Convert.ToInt32(userInfo[0]);
                    user.ProfileImagePath = userInfo[1];
                    user.Name = userInfo[2];
                    user.About = userInfo[3];
                    user.PhoneNumber = userInfo[4];
                    //user id-ne gore datani almaliyam
                    var chatPath = string.Empty;
                    var spliedInfo = path.Split('\\');
                    for (int i = 0; i < spliedInfo.Length - 1; i++)
                        chatPath += spliedInfo[i] + "\\";
                    chatPath += "chat.log";
                    spliedInfo = File.ReadAllLines(chatPath);

                    ListBoxItem item = new ListBoxItem();
                    Button button = new Button()
                    {
                        Width = 390,
                        Height = 50,
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderThickness = new Thickness(0)
                    };
                    Grid grid = new Grid()
                    {
                        Width = 390,
                        Height = 50
                    };

                    StackPanel mainPanel = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };

                    Image profilImg;

                    while (true)
                    {
                        try
                        {
                            profilImg = new Image()
                            {
                                Width = 50,
                                Height = 50,
                                Source = new BitmapImage(new Uri(userInfo[1]))
                            };

                            break;
                        }
                        catch (Exception) { }
                    }

                    EllipseGeometry ellipse = new EllipseGeometry(new Point(25, 25), 25, 25);
                    StackPanel childrenPanel = new StackPanel() { Width = 180 };
                    Label nameLabel = new Label()
                    {
                        Content = userInfo[2],
                        Foreground = new SolidColorBrush(Colors.White),
                        FontSize = 20,
                        Padding = new Thickness(0, 0, 0, 0),
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    var lastMessageStr = string.Empty;
                    var content = string.Empty;
                    if (spliedInfo.Length > 0)
                    {
                        content = spliedInfo[spliedInfo.Length - 1].Split('|')[2];
                        lastMessageStr = spliedInfo[spliedInfo.Length - 1].Split('|')[0];
                    }
                    else
                    {
                        lastMessageStr = user.About;
                    }

                    Label lastMessage = new Label()
                    {
                        Content = lastMessageStr,
                        Foreground = new SolidColorBrush(Color.FromRgb(155, 161, 163)),
                        FontSize = 12,
                        Padding = new Thickness(0, 0, 0, 0),
                        Margin = new Thickness(10, 0, 0, 0)
                    };

                    StackPanel mainPanel2 = new StackPanel()
                    {
                        Width = 80,
                        HorizontalAlignment = HorizontalAlignment.Right
                    };


                    Label time = new Label()
                    {
                        Content = content,
                        Foreground = new SolidColorBrush(Color.FromRgb(155, 161, 163)),
                        FontSize = 12,
                        Padding = new Thickness(0, 0, 0, 0),
                        Margin = new Thickness(-50, 0, 40, 0),
                        HorizontalAlignment = HorizontalAlignment.Right
                    };
                    Label objectLabel = new Label()
                    {
                        Content = JsonConvert.SerializeObject(user),
                        Visibility = Visibility.Hidden
                    };
                    Label countLabel = new Label()
                    {
                        Content = count.ToString(),
                        Visibility = Visibility.Hidden
                    };

                    profilImg.Clip = ellipse;
                    childrenPanel.Children.Add(nameLabel);
                    childrenPanel.Children.Add(lastMessage);
                    mainPanel.Children.Add(profilImg);
                    mainPanel.Children.Add(childrenPanel);

                    mainPanel2.Children.Add(time);
                    mainPanel2.Children.Add(objectLabel);
                    mainPanel2.Children.Add(countLabel);

                    grid.Children.Add(mainPanel);
                    grid.Children.Add(mainPanel2);

                    button.Content = grid;
                    button.Click += Button_Click;
                    item.Content = button;

                    Items.Add(item);
                });


                //   <Label Content="1" Background="#57f573" Width="20" Height="20" Padding="7 2 0 0" Margin="0 5 10 0" HorizontalAlignment="Right">
                //   <Label.Clip>
                //       <EllipseGeometry Center="10 10" RadiusX="10" RadiusY="10"/>
                //  </Label.Clip>
                //
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            var number = Convert.ToInt32(((((button.Content as Grid).Children[1] as StackPanel).Children[2] as Label).Content.ToString()));

            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                Button button1 = (Button)item.Content;
                var number1 = Convert.ToInt32(((((button1.Content as Grid).Children[1] as StackPanel).Children[2] as Label).Content.ToString()));
                var data = (((button1.Content as Grid).Children[1] as StackPanel).Children[1] as Label).Content.ToString();

                if (number == number1)
                {
                    AppViewModel.user = JsonConvert.DeserializeObject<UserLow>(data);
                    ViewModel.ChangeViewControl("chat");
                    return;
                }
            }

        }

        private void Panel_Checked(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (PersonsControl.chatsPanel.IsChecked == true)
                {
                    PersonsControl.chatsPanel.Foreground = new SolidColorBrush(Color.FromRgb(87, 245, 115));
                    PersonsControl.statusPanel.Foreground = new SolidColorBrush(Color.FromRgb(155, 161, 163));
                    PersonsControl.callsPanel.Foreground = new SolidColorBrush(Color.FromRgb(155, 161, 163));
                }
                else if (PersonsControl.statusPanel.IsChecked == true)
                {
                    PersonsControl.chatsPanel.Foreground = new SolidColorBrush(Color.FromRgb(155, 161, 163));
                    PersonsControl.statusPanel.Foreground = new SolidColorBrush(Color.FromRgb(87, 245, 115));
                    PersonsControl.callsPanel.Foreground = new SolidColorBrush(Color.FromRgb(155, 161, 163));
                }
                else if (PersonsControl.callsPanel.IsChecked == true)
                {
                    PersonsControl.chatsPanel.Foreground = new SolidColorBrush(Color.FromRgb(155, 161, 163));
                    PersonsControl.statusPanel.Foreground = new SolidColorBrush(Color.FromRgb(155, 161, 163));
                    PersonsControl.callsPanel.Foreground = new SolidColorBrush(Color.FromRgb(87, 245, 115));
                }
            });
        }
    }
}
