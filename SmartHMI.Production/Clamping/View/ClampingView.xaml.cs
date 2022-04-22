// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClampingView.xaml.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;

using Kuka.FlexDrill.SmartHMI.Production.Clamping.ViewModel;

using KukaRoboter.Common.Attributes;

namespace Kuka.FlexDrill.SmartHMI.Production.Clamping.View
{
   /// <inheritdoc cref="ClampingView" />
   [ViewModelType(typeof(ClampingViewModel))]
   public partial class ClampingView
   {
      #region Constants and Fields

      private ClampingViewModel viewModel;

      #endregion

      #region Constructors and Destructor

      public ClampingView() : base(true)
      {
         InitializeComponent();
         Loaded += OnLoaded;
      }

      #endregion

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

      #endregion

      #region Methods

      private void OnLoaded(object sender, RoutedEventArgs e)
      {
         ViewModel.InitializePlugin();
      }

      #endregion
   }
}