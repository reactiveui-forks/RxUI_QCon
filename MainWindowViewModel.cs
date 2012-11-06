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
            var whenAnyColorChanges = this.WhenAny(x => x.Red, x => x.Green, x => x.Blue,
                    (r, g, b) => Tuple.Create(r.Value, g.Value, b.Value))
                .Select(stringsToColor);

            whenAnyColorChanges
                .Where(x => x != null)
                .Select(x => new SolidColorBrush(x.Value))
                .ToProperty(this, x => x.FinalColor);

            Ok = new ReactiveCommand(whenAnyColorChanges.Select(x => x != null));
        }

        Color? stringsToColor(Tuple<string, string, string> colorsAsStrings)
        {
            byte r, g, b;
            if (!byte.TryParse(colorsAsStrings.Item1, out r) || !byte.TryParse(colorsAsStrings.Item2, out g) || !byte.TryParse(colorsAsStrings.Item3, out b)) {
                return null;
            }

            return Color.FromRgb(r, g, b);
        }
    }
}
