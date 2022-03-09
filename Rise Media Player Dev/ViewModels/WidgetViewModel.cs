using Rise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.App.ViewModels
{
    public class WidgetViewModel : ViewModel<Widget>
    {
        /// <summary>
        /// Gets or sets the widget title.
        /// </summary>
        public string Title
        {
            get => Model.Title;
            set
            {
                if (value != Model.Title)
                {
                    Model.Title = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the widget icon, which is a Unicode character.
        /// </summary>
        public string Icon
        {
            get => Model.Icon;
            set
            {
                if (value != Model.Icon)
                {
                    Model.Icon = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the widget tag for the page.
        /// </summary>
        public string PageTag
        {
            get => Model.PageTag;
            set
            {
                if (value != Model.PageTag)
                {
                    Model.PageTag = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                App.MViewModel.Widgets.Add(this);
            }
            finally
            {
                await App.WBackendController.UpsertAsync(this);
            }
        }

        public async Task RemoveAsync()
        {
            try
            {
                App.MViewModel.Widgets.Remove(this);
            } finally
            {
                await App.WBackendController.DeleteAsync(this);
            }
        }
    }
}
