using Google.Apis.Upload;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubePlayground.UI
{
    public class YoutubeUploader : INewFileEvent
    {
        private HashSet<String> videoToUpload;

        public YoutubeUploader()
        {
            videoToUpload = new HashSet<string>();
        }
        
        public void NewFileAdded(string path)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            if (!videoToUpload.Contains(fileName))
            {
                videoToUpload.Add(fileName);
                Upload(path);
            }
        }

        public async void Upload(string path)
        {
            var youtubeService = await YoutubeServices.GetService();
            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = Path.GetFileNameWithoutExtension(path);
            video.Snippet.Description = "Default Video Description";
            video.Snippet.Tags = new string[] { "tag1", "tag2" };
            video.Snippet.CategoryId = "22"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "private"; // or "private" or "public"
            var filePath = path;

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }

        }
        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            videoToUpload.Remove(video.Snippet.Title);
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
        }
    }
}
