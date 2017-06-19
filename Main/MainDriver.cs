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
                bool isExit = false;
                while (!isExit)
                {
                    Console.Write("Video to delete = ");
                    char inputChar = Console.ReadKey().KeyChar;
                    if (inputChar >= '0' && inputChar <= '9' )
                    {
                        main.RemoveVideo(main.videos[(inputChar - '1')]).Wait();                        
                    }else
                    {
                        isExit = true;
                    }
                }
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
            
        }


        private string currentPlaylistID;
        private List<String> videos = new List<String>();
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
                throw new Exception("Failed to get the channel \n ex = "+ex.Message);
            }

            currentPlaylistID = result;
        }

        private async Task RemoveVideo(string videoID)
        {
            try
            {
                var youtubeService = await YoutubeServices.GetService();
                var deleteVideo = youtubeService.Videos.Delete(videoID);
                // call the delete video
                var deleteResponse = await deleteVideo.ExecuteAsync();

                Console.WriteLine(deleteResponse);

            }catch (Exception ex)
            {
                throw new Exception("Failed to delete video \n ex = " + ex.Message);
            }
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
                    videos.Add(searchResult.Snippet.ResourceId.VideoId);
                    Console.WriteLine("------------------------------------");
                }
            }
        }
    }
}
