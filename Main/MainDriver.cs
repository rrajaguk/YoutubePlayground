/*
 * Copyright 2015 Google Inc. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YoutubePlayground
{
    /// <summary>
    /// YouTube Data API v3 sample: search by keyword.
    /// Relies on the Google APIs Client Library for .NET, v1.7.0 or higher.
    /// See https://code.google.com/p/google-api-dotnet-client/wiki/GettingStarted
    ///
    /// Set ApiKey to the API key value from the APIs & auth > Registered apps tab of
    ///   https://cloud.google.com/console
    /// Please ensure that you have enabled the YouTube Data API for your project.
    /// </summary>
    internal class MainDriver
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("YouTube Data API: Search");
            Console.WriteLine("========================");

            try
            {
                var main = new MainDriver();
                main.GetPlaylistID().Wait();
                Console.WriteLine("look for channel = [" + main.currentPlaylistID+"]");
                main.GetAllVideo().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }


        private string currentPlaylistID;
        private async Task GetPlaylistID()
        {
            String result = "";
            try
            {
                var youtubeService = await YoutubeServices.GetService();
                var searchPlaylist = youtubeService.Channels.List("snippet,contentDetails,statistics");
                searchPlaylist.Mine = true;

                var searchPlaylistResponse = await searchPlaylist.ExecuteAsync();

                foreach (var searchResult in searchPlaylistResponse.Items)
                {
                    result = (searchResult.ContentDetails.RelatedPlaylists.Uploads);
                }
            }catch (Exception ex)
            {
                throw new Exception("Failed to get the channel");
            }

            currentPlaylistID = result;
        }

        private async Task GetAllVideo()
        {
            var youtubeService = await YoutubeServices.GetService();
            var searchVideo = youtubeService.PlaylistItems.List("snippet,contentDetails");
            searchVideo.PlaylistId = currentPlaylistID;
            searchVideo.MaxResults = 50;
            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchVideo.ExecuteAsync();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            DateTime yesterday = DateTime.Now.AddDays(-1);
            foreach (var searchResult in searchListResponse.Items)
            {

                if (searchResult.Snippet.PublishedAt.Value.CompareTo(yesterday) > 0 && searchResult.Snippet.Title.Equals("Default Video Title"))
                {
                    Console.WriteLine("------------------------------------");
                    Console.WriteLine(searchResult.Snippet.Title);
                    Console.WriteLine(searchResult.Snippet.ResourceId.VideoId);
                    Console.WriteLine(searchResult.Snippet.PublishedAt);
                    Console.WriteLine("------------------------------------");
                }
            }
        }
    }
}
