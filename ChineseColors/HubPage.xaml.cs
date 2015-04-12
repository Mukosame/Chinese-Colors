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
using Windows.Storage;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace ChineseColors
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        String appversion = GetAppVersion();
        int flag=0;

        public HubPage()
        {
            this.InitializeComponent();
            InitRandom();
        
            // Hub is only supported in Portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
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
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
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
            // TODO: Save the unique state of the page here.
        }

        /// <summary>
        /// Shows the details of a clicked group in the <see cref="SectionPage"/>.
        /// </summary>
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
           /*
            if (!Frame.Navigate(typeof(SectionPage), groupId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
            * */
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
            CMYK.Text = ((SampleDataItem)e.ClickedItem).Content;
            des.Text ='“' + ((SampleDataItem)e.ClickedItem).Description + '”';

            changeColorAnimation.To = Color.FromArgb(Convert.ToByte(itemColor.Substring(1, 2), 16),
                Convert.ToByte(itemColor.Substring(3, 2), 16),
                Convert.ToByte(itemColor.Substring(5, 2), 16),
                Convert.ToByte(itemColor.Substring(7, 2), 16));
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
        /// <param name="e">Event data that describes how this page was reached.</param>
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
        //using Windows.ApplicationModel;
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            string temp = String.Format("{0}.{0}.{0}.{0}版", version.Major, version.Minor, version.Build, version.Revision);
            return temp;
        }

        private void aclick(object sender, RoutedEventArgs e)
        {
            //设置壁纸

        }
        private void cclick(object sender, RoutedEventArgs e)
        {
            //this.NavigationService.Navigate(new Uri("/Info_Page.xaml", UriKind.Relative));
            Frame.Navigate(typeof(BasicPage1), appversion);
        }

        private async void bclick(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(
    new Uri(string.Format("ms-windows-store:reviewapp?appid=" + "f840285e-0a27-49a5-81d6-78edf83e82b9")));
        }

        private void ChangeForeground(object sender, RoutedEventArgs e)
        {
            if (flag == 0) flag = 1;
            else flag = 0;
            Title.Foreground = new SolidColorBrush(FontColor[flag]);
            RGB.Foreground = new SolidColorBrush(FontColor[flag]);
            CMYK.Foreground = new SolidColorBrush(FontColor[flag]);
            des.Foreground = new SolidColorBrush(FontColor[flag]);

        }

        static Color[] FontColor = { Colors.White, Colors.LightGray};

#endregion
        #region InitFeature
        //not finished
        private async void InitRandom()
        {
            Random r = new Random();
            int randomObjectIndext = r.Next(156 - 0) + 0;
            var res = await SampleDataSource.GetGroupAsync("Group-1");
            var trends = res.Items[randomObjectIndext];
            //int Size = trends.Count;

           var item = trends;

            Title.Text = item.Title;
            RGB.Text = item.Subtitle;
            CMYK.Text = item.Content;
            des.Text = '“'+ item.Description + '”';
            var itemColor = item.Color;
            var Brush = new SolidColorBrush();
            Brush.Color = Color.FromArgb(Convert.ToByte(itemColor.Substring(1, 2), 16),
                Convert.ToByte(itemColor.Substring(3, 2), 16),
                Convert.ToByte(itemColor.Substring(5, 2), 16),
                Convert.ToByte(itemColor.Substring(7, 2), 16));
            Hub.Background = Brush;
        }
        #endregion
    }
}
