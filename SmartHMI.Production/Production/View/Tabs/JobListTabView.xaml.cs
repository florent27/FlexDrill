using Kuka.FlexDrill.SmartHMI.Production.Production.ViewModel;
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

namespace Kuka.FlexDrill.SmartHMI.Production.Production.View.Tabs
{
   /// <summary>
   /// Interaction logic for JobListTabView.xaml
   /// </summary>
   public partial class JobListTabView
   {
      #region Constants and Fields

      private ProductionViewModel viewModel;

      #endregion

      #region Constructors and Destructor

      public JobListTabView()
      {
         InitializeComponent();

         Root.SizeChanged += WindowSizeChanged;
      }

      ~JobListTabView()
      {
         Root.SizeChanged -= WindowSizeChanged;
      }

      #endregion

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

      #endregion

      #region Methods

      private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
      {
         JobListView.Height = Root.ActualHeight - topPanel.ActualHeight - bottomPanel.ActualHeight;
      }

      #endregion
   }
}
