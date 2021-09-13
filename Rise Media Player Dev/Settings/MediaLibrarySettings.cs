using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMP.App.Settings
{
    public class MediaLibrarySettings
    {
        public static int Deletion
        {
            get => SettingsManager.GetIntSetting("MediaLibrary", "Deletion", 0);
            set => SettingsManager.SetIntSetting("MediaLibrary", "Deletion", value);
        }

        public static bool MatchAlbumArt
        {
            get => SettingsManager.GetBoolSetting("MediaLibrary", "MatchAlbumArt", true);
            set => SettingsManager.SetBoolSetting("MediaLibrary", "MatchAlbumArt", value);
        }

        public static bool SeparateLocal
        {
            get => SettingsManager.GetBoolSetting("MediaLibrary", "SeparateLocal", true);
            set => SettingsManager.SetBoolSetting("MediaLibrary", "SeparateLocal", value);
        }

        public static bool DisableOnline
        {
            get => SettingsManager.GetBoolSetting("MediaLibrary", "DisableOnline", true);
            set => SettingsManager.SetBoolSetting("MediaLibrary", "DisableOnline", value);
        }

        public static bool DisableLocal
        {
            get => SettingsManager.GetBoolSetting("MediaLibrary", "DisableLocal", true);
            set => SettingsManager.SetBoolSetting("MediaLibrary", "DisableLocal", value);
        }

        #region Unused
        public bool MergeAM
        {
            get => SettingsManager.GetBoolSetting("MediaLibrary", "MergeAM", false);
            set => SettingsManager.SetBoolSetting("MediaLibrary", "MergeAM", value);
        }

        public bool MergeDZ
        {
            get => SettingsManager.GetBoolSetting("MediaLibrary", "MergeDZ", false);
            set => SettingsManager.SetBoolSetting("MediaLibrary", "MergeDZ", value);
        }

        public bool MergeSPT
        {
            get => SettingsManager.GetBoolSetting("MediaLibrary", "MergeSPT", false);
            set => SettingsManager.SetBoolSetting("MediaLibrary", "MergeSPT", value);
        }

        public bool MergeYTM
        {
            get => SettingsManager.GetBoolSetting("MediaLibrary", "MergeYTM", false);
            set => SettingsManager.SetBoolSetting("MediaLibrary", "MergeYTM", value);
        }
        #endregion
    }
}
