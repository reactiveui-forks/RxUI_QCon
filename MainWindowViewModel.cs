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
        string _Red = "0";
        public string Red {
            get { return _Red; }
            set { this.RaiseAndSetIfChanged(x => x.Red, value); }
        }

        string _Green = "0";
        public string Green {
            get { return _Green; }
            set { this.RaiseAndSetIfChanged(x => x.Green, value); }
        }

        string _Blue = "0";
        public string Blue {
            get { return _Blue; }
            set { this.RaiseAndSetIfChanged(x => x.Blue, value); }
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
                .Select(stringsToColor);

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

        Color? stringsToColor(Tuple<string, string, string> colorsAsStrings)
        {
            byte r, g, b;
            if (!byte.TryParse(colorsAsStrings.Item1, out r) || !byte.TryParse(colorsAsStrings.Item2, out g) || !byte.TryParse(colorsAsStrings.Item3, out b)) {
                return null;
            }

            return Color.FromRgb(r, g, b);
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