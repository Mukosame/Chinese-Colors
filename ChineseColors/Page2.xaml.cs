using ChineseColors.Common;
using ChineseColors.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Graphics.Imaging;
using System.Text;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Media.Animation;//coloranimation
using Windows.Data.Json;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace ChineseColors
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page2 : Page
    {
        private NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private ApplicationDataContainer localSettings;
        int flag = 0;
        String appversion = GetAppVersion();

        public Page2()
        {
            this.InitializeComponent();
            InitRandom();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            localSettings = ApplicationData.Current.LocalSettings;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (!e.Handled && Frame.CurrentSourcePageType.FullName == "ChineseColors.Page2")
                Application.Current.Exit();
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            this.DefaultViewModel["Groups"] = sampleDataGroups;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
        }

        /// <summary>
        /// Shows the details of an item clicked on in the <see cref="ItemPage"/>
        /// </summary>
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter       
            Storyboard animation = new Storyboard();
            ColorAnimation changeColorAnimation = new ColorAnimation();
            changeColorAnimation.EnableDependentAnimation = true;
            changeColorAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));
            Storyboard.SetTarget(changeColorAnimation, Hub);
            PropertyPath p = new PropertyPath("(Hub.Background).(SolidColorBrush.Color)");
            Storyboard.SetTargetProperty(changeColorAnimation, p.Path);

            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            var itemColor = ((SampleDataItem)e.ClickedItem).Color;
            Title.Text = ((SampleDataItem)e.ClickedItem).Title;
            RGB.Text = ((SampleDataItem)e.ClickedItem).Subtitle;
            CMYK.Text = "CMYK:" + ((SampleDataItem)e.ClickedItem).Content;
            pinyin.Text = ((SampleDataItem)e.ClickedItem).UniqueId.ToUpper();
            HEX.Text = "HEX:" + ((SampleDataItem)e.ClickedItem).Color.ToUpper();

            changeColorAnimation.To = Color.FromArgb(255,
                Convert.ToByte(itemColor.Substring(1, 2), 16),
                Convert.ToByte(itemColor.Substring(3, 2), 16),
                Convert.ToByte(itemColor.Substring(5, 2), 16));
            //Hub.Background=Brush;
            animation.Children.Add(changeColorAnimation);
            animation.Begin();
            /*
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
             */
        }


        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        #region APPBARBUTTON
        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            string temp = String.Format("{0}.{0}.{0}.{0}版", version.Major, version.Minor, version.Build, version.Revision);
            return temp;
        }

        private void Spectrum(object sender, RoutedEventArgs e)
        {
            //设置绑定的数据集
            Frame.Navigate(typeof(HubPage));
            if (!Frame.Navigate(typeof(HubPage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }
        private async void bclick(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(
    new Uri(string.Format("ms-windows-store:reviewapp?appid=" + "f840285e-0a27-49a5-81d6-78edf83e82b9")));
        }

        private void cclick(object sender, RoutedEventArgs e)
        {
            //this.NavigationService.Navigate(new Uri("/Info_Page.xaml", UriKind.Relative));
            Frame.Navigate(typeof(BasicPage1), appversion);
        }

        private void ChangeForeground(object sender, RoutedEventArgs e)
        {
            if (flag == 0) flag = 1;
            else flag = 0;
            Title.Foreground = new SolidColorBrush(FontColor[flag]);
            RGB.Foreground = new SolidColorBrush(FontColor[flag]);
            CMYK.Foreground = new SolidColorBrush(FontColor[flag]);
            HEX.Foreground = new SolidColorBrush(FontColor[flag]);
            pinyin.Foreground = new SolidColorBrush(FontColor[flag]);
        }

        static Color[] FontColor = { Colors.White, Colors.LightGray};

        private async void SaveForeground(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap();
            //source
            Hubsection1.Visibility = Visibility.Collapsed;
            await bitmap.RenderAsync(Hub);
            Hubsection1.Visibility = Visibility.Visible;
            var pixelBuffer = await bitmap.GetPixelsAsync();

            string filename = Title.Text+".png";
            StorageFolder storageFolder = KnownFolders.PicturesLibrary;
            var saveFile = await storageFolder.CreateFileAsync(filename,
                CreationCollisionOption.ReplaceExisting);

            using (var fileStream = await saveFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);

                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)bitmap.PixelWidth,
                    (uint)bitmap.PixelHeight,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    DisplayInformation.GetForCurrentView().LogicalDpi,
                    pixelBuffer.ToArray());

                await encoder.FlushAsync();
            }
            if (localSettings.Values.ContainsKey("ok"))
            { ;}
            else
            {
                var messageDialog = new MessageDialog("当前颜色已保存，您可以进入相册中查看，并将其设为锁屏或背景", "保存成功");
                messageDialog.Commands.Add(new UICommand("知道了"));
                messageDialog.Commands.Add(new UICommand(
        "不再提醒",
        new UICommandInvokedHandler(this.CommandInvokedHandler)));
                
                await messageDialog.ShowAsync();
            }
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            localSettings.Values["ok"] = "ok";
        }

#endregion
        #region InitFeature
        //not finished
        private void InitRandom()
        {
            var Brush = new SolidColorBrush();

            Title.Text = "百草霜";
            RGB.Text = "RGB:48,48,48";
            CMYK.Text = "CMYK:0,0,0,81";
            pinyin.Text = "BAICAOSHUANG";
                HEX.Text = "HEX:#303030";
                Brush.Color = Color.FromArgb(255, 48, 48, 48);
                Hub.Background = Brush;
        }
        #endregion

    }
}
