// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductionView.xaml.cs" company="KUKA System Aerospace">
//    Copyright (c) KUKA System Aerospace 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.OlpGenerator.ViewModel;
using KukaRoboter.Common.Attributes;
using System.Windows;

namespace Kuka.FlexDrill.SmartHMI.Production.OlpGenerator.View
{
    /// <summary>Interaction logic for TextBoxTestView.xaml</summary>
    [ViewModelType(typeof(OlpGeneratorViewModel))]
    public partial class OlpGeneratorView
    {
        #region Constants and Fields

        private OlpGeneratorViewModel viewModel;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public OlpGeneratorView()
           : base(true)
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        #endregion Constructors and Destructor

        #region Interface

        /// <summary>Gets the view model.</summary>
        public OlpGeneratorViewModel ViewModel
        {
            get
            {
                if (viewModel == null)
                {
                    viewModel = DataContext as OlpGeneratorViewModel;
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