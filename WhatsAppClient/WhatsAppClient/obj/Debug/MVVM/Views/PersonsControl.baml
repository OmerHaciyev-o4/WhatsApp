   M S B A M L   `   `   `  ����I  EWhatsAppClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullE'clr-namespace:WhatsAppClient.MVVM.ViewsWhatsAppClient.MVVM.Views  5      .    (WhatsAppClient.MVVM.Views.PersonsControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      9local'clr-namespace:WhatsAppClient.MVVM.Views     5      _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5      ���#101D24.��5      �   � RowDefinitions  5   
   �� 5      �� 5      �� 5      $	��10* �6   .+�5   
   � -   5         �Name$ 
firstPanel��5      $��0��5      $y�Visible=�5      .+�5      �� $��WhatsApp��5      ���#9ba1a3$��25(�5      $��DemiBold"�5      $
��Left=�5      $��5 0 0 0q�5      �� 5       �� Orientation$ 
Horizontal=�5      $��Right=�5       .+�5!      �� $��45��5"      $��45��5#      $��0 0 0 0q�5$      
������ $��0q�5&      ��5'      �� *SearchCommand+��5(      '  !MaterialDesignThemes.Wpf.PackIcon  5)         Kind$ Magnify=�5*      $��30��5+      $��30��5,      ���#9BA1A35-      5/      '  !MaterialDesignThemes.Wpf.PopupBox  50      ���#9BA1A3$��2 0 2 0q�52         PopupContent. 53      �� ���#101C24.+�54      �� $��WhatsApp Web pass��55      ���#9BA1A356      �� 57      $��Contact��58      ���#9BA1A3��5:      �� *OpenContactViewCommand+�� 5;      $
��Info��5<      ���#9BA1A35=      5>      5?      5@      5A      �� 5C   
   -   5         ��Name$ 	lastPanel��5D      $��0��5E      $ 
Horizontal=�5F      $y�Hidden=�5G      .+�5H      �� $��30��5I      $��30��5J      
������ $��0q�5L      $��0 0 0 0q�5M      ��5N      �� *DefaultCommand+��5O        5P      $ 	ArrowLeft=�5Q      $��30��5R      $��30��5S      ���#9BA1A35T      5V      �� 5W      -   5         ��Name$ 	searchBox��5X      )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ 	Search...��5Z      ���#9BA1A3$��30��5\      $��20(�5]      $	��340��5^      $��10 0 0 0q�5_      $��	0 -10 0 0q�5`      ��5Y      �� *
SearchText+ �� Mode$ TwoWay=�	 �� UpdateSourceTrigger$	 PropertyChanged=�5`      5a      �� 5c   
   $��1��6   $ 
Horizontal=�6"   .+�5d      �� 
������ $��0q�5f      $��40��5g      $��40��5h      $��0 0 0 0q�5i      ��5j        5k      $ Camera=�5l      ���#9BA1A3$��30��5n      $��30��5o      5q      �� 5r      $ 
Horizontal=�5s      .+�5t      	� -   5        
 	�Name$
 
chatsPanel��5u      $	��110��5v          MaterialDesignTabRadioButton#��[  5w      $��4q�5x       d� 	IsChecked$
 TrueY�5y      $��CHATS��5z      ���#57f573���#57f5735|      	� 5}      -   5        $
 statusPanel��5~      $	��110��5      #��[  5�      $��4q�5�      $ FalseY�5�      $��STATUS��5�      ���#9BA1A3���#57f5735�      	� 5�      -   5        $
 
callsPanel��5�      $	��110��5�      #��[  5�      $��4q�5�      $ FalseY�5�      $��CALLS��5�      ���#9BA1A3���#57f5735�      5�      5�      �� 5�   
   $��2��6   .+�5�      �� -   5         ��Name$ Persons��5�      $	��545��5�      ��5�      �� *Items+$ TwoWay=�$	 PropertyChanged=�5�      5�      5�      