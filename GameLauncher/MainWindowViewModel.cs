using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GameLauncher
{
	class MainWindowViewModel:ViewModelBase
	{
		private GameViewModel selectedGame;
		private Visibility visibility;
		private Theme theme;

		public IEnumerable<GameViewModel> Games{get;private set;}
		public Command ExitCommand{get;private set;}
		public Process LaunchedProcess{get;private set;}

		public GameViewModel SelectedGame
		{
			get
			{
				return selectedGame;
			}
			set
			{
				selectedGame=value;
				NotifyPropertyChanged("SelectedGame");
			}
		}

		public Visibility Visibility
		{
			get
			{
				return visibility;
			}
			set
			{
				visibility=value;
				NotifyPropertyChanged("Visibility");
			}
		}

		public Theme Theme
		{
			get
			{
				return theme;
			}
			set
			{
				theme=value;
				NotifyPropertyChanged("Theme");
				Messenger.Default.Send(this,new ChangeThemeMessage(value));
			}
		}

		public MainWindowViewModel()
		{
			Environment.CurrentDirectory=Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
			var defaultSettings=new[]{"","White","True"};
			var settingPath=Environment.CurrentDirectory+"\\setting.txt";
			var settings=File.Exists(settingPath)?File.ReadAllLines(settingPath,Encoding.Default):defaultSettings;
			if(settings.Length<3){
				settings=defaultSettings;
				try{
					File.WriteAllText(settingPath,string.Join("\r\n",defaultSettings),Encoding.Default);
					MessageBox.Show("設定が壊れていたのでデフォルトの設定で上書きしました。\n書き直してください。","GameLauncher",MessageBoxButton.OK,MessageBoxImage.Error);
				}catch(Exception){
				}
				System.Windows.Application.Current.Shutdown();
				return;
			}
			if(!File.Exists(settingPath)){
				try{
					File.WriteAllText(settingPath,string.Join("\r\n",defaultSettings),Encoding.Default);
				}catch(Exception){
				}
			}
			bool showExitMenu=true;
			bool.TryParse(settings[2],out showExitMenu);
			var gameDirectory=string.IsNullOrEmpty(settings[0])||string.IsNullOrWhiteSpace(settings[0])?Environment.CurrentDirectory:settings[0];
			if(showExitMenu) Games=GameViewModel.GetAllGamesWithExitMenu(gameDirectory);
			else Games=GameViewModel.GetAllGames(gameDirectory);
			var parseResult=Theme.White;
			Enum.TryParse(settings[1],true,out parseResult);
			Theme=Enum.IsDefined(typeof(Theme),parseResult)?parseResult:Theme.White;
			ExitCommand=new Command(()=>System.Windows.Application.Current.Shutdown());
			LaunchedProcess=null;
			Visibility=Visibility.Visible;
		}

		public void Launch()
		{
			LaunchedProcess=SelectedGame.Launch();
			if(LaunchedProcess==null) return;
			LaunchedProcess.EnableRaisingEvents=true;
			LaunchedProcess.Exited+=Exited;
			Visibility=Visibility.Hidden;
		}

		private void Exited(object sender,EventArgs e)
		{
			LaunchedProcess=null;
			Visibility=Visibility.Visible;
		}
	}
}
