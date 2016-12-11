using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GameLauncher
{
	public class GameViewModel:ViewModelBase
	{
		private static readonly string exitMenuString="[[[[[exit]]]]]";

		public string Name{get;private set;}
		public string ReadmeText{get;private set;}
		public string ExecutableFileName{get;private set;}
		public ImageSource Icon{get;private set;}

		private GameViewModel(string name,string readmeText,string executableFileName,ImageSource icon)
		{
			Name=name;
			ReadmeText=readmeText;
			ExecutableFileName=executableFileName;
			Icon=icon;
		}

		private static bool IsReadmeFile(string fileName)
		{
			var patternString="(read\\s?me|(取扱?い?)?説明?書?|(始|初|はじ)め?に|(遊|あそ)び(方|かた))\\.txt";
			return Regex.IsMatch(fileName,patternString,RegexOptions.IgnoreCase);
		}

		public static IEnumerable<GameViewModel> GetAllGames(string path)
		{
			path=Path.IsPathRooted(path)?path:Environment.CurrentDirectory+"\\"+path;
			var directory=new DirectoryInfo(path);

			foreach(var dir in directory.GetDirectories()){
				FileInfo[] files;
				try{
					files=dir.GetFiles();
				}catch(Exception){
					continue;
				}
				var readmeFileName=files.Where(file=>IsReadmeFile(file.Name)).Select(file=>file.FullName).FirstOrDefault();
				var executableFileName=files.Where(file=>Regex.IsMatch(file.Name,".+\\.exe",RegexOptions.IgnoreCase)).Select(file=>file.FullName).FirstOrDefault();
				ImageSource image=null;
				if(executableFileName!=null){
					var icon=new System.Drawing.Icon(System.Drawing.Icon.ExtractAssociatedIcon(executableFileName),new System.Drawing.Size(32,32));
					image=Imaging.CreateBitmapSourceFromHIcon(icon.Handle,Int32Rect.Empty,BitmapSizeOptions.FromEmptyOptions());
				}
				var readmeText=readmeFileName!=null?File.ReadAllText(readmeFileName,Encoding.Default):"説明なし";
				yield return new GameViewModel(dir.Name,readmeText,executableFileName,image);
			}
		}

		public static IEnumerable<GameViewModel> GetAllGamesWithExitMenu(string path)
		{
			var games=GetAllGames(path);
			var exit=new GameViewModel("終了","このランチャーを終了します。",exitMenuString,null);
			return new[]{exit}.Concat(games);
		}

		public Process Launch()
		{
			if(ExecutableFileName==null) return null;
			else if(ExecutableFileName==exitMenuString){
				Application.Current.Shutdown();
				return null;
			}
			var file=new FileInfo(ExecutableFileName);
			var startInfo=new ProcessStartInfo(ExecutableFileName);
			startInfo.WorkingDirectory=file.DirectoryName;
			return Process.Start(startInfo);
		}
	}
}
