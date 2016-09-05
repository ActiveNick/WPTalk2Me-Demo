using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Info;

namespace WPTalktoMe
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void btnFollow_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://mobile.twitter.com/activenick", UriKind.Absolute);
            webBrowserTask.Show();
        }

        private void btnReview_Click(object sender, RoutedEventArgs e)
        {
            // pop up the link to rate and review the app 
            MarketplaceReviewTask review = new MarketplaceReviewTask(); 
            review.Show(); 
        }

        private void urlWeb_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://www.activenick.net", UriKind.Absolute);
            webBrowserTask.Show();

        }

        private void urlFeedback_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().FullName;
            Version assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            string msg = "AUTO-GENERATED APP METADATA: DO NOT DELETE" + Environment.NewLine;
            msg += "Assembly Name: " + assemblyName + Environment.NewLine;
            msg += "Version: " + assemblyVersion + Environment.NewLine;
            msg += "Device OEM: " + DeviceStatus.DeviceManufacturer + Environment.NewLine;
            msg += "Total Memory: " + DeviceStatus.DeviceTotalMemory.ToString("N") + Environment.NewLine;
            msg += "Allowed Memory: " + DeviceStatus.ApplicationMemoryUsageLimit.ToString("N") + Environment.NewLine;
            msg += "Current Memory: " + DeviceStatus.ApplicationCurrentMemoryUsage.ToString("N") + Environment.NewLine;
            msg += Environment.NewLine;
            msg += "{type your message here}";

            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Windows Phone App Feedback: Talk to Me";
            emailComposeTask.Body = msg;
            emailComposeTask.To = "feedback@activenick.net";

            emailComposeTask.Show();

        }
    }
}