using Rise.Common.Extensions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Rise.App.Web
{
    public sealed partial class WhatsNew : Page
    {
        public WhatsNew()
        {
            InitializeComponent();
            TitleBar.SetTitleBarForCurrentView();
        }

        public static Task<bool> TryShowAsync()
            => ViewHelpers.OpenViewAsync<WhatsNew>(minSize: new(500, 500));
    }
}
