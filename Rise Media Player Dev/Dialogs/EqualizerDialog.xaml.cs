using Rise.App.ViewModels;
using Rise.Data.ViewModels;
using Rise.Effects;
using System.Collections.Generic;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Dialogs
{
    public sealed partial class EqualizerDialog : ContentDialog
    {
        private MediaPlaybackViewModel MPViewModel => App.MPViewModel;
        private SettingsViewModel SViewModel => App.SViewModel;

        private readonly List<EqualizerSliderViewModel> list;

        public EqualizerDialog()
        {
            InitializeComponent();

            list = new();
            for (int i = 0; i < 10; i++)
            {
                var vm = new EqualizerSliderViewModel
                {
                    Gain = SViewModel.EqualizerGain[i],
                    Index = i
                };
                list.Add(vm);
            }

            SlidersList.ItemsSource = list;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            float[] gains = new float[10];
            for (int i = 0; i < list.Count; i++)
                gains[i] = list[i].Gain;
            SViewModel.EqualizerGain = gains;

            if (MPViewModel.PlayerCreated)
                EqualizerEffect.Current.SetProperties(new PropertySet() { ["Gain"] = gains, ["Enabled"] = SViewModel.EqualizerEnabled });
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ToggleSwitch_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (MPViewModel.PlayerCreated)
                EqualizerEffect.Current.IsEnabled = SViewModel.EqualizerEnabled;
        }
    }
}
