   M S B A M L   `   `   `  ����I  EWhatsAppClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WhatsAppClient.MVVM.ViewsWhatsAppClient.MVVM.Views  5   	   *    $WhatsAppClient.MVVM.Views.InfoScreen   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:WhatsAppClient.MVVM.Views     5   	   _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   $	��560��5	   	   $	��500��5
   	     � WindowStartupLocation$  CenterScreen=�5   	    � WindowStyle$
 None=�5   	    � AllowsTransparency .  � 
ResizeMode$ NoResize=�5   	    � Icon$ ../Resource/Logo.png��5   	    � Title$
 Info��5   	    J� Clip 5      �� 5   
    �� Rect$ 0,0,500,560�6    �� RadiusX$ 10Q�6/   	 �� RadiusY$	 10Q�6<   .��5      �� 
 �� Orientation$
 Vertical=�6   .+�5   
   �  � RowDefinitions 5      �� 5      $��30 �6    �� 5      $	��530 �6    .+�5      � $��0��6   ���#101d24 � ColumnDefinitions 5      �� 5      $
��0.6* �6'   �� 5      $��6* �6'   �� 5      $
��1.7* �6'   .+�5!      �� $��0��6   $��../Resource/Logo.png��6(   $��30��6F   $��30��6Q   $
��Left=�6]   � 5"      -   5         �Name$ 
contextBar��5"      $��1��6+   .+�5#      �� $��Info Screen��6   
�������$��15(�6E   $
��Left=�6S   $	��362��6n   5$      �� 5&      -   5         ��Name$ minimizeButton��5&      $��2��61   $��_��6A   $��25(�6M   $��50��6[   
�������
������ $��0q�6�    � ToolTip$ Minimize��6�   $
��Left=�6�   $��0 -5 0 0q�6�   ��6�   �� *MinimizeCommand+6�   �� 5'      -   5        $ closeButton��5'      $��2��6.   $��X��6>   $��20(�6J   $��50��6X   
�������
������ $��0q�6�   $ Close��6�   $��Right=�6�   $��0 0 0 0q�6�   ��6�   �� *CloseCommand+6�   5(      �� 5*      $��1��6   ���#101d24.+�5+      � $��Center=�6   $��0 10 0 0q�64   .+�5,      �� -   5         ��Name$ profileImage��5,      $	��200��62   $#��../Resource/defaultAvatar.png��6>   $	��200��6f    J� 	AllowDrop .  5-      @� 5.       @� Center$ 100,100&�6.    @� RadiusX$	 100Q�6?    @� RadiusY$	 100Q�6M   50      �� 51      -   5         ��Name$ dragDrop��51       . $	��200��6A   $	��200��6M   
������ $��0q�6s   
������  52      @� 53      $ 100,100&�6.   $	 100Q�6?   $	 100Q�6M   55      56      � 58       59      �� 5:      $
��0.5* �6+   �� 5;      $	��20* �6+    5=      �� 5>      �� 5?      �� 5@      .+�5C      �� $��1��6!   $��0��61   $
 
Horizontal=�6>   $��Center=�6W   .+�5D      '  !MaterialDesignThemes.Wpf.PackIcon     Kind$ Account=�5E      $��30��5F      $��30��5G      $��	0 10 10 0q�5H      ���#9ba1a35I      �� 5J      $��45��5K      $	��300��5L      $��15(�5M      )  #MaterialDesignThemes.Wpf.HintAssist   Hint$
 Name��5N         
IsFloating . 
������� �� 	MaxLength$ 20��5Q      ��5R      �� *
NameText+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�5S      �� 5U      $��1��6!   $��1��61   $
 
Horizontal=�6>   $��Center=�6W   $��0 30 0 0q�6t   .+�5V        $ InformationOutline=�5W      $��30��5X      $��30��5Y      $��	0 10 10 0q�5Z      ���#9ba1a35[      �� 5\      $��45��5]      $	��300��5^      $��15(�5_      $ About��5`       . 
�������$	 150��5c      ��5d      �� *	AboutText+$ TwoWay=�$ PropertyChanged=�5e      �� 5g      $��1��6!   $��2��61   $
 
Horizontal=�6>   $��Center=�6W   $��0 30 0 0q�6t   .+�5h        $ Phone=�5i      $��35��5j      $��35��5k      $��	0 12 10 0q�5l      ���#9ba1a35m      �� 5n      $
 Vertical=�6%   .+�5o      �� $��Phone��5p   !   ���#9ba1a3$��12(�5r   !   �� 5s      $��45��5t   !   $	��300��5u   !   $��15(�5v   !   $ Phone��5w   !    . 
���������5z   !   �� *PhoneNumber+$ TwoWay=�$ PropertyChanged=�5{      5|      5}      �� 5      $	��150��5�      $��Finish��5�      
�������$��DemiBold"�5�      $��20(�5�      $��0 20 0 0q�5�      $��0 0 0 0q�5�      $��Center=�5�      $��Center=�5�      ��5�      �� *FinishCommand+5�      5�      5�      5�      5�      