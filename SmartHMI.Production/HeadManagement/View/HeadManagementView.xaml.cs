// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeadManagementView.xaml.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.HeadManagement.ViewModel;
using KukaRoboter.Common.Attributes;
using System.Windows;

namespace Kuka.FlexDrill.SmartHMI.Production.HeadManagement.View
{
    /// <inheritdoc cref="HeadManagementView" />
    [ViewModelType(typeof(HeadManagementViewModel))]
    public partial class HeadManagementView
    {
        #region Constants and Fields

        private HeadManagementViewModel viewModel;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public HeadManagementView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        #endregion Constructors and Destructor

        #region Interface

        /// <inheritdoc />
        public override void RequestClose()
        {
            ViewModel.ReleaseEvents();
            base.RequestClose();
        }

        public HeadManagementViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = DataContext as HeadManagementViewModel;
                }

                return viewModel;
            }
        }

        #endregion Interface

        #region Methods

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.InitializePlugin();
        }

        #endregion Methods
    }
}