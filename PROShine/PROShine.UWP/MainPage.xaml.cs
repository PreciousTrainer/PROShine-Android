using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms;

namespace PROShine.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            PROShine.App.DisplayScreenHeight = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds.Height;
            PROShine.App.DisplayScreenWidth = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds.Width;
            PROShine.App.DisplayScaleFactor = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            using (StreamReader sm = new StreamReader(@"Lang.txt"))
            {
                PROShine.App.LanguageXMLFile = sm.ReadToEnd();
            }

            LoadApplication(new PROShine.App());
        }
    }
}
