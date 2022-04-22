using Kuka.FlexDrill.SmartHMI.Production.TcpCalibration.ViewModel;
using KukaRoboter.Common.Attributes;
using System.Windows;

namespace Kuka.FlexDrill.SmartHMI.Production.TcpCalibration.View
{
    /// <summary>Interaction logic for TextBoxTestView.xaml</summary>
    [ViewModelType(typeof(TcpCalibrationViewModel))]
    public partial class TcpCalibrationView
    {
        #region Constants and Fields

        private TcpCalibrationViewModel viewModel;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public TcpCalibrationView()
           : base(true)
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        #endregion Constructors and Destructor

        #region Interface

        /// <summary>Gets the view model.</summary>
        public TcpCalibrationViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = DataContext as TcpCalibrationViewModel;
                }

                return viewModel;
            }
        }

        public override void RequestClose()
        {
            ViewModel.ReleaseEvents();
            base.RequestClose();
        }

        #endregion Interface

        #region Methods

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.InitializePlugin();
        }

        #endregion Methods
    }
}