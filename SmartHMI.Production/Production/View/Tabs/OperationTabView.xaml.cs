// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationTabView.xaml.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Kuka.FlexDrill.Process.CustomClasses;
using Kuka.FlexDrill.SmartHMI.Production.Production.ViewModel;
using KukaRoboter.Controls.CodeCompletion;

namespace Kuka.FlexDrill.SmartHMI.Production.Production.View.Tabs
{
   /// <summary>Interaction logic for TextBoxTestView.xaml</summary>
   public partial class OperationTabView
   {
      private ProductionViewModel viewModel;

      #region Constructors and Destructor

      public OperationTabView()
      {
         InitializeComponent();

         Root.SizeChanged += WindowSizeChanged;

         Loaded += OnLoaded;

      }

      private void OnLoaded(object sender, RoutedEventArgs e)
      {
         ViewModel.SelectedOperationChangedEvent += SelectedOperationChanged;
      }

      ~OperationTabView()
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

      private void SelectedOperationChanged(Operation Operation)
      {
         if (Operation != null)
         {
            OperationListView.ScrollIntoView(ViewModel.SelectedOperation);
         }
      }
      #endregion

      #region Methods

      private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
      {
         OperationListView.Height = Root.ActualHeight - TopPanel.ActualHeight - BottomPanel.ActualHeight;
      }
      #endregion

   }
}