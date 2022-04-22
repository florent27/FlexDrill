﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgramTabView.xaml.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kuka.FlexDrill.SmartHMI.Production.Production.ViewModel;
using System.Windows;

namespace Kuka.FlexDrill.SmartHMI.Production.Production.View.Tabs
{
    /// <summary>Interaction logic for TextBoxTestView.xaml</summary>
    public partial class ProgramTabView
    {
        #region Constants and Fields

        private ProductionViewModel viewModel;

        #endregion Constants and Fields

        #region Constructors and Destructor

        public ProgramTabView()
        {
            InitializeComponent();

            Root.SizeChanged += WindowSizeChanged;
        }

        ~ProgramTabView()
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
            progListView.Height = Root.ActualHeight - topPanel.ActualHeight - bottomPanel.ActualHeight;
        }

        #endregion Methods
    }
}