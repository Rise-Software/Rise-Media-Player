using Rise.App.ViewModels;
using Rise.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Rise.Common.Extensions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using System.IO;
using System.Threading;
using Rise.Data.ViewModels;
using Rise.Common.Enums;
using System.Linq;

namespace Rise.App.ChangeTrackers
{
    public sealed class VideosTracker
    {
        /// <summary>
        /// Gets the app-wide MViewModel instance.
        /// </summary>
        private static MainViewModel MViewModel => App.MViewModel;

        /// <summary>
        /// Checks for duplicates in the video folders.
        /// </summary>
        public static async Task CheckDuplicatesAsync(CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                return;

            List<VideoViewModel> duplicates = new();

            // Check for duplicates and remove if any duplicate is found.
            for (int i = 0; i < MViewModel.Videos.Count; i++)
            {
                if (token.IsCancellationRequested)
                    return;

                for (int j = i + 1; j < MViewModel.Videos.Count; j++)
                {
                    if (token.IsCancellationRequested)
                        return;

                    if (MViewModel.Videos[i].Location == MViewModel.Videos[j].Location)
                    {
                        duplicates.Add(MViewModel.Videos[j]);
                    }
                }
            }

            foreach (VideoViewModel video in duplicates)
            {
                if (token.IsCancellationRequested)
                    return;

                await video.DeleteAsync(true);
            }
        }

        public static async Task HandleLibraryChangesAsync(bool queue = false)
        {
            await using var changes = await App.VideoLibrary.GetLibraryChangesAsync();

            if (changes.Status != StorageLibraryChangeStatus.HasChange)
                return;

            foreach (var addedItem in changes.AddedItems)
            {
                _ = await MViewModel.SaveVideoModelAsync(addedItem, queue);
            }

            foreach (var removedItemPath in changes.RemovedItems)
            {
                if (string.IsNullOrEmpty(removedItemPath))
                    continue;

                var video = App.MViewModel.Videos.FirstOrDefault(v => v.Location.Equals(removedItemPath, StringComparison.OrdinalIgnoreCase));

                if (video == null)
                    continue;

                await video.DeleteAsync(queue);
            }
        }
    }
}
