using Kuka.FlexDrill.SmartHMI.Production.TcpCalibration.ViewModel;
using KukaRoboter.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kuka.FlexDrill.SmartHMI.Production.TcpCalibration.View
{
   /// <summary>Interaction logic for TextBoxTestView.xaml</summary>
   [ViewModelType(typeof(TcpCalibrationViewModel))]
   public partial class TcpCalibrationView
   {
      #region Constants and Fields

      private TcpCalibrationViewModel viewModel;

      #endregion

      #region Constructors and Destructor

      public TcpCalibrationView()
         : base(true)
      {
         InitializeComponent();

         Loaded += OnLoaded;
      }

      #endregion

      #region Interface

      /// <summary>Gets the view model.</summary>
      public TcpCalibrationViewModel ViewModel
      {
         get
         {
            if (viewModel == null)
            {
               viewModel = DataContext as TcpCalibrationViewModel;
            }

            return viewModel;
         }
      }
 
      public override void RequestClose()
      {
         ViewModel.ReleaseEvents();
         base.RequestClose();
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
