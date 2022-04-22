// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetiTecInfoView.xaml.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;

using Kuka.FlexDrill.SmartHMI.Production.SetiTecInfo.ViewModel;

using KukaRoboter.Common.Attributes;

namespace Kuka.FlexDrill.SmartHMI.Production.SetiTecInfo.View
{
   [ViewModelType(typeof(SetiTecInfoViewModel))]
   public partial class SetiTecInfoView
   {
      #region Constants and Fields

      private SetiTecInfoViewModel viewModel;

      #endregion

      #region Constructors and Destructor

      public SetiTecInfoView() : base(true)
      {
         InitializeComponent();
         Loaded += OnLoaded;
      }

      #endregion

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

      #endregion

      #region Methods

      private void OnLoaded(object sender, RoutedEventArgs e)
      {
         ViewModel.InitializePlugin();
      }

      #endregion
   }
}