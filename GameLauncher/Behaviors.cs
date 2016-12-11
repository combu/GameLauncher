using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace GameLauncher
{
	public class ListBoxBehavior:Behavior<ListBox>
	{
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.Focus();
			AssociatedObject.KeyDown+=KeyDown;
			AssociatedObject.MouseDoubleClick+=MouseDoubleClick;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
		}

		private void KeyDown(object sender,KeyEventArgs e)
		{
			if(e.Key==Key.Return||e.Key==Key.Space){
				var viewModel=AssociatedObject.DataContext as MainWindowViewModel;
				viewModel.Launch();
			}
		}

		private void MouseDoubleClick(object sender,MouseButtonEventArgs e)
		{
			var viewModel=AssociatedObject.DataContext as MainWindowViewModel;
			viewModel.Launch();
		}
	}
}
