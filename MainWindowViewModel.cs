using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
