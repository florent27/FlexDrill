// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetiTecInfoView.xaml.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.SetiTecInfo.ViewModel;
using KukaRoboter.Common.Attributes;
using System.Windows;

namespace Kuka.FlexDrill.SmartHMI.Production.SetiTecInfo.View
{
    [ViewModelType(typeof(SetiTecInfoViewModel))]
    public partial class SetiTecInfoView
    {
        #region Constants and Fields

        private SetiTecInfoViewModel viewModel;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public SetiTecInfoView() : base(true)
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        #endregion Constructors and Destructor

        #region Interface

        public override void RequestClose()
        {
            ViewModel.DoStopTimer();
            base.RequestClose();
        }

        public SetiTecInfoViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = DataContext as SetiTecInfoViewModel;
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