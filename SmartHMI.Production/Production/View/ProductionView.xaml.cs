// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductionView.xaml.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Production.ViewModel;
using KukaRoboter.Common.Attributes;
using System.Windows;

namespace Kuka.FlexDrill.SmartHMI.Production.Production.View
{
    /// <summary>Interaction logic for TextBoxTestView.xaml</summary>
    [ViewModelType(typeof(ProductionViewModel))]
    public partial class ProductionView
    {
        #region Constants and Fields

        private ProductionViewModel viewModel;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public ProductionView()
           : base(true)
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        #endregion Constructors and Destructor

        #region Interface

        public override void RequestClose()
        {
            ViewModel.ReleaseEvents();
            base.RequestClose();
        }

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

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            ViewModel.InitializePlugin();
        }

        #endregion Methods
    }
}