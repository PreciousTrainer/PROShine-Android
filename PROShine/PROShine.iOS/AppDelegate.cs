using Foundation;
using System.IO;
using UIKit;

namespace PROShine.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());
            PROShine.App.DisplayScreenWidth = (double)UIScreen.MainScreen.Bounds.Width;
            PROShine.App.DisplayScreenHeight = (double)UIScreen.MainScreen.Bounds.Height;
            PROShine.App.DisplayScaleFactor = (double)UIScreen.MainScreen.Scale;
            using (StreamReader sm = new StreamReader("Lang.xml"))
            {
                PROShine.App.LanguageXMLFile = sm.ReadToEnd();
            }
            PROShine.App.LockScreeniOS += lockScreen;
            PROShine.App.UnlockiOS += unLockScreen;
            return base.FinishedLaunching(app, options);
        }
        public void lockScreen()
        {
            UIApplication.SharedApplication.IdleTimerDisabled = true;
        }
        public void unLockScreen()
        {
            UIApplication.SharedApplication.IdleTimerDisabled = false;
        }
    }
}
