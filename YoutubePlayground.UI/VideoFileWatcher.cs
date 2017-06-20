using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubePlayground.UI
{
    public class VideoFileWatcher
    {
        FileSystemWatcher watcher;
        List<INewFileEvent> newFileList;
        public VideoFileWatcher()
        {
            watcher = new FileSystemWatcher();
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(OnChanged);


            newFileList = new List<INewFileEvent>();
        }

        public void addNewFileWatcher(INewFileEvent fileWatcher)
        {
            newFileList.Add(fileWatcher);
        }

        public void SetPath(String path)
        {
            watcher.Path = path;
        }

        public void Pause()
        {
            watcher.EnableRaisingEvents = false;
        }
        public void Start()
        {
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            foreach(INewFileEvent member in newFileList)
            {
                member.NewFileAdded(e.FullPath);
            }
        }
    }
}
