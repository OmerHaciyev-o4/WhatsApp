   M S B A M L   `   `   `  ����I  EWhatsAppClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WhatsAppClient.MVVM.ViewsWhatsAppClient.MVVM.Views  5   	   ,    &WhatsAppClient.MVVM.Views.PolicyScreen   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:WhatsAppClient.MVVM.Views     5   	     � Title$  Welcome��5   	   $	��720��5	   	   $	��600��5
   	    � WindowStartupLocation$ CenterScreen=�5   	    � 
ResizeMode$ NoResize=�5   	    � WindowStyle$
 None=�5   	    � AllowsTransparency . 
������  � Icon$ \MVVM\Resource\Logo.png��5   	    J� Clip 5      �� 5   
    �� Rect$ 0,0,600,720�6    �� RadiusX$ 10Q�6/   	 �� RadiusY$	 10Q�6<   .��5      �� 
 �� Orientation$
 Vertical=�6   .+�5   
   �  � RowDefinitions 5      �� 5      $��30 �6    �� 5      $	��690 �6    .+�5      � $��0��6   ���#101d24 � ColumnDefinitions 5      �� 5      $
��0.4* �6'   �� 5      $��6* �6'   �� 5      $��1.35* �6'   .+�5!      �� $��0��6   $��../Resource/Logo.png��6(   $��30��6F   $��30��6Q   $
��Left=�6]   � 5"      -   5         �Name$ 
contextBar��5"      $��1��6+   .+�5#      �� $��Verification Screen��6   
�������$��15(�6M   $
��Left=�6[   $	��362��6v   5$      �� 5&      -   5         ��Name$ minimizeButton��5&      $��2��61   $��_��6A   $��25(�6M   $��50��6[   
�������
������ $��0q�6�    � ToolTip$ Minimize��6�   $
��Left=�6�   $��0 -5 0 0q�6�   ��6�   �� *MinimizeCommand+6�   �� 5'      -   5        $ closeButton��5'      $��2��6.   $��X��6>   $��20(�6J   $��50��6X   
�������
������ $��0q�6�   $ Close��6�   $��Right=�6�   $��0 0 0 0q�6�   ��6�   �� *CloseCommand+6�   5(      � 5*      $��1��6   ���#101d24 5+      �� 5,      �� 5-      $��5* �6$   �� 5.      �� 5/      �� 50      .+�53      �� $��Welcome to Whatsapp��6   
�������$��40(�6I   $��Center=�6W   �� 54      $��1��6   $*��$../Resource/LoginScreenCenterImg.png��6%   $��Center=�6S   �� 55      $��2��6   .+�56      �� $
 
Horizontal=�6!   $��Center=�6:   .+�57      �� $��15(�6    ���#9ba1a3$��Read our��6C   �� 58      $��15(�6    ���#68bbe4$��Privacy Policy.��6C   �� 59      $��15(�6    ���#9ba1a3$!��Tap "Agree and continue" to��6C   5:      �� 5;      $
 
Horizontal=�6!   $��Center=�6:   .+�5<      �� $��15(�6    ���#9ba1a3$��
accept the��6C   �� 5=      $��15(�6    ���#68bbe4$��Terms of Service��6C   �� 5>      $��15(�6    ���#9ba1a3$��.��6C   $��	-10 0 0 0q�6O   5?      5@      �� 5A      $��3��6   $��45��6&   $	��295��62   ���#00af9c$��AGREE AND CONTINUE��6S   $��DemiBold"�6p   $��15(�6�   ��6�   �� *ContinueCommand+�� 5B      $��4��6   .+�5C      �� $��15(�6   ���#9ba1a3$��Center=�6?   $
��from��6\   �� 5D      $��15(�6   ���#9ba1a3$��Center=�6?   $��IT Step Academy��6\   5E      5F      5G      5H      5I      