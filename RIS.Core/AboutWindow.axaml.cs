using Avalonia;
using Avalonia.Platform;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Runtime.InteropServices;
using System;

namespace RIS.Core
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.PreferSystemChrome;
            this.ExtendClientAreaToDecorationsHint = true;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (Environment.OSVersion.Version.Major >= 10)
                {
                    if (Environment.OSVersion.Version.Build >= 22000)
                    {
                        TransparencyLevelHint = WindowTransparencyLevel.Mica;
                    }
                    else
                    {
                        TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur;
                    }
                }
            }
            VersionText.Text = $"Version:{this.GetType().Assembly.GetName().Version}-Preview";
        }

    }
}
