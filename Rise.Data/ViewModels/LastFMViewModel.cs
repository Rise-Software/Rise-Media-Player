using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rise.Data.ViewModels
{
    public class LastFMViewModel : ViewModel
    {
        /// <summary>
        /// Session key used for the LastFM API.
        /// </summary>
        private string _sessionKey;

        private string _username;
        /// <summary>
        /// Username for the currently logged in user.
        /// </summary>
        public string Username
        {
            get => _username;
            private set => Set(ref _username, value);
        }
    }
}
