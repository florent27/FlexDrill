using Kuka.FlexDrill.SmartHMI.VisionCognex.ViewModels;
using KukaRoboter.Common.Attributes;
using KukaRoboter.SmartHMI.UIFramework.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Kuka.FlexDrill.SmartHMI.VisionCognex.Views
{
    /// <summary>
    /// Interaction logic for VisionCognexView.xaml
    /// </summary>

    [ViewModelType(typeof(VisionCognexViewModel))]
    public partial class VisionCognexView : TouchViewBase
    {
        public VisionCognexView()
        {
            InitializeComponent();
        }

        protected override void OnConnected(string viewSystemName, object connectionArgument)
        {
            base.OnConnected(viewSystemName, connectionArgument);

            cvsInSightDisp.InSight = ViewModel.CognexCamera;

            chkShoxGraph.IsChecked = cvsInSightDisp.ShowGraphics;
            cvsInSightDisp.ShowCustomView = (!ViewModel.DisplayFullTab);
            cvsInSightDisp.ShowGrid = true;
            cvsInSightDisp.GridScale = 0.8;
            chkShowGrid.IsChecked = true;

            cvsInSightDisp.ScrollMode = Cognex.InSight.Controls.Display.CvsDisplayScrollMode.Image;
        }

        protected override void OnDisconnecting()
        {
            base.OnDisconnecting();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (root.ActualHeight == 368)
            {
                stckZoomGridGraph.Visibility = Visibility.Collapsed;
                stckConnectionAction.Visibility = Visibility.Collapsed;

                ctrlCognex.Height = 310;
                ctrlCognex.Width = 310;

                grbReticule.Width = 170;
                grbReticule.HorizontalAlignment = HorizontalAlignment.Left;

                stckCommands.Width = 170;
                stckCommands.Height = 170;

                btTakePicture.Width = 44;
                btSkip.Width = 44;
                btOk.Width = 44;
                btNok.Width = 44;

                btTakePicture.Margin = new Thickness(28, 33, 15, 15);
                btSkip.Margin = new Thickness(10, 33, 15, 15);
                btOk.Margin = new Thickness(28, 15, 15, 15);
                btNok.Margin = new Thickness(15);
            }
            else
            {
                stckZoomGridGraph.Visibility = Visibility.Visible;
                stckConnectionAction.Visibility = Visibility.Visible;

                ctrlCognex.Height = 370;
                ctrlCognex.Width = 492;

                grbReticule.Width = 200;
                grbReticule.HorizontalAlignment = HorizontalAlignment.Center;

                stckCommands.Width = 511.8;
                stckCommands.Height = 60;

                btTakePicture.Width = 100;
                btSkip.Width = 100;
                btOk.Width = 100;
                btNok.Width = 100;

                double interSpace = (stckCommands.Width - 4 * btTakePicture.Width) / 5;
                btTakePicture.Margin = new Thickness(interSpace, 10, interSpace / 2, 10);
                btSkip.Margin = new Thickness(interSpace / 2, 10, interSpace / 2, 10);
                btOk.Margin = new Thickness(interSpace / 2, 10, interSpace / 2, 10);
                btNok.Margin = new Thickness(interSpace / 2, 10, interSpace, 10);

                btConnection.Margin = new Thickness(interSpace, 10, interSpace / 2, 10);
                btDeconnection.Margin = new Thickness(interSpace / 2, 10, interSpace / 2, 10);
            }
        }

        /// <summary>
        /// Reference to the ViewModel (in order to give view arguments when loading the plugin
        /// </summary>
        public VisionCognexViewModel ViewModel
        {
            get { return ConnectedViewModel as VisionCognexViewModel; }
        }

        private void chkShowGrid_Checked(object sender, RoutedEventArgs e)
        {
            cvsInSightDisp.ShowCustomView = (!ViewModel.DisplayFullTab);
            cvsInSightDisp.ShowGrid = chkShowGrid.IsChecked.Value;
            cvsInSightDisp.GridScale = 0.8;
            cvsInSightDisp.ScrollMode = Cognex.InSight.Controls.Display.CvsDisplayScrollMode.Image;
        }

        private void chkShowGrid_Unchecked(object sender, RoutedEventArgs e)
        {
            cvsInSightDisp.ShowCustomView = true;
            cvsInSightDisp.ShowGrid = chkShowGrid.IsChecked.Value;
            cvsInSightDisp.ScrollMode = Cognex.InSight.Controls.Display.CvsDisplayScrollMode.Image;
        }

        private void chkShoxGraph_Checked(object sender, RoutedEventArgs e)
        {
            cvsInSightDisp.ShowGraphics = chkShoxGraph.IsChecked.Value;
        }

        private void chkShoxGraph_Unchecked(object sender, RoutedEventArgs e)
        {
            cvsInSightDisp.ShowGraphics = chkShoxGraph.IsChecked.Value;
        }

        private void btZoomPlus_Click(object sender, RoutedEventArgs e)
        {
            cvsInSightDisp.ImageZoomMode = Cognex.InSight.Controls.Display.CvsDisplayZoom.None;
            cvsInSightDisp.ImageScale = cvsInSightDisp.ImageScale * 2;
        }

        private void btZoomMinus_Click(object sender, RoutedEventArgs e)
        {
            cvsInSightDisp.ImageZoomMode = Cognex.InSight.Controls.Display.CvsDisplayZoom.None;
            cvsInSightDisp.ImageScale = cvsInSightDisp.ImageScale / 2;
        }

        private void btZommFit_Click(object sender, RoutedEventArgs e)
        {
            cvsInSightDisp.ImageZoomMode = Cognex.InSight.Controls.Display.CvsDisplayZoom.Fit;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPixel.Text.Contains("."))
            {
                if (txtPixel.Text.Length > 5)
                {
                    txtPixel.Text = txtPixel.Text.Remove(5);
                    txtPixel.Select(txtPixel.Text.Length, 0);
                }
            }
        }
    }
}