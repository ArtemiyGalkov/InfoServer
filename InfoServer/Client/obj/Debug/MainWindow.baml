   M S B A M L   `   `   `  ����A  =Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:ClientGUI	ClientGUI  5   	       ClientGUI.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   )localclr-namespace:ClientGUI     5   	     � Title$  
InfoClient��5   	   $	��450��6   $	��300��6)    � 	Resources 5	         listTemplate �� Path��%  &	        �� 5
   
   .R�5      �� -   5        $��5q�5      .+�5      �� $y�	Collapsed=�60   ��6   �� *Id+60   �� 5      ��6   �� *Image+�� 5      $��16(�6   $��Center=�6G   ��6*   �� $
 Name�6G   5      5      5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��10 �6   �� 5      $
��0.4* �6   �� 5      $��0 �6    � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $
��auto �6   �� 5      �� 5      $
��auto �6   �� 5      $��10 �6   .+�5    
   o� $��0��6   $��0��6   $��5��6,   $
��auto��6@   $	��Top=�6N   .@�5!      m� -   5        $��
Load cards��5!      6+   m� 5"      -   5        $��New card��5"      6)   m� 5#      $
��Sort��6   .@�5$      m� -   5        $��By name��5$      6,   m� 5%      -   5        $��Default��5%      6,   5&      m� 5'      -   5        $��About��5'      6&   5(      �� 5*   
   $��1��6   $��3��6$   $��1��65   .��5+      �� -   5         ��Name$ recordsList��5+      #��[  6+   $��Disabled=�5,      5-      5.      5/      