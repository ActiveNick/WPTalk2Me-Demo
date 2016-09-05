using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Serialization;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Phone.Speech.Synthesis;
using Windows.Storage;
using Infragistics.Toolkit.Controls.Menus;

namespace WPTalktoMe
{
    public partial class MainView : PhoneApplicationPage
    {
        private const string MESSAGE_FILE = "messages.xml";

        private ObservableCollection<VoiceItem> VoiceItems;
        private SpeechSynthesizer synth;

        public MainView()
        {
            InitializeComponent();

            synth = new SpeechSynthesizer();
            
            // Load Voice Items
            VoiceItems = LoadMessagesFromFile();

            favoritesList.DisplayMemberPath = "voiceString";
            favoritesList.ItemsSource = VoiceItems;

            lstVoices.ItemsSource = InstalledVoices.All;
        }

        private async void btnSayIt_Click(object sender, EventArgs e)
        {
            if (txtInput.Text.Trim().Length > 0)
            {
                this.Focus();

                VoiceItem vi = new VoiceItem();
                vi.voiceString = txtInput.Text.Trim();

                if (chkSaveItem.IsChecked == true)
                {
                    if (!VoiceItems.Any(x=>x.voiceString == vi.voiceString))
                    {
                        VoiceItems.Add(vi);
                        WriteMessagesToFileAsync(VoiceItems);
                    }
                }

                await SpeakStringAsync(vi.voiceString);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtInput.Text = "";
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private async Task SpeakStringAsync(string message)
        {
            try
            {
                // Set the voice as identified by the query.
                synth.SetVoice(lstVoices.SelectedItem as VoiceInformation);

                await synth.SpeakTextAsync(message);
            }
            catch (Exception exc)
            {
                // TO DO: Log the exception somewhere
            }
        }

        private async Task WriteMessagesToFileAsync(ObservableCollection<VoiceItem> voiceitems)
        {
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = iso.CreateFile(MESSAGE_FILE);
            StreamWriter writer = new StreamWriter(stream);

            XmlSerializer ser = new XmlSerializer(typeof(ObservableCollection<VoiceItem>));

            ser.Serialize(writer, voiceitems);

            writer.Close();
            writer.Dispose();
        }

        private ObservableCollection<VoiceItem> LoadMessagesFromFile()
        {
            ObservableCollection<VoiceItem> voiceitems = new ObservableCollection<VoiceItem>();

            IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();

            if (storage.FileExists(MESSAGE_FILE))
            {
                IsolatedStorageFileStream stream = storage.OpenFile(MESSAGE_FILE, FileMode.Open);
                XmlSerializer xml = new XmlSerializer(typeof(ObservableCollection<VoiceItem>));
                voiceitems = xml.Deserialize(stream) as ObservableCollection<VoiceItem>;
                stream.Close();
                stream.Dispose();
            }

            return voiceitems;
        }

        private void favoritesList_ItemClicked(object sender, Infragistics.Controls.Grids.ListItemEventArgs e)
        {
            VoiceItem vi = e.Item.Data as VoiceItem;
            SpeakStringAsync(vi.voiceString);
        }

        private async void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            VoiceItem vi = GetSelectedVoiceItem(sender);
            VoiceItems.Remove(vi);
            await WriteMessagesToFileAsync(VoiceItems);
        }

        private async void mnuPlay_Click(object sender, RoutedEventArgs e)
        {
            VoiceItem vi = GetSelectedVoiceItem(sender);
            await SpeakStringAsync(vi.voiceString);
        }

        static VoiceItem GetSelectedVoiceItem(Object MenuItem)
        {
            var parentControl = ((Infragistics.Toolkit.Controls.Menus.MenuItem)MenuItem).ContextMenuOwner as FrameworkElement;
            return (VoiceItem)parentControl.DataContext;
        }

        private async void txtSayTime_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string msg = "The current time is " + DateTime.Now.ToShortTimeString();
            await SpeakStringAsync(msg);
            lstExtras.SelectedIndex = -1;
        }

        private async void txtSayDate_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string msg = "Today's date is " + DateTime.Now.ToLongDateString();
            await SpeakStringAsync(msg);
            lstExtras.SelectedIndex = -1;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    ApplicationBar = ((ApplicationBar)Resources["appBar1"]);
                    break;

                case 1:
                case 2:
                    ApplicationBar = ((ApplicationBar)Resources["appBar2"]);
                    break;
            }
        }

    }
}