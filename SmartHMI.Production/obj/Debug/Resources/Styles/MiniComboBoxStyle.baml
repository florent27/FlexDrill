   M S B A M L   `   `   `  ����b  ^KukaRoboter.Common.Controls, Version=8.3.0.0, Culture=neutral, PublicKeyToken=eae976646d224940nNclr-namespace:KukaRoboter.Common.Controls;assembly=KukaRoboter.Common.ControlsKukaRoboter.Common.Controls  5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089  �� MergedDictionaries �� Source   KukaMiniComboBoxStyle �� 
TargetType �� Property �� Value �� 
TargetType d�Name  FocusVisualStyle � FocusVisualStyle �� 	ClickMode	 d� 	IsChecked
 �� Path �� Mode �� RelativeSource ��Name �� RadiusX �� RadiusY  NormalBorderBrush F�Name  
GlyphBrush �� Triggers P� Property P� Value  DisabledForegroundBrush  DisabledBorderBrush �� 
TargetName-    'KukaRoboter.Common.Controls.LockManager4  .System.Windows.Automation.AutomationProperties  
FocusBrush ��Name �� SelectionBoxItem �� SelectionBoxItemTemplate J� IsHitTestVisible ��Name  ComboBoxTextBox �� 
IsReadOnly �� 
IsReadOnly �Name �� IsDropDownOpen � AllowsTransparency �Name  �� MaxDropDownHeight! � ActualWidth" J� SnapsToDevicePixels# ��Name$ K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation%   DirectionalNavigation& P� 
SourceName�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      [controlsNclr-namespace:KukaRoboter.Common.Controls;assembly=KukaRoboter.Common.Controls  5        5      �� 5      $V P/KukaRoboter.SmartHMI.UIFrameworkServices;component/Resources/DefaultStyles.xaml?�5   
   %x  &	        7  7  7  7  7  7  7  7  �� 5      " ��6)   .�5      ��  �@J�SnapsToDevicePixels . 6.   �� 5       �@�FocusVisualStyle 6+   Z� �� 5       � |� .  6$   �� 5       � ��$ 40��6!   �� 5       � �� 5      �� 5      " ��6   .R�5      � .+�5      d� $ PART_ToggleButton��6!   |�. 8   $ Press=�5      	 5      �� $
 IsDropDownOpen�$ TwoWay=� �� *TemplatedParent+��5      �� 5      " d�6*   .R�5      � .+�5       �� $ PART_ComboTextBox��6*   $��Stretch=�5   "   $��Stretch=�5   "   $
��Auto��5    "   $
��Auto��5!   "   $ 2��5"   "   $ 2��5#   "   ���	#FFFFFFFF#���  5%   "   F� 5&       $ 
PART_Arrow��6%   $��Right=�5'   "   $	��Top=�5(   "   $��2q�5)   "   8�� "����        � 	=  	= � z     	5*   "   5+       5,      P� 5-        � {� .  . �5.   #   �� # �  5/   %    � ���� 50   #   8  $ 
PART_Arrow��51   %    � ��52   !   P� 53        �@  IsLocked . . �54   #   �� # �  55   %    � ���� 56   #   8  $ 
PART_Arrow��57   %    � ���� 58   #    �@ 
ItemStatus$ Locked��6U   59   !   P� 5:        �@  IsLocked .  . �5;   #   ��  �@ 
ItemStatus$  ��6U   5<   !   P� 5=        �@J�	IsFocused . . �5>   #   �� 8  $ PART_ComboTextBox��5?   %    � ��5@   !   5B      5D      �� 5E      $ PART_ContentSite��6%   $
��Left=�5F      $��8,3,8,3q�5G      $��Center=�5H      #��z 5I      #��z 5J      #��z��5K       .  |�.  5M      �� 5O      $ PART_EditableTextBox��6   $��Center=�5Q      $��	0,10,0,10q�5R      $��4,3,64,3q�5S      $y�Hidden=�5T      8�� |�. 
������ # z 5X      ��5P      Z� 5X      � 5Z      $ 
PART_Popup��6   #��z 5[      $��Bottom=�5\      |�.   .  $
��None=�5_      .��5`      � $ PART_DropDown��6   #��z  5a      #��z! 5b      " . .+�5d      �� $# PART_DropDownBorder��6!   ���	#FFFFFFFF8�� $��1q�5g      �� 5h      $��4,6,4,4q�6'   " . $
��Auto=�5i      $
��Auto=�5j      ��. .��5l      �� $ . $% 	Contained=�6;   5m      5n      5o      5p       5q      P� 5s       �@��HasItems .  . �5t      �� $ PART_DropDownBorder��6>    � ��$ 95��63   6>   5u      P� 5v       � {� .  . �5w      �� 8   � ��64   5x      P� 5y       �@  IsLocked . . �5z      �� $ PART_ToggleButton��6A    � {� .  �� 5{      $ PART_ContentSite��6A    � {� .  �� 5|      $ PART_EditableTextBox��6A    � {� .  �� 5}       �@ 
ItemStatus$ Locked��6I   5~      P� 5       �@  IsLocked .  . �5�      ��  �@ 
ItemStatus$  ��6I   5�      P� 5�       �@��
IsGrouping . . �5�      ��  � �� .  6G   5�      P� 5�      $& 
PART_Popup��6:    �@�AllowsTransparency . . �5�      �� $ PART_DropDownBorder��6@    �@��CornerRadius$ 2��66   6@   �� 5�      $ PART_DropDownBorder��6A    � ��$ 0,-3,0,0q�60   6A   5�      P� 5�       �@��
IsEditable . . �5�      ��  � �� .  63   �� 5�      $ PART_ContentSite��6C    � y�$ Hidden=�64   6C   �� 5�      $ PART_EditableTextBox��6D    � y�$ Visible=�64   6D   5�      5�      5�   	   5�      5�      