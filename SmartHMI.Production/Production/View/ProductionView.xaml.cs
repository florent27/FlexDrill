// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductionView.xaml.cs" company="KUKA Deutschland GmbH">
//    Copyright (c) KUKA Deutschland GmbH 2006 - 2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;

using Kuka.FlexDrill.SmartHMI.Production.Production.ViewModel;

using KukaRoboter.Common.Attributes;

namespace Kuka.FlexDrill.SmartHMI.Production.Production.View
{
   /// <summary>Interaction logic for TextBoxTestView.xaml</summary>
   [ViewModelType(typeof(ProductionViewModel))]
   public partial class ProductionView
   {
      #region Constants and Fields

      private ProductionViewModel viewModel;

      #endregion

      #region Constructors and Destructor

      public ProductionView()
         : base(true)
      {
         InitializeComponent();

         Loaded += OnLoaded;
      }

      #endregion

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

      #endregion

      #region Methods

      private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
      {
         ViewModel.InitializePlugin();
      }
      #endregion
   }
}