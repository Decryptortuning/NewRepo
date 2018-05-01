using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace SerialPortRead
{
	public partial class Help : Window
	{
		public Help()
		{
			this.InitializeComponent();
		}

		private void Help_OK_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}
	}
}