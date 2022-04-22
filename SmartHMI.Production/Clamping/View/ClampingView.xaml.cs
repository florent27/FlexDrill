// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClampingView.xaml.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Clamping.ViewModel;
using KukaRoboter.Common.Attributes;
using System.Windows;

namespace Kuka.FlexDrill.SmartHMI.Production.Clamping.View
{
    /// <inheritdoc cref="ClampingView" />
    [ViewModelType(typeof(ClampingViewModel))]
    public partial class ClampingView
    {
        #region Constants and Fields

        private ClampingViewModel viewModel;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public ClampingView() : base(true)
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        #endregion Constructors and Destructor

        #region Interface

        public ClampingViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = DataContext as ClampingViewModel;
                }

                return viewModel;
            }
        }

        public override void RequestClose()
        {
            if (ViewModel.CanClose())
            {
                base.RequestClose();
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