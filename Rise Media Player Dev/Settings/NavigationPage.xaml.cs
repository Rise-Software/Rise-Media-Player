using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace RMP.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NavigationPage : Page
    {
        private List<string> Show { get; set; }
        private Windows.ApplicationModel.Resources.ResourceLoader resourceLoader =
            Windows.ApplicationModel.Resources.
                ResourceLoader.GetForCurrentView("Navigation");

        public NavigationPage()
        {
            this.InitializeComponent();
            Show = new List<string>
            {
                resourceLoader.GetString("NoIcons"),
                resourceLoader.GetString("OnlyIcons"),
                resourceLoader.GetString("Everything")
            };
        }

        #region Checkboxes
        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void SelectAll_Indeterminate(object sender, RoutedEventArgs e)
        {

        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Option_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        #endregion
    }
}
