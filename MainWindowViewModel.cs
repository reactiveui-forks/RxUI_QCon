using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ReactiveUI;
using ReactiveUI.Xaml;

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

        public ReactiveCommand Ok { get; protected set; }

        public MainWindowViewModel()
        {
            this.WhenAny(x => x.Red, x => x.Green, x => x.Blue,
                    (r, g, b) => new {Red = Int32.Parse(r.Value), Green = Int32.Parse(g.Value), Blue = Int32.Parse(b.Value)})
                .Where(x => inColorRange(x.Red) && inColorRange(x.Green) && inColorRange(x.Blue))
                .Select(x => new SolidColorBrush(Color.FromRgb((byte)x.Red, (byte)x.Green, (byte)x.Blue)))
                .ToProperty(this, x => x.FinalColor);
        }

        bool inColorRange(int colorValue)
        {
            return (colorValue >= 0 && colorValue <= 255);
        }
    }
}
