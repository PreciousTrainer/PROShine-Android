
using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using System.IO;

namespace PROShine.Droid
{
    [Activity(Label = "PROShine", Icon = "@drawable/botIcon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
            AssetManager assets = this.Assets;
            using (StreamReader sr = new StreamReader(assets.Open("Lang.xml")))
            {
                PROShine.App.LanguageXMLFile = sr.ReadToEnd();
            }
            PROShine.App.DisplayScreenWidth = (double)Resources.DisplayMetrics.WidthPixels / (double)Resources.DisplayMetrics.Density;
            PROShine.App.DisplayScreenHeight = (double)Resources.DisplayMetrics.HeightPixels / (double)Resources.DisplayMetrics.Density;
            PROShine.App.DisplayScaleFactor = (double)Resources.DisplayMetrics.Density;
            PROShine.App.LockScreenAndroid += LockScreen;
            PROShine.App.UnlockAndroid += UnLockScreen;
        }
        public void LockScreen()
        {
            this.Window.SetFlags(WindowManagerFlags.KeepScreenOn, WindowManagerFlags.KeepScreenOn);
        }
        public void UnLockScreen()
        {
            this.Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
        }
    }
}

