using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WhatsAppClient.MVVM.Commands;
using WhatsAppClient.MVVM.Views;

namespace WhatsAppClient.MVVM.ViewModels
{
    class PolicyViewModel
    {
        #region References
        public PolicyScreen PolicyScreen { get; set; }
        #endregion

        #region Commands
        public ICommand ContinueCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        #endregion

        public PolicyViewModel()
        {
            ContinueCommand = new RelayCommand((sender) =>
            {
                App.Current.Dispatcher.Invoke(() => { PolicyScreen.Hide(); });

                if (!File.Exists("state.log"))
                    File.Create("state.log");

                App.Current.Dispatcher.Invoke(() =>
                {
                    File.WriteAllText("state.log", "\"policy\":\"true\"");
                });


                LoginScreen loginScreen = new LoginScreen() { };
                loginScreen.Show();
            });

            MinimizeCommand = new RelayCommand((sender) =>
            {
                if (PolicyScreen.WindowState != WindowState.Minimized)
                    App.Current.Dispatcher.Invoke(() => { PolicyScreen.WindowState = WindowState.Minimized; });
            });

            CloseCommand = new RelayCommand((sender) =>
            {
                App.Current.Shutdown();
            });

            Thread setEvent = new Thread(() =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    PolicyScreen.contextBar.MouseDown += ContextBar_MouseDown;
                    PolicyScreen.closeButton.MouseMove += CloseButton_MouseMove;
                    PolicyScreen.closeButton.MouseLeave += CloseButton_MouseLeave;
                });
            });
            setEvent.Start();
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => { PolicyScreen.closeButton.Background = new SolidColorBrush(Colors.Transparent); });
        }

        private void CloseButton_MouseMove(object sender, MouseEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => { PolicyScreen.closeButton.Background = new SolidColorBrush(Colors.Red); });
        }

        private void ContextBar_MouseDown(object sender, MouseEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => { PolicyScreen.DragMove(); });
        }
    }
}
