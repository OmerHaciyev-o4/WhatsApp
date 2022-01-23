using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WhatsAppClient.MVVM.Commands;
using WhatsAppClient.MVVM.Views;

namespace WhatsAppClient.MVVM.ViewModels
{
    class TakeImageViewModel : BaseViewModel
    {
        #region Private Variable
        private FilterInfoCollection _captureDevice;
        private VideoCaptureDevice _videoDevices;
        private Bitmap _source;
        #endregion

        #region References
        public TakeImageControl ImageControl { get; set; }
        public AppViewModel ViewModel { get; set; }
        #endregion


        #region Commands
        public ICommand PhotoCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        #endregion

        public TakeImageViewModel()
        {
            getAllCameraList();

            PhotoCommand = new RelayCommand((sender) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    ImageControl.PhotoButton.Visibility = Visibility.Hidden;
                    ImageControl.PhotoSendButton.Visibility = Visibility.Visible;
                    ImageControl.ClearButton.Visibility = Visibility.Visible;
                });
                Task.Run(() =>
                {
                    _videoDevices.Stop();
                });
            });

            ClearCommand = new RelayCommand((sender) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    ImageControl.PhotoButton.Visibility = Visibility.Visible;
                    ImageControl.PhotoSendButton.Visibility = Visibility.Hidden;
                    ImageControl.ClearButton.Visibility = Visibility.Hidden;
                });
                _videoDevices.Start();
            });

            SendCommand = new RelayCommand((sender) =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    ChatViewModel.IsSetPhoto = 1;
                    ChatViewModel.Source = _source;
                });
            });

            ExitCommand = new RelayCommand((sender) =>
            {
                ChatViewModel.IsSetPhoto = -1;
            });
        }

        private void getAllCameraList()
        {
            _captureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            try
            {
                _videoDevices = new VideoCaptureDevice(_captureDevice[0].MonikerString);
                _videoDevices.NewFrame += _videoDevices_NewFrame;
                _videoDevices.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void _videoDevices_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                _source = (Bitmap)eventArgs.Frame.Clone();
                ImageControl.Photo.Source = ImageSourceFromBitmap(_source);
            });
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
        public static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }
    }
}
