using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubePlayground.UI
{
    public interface INewFileEvent
    {
        void NewFileAdded(String path);
    }
}
