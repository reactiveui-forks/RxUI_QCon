using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Web;
using ReactiveUI;
using Newtonsoft.Json;
using Splat;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Drawing;

namespace RxUI_QCon
{
    public class MainWindowViewModel : ReactiveObject
    {
        int _Red;
        public int Red {
            get { return _Red; }
            set { this.RaiseAndSetIfChanged(ref _Red, value); }
        }

        int _Green;
        public int Green {
            get { return _Green; }
            set { this.RaiseAndSetIfChanged(ref _Green, value); }
        }

        int _Blue;
        public int Blue {
            get { return _Blue; }
            set { this.RaiseAndSetIfChanged(ref _Blue, value); }
        }

        ObservableAsPropertyHelper<Color> _FinalColor;
        public Color FinalColor {
            get { return _FinalColor.Value; }
        }

        bool _IsBusy;
        public bool IsBusy {
            get { return _IsBusy; }
            set { this.RaiseAndSetIfChanged(ref _IsBusy, value); }
        }

#if MONO
        ObservableAsPropertyHelper<IList<NSImage>> _Images;
        public IList<NSImage> Images {
            get { return _Images.Value; }
        }
#else
        ObservableAsPropertyHelper<IList<IBitmap>> _Images;
        public IList<IBitmap> Images {
            get { return _Images.Value; }
        }
#endif

        public ReactiveCommand<Object> Ok { get; protected set; }

        public MainWindowViewModel()
        {
            var whenAnyColorChanges = this.WhenAny(x => x.Red, x => x.Green, x => x.Blue,
                    (r, g, b) => Tuple.Create(r.Value, g.Value, b.Value))
                .Select(intsToColor);

            _FinalColor = whenAnyColorChanges
                .Where(x => x != null)
                .Select(x => x.Value)
                .ToProperty(this, x => x.FinalColor);

            Ok = ReactiveCommand.Create(whenAnyColorChanges.Select(x => x != null));

            _Images = this.WhenAny(x => x.FinalColor, x => x.Value)
                .Throttle(TimeSpan.FromSeconds(0.7), RxApp.MainThreadScheduler)
                .Do(_ => IsBusy = true)
                .Select(x => imagesForColor(x))
                .Switch()
                .SelectMany(imageListToImages)
                .Do(_ => IsBusy = false)
                .ToProperty(this, x => x.Images);

            _Images.ThrownExceptions.Subscribe(ex => this.Log().WarnException("Can't load images", ex));
        }

        Color? intsToColor(Tuple<int, int, int> colorsAsInts)
        {
            byte? r = inRange(colorsAsInts.Item1), g = inRange(colorsAsInts.Item2), b = inRange(colorsAsInts.Item3);

            if (r == null || g == null || b == null) return null;

            return Color.FromArgb(r.Value, g.Value, b.Value);
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

        IObservable<IList<IBitmap>> imageListToImages(ImageList imageList)
        {
            return imageList.result.ToObservable(RxApp.MainThreadScheduler)
                .Select(x => "http://img.tineye.com/flickr-images/?filepath=labs-flickr/" + x.filepath)
                .SelectMany(async x => {
                    var wc = new WebClient();
                    var bytes = await wc.DownloadDataTaskAsync(x);
                    return await BitmapLoader.Current.Load(new MemoryStream(bytes), null, null);
                }).ToList();
        }
    }
}