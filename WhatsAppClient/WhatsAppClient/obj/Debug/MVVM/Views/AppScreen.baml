   M S B A M L   `   `   `  ����I  EWhatsAppClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WhatsAppClient.MVVM.ViewsWhatsAppClient.MVVM.Views  5   	   )    #WhatsAppClient.MVVM.Views.AppScreen   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5   	   3local'clr-namespace:WhatsAppClient.MVVM.Views  5   	     � Title$  Chat - WhatsApp��5	   	   $	��660��5
   	   $	��400��5   	    � WindowStartupLocation$ CenterScreen=�5   	    � WindowStyle$
 None=�5   	    � AllowsTransparency .  � 
ResizeMode$ NoResize=�5   	    � Icon$ ../Resource/Logo.png��5   	    J� Clip 5      �� 5   
    �� Rect$ 0,0,400,660�6    �� RadiusX$ 10Q�6/   	 �� RadiusY$	 10Q�6<   .��5      �� 
 �� Orientation$
 Vertical=�6   .+�5   
   �  � RowDefinitions 5      �� 5      $��30 �6    �� 5      $	��630 �6    .+�5      � $��0��6   ���#101D24 � ColumnDefinitions 5      �� 5      $
��0.7* �6'   �� 5      $��6* �6'   �� 5      $
��2.1* �6'   .+�5!      �� $��0��6   $��../Resource/Logo.png��6(   $��30��6F   $��30��6Q   $
��Left=�6]   � 5"      -   5         �Name$ 
contextBar��5"      $��1��6+   .+�5#      �� $�� ��6   
�������$��15(�6:   $
��Left=�6H   $	��362��6c   5$      �� 5&      -   5         ��Name$ minimizeButton��5&      $��2��61   $��_��6A   $��25(�6M   $��50��6[   
�������
������ $��0q�6�    � ToolTip$ Minimize��6�   $
��Left=�6�   $��0 -5 0 0q�6�   ��6�   �� *MinimizeCommand+6�   �� 5'      -   5        $ closeButton��5'      $��2��6.   $��X��6>   $��20(�6J   $��50��6X   
�������
������ $��0q�6�   $ Close��6�   $��Right=�6�   $��0 0 0 0q�6�   ��6�   �� *CloseCommand+6�   5(      � 5*      -   5        $ MainGrid��5*      $��1��6%   ���#101D245+      5,      5-      5.      