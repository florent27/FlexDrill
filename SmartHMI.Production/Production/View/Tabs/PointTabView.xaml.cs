// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointTabView.xaml.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.Process.CustomClasses;
using Kuka.FlexDrill.SmartHMI.Production.Production.ViewModel;
using System.Windows;

namespace Kuka.FlexDrill.SmartHMI.Production.Production.View.Tabs
{
    /// <summary>Interaction logic for TextBoxTestView.xaml</summary>
    public partial class PointTabView
    {
        private ProductionViewModel viewModel;

        #region Constructors and Destructor

        public PointTabView()

        {
            InitializeComponent();

            Root.SizeChanged += WindowSizeChanged;

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedPointChangedEvent += SelectPointChanged;
        }

        ~PointTabView()
        {
            Root.SizeChanged -= WindowSizeChanged;
        }

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

        private void SelectPointChanged(RobotPoint RobotPoint)
        {
            if (RobotPoint != null)
            {
                WorkingPointListView.ScrollIntoView(RobotPoint);
            }
        }

        #endregion Constructors and Destructor

        #region Methods

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            WorkingPointListView.Height = Root.ActualHeight - TopPanel.ActualHeight - BottomPanel.ActualHeight;
        }

        #endregion Methods
    }
}