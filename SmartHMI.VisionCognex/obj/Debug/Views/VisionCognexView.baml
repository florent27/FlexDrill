   M S B A M L   `   `   `  ����c  _KukaRoboter.SmartHMI.UIFrameworkServices, Version=8.1.1.0, Culture=neutral, PublicKeyToken=null�jclr-namespace:KukaRoboter.SmartHMI.UIFramework.ViewModel;assembly=KukaRoboter.SmartHMI.UIFrameworkServices*KukaRoboter.SmartHMI.UIFramework.ViewModel  5   	   g cKukaRoboter.Common.ViewModelBase, Version=8.1.0.0, Culture=neutral, PublicKeyToken=eae976646d224940uTclr-namespace:KukaRoboter.Common.ViewModel;assembly=KukaRoboter.Common.ViewModelBaseKukaRoboter.Common.ViewModel 5   	   h dKukaRoboter.Common.ComponentModel, Version=8.3.0.0, Culture=neutral, PublicKeyToken=eae976646d224940�eclr-namespace:KukaRoboter.Common.ComponentModel.Validation;assembly=KukaRoboter.Common.ComponentModel,KukaRoboter.Common.ComponentModel.Validation 5   	   h dCognex.InSight.Controls.Display, Version=4.9.101.3, Culture=neutral, PublicKeyToken=6b9774afddb09935zVclr-namespace:Cognex.InSight.Controls.Display;assembly=Cognex.InSight.Controls.DisplayCognex.InSight.Controls.Display 5	   	   b ^KukaRoboter.Common.Controls, Version=8.3.0.0, Culture=neutral, PublicKeyToken=eae976646d224940nNclr-namespace:KukaRoboter.Common.Controls;assembly=KukaRoboter.Common.ControlsKukaRoboter.Common.Controls 5
   	   sSclr-namespace:KukaRoboter.Common.Controls;assembly=KukaRoboter.Common.ViewModelBaseKukaRoboter.Common.Controls 5   	   ( $Kuka.FlexDrill.SmartHMI.VisionCognexA   ;Kuka.FlexDrill.SmartHMI.VisionCognex.Views.VisionCognexView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^	 ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    	 5   	   R
 NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml 
    5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006 
    5   	   }KukaUIjclr-namespace:KukaRoboter.SmartHMI.UIFramework.ViewModel;assembly=KukaRoboter.SmartHMI.UIFrameworkServices 
    5   	   gKukaVMTclr-namespace:KukaRoboter.Common.ViewModel;assembly=KukaRoboter.Common.ViewModelBase 
    5   	   �KukaValidationeclr-namespace:KukaRoboter.Common.ComponentModel.Validation;assembly=KukaRoboter.Common.ComponentModel 
    5   	   @d2http://schemas.microsoft.com/expression/blend/2008 
    5   	   icognexVclr-namespace:Cognex.InSight.Controls.Display;assembly=Cognex.InSight.Controls.Display 
    5	   	   gkukacontrolsNclr-namespace:KukaRoboter.Common.Controls;assembly=KukaRoboter.Common.Controls 
    5
   	   divtbSclr-namespace:KukaRoboter.Common.Controls;assembly=KukaRoboter.Common.ViewModelBase 
    5   	   -   5        >   8KukaRoboter.SmartHMI.UIFramework.ViewModel.TouchViewBase   Name$
  root��5   4    � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $Z T/Kuka.FlexDrill.SmartHMI.VisionCognex;component/Resources/VisionCognexResources.xaml?�6%   %    5      .��5      � -   5         �Name$
 grid��5      .+�5   
   �� -   5        $��TabControleVision��5      .@�5      �� ��6   �� *Resources.Strings[tabCamera]+.��5      �  � Orientation$ Vertical=�6   .+�5      �� -   5        $��stckCtrlCognex��5   !   $	��370��67   $��
10,10,10,0q�6E   .+�5      7 	 1System.Windows.Forms.Integration.WindowsFormsHost  -   5        $��
ctrlCognex��5   +   $��Stretch=�6=   $��Stretch=�6Y      Child. 5      7  1Cognex.InSight.Controls.Display.CvsInSightDisplay  -   5        cvsInSightDisp��   ShowGraphics .    ShowCustomView . 	   ImageZoomMode$		 Fit=�5   y   [ WSystem.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089,  &System.Windows.Forms.ScrollableControl
   AutoScrollMinSizeU QSystem.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"  System.Drawing.SizeConverter$

 0, 0 6�   5      5       �� 5"      -   5        $��stckPictureAction��5"   !   $	��140��6:   .+�5#      �  � ColumnDefinitions 5$      �� 5%   "   $��* �63   �� 5&   "   $��* �63   .+�5(      �� -   5         ��Name$ grbReticule��5(   '      KukaCroupBoxStyle#��[  6j   $	��200��6�   $	��140��6�   ��6<   �� *Resources.Strings[gbPixel]+.��5)   "   �  5*   &   �� 5+   *   $��* �6;   �� 5,   *   $
��Auto �6;   �� 5-   *   $��* �6;   �� 5.   *   $
��Auto �6;   �� 5/   *   $��* �6;   �� 50   *   $
��Auto �6;   �� 51   *   $��* �6;    � RowDefinitions 53   &   �� 54   *   $��10 �68   �� 55   *   $
��Auto �68   �� 56   *   $��* �68   �� 57   *   $
��Auto �68   �� 58   *   $��* �68   �� 59   *   $
��Auto �68   �� 5:   *   $��* �68   .+�5<   &   �� -	   5        $��txtPixel��5<   .   $��3��6�   $��3��6�   
 � Tag$ NumericUFloat��6�   $��30��6�   $��72��6�    �� TextAlignment$ Center=�6�   ��6>   D  >KukaRoboter.Common.ComponentModel.Validation.ValidationBinding     Path$ NbPixel�   ForceValidation .    UpdateSourceTrigger$ PropertyChanged=�6  �� 5=   &   -
   5         ��Name$ btRight��5=   -   $��5��6e   $��3��6u   $��30��6�   $��30��6�   ��6>   ��  �� Path$ RightReticule�.��5>   *   F�  	 Arrow#���  6/   ���	#FF44A6D1 �� Stretch$
 Fill=�6_   5?   '   �� 5@   &   -   5        $ btLeft��5@   -   $��1��6c   $��3��6s   $��30��6�   $��30��6�   ��6=   �� $ LeftReticule�.��5A   *   F� #���  6/   ���	#FF44A6D1$
 Fill=�6_    J� RenderTransformOrigin$ 0.5,0.5&�6n   z�5B   .   V� 5C   2   .~�5D   6   ��  �� Angle$	 180Q�6F   5E   3   5G   +   5H   '   �� 5I   &   -   5        $
 btUp��5I   -   $��3��6b   $��1��6r   $��30��6   $��30��6�   ��6;   �� $ MountReticule�.��5J   *   F� #���  6/   ���	#FF44A6D1$
 Fill=�6_   $ 0.5,0.5&�6n   z�5K   .   V� 5L   2   .~�5M   6   �� $	 -90Q�6F   5N   3   5P   +   5Q   '   �� 5R   &   -   5        $ bDown��5R   -   $��3��6b   $��5��6r   $��30��6   $��30��6�   ��6<   �� $ DownReticule�.��5S   *   F� #���  6/   ���	#FF44A6D1$
 Fill=�6_   $ 0.5,0.5&�6n   z�5T   .   V� 5U   2   .~�5V   6   �� $ 90Q�6F   5W   3   5Y   +   5Z   '   5[   #   5\      �� 5]      -   5        $��stckZoomGridGraph��5]   )   $��1��6B   .+�5^   "   �  5_   &   �� 5`   *   $��15 �68   �� 5a   *   $
��Auto �68   �� 5b   *   $��30 �68   �� 5c   *   $
��Auto �68   .+�5e   &   �� $��3��61   .+�5f   *   �  5g   .   �� 5h   2   $��* �6C   �� 5i   2   $
��Auto �6C   �� 5j   2   $��* �6C   �� 5k   2   $
��Auto �6C   �� 5l   2   $��* �6C   �� 5m   2   $
��Auto �6C   �� 5n   2   $��* �6C   .+�5p   .   �� -   5        $ 
btZoomPlus��5p   5   $��5��6I   $��40��6Y   $��40��6e   .��5q   2   F� $��1��67     ZoomPlus#���  6G   
���   �$
 Fill=�6v   5r   /   �� 5s   .   -   5        $ btZoomMinus��5s   5   $��1��6J   $��40��6Z   $��40��6f   .��5t   2   F� $��1��67     	ZoomMoins#���  6G   
���   �$
 Fill=�6w   5u   /   �� 5v   .   -   5        $ 	btZommFit��5v   5   $��3��6H   $��40��6X   $��40��6d   .��5w   2   F� $��1��67     ZoomFit#���  6G   
���   �$
 Fill=�6u   5x   /   5y   +   5z   '   �� 5{   &   $��1��61   .+�5|   *   �  5}   .   �� 5~   2   $��* �6C   �� 5   2   $
��Auto �6C   �� 5�   2   $��* �6C   �� 5�   2   $
��Auto �6C   �� 5�   2   $��* �6C   �� 5�   2   $
��Auto �6C   �� 5�   2   $��* �6C   .+�5�   .   �� -   5         ��Name$ chkShowGrid��5�   7   $��1��6L   $
��Left=�6\   $��40��6w   $��64��6�   .��5�   2   F� $��1��67     Grid#���  6G   
���   �$
 Fill=�6r   5�   /   �� 5�   .   -   5        $ chkShoxGraph��5�   7   $��3��6M   $
��Left=�6]   $��40��6x   $��64��6�   .��5�   2   F� $��1��67     Graphic#���  6G   
���   �$
 Fill=�6u   5�   /   5�   +   5�   '   5�   #   5�      5�      5�      �� 5�      -   5        $��stckCommands��5�   !   $��60��65   .+�5�      � $ 
Horizontal=�6$   .+�5�      �� -   5        $��btTakePicture��5�   %     ParamButton#��[ 6i   $��Stretch=�5�   %   $��44��6J   ��5�   :   �� $ TakeNewPictureCommand�.��5�   "   �  5�   &   �� 5�   *   $��* �6;   �� 5�   *   $��35 �6;   .+�5�   &   �� $
��Wrap=�6�   $��10(�6�   $��Center=�6�   $��Center=�6�   ��60   �� *$"Resources.Strings[btManualAcquire]+ �� FallbackValue$ btManualAcquire��6�   F� 5�   &   $��1��6+    
 Camera#���  6;   
���   �$
 Fill=�6h   $
��Left=�6w   5�   #   5�      �� 5�      -   5        $��btSkip��5�   %   $��1��6X   #��[ 6h   $��Stretch=�5�   %   $��44��6J   ��5�   3   �� $ SkipCommand�.��5�   "   �  5�   &   �� 5�   *   $��* �6;   �� 5�   *   $��35 �6;   .+�5�   &   �� $��Center=�6q   $��Center=�6�   $��10(�6�   ��6,   �� *Resources.Strings[btSkip]+$ btSkip��6�   F� 5�   &   $��1��6+    	 Skip#��� 	 6;   
��� ���$
 Fill=�6g   $
��Left=�6v   5�   #   5�      �� 5�      -   5        $
��btOk��5�   %   $��2��6T   #��[ 6d   $��Stretch=�5�   %   $��44��6J   ��5�   1   �� $ 	OkCommand�.��5�   "   �  5�   &   �� 5�   *   $��* �6;   �� 5�   *   $��35 �6;   .+�5�   &   �� $��Center=�6l   $��Center=�6�   $��10(�6�   ��6,   �� *Resources.Strings[btOk]+$
 btOk��6�   F� 5�   &   $��1��6+    
 OK#��� 
 6<   
��� � �$
 Fill=�6e   $
��Left=�6t   5�   #   5�      �� 5�      -   5        $��btNok��5�   %   $��3��6V   #��[ 6f   $��Stretch=�5�   %   $��44��6J   ��5�   2   �� $ 
NokCommand�.��5�   "   �  5�   &   �� 5�   *   $��* �6;   �� 5�   *   $��35 �6;   .+�5�   &   �� $��Center=�6n   $��Center=�6�   $��10(�6�   ��6,   �� *Resources.Strings[btNok]+$ btNok��6�   F� 5�   &   $��1��6+     NOK#���  6;   
���  ��$
 Fill=�6c   $
��Left=�6r   5�   #   5�      5�      5�      �� 5�      -   5        $��stckConnectionAction��5�   !   $��50��6=   .+�5�      �  5�      �� 5�   "   $	��256 �63   �� 5�   "   $	��100 �63   �� 5�   "   $��70 �63   �� 5�   "   �� 5�   "   .+�5�      � $��Center=�6(   .+�5�   "   �� -   5        $��btConnection��5�   )   #��[ 6h   ��6=   �� $ ConnectionCommand�{�6�   �� *CamIsDisconnected+��5�   %   �� *!Resources.Strings[btconnection]+$ btconnection���� 5�   "   -   5        $��btDeconnection��5�   )   #��[ 6m   ��6?   �� $ DisconnectionCommand�{�6�   �� *CamIsConnected+��5�   %   �� *$"Resources.Strings[btdisconnection]+$ btdisconnection��5�      �� 5�      $��2��6$   $��Right=�6~   $��Center=�6�   ��64   �� *Resources.Strings[lblOnLine]+$ 	lblOnLine��6�   ,  &KukaRoboter.Common.Controls.LEDControl  5�      $��3��66   $��40��6g   $��Center=�6t      IsActive 6F   �� *CamIsOnLine+6t   5�      5�      5�      5�      �� 5�      ��6   �� *)'Resources.Strings[tabCameraCalibration]+.��5�      �� .+�5�      �  5�      �� 5�      $��20 �6,   �� 5�      $
��Auto �6,   �� 5�      $��20 �6,   �� 5�      $
��Auto �6,   �� 5�      $��20 �6,   �� 5�      $
��Auto �6,    5�      �� 5�      $��* �6/   �� 5�      $
��auto �6/   �� 5�      $��* �6/   .+�5�      �� $��1��6#   $��1��60   ��6@   �� *$"Resources.Strings[CalibrateVision]+.��5�      �  5�   "   �� 5�   &   $��50 �67   �� 5�   &   $
��Auto �67   �� 5�   &   $	��270 �67   �� 5�   &   $
��Auto �67   .+�5�   "     $��1��6:   $��40��6s    6J   �� *VisionCalibrationOk+6s   �� 5�   "   $��3��6)   #��[ 5�   )   ��5�   9   �� $ CalibrateVisionCommand���5�   )   �� * Resources.Strings[btCalibrate]+$ btCalibrate��{�5�   )   �� *CalibrateVisionNotRunning+5�      5�      �� 5      $��3��6#   $��1��60   ��6@   �� *'%Resources.Strings[CalibrateNormality]+.��5     �  5  "   �� 5  &   $��50 �67   �� 5  &   $
��Auto �67   �� 5  &   $	��270 �67   �� 5  &   $
��Auto �67   .+�5  "     $��1��6:   $��40��6v    6J   �� *NormalityCalibrationOk+6v   �� 5	  "   $��3��6)   #��[ 5  )   ��5	  9   �� $ CalibrateNormalityCommand���5
  )   �� * Resources.Strings[btCalibrate]+$ btCalibrate��{�5  )   �� *$"CalibrateVisionNormalityNotRunning+5     5     5     5     5     5     5     5     