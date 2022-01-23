using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Twilio.Rest.Verify.V2.Service;
using WhatsAppClient.MVVM.Commands;
using WhatsAppClient.MVVM.Views;

namespace WhatsAppClient.MVVM.ViewModels
{
    public class VerificationViewModel : BaseViewModel
    {
        #region Private Varibale
        private int _firstState;
        private int _symbolDigit;
        private int _milSecond;
        private int _second;
        private int _minute;
        private int _hour;
        private int _clickCount;
        private string _stopWord;
        private DispatcherTimer _timer;
        #endregion

        #region Full Property
        private string partCodeTextBoxText;
        public string PartCodeTextBoxText
        {
            get { return partCodeTextBoxText; }
            set { partCodeTextBoxText = value; OnPropertyChanged(); }
        }

        private string partCodeLabelText;
        public string PartCodeLabelText
        {
            get { return partCodeLabelText; }
            set { partCodeLabelText = value; OnPropertyChanged(); }
        }

        private string phoneNumber;
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; OnPropertyChanged(); }
        }

        private string time;
        public string Time
        {
            get { return time; }
            set { time = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand NextCommand { get; set; }
        public ICommand SendVerificationCommand { get; set; }
        public ICommand PreviewScreenCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion

        #region References
        public VerificationScreen VerificationScreen { get; set; }
        #endregion

        public VerificationViewModel()
        {
            setDataMethod();

            SendVerificationCommand = new RelayCommand((sender) =>
            {
                if (_clickCount == 0)
                    _stopWord = "1minute";
                else if (_clickCount == 1)
                    _stopWord = "5minute";
                else if (_clickCount == 2)
                    _stopWord = "30minute";
                else if (_clickCount == 3)
                    _stopWord = "1hour";

                //var verification = VerificationResource.Create(
                //    to: PhoneNumber,
                //    channel: "sms",
                //    pathServiceSid: "VAe05df72632429af144c249c8b5ffaaa4");



                _clickCount++;
                App.Current.Dispatcher.Invoke(() => { VerificationScreen.timeButton.IsEnabled = false; });
                _timer.Start();
            });

            NextCommand = new RelayCommand((sender) =>
            {
                var splitedInfo = PartCodeTextBoxText.Split(' ');

                string code = string.Empty;

                for (int i = 0; i < splitedInfo.Length; i++)
                {
                    if (Regex.IsMatch(splitedInfo[i].ToString(), "^[0-9]"))
                        code += splitedInfo[i];
                }

                try
                {
                    if (code != "110418") throw new Exception();

                    //var verificationCheck =
                    //CheckResource.Create(
                    //    to: PhoneNumber,
                    //    code: code,
                    //    pathServiceSid: "VAe05df72632429af144c249c8b5ffaaa4");



                    File.AppendAllText("state.log", "\n\"login\":\"true\"");
                    File.WriteAllText("data.log", $"{PhoneNumber}");

                    App.Current.Dispatcher.Invoke(() => { VerificationScreen.Hide(); });

                    InfoScreen infoScreen = new InfoScreen();
                    infoScreen.Show();
                }
                catch (Exception)
                {
                    MessageBox.Show("Please correct code.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            },
            (pred) =>
            {
                if (PartCodeTextBoxText.Length == 13)
                    return true;
                return false;
            });

            PreviewScreenCommand = new RelayCommand((sender) =>
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            });

            MinimizeCommand = new RelayCommand((sender) =>
            {
                if (VerificationScreen.WindowState != WindowState.Minimized)
                    App.Current.Dispatcher.Invoke(() => { VerificationScreen.WindowState = WindowState.Minimized; });
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
                try
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        VerificationScreen.partCodeTextBox.TextChanged += PartCodeOneOnTextChanged;
                        VerificationScreen.Loaded += VerificationScreen_Loaded;
                        VerificationScreen.contextBar.MouseDown += ContextBar_MouseDown;
                    });
                }
                catch (Exception) { }
            });
            setData.Start();
        }

        private void ContextBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VerificationScreen.DragMove();
        }

        private void VerificationScreen_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            VerificationScreen.partCodeTextBox.Focus();
        }

        private void setDataMethod()
        {
            PartCodeLabelText = "- - -   - - -";
            PartCodeTextBoxText = string.Empty;
            PhoneNumber = LoginViewModel.PhoneNumber;

            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _milSecond++;
            if (_milSecond >= 60)
            {
                _milSecond = _milSecond - 60;
                _second++;
            }
            if (_second >= 60)
            {
                _second = _second - 60;
                _minute++;
            }
            if (_minute >= 60)
            {
                _minute = _minute - 60;
                _hour++;
            }

            string second = string.Empty, minute = string.Empty, hour = string.Empty;

            if (_second >= 0 && _second <= 9) { second = "0" + _second; }
            else second = _second.ToString();

            if (_minute >= 0 && _minute <= 9) { minute = "0" + _minute; }
            else { minute = _minute.ToString(); }

            if (_hour >= 0 && _hour <= 9) { hour = "0" + _hour; }
            else { hour = _hour.ToString(); }


            if (_hour == 0)
            {
                string state = _minute + "minute";

                if (state == _stopWord)
                {
                    Time = "";
                    App.Current.Dispatcher.Invoke(() => { VerificationScreen.timeButton.IsEnabled = true; });
                    _timer.Stop();
                    _milSecond = 0;
                    _second = 0;
                    _minute = 0;
                    _hour = 0;
                    return;
                }
                else
                    Time = $"{_minute}:{_second}";
            }
            else
            {
                string state = _hour + "hour";
                if (state == _stopWord)
                {
                    Time = "";
                    _timer.Stop();
                    _milSecond = 0;
                    _second = 0;
                    _minute = 0;
                    _hour = 0;
                    return;
                }
            }
        }
        private void PartCodeOneOnTextChanged(object sender, TextChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => { VerificationScreen.partCodeTextBox.TextChanged -= PartCodeOneOnTextChanged; });

            if (PartCodeTextBoxText.Length > 0 && PartCodeTextBoxText.Length <= 13)
            {
                if (PartCodeTextBoxText.Length > _firstState)
                {
                    if (!Regex.IsMatch(PartCodeTextBoxText[PartCodeTextBoxText.Length - 1].ToString(), "^[0-9]"))
                        PartCodeTextBoxText = PartCodeTextBoxText.Remove(PartCodeTextBoxText.Length - 1);
                    else
                    {
                        _symbolDigit++;
                        if (_symbolDigit == 3)
                            PartCodeTextBoxText += "   ";
                        else if (PartCodeTextBoxText.Length == 13)
                            _symbolDigit = _symbolDigit;
                        else
                            PartCodeTextBoxText += " ";
                    }

                    string tempData = string.Empty;

                    for (int i = 0; i < PartCodeTextBoxText.Length; i++)
                        tempData += ' ';

                    PartCodeLabelText = PartCodeLabelText.Remove(0, tempData.Length);
                    tempData += PartCodeLabelText;
                    PartCodeLabelText = tempData;
                }
                else if (PartCodeTextBoxText.Length < _firstState)
                {
                    string tempData = string.Empty;

                    _symbolDigit--;
                    if (_symbolDigit == 2)
                        PartCodeTextBoxText = PartCodeTextBoxText.Remove(PartCodeTextBoxText.Length - 3);
                    else if (PartCodeTextBoxText.Length == 12)
                        _symbolDigit = _symbolDigit;
                    else
                        PartCodeTextBoxText = PartCodeTextBoxText.Remove(PartCodeTextBoxText.Length - 1);

                    for (int i = 0; i < PartCodeTextBoxText.Length; i++)
                        tempData += " ";

                    string temp = "-";
                    if (_symbolDigit < 5)
                    {
                        for (int i = 0; i < (6 - _symbolDigit - 1); i++)
                        {
                            if (i == 2)
                                temp = "-   " + temp;
                            else
                                temp = "- " + temp;
                        }
                    }

                    tempData += temp;
                    PartCodeLabelText = tempData;
                }
                _firstState = PartCodeTextBoxText.Length;
            }
            else if (PartCodeTextBoxText.Length == 0)
            {
                _firstState = 0;
                _symbolDigit = 0;
                PartCodeLabelText = "- - -   - - -";
            }
            else
                PartCodeTextBoxText = PartCodeTextBoxText.Remove(PartCodeTextBoxText.Length - 1);

            App.Current.Dispatcher.Invoke(() =>
            {
                VerificationScreen.partCodeTextBox.TextChanged += PartCodeOneOnTextChanged;
                VerificationScreen.partCodeTextBox.SelectionStart = PartCodeTextBoxText.Length;
                VerificationScreen.partCodeTextBox.SelectionLength = 0;
            });
        }
    }
}
