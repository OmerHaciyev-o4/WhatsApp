   M S B A M L   `   `   `  ����I  EWhatsAppClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WhatsAppClient.MVVM.ViewsWhatsAppClient.MVVM.Views  5      .    (WhatsAppClient.MVVM.Views.ContactControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      3local'clr-namespace:WhatsAppClient.MVVM.Views  5      ���#101D24.��5      �� .+�5   
   ��   �� Orientation$  
Horizontal=�5      .+�5      �� 
������ $��0q�5      $��30��5      $��30��5      $��0 0 0 0q�5      ��5      �� *BeforeViewCommand+��5      '  !MaterialDesignThemes.Wpf.PackIcon  5         Kind$ 	ArrowLeft=�5      $��30��5      $��30��5      ���#9BA1A35       J� Clip 5      @� 5       @� Center$ 15 15&�6&    @� RadiusX$ 15Q�65    @� RadiusY$ 15Q�6B   5       � 5!      $	��340��5"      .+�5#      �� $��Choose:��5$      
�������$��20(�5&      $��Center=�5'      5(      5*      �� 5,   
   $  
Horizontal=�5-      $��Center=�5.      .+�5/      	� -   5         	�Name$ addRadioButton��50      $	��Add��51      
������� d� 	IsChecked$
 TrueY�53      	� 54      -   5        $ deleteRadioButton��55      $��Delete��56      
�������$��20 0 0 0q�58      59      � 5;   
   .+�5<      � -   5         �Name$ addPanel��5=      $	��400��5>      $	��563��5?      $��0 15 0 0q�5@      .+�5B      �� $��0��5C      $	��400��5D      $��Center=�5E      $	��Top=�5F      .+�5H      �� -   5        	 ��Name$	 	countries��5K      
 %� IsVirtualizing
 . ��. 
�������$	��250��5M      $��30��5N      $��0 40 0 0q�5P       �� SelectedIndex$ 0��5Q      ��5O      �� *Items+5Q      �� 5S      $  
Horizontal=�5T      $��Center=�5U      $��0 15 0 0q�5V      .+�5W      �� -   5         ��Name$ 
regionCode��5X      
�������$��70��5[      ��5Y      �� *RegionCodeText+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�5[      �� 5]      -   5        $ phoneNumber��5^      
�������$	��150��5a      $��30 0 0 0q�5b       �� 	MaxLength$ 13��5c      ��5_      �� *PhoneNumberText+$ TwoWay=�$ PropertyChanged=�5c      5d      �� 5f      $��0 20 0 0q�5g      $	��100��5h      $��Center=�5i      $
��Send��5k      $��DemiBold"�5l      $��15(�5m      
����������#57F573��5j      �� *SendCodeCommand+5o      �� 5q      -   5        $ codeTextBox��5r      $	��150��5t      
�������$ 6��5v      $��0 50 0 0q�5w      ��5s      �� *VerificationText+$ TwoWay=�$ PropertyChanged=�5w      �� 5y      -   5         ��Name$ addContactButton��5z      $��Add Contact��5{      $	��150��5|      ���#57F573
�������$��0 15 0 0q�5      ��5�      �� *
AddCommand+5�      5�      � 5�      -	   5        $ deletePanel��5�      $	��400��5�      $	��563��5�      $��0 15 0 0q�5�      $y�Hidden=�5�      .+�5�      �� -
   5         ��Name$ contact��5�      $	��400��5�      $	��200��5�      
�������$	��Top=�5�      ��5�      �� *NumberItems+$ TwoWay=�$ PropertyChanged=� 5�      �� *SelectItemBox+$ TwoWay=�$ PropertyChanged=��� 5�      ���#57F573$��0q�5�      $	��100��5�      $��Delete��5�      
�������$��15(�5�      $��DemiBold"�5�      ��5�      �� *DeleteCommand+5�      5�      5�      5�      