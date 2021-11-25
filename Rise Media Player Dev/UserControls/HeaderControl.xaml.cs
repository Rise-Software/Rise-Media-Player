using Rise.App.Common;
using Rise.App.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Rise.App.UserControls
{
    public sealed partial class HeaderControl : UserControl
    {
        private static readonly DependencyProperty HeaderModelProperty =
            DependencyProperty.Register("HeaderModel", typeof(object), typeof(HeaderControl), null);

        public object HeaderModel
        {
            get => GetValue(HeaderModelProperty);
            set
            {
                SetValue(HeaderModelProperty, value);
                Type type = value.GetType();

                if (type == typeof(AlbumViewModel))
                {
                    HeaderTemplateSelector.Index = 0;
                }
                else if (type == typeof(ArtistViewModel))
                {
                    HeaderTemplateSelector.Index = 1;
                }
                else if (type == typeof(GenreViewModel))
                {
                    HeaderTemplateSelector.Index = 2;
                }
            }
        }

        public HeaderControl()
        {
            InitializeComponent();
        }

        private static readonly DependencyProperty IsItemClickedProperty =
            DependencyProperty.Register("IsItemClicked", typeof(bool), typeof(HeaderControl), null);
        public bool IsItemClicked
        {
            get => (bool)GetValue(IsItemClickedProperty);
            set => SetValue(IsItemClickedProperty, value);
        }

        private static readonly DependencyProperty HyperlinkClickProperty =
            DependencyProperty.Register("HyperlinkClick",
                typeof(Action<Hyperlink, HyperlinkClickEventArgs>),
                typeof(HeaderControl), null);
        public Action<Hyperlink, HyperlinkClickEventArgs> HyperlinkClick
        {
            get => (Action<Hyperlink, HyperlinkClickEventArgs>)GetValue(HyperlinkClickProperty);
            set => SetValue(HyperlinkClickProperty, value);
        }

        private static readonly DependencyProperty PlayCommandProperty =
            DependencyProperty.Register("PlayCommand", typeof(RelayCommand), typeof(HeaderControl), null);
        public RelayCommand PlayCommand
        {
            get => (RelayCommand)GetValue(PlayCommandProperty);
            set => SetValue(PlayCommandProperty, value);
        }

        private static readonly DependencyProperty ShuffleCommandProperty =
            DependencyProperty.Register("ShuffleCommand", typeof(RelayCommand), typeof(HeaderControl), null);
        public RelayCommand ShuffleCommand
        {
            get => (RelayCommand)GetValue(ShuffleCommandProperty);
            set => SetValue(ShuffleCommandProperty, value);
        }

        private static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(RelayCommand), typeof(HeaderControl), null);
        public RelayCommand EditCommand
        {
            get => (RelayCommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        private static readonly DependencyProperty ViewCommandProperty =
            DependencyProperty.Register("ViewCommand", typeof(RelayCommand), typeof(HeaderControl), null);
        public RelayCommand ViewCommand
        {
            get => (RelayCommand)GetValue(ViewCommandProperty);
            set => SetValue(ViewCommandProperty, value);
        }

        private static readonly DependencyProperty SortCommandProperty =
            DependencyProperty.Register("SortCommand", typeof(RelayCommand), typeof(HeaderControl), null);
        public RelayCommand SortCommand
        {
            get => (RelayCommand)GetValue(SortCommandProperty);
            set => SetValue(SortCommandProperty, value);
        }

        private void Hyperlink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
            => HyperlinkClick(sender, args);
    }

    public class HeaderTemplateSelector : DataTemplateSelector
    {
        public static int Index { get; set; }

        public DataTemplate AlbumTemplate { get; set; }
        public DataTemplate ArtistTemplate { get; set; }
        public DataTemplate GenreTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (Index)
            {
                case 0:
                    return AlbumTemplate;
                case 1:
                    return ArtistTemplate;
                default:
                    return GenreTemplate;
            }
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (Index)
            {
                case 0:
                    return AlbumTemplate;
                case 1:
                    return ArtistTemplate;
                default:
                    return GenreTemplate;
            }
        }
    }
}
