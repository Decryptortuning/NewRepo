using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SerialPortRead
{
	public partial class About : Window
	{
		public About()
		{
			this.InitializeComponent();
		}

		private void About_OK_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}
	}
}