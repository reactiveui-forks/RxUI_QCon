using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxUI_QCon
{
    public class Image
    {
        public string filepath { get; set; }
        public int height { get; set; }
        public string id { get; set; }
        public string photosite { get; set; }
        public string score { get; set; }
        public int width { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }

    public class ImageList
    {
        public List<string> error { get; set; }
        public string method { get; set; }
        public List<Image> result { get; set; }
        public string status { get; set; }
    }
}
