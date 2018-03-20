using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using PROBot;

namespace PROShine
{
	public partial class App : Application
	{
		public static double DisplayScreenWidth = 0f;
		public static double DisplayScreenHeight = 0f;
		public static double DisplayScaleFactor = 0f;
		public static string LanguageXMLFile = "";
		public static Action LockScreenAndroid;
		public static Action LockScreeniOS;
		public static Action UnlockAndroid;
		public static Action UnlockiOS;
		public App ()
		{
			InitializeComponent();
			MainPage = new PROShine.MainPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
