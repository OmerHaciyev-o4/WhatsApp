   M S B A M L   `   `   `  ����I  EWhatsAppClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WhatsAppClient.MVVM.ViewsWhatsAppClient.MVVM.Views  5   	   +    %WhatsAppClient.MVVM.Views.LoginScreen   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   9local'clr-namespace:WhatsAppClient.MVVM.Views     5   	   _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	     � Title$  Login Screen��5	   	   $	��700��5
   	   $	��550��5   	   ���#101d24 � WindowStartupLocation$ CenterScreen=�5   	    � AllowsTransparency .  � 
ResizeMode$ NoResize=�5   	    � WindowStyle$
 None=�5   	    � Icon$ \MVVM\Resource\Logo.png��5   	    J� Clip 5      �� 5   
    �� Rect$ 0,0,550,700�6    �� RadiusX$ 10Q�6/   	 �� RadiusY$	 10Q�6<   .��5      �� 
 �� Orientation$
 Vertical=�6   .+�5   
   �  � RowDefinitions 5      �� 5      $��30 �6    �� 5      $	��700 �6    .+�5      � $��0��6   ���#101d24 � ColumnDefinitions 5      �� 5      $
��0.4* �6'   �� 5      $��6* �6'   �� 5       $��1.35* �6'   .+�5"      �� $��0��6   $��../Resource/Logo.png��6(   $��30��6F   $��30��6Q   $
��Left=�6]   � 5#      -   5         �Name$ 
contextBar��5#      $��1��6+   .+�5$      �� $��Verification Screen��6   
�������$��15(�6M   $
��Left=�6[   $	��362��6v   5%      �� 5'      -   5         ��Name$ minimizeButton��5'      $��2��61   $��_��6A   $��25(�6M   $��50��6[   
�������
������ $��0q�6�    � ToolTip$ Minimize��6�   $
��Left=�6�   $��0 -5 0 0q�6�   ��6�   �� *MinimizeCommand+6�   �� 5(      -   5        $ closeButton��5(      $��2��6.   $��X��6>   $��20(�6J   $��50��6X   
�������
������ $��0q�6�   $ Close��6�   $��Right=�6�   $��0 0 0 0q�6�   ��6�   �� *CloseCommand+6�   5)      � 5+      $��1��6    5,      �� 5-      $
��0.5* �6$   �� 5.      �� 5/      $
��0.5* �6$   �� 50      �� 51      �� 52      .+�55      �� $��Verify your phone number��6   $��45(�6;   
�������$��Center=�6\   �� 56      $��1��6   .+�57      �� $0��*WhatsApp will send an SMS message (carrier��6   $��20(�6Q   ���#9ba1a3$��Center=�6t   �� 58      $5��/charges may apply) to verify your phone number.��6   $��20(�6V   ���#9ba1a3$��Center=�6y   �� 59      $/��)Enter your country code and phone number:��6   $��20(�6P   ���#9ba1a3$��Center=�6s   5:      � 5;      $��2��6    5<      �� 5=      �� 5>      $	��10* �6+   �� 5?      .+�5A      �� -   5         ��Name$ 	Countries��5A      $��1��62   $	��480��6B   $��40��6N   
�������$��Center=�6m    �� SelectedIndex$ 0��6�   ��6�   �� *Items+6�   5B      � 5C      $��3��6    5D      �� 5E      �� 5F      $	��10* �6+   �� 5G      .+�5I      �� $��1��6!   $
 
Horizontal=�61   .+�5J      �� -   5         ��Name$ 
regionCode��5J   "   $��40��6�   $��25(�6�   $	��110��6�   
�������$��	12 0 37 0q�6�   $ Region code��6�   ��66   �� *RegionCodeText+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5K      -   5        $ phonNum��5K   "    �� 	MaxLength$ 13��6T   $��40��6c   $��25(�6o   $	��330��6}   
�������)  #MaterialDesignThemes.Wpf.HintAssist   Hint$ Phone number��6�   $ Phone number��6�   ��63   �� *PhoneNumberText+6�   5L      5M      � 5N      $��4��6    5O      �� 5P      �� 5Q      $
��0.7* �6+   �� 5R      .+�5T      �� $��1��6   $
��Next��6-   $��50��6<   $��DemiBold"�6H   
�������$��25(�6q   ��6   �� *NextCommand+5U      �� 5V      $��5��6   .+�5W      �� $
 
Horizontal=�6!   $��Center=�6:   .+�5X      �� $��You must be��6    ���#9ba1a3$��20(�6K   �� 5Y      $��at least 16 years old��6    ���#68bbe4$��20(�6U   �� 5Z      $��to register.  Learn how��6    ���#9ba1a3$��20(�6W   5[      �� 5\      $
 
Horizontal=�6!   $��Center=�6:   .+�5]      �� $��WhatsApp works with the��6    ���#9ba1a3$��20(�6W   �� 5^      $��IT Step Companies��6    ���#68bbe4$��20(�6Q   �� 5_      $��.��6    ���#9ba1a3$��-8 0 0 0q�6A   $��20(�6S   5`      5a      5b      5c      5d      5e      