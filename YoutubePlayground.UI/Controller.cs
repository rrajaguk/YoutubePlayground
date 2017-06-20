using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubePlayground.UI
{
    public class Controller
    {
        public VideoFileWatcher VFW;
        private YoutubeUploader uploader;
        private Controller()
        {
            VFW = new VideoFileWatcher();
            uploader = new YoutubeUploader();

            VFW.addNewFileWatcher(uploader);
        }

        private static Controller SingletonController; 
        public static Controller GetController()
        {
           if (SingletonController == null)
            {
                SingletonController = new Controller();
            }
            return SingletonController;
        }

    }
}
