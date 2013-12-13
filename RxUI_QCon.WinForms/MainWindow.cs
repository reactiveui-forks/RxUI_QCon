using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace RxUI_QCon.WinForms
{
    public partial class MainWindow : Form, IViewFor<MainWindowViewModel>
    {
        public MainWindow()
        {
            var resolver = (IMutableDependencyResolver) RxApp.DependencyResolver;
            resolver.Register(() => new WpfColorToWinformsColor(), typeof(IBindingTypeConverter));
            resolver.Register(() => new CommandToWinformsButton(), typeof(ICreatesCommandBinding));

            InitializeComponent();
            ViewModel = new MainWindowViewModel();

            this.Bind(ViewModel, x => x.Red, x => x.txtRed.Text, getTextChangedForTextBox(txtRed));
            this.Bind(ViewModel, x => x.Green, x => x.txtGreen.Text, getTextChangedForTextBox(txtGreen));
            this.Bind(ViewModel, x => x.Blue, x => x.txtBlue.Text, getTextChangedForTextBox(txtBlue));

            this.OneWayBind(ViewModel, x => x.FinalColor, x => x.pnlColor.BackColor);

            this.BindCommand(ViewModel, x => x.Ok, x => x.btnOk);
        }

        IObservable<Unit> getTextChangedForTextBox(TextBox textBox)
        {
            return Observable.FromEventPattern<EventHandler, EventArgs>(x => textBox.TextChanged += x, x => textBox.TextChanged -= x)
                .Select(_ => Unit.Default);
        }

        public MainWindowViewModel ViewModel { get; set; }

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (MainWindowViewModel)value; }
        }
    }

    public class WpfColorToWinformsColor : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type lhs, Type rhs)
        {
            if (lhs != typeof(System.Windows.Media.SolidColorBrush)) return 0;
            if (rhs != typeof(System.Drawing.Color)) return 0;

            return 10;
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            if (from == null) {
                result = null;
                return false;
            }

            var brush = (System.Windows.Media.SolidColorBrush)from;
            var src = brush.Color;
            result = System.Drawing.Color.FromArgb(src.A, src.R, src.G, src.B);
            return true;
        }
    }

    public class CommandToWinformsButton : ICreatesCommandBinding
    {
        public int GetAffinityForObject(Type type, bool hasEventTarget)
        {
            if (type != typeof(Button)) return 0;
            if (hasEventTarget) return 0;
            return 10;
        }

        public IDisposable BindCommandToObject<TEventArgs>(System.Windows.Input.ICommand command, object target, IObservable<object> commandParameter, string eventName)
        {
            throw new NotImplementedException();
        }

        public IDisposable BindCommandToObject(System.Windows.Input.ICommand command, object target, IObservable<object> commandParameter)
        {
            var button = target as Button;

            var clicked = Observable.FromEventPattern<EventHandler, EventArgs>(x => button.Click += x, x => button.Click -= x);
            var canExecute = Observable.FromEventPattern<EventHandler, EventArgs>(x => command.CanExecuteChanged += x, x => command.CanExecuteChanged -= x);

            return new CompositeDisposable(
                clicked.InvokeCommand(command),
                canExecute
                    .Select(_ => command.CanExecute(null))
                    .StartWith(command.CanExecute(null))
                    .Subscribe(x => button.Enabled = x));
        }
    }
}
