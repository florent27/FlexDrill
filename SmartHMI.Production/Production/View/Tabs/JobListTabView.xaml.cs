using Kuka.FlexDrill.SmartHMI.Production.Production.ViewModel;
using System.Windows;

namespace Kuka.FlexDrill.SmartHMI.Production.Production.View.Tabs
{
    /// <summary>
    /// Interaction logic for JobListTabView.xaml
    /// </summary>
    public partial class JobListTabView
    {
        #region Constants and Fields

        private ProductionViewModel viewModel;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public JobListTabView()
        {
            InitializeComponent();

            Root.SizeChanged += WindowSizeChanged;
        }

        ~JobListTabView()
        {
            Root.SizeChanged -= WindowSizeChanged;
        }

        #endregion Constructors and Destructor

        #region Interface

        /// <summary>Gets the view model.</summary>
        public ProductionViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = DataContext as ProductionViewModel;
                }

                return viewModel;
            }
        }

        #endregion Interface

        #region Methods

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            JobListView.Height = Root.ActualHeight - topPanel.ActualHeight - bottomPanel.ActualHeight;
        }

        #endregion Methods
    }
}