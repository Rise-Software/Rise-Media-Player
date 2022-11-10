﻿using Windows.ApplicationModel;

namespace Rise.Common.Constants
{
    public static class AppVersion
    {
        private static PackageVersion _version => Package.Current.Id.Version;

        public static string Version => $"{_version.Major}.{_version.Minor}.{_version.Build}.{_version.Revision}";

        public static string VersionName => "Alpha 3";
    }
}