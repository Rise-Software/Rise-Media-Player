using Windows.Networking.Connectivity;

namespace Rise.Common.Helpers
{
    public static class WebHelpers
    {
        /// <summary>
        /// Checks whether internet access is available.
        /// </summary>
        /// <returns>true if access is available, false otherwise.</returns>
        /// <remarks>This method only checks for the available network resources.</remarks>
        public static bool IsInternetAccessAvailable()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return connectionProfile != null && connectionProfile.
                GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
        }
    }
}
