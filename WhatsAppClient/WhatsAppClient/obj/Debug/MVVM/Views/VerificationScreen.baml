   M S B A M L   `   `   `  ����I  EWhatsAppClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WhatsAppClient.MVVM.ViewsWhatsAppClient.MVVM.Views  5   	   2    ,WhatsAppClient.MVVM.Views.VerificationScreen   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:WhatsAppClient.MVVM.Views     5   	   _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	     � Title$  Verification Screen��5	   	   $	��560��5
   	   $	��600��5   	    � WindowStartupLocation$ CenterScreen=�5   	    � WindowStyle$
 None=�5   	    � AllowsTransparency .  � 
ResizeMode$ NoResize=�5   	    � Icon$ ../Resource/Logo.png��5   	    J� Clip 5      �� 5   
    �� Rect$ 0,0,600,560�6    �� RadiusX$ 10Q�6/   	 �� RadiusY$	 10Q�6<   .��5      �� 
 �� Orientation$
 Vertical=�6   .+�5   
   �  � RowDefinitions 5      �� 5      $��30 �6    �� 5      $	��530 �6    .+�5      � $��0��6   ���#101d24 � ColumnDefinitions 5      �� 5      $
��0.4* �6'   �� 5      $��6* �6'   �� 5      $��1.35* �6'   .+�5!      �� $��0��6   $��../Resource/Logo.png��6(   $��30��6F   $��30��6Q   $
��Left=�6]   � 5"      -   5         �Name$ 
contextBar��5"      $��1��6+   .+�5#      �� $��Verification Screen��6   
�������$��15(�6M   $
��Left=�6[   $	��362��6v   5$      �� 5&      -   5         ��Name$ minimizeButton��5&      $��2��61   $��_��6A   $��25(�6M   $��50��6[   
�������
������ $��0q�6�    � ToolTip$ Minimize��6�   $
��Left=�6�   $��0 -5 0 0q�6�   ��6�   �� *MinimizeCommand+6�   �� 5'      -   5        $ closeButton��5'      $��2��6.   $��X��6>   $��20(�6J   $��50��6X   
�������
������ $��0q�6�   $ Close��6�   $��Right=�6�   $��0 0 0 0q�6�   ��6�   �� *CloseCommand+6�   5(      �� 5*      $��1��6   ���#101d24.+�5+      �  5,      �� 5-      �� 5.      $��8* �6+   �� 5/      .+�52      �� 
������ 
������ $��0q�55      $��0 0 0 0q�56      ��57      �� *PreviewScreenCommand+��58      '  !MaterialDesignThemes.Wpf.PackIcon  59         Kind$ 	ArrowLeft=�66   ���#9ba1a3$��30��6\   $��30��6g   5;      �� 5<      $��1��6   $��Verifying your number��6,   $��30(�6L   
�������$��Center=�6m   5=      �� 5?      $5��/Waiting to automatically detect an SMS send to ��6   $��18(�6R   ���#9ba1a3$��Center=�6u   �� 5@      $��18(�6j   $��DemiBold"�6x   
�������$��Center=�6�   ��6   �� *PhoneNumber+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   � 5B      $��0 70 0 0q�6    5C      �� 5D      �� 5E      $��4* �6+   �� 5F      .+�5I      �� $��1��6!   $��Center=�61   .+�5J      � .+�5K      �� $	��200��6|   $��55��6�   
������ ���#9ba1a3$��35(�6�   $��Center=�6�   ��6$   �� *PartCodeLabelText+$ TwoWay=�$ PropertyChanged=�6�   �� 5L      -   5         ��Name$ partCodeTextBox��5L   &   $	��200��6�   $��55��6�   
������ 
�������$��35(�6�   $
��Left=�6�   $��3 0 0 0q�6  ��6?   �� *PartCodeTextBoxText+$ TwoWay=�$ PropertyChanged=�6  5M      �� 5N      $��Enter 6-digit code��6    ���#9ba1a3$��15(�6R   $��Center=�6`   5O      5P      �� 5R      -   5        $ 
timeButton��5S      $��0 30 0 0q�5T      
������ ���#4caf50$��0q�5W      $��20(�5X      $��40��5Y      ��5Z      �� *SendVerificationCommand+��5[      � 5\      $	��580��6   $��40��6+    5]      �� 5^   "   �� 5_   "   $	��15* �63   �� 5`   "   $��5* �63   .+�5b        $��0��66   $��30��6F   $��40��6Q   $ MessageProcessing=�6]   �� 5c      $��1��6$   $��
Resend SMS��64   �� 5d      $��2��6$   $��Right=�6M   $��0 0 32 0q�6p   ��64   �� *Time+6p   5e      5g      �� 5i      $
��Next��5j      $��40��5k      $	��100��5l      $��DemiBold"�5m      
�������$��20(�5o      $��0 30 0 0q�5q      $��0 -5 0 0q�5r      ��5p      �� *NextCommand+5r      5t      5u      5v      5w      