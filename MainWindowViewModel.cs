using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Xaml;
using Newtonsoft.Json;

namespace RxUI_QCon
{
    public class MainWindowViewModel : ReactiveObject
    {
        int _Red;
        public int Red {
            get { return _Red; }
            set { this.RaiseAndSetIfChanged(value); }
        }

        int _Green;
        public int Green {
            get { return _Green; }
            set { this.RaiseAndSetIfChanged(value); }
        }

        int _Blue;
        public int Blue {
            get { return _Blue; }
            set { this.RaiseAndSetIfChanged(value); }
        }

        ObservableAsPropertyHelper<SolidColorBrush> _FinalColor;
        public SolidColorBrush FinalColor {
            get { return _FinalColor.Value; }
        }

        ObservableAsPropertyHelper<List<BitmapImage>> _Images;
        public List<BitmapImage> Images {
            get { return _Images.Value; }
        }

        public ReactiveCommand Ok { get; protected set; }

        public MainWindowViewModel()
        {
            var whenAnyColorChanges = this.WhenAny(x => x.Red, x => x.Green, x => x.Blue,
                    (r, g, b) => Tuple.Create(r.Value, g.Value, b.Value))
                .Select(intsToColor);

            whenAnyColorChanges
                .Where(x => x != null)
                .Select(x => new SolidColorBrush(x.Value))
                .ToProperty(this, x => x.FinalColor);

            Ok = new ReactiveCommand(whenAnyColorChanges.Select(x => x != null));

            this.WhenAny(x => x.FinalColor, x => x.Value)
                .Throttle(TimeSpan.FromSeconds(0.7), RxApp.DeferredScheduler)
                .Select(x => imagesForColor(x.Color))
                .Switch()
                .ObserveOn(RxApp.DeferredScheduler)
                .Select(imageListToImages)
                .ToProperty(this, x => x.Images);
        }

        Color? intsToColor(Tuple<int, int, int> colorsAsInts)
        {
            byte? r = inRange(colorsAsInts.Item1), g = inRange(colorsAsInts.Item2), b = inRange(colorsAsInts.Item3);

            if (r == null || g == null || b == null) return null;
            return Color.FromRgb(r.Value, g.Value, b.Value);
        }

        static byte? inRange(int value)
        {
            if (value < 0 || value > 255) {
                return null;
            }

            return (byte) value;
        }

        IObservable<ImageList> imagesForColor(Color sourceColor)
        {
            var queryParams = new[] {
                new { k = "method", v = "flickr_color_search" },
                new { k = "limit", v = "73" },
                new { k = "offset", v = "0" },
                new { k = "colors[0]", v = String.Format("{0:x2}{1:x2}{2:x2}", sourceColor.R, sourceColor.G, sourceColor.B) },
                new { k = "weights[0]", v = "1" },
            };

            var query = queryParams.Aggregate("",
                (acc, x) => String.Format("{0}&{1}={2}", acc, HttpUtility.UrlEncode(x.k), HttpUtility.UrlEncode(x.v)));

            query = query.Substring(1);

            var wc = new HttpClient();
            var url = "http://labs.tineye.com/rest/?" + query;
            wc.BaseAddress = new Uri(url);

            return wc.GetStringAsync("").ToObservable()
                .Select(JsonConvert.DeserializeObject<ImageList>);
        }

        List<BitmapImage> imageListToImages(ImageList imageList)
        {
            return imageList.result
                .Select(x => "http://img.tineye.com/flickr-images/?filepath=labs-flickr/" + x.filepath)
                .Select(x => {
                    var ret = new BitmapImage(new Uri(x));
                    return ret;
                }).ToList();
        }
    }
}