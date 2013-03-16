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
using ReactiveUI.Xaml;

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

            this.Bind(ViewModel, x => x.Red);
            this.Bind(ViewModel, x => x.Green);
            this.Bind(ViewModel, x => x.Blue);

            this.OneWayBind(ViewModel, x => x.FinalColor, x => x.FinalColor.Background);
            this.BindCommand(ViewModel, x => x.Ok);

            this.WhenAny(x => x.ViewModel.Ok, x => x.Value).Merge()
                .Subscribe(_ => MessageBox.Show("It worked!"));

            this.OneWayBind(ViewModel, x => x.Images, x => x.Images.ItemsSource);
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
