using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;

namespace GameLauncher
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App:Application
	{
		private Mutex mutex=null;

		protected override void OnStartup(StartupEventArgs e)
		{
			var result=false;
			mutex=new Mutex(true,"GameLauncher",out result);
			if(result){
				MainWindow=new MainWindow();
				MainWindow.ShowDialog();
			}else Application.Current.Shutdown();
		}
	}

	public class ViewModelBase:INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChanged(string propertyName)
		{
			if(PropertyChanged!=null) PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
		}
	}

	public enum Theme
	{
		White,Black
	}
}
