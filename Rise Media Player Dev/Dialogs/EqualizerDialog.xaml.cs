using Rise.Models;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Dialogs
{
    public sealed partial class EqualizerDialog : ContentDialog
    {
        List<EqualizerSliderViewModel> list;

        public EqualizerDialog()
        {
            InitializeComponent();

            list = new List<EqualizerSliderViewModel>()
            {
                new()
                {
                    Gain = (float)App.SViewModel.Gain["0"],
                    Index = 0
                },
                new()
                {
                    Gain = (float)App.SViewModel.Gain["1"],
                    Index = 1
                },
                new()
                {
                    Gain = (float)App.SViewModel.Gain["2"],
                    Index = 2
                },
                new()
                {
                    Gain = (float)App.SViewModel.Gain["3"],
                    Index = 3
                },
                new()
                {
                    Gain = (float)App.SViewModel.Gain["4"],
                    Index = 4
                },
                new()
                {
                    Gain = (float)App.SViewModel.Gain["5"],
                    Index = 5
                },
                new()
                {
                    Gain = (float)App.SViewModel.Gain["6"],
                    Index = 6
                },
                new()
                {
                    Gain = (float)App.SViewModel.Gain["7"],
                    Index = 7
                },
                new()
                {
                    Gain = (float)App.SViewModel.Gain["8"],
                    Index = 8
                },
                new()
                {
                    Gain = (float)App.SViewModel.Gain["9"],
                    Index = 9
                }
            };

            SlidersList.ItemsSource = list;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ApplicationDataCompositeValue composite = new();
            
            for (int i = 0; i < list.Count; i++)
            {
                composite[i.ToString()] = list[i].Gain;
            }

            App.SViewModel.Gain = composite;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
