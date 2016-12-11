using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace GameLauncher
{
	public class Messenger
	{
		private static Messenger instance=new Messenger();
		
		public static Messenger Default
		{
			get
			{
				return instance;
			}
		}

		private List<ActionInfo> info=new List<ActionInfo>();

		public void Register<MessageType>(FrameworkElement element,Action<MessageType> action)
		{
			info.Add(new ActionInfo(typeof(MessageType),element.DataContext as INotifyPropertyChanged,action));
		}
		
		public void Send<MessageType>(INotifyPropertyChanged sender,MessageType message)
		{
			info.ForEach(i=>
			{
				if(i.Sender==sender&&i.Type==typeof(MessageType)) (i.Action as Action<MessageType>)(message);
			});
		}

		private class ActionInfo
		{
			public Type Type{get;private set;}
			public INotifyPropertyChanged Sender{get;private set;}
			public Delegate Action{get;private set;}

			public ActionInfo(Type type,INotifyPropertyChanged sender,Delegate action)
			{
				Type=type;
				Sender=sender;
				Action=action;
			}
		}
	}

	public class ChangeThemeMessage
	{
		public Theme Theme{get;private set;}

		public ChangeThemeMessage(Theme theme)
		{
			Theme=theme;
		}
	}
}
