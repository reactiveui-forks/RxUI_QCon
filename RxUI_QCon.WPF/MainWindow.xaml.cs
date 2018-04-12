using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using Splat;

namespace RxUI_QCon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainWindowViewModel>
    {
        public MainWindow()
        {
            ViewModel = new MainWindowViewModel();
            InitializeComponent();

            this.Bind(ViewModel, x => x.Red, x => x.Red.Text);
            this.Bind(ViewModel, x => x.Green, x => x.Green.Text);
            this.Bind(ViewModel, x => x.Blue, x => x.Blue.Text);

            this.WhenAnyValue(x => x.ViewModel.FinalColor)
                .Select(x => x.ToNativeBrush())
                .BindTo(this, x => x.FinalColor.Background);

            this.BindCommand(ViewModel, x => x.Ok, x => x.Ok);

            this.WhenAnyObservable(x => x.ViewModel.Ok)
                .Subscribe(_ => MessageBox.Show("It worked!"));

            this.WhenAnyValue(x => x.ViewModel.Images)
                .Where(x => x != null)
                .Select(x => x.Where(y => y != null).Select(y => y.ToNative()).ToList())
                .BindTo(this, x => x.Images.ItemsSource);

            this.OneWayBind(ViewModel, x => x.IsBusy, x => x.IsBusy.Visibility, () => false, BooleanToVisibilityHint.Inverse);
        }
        
        public MainWindowViewModel ViewModel {
            get { return (MainWindowViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainWindowViewModel), typeof(MainWindow), new PropertyMetadata(null));

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (MainWindowViewModel)value; }
        }
    }
}
