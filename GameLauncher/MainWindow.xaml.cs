using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameLauncher
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow:Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Messenger.Default.Register<ChangeThemeMessage>(this,ChangeTheme);
#if DEBUG
			Topmost=false;
#endif
			//RenderOptions.SetBitmapScalingMode(image,BitmapScalingMode.NearestNeighbor);
		}

		private void ChangeTheme(ChangeThemeMessage message)
		{
			var theme=message.Theme.ToString()+"_";
			var brushNames=from name in Resources.Keys.Cast<string>()
						   where name.StartsWith("White_")
						   select name.Replace("White_","");
			foreach(var brushName in brushNames) Resources[brushName]=Resources[theme+brushName];
		}

		private void Window_Closing(object sender,System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel=true;
		}

		private void Window_Loaded(object sender,RoutedEventArgs e)
		{
			var viewModel=DataContext as MainWindowViewModel;
			ChangeTheme(new ChangeThemeMessage(viewModel.Theme));
		}
	}
}
