using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PROBot;
using PROProtocol;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;

namespace PROShine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatView : TabbedPage
    {
        private BotClient _bot;
        private ContentPage _localChatTab;
        private Dictionary<string, ContentPage> _channelTabs;
        private Dictionary<string, ContentPage> _pmTabs;
        private Dictionary<string, ContentPage> _channelPmTabs;
        private Dictionary<string, Entry> TextInputs;
        private Dictionary<string, Editor> ChatPanels;
        private TabbedPage TabControl;
        private Editor ChatPanel;
        private Entry InputEntry;
        public ChatView (BotClient bot)
        {
            _bot = bot;
            InitializeComponent();
            Title = "Chat";
            InputEntry = Device.Android == Device.RuntimePlatform ? new Entry { WidthRequest = 50, FontSize = 8, Placeholder = "Send message" } : new Entry { WidthRequest = 50, Placeholder = "Send message" };
            ChatPanel = Device.Android == Device.RuntimePlatform ? new Editor { HeightRequest = 130, FontSize = 8 } : new Editor { HeightRequest = 130 };
            
            TextInputs = new Dictionary<string, Entry>();
            ChatPanels = new Dictionary<string, Editor>();
            TabControl = new TabbedPage();
            Button sendButton = Device.Android == Device.RuntimePlatform ? new Button { Text = "Send", FontSize = 8 } : new Button { Text = "Send" };
            var grid = new Grid { RowSpacing = 1, ColumnSpacing = 1 };
            grid.HorizontalOptions = LayoutOptions.FillAndExpand;
            grid.RowDefinitions = new RowDefinitionCollection
            {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
            };
            var SL = new StackLayout
            {
                Children =
                {
                    ChatPanel,
                }
            };
            var stL = new StackLayout
            {
                Children =
                {
                    InputEntry,
                    sendButton,
                },
                VerticalOptions = LayoutOptions.Center,
            };
            grid.Children.Add(SL, 0, 0);
            grid.Children.Add(stL, 0, 1);
            _localChatTab = new ContentPage
            {
                Title = "LCL",
                Content = grid
            };
            TabControl.Children.Add(_localChatTab);
            ChatPanels["Local"] = ChatPanel;
            TextInputs["Local"] = InputEntry;
            sendButton.Clicked += SendButton_Clicked; ;
            _channelTabs = new Dictionary<string, ContentPage>();
            AddChannelTab("All");
            AddChannelTab("Trade");
            AddChannelTab("Battle");
            AddChannelTab("Other");
            AddChannelTab("Help");
            _pmTabs = new Dictionary<string, ContentPage>();
            _channelPmTabs = new Dictionary<string, ContentPage>();
            Children.Add(TabControl);
        }

        private void SendButton_Clicked(object sender, EventArgs e)
        {
            if (_bot.Game != null && _bot.Game.IsMapLoaded)
            {
                ContentPage tab = TabControl.CurrentPage as ContentPage;
                SendChatInput(TextInputs[tab.Title.Replace("#", "")].Text);
            }
        }

        public void Client_RefreshChannelList()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                IList<ChatChannel> channelList;
                lock (_bot)
                {
                    channelList = _bot.Game.Channels.ToArray();
                }
                if (channelList.Count > 0 || channelList != null)
                {
                    foreach (ChatChannel channel in channelList)
                    {
                        if (!_channelTabs.ContainsKey(channel.Name))
                        {
                            AddChannelTab(channel.Name);
                        }
                    }
                    foreach (string key in _channelTabs.Keys.ToArray())
                    {
                        if (!(channelList.Any(e => e.Name == key)))
                        {
                            RemoveChannelTab(key);
                        }
                    }
                }
            });
        }

        public void Client_LeavePrivateMessage(string conversation, string mode, string leaver)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (leaver == _bot.Game.PlayerName)
                {
                    return;
                }
                AddPrivateSystemMessage(conversation, mode, leaver, "has closed the PM window");
            });
        }
        private void AddPrivateSystemMessage(string conversation, string mode, string author, string message)
        {
            message = Regex.Replace(message, @"\[.+?\]", "");

            if (mode != null)
            {
                author = "[" + mode + "]" + author;
            }
            if (!_pmTabs.ContainsKey(conversation))
            {
                AddPmTab(conversation);
            }
            ChatPanels[conversation].Text += "[" + DateTime.Now.ToLongTimeString() + "] " + author + ": " + message + Environment.NewLine;
        }

        public void Client_ChatMessage(string mode, string author, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                AddChatMessage(mode, author, message);
            });
        }
        private void AddChatMessage(string mode, string author, string message)
        {
            if (mode != null)
            {
                author = "[" + mode + "]" + author;
            }
            message = Regex.Replace(message, @"\[.+?\]", "");
            ChatPanels["Local"].Text += "[" + DateTime.Now.ToLongTimeString() + "] " + author + ": " + message + Environment.NewLine;
        }
        public void Client_ChannelMessage(string channelName, string mod, string author, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                AddChannelMessage(channelName, mod, author, message);
            });
        }
        private void AddChannelMessage(string channelName, string mode, string author, string message)
        {
            message = Regex.Replace(message, @"\[.+?\]", "");

            if (mode != null)
            {
                author = "[" + mode + "]" + author;
            }
            if (!_channelTabs.ContainsKey(channelName))
            {
                AddChannelTab(channelName);
            }
            ChatPanels[channelName].Text += "[" + DateTime.Now.ToLongTimeString() + "] " + author + ": " + message + Environment.NewLine;
        }
        public void Client_ChannelSystemMessage(string channelName, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                AddChannelSystemMessage(channelName, message);
            });
        }
        private void AddChannelSystemMessage(string channelName, string message)
        {

            message = Regex.Replace(message, @"\[.+?\]", "");

            if (!_channelTabs.ContainsKey(channelName))
            {
                AddChannelTab(channelName);
            }

            ChatPanels[channelName].Text += "[" + DateTime.Now.ToLongTimeString() + "] SYSTEM: " + message + Environment.NewLine;

            //(_channelTabs[channelName].Content as ChatPanel).ChatBox.AppendText(
            //    "[" + DateTime.Now.ToLongTimeString() + "] SYSTEM: " + message + '\r');
        }
        private void SendChatInput(string text)
        {
            if (text == "" || text.Replace(" ", "") == "")
            {
                return;
            }
            lock (_bot)
            {
                if (_bot.Game == null)
                {
                    return;
                }
                ContentPage tab = TabControl.CurrentPage as ContentPage;
                text = Regex.Replace(text, @"\[(-|.{6})\]", "");
                TextInputs[tab.Title.Replace("#", "")].Text = "";

                if (text.Length == 0) return;
                if (_localChatTab == tab)
                {
                    text = text.Replace('|', '#');
                    _bot.Game.SendMessage(text);
                }
                else if (_channelTabs.ContainsValue(tab as ContentPage))
                {
                    text = text.Replace('|', '#');
                    if (text[0] == '/')
                    {
                        _bot.Game.SendMessage(text);
                        return;
                    }
                    string channelName = (string)tab.Title.Replace("#", "");
                    ChatChannel channel = _bot.Game.Channels.FirstOrDefault(e => e.Name == channelName);
                    if (channel == null)
                    {
                        return;
                    }
                    _bot.Game.SendMessage("/" + channel.Id + " " + text);
                }
                else if (_pmTabs.ContainsValue(tab as ContentPage))
                {
                    text = text.Replace("|.|", "");
                    _bot.Game.SendPrivateMessage((string)tab.Title.Replace("#", ""), text);
                }
                else if (_channelPmTabs.ContainsValue(tab as ContentPage))
                {
                    text = text.Replace('|', '#');
                    if (text[0] == '/')
                    {
                        _bot.Game.SendMessage(text);
                        return;
                    }
                    string conversation = (string)tab.Title.Replace("#", "");
                    _bot.Game.SendMessage("/send " + conversation + ", " + text);
                }
            }
        }
        public void Client_ChannelPrivateMessage(string conversation, string mode, string author, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                AddChannelPrivateMessage(conversation, mode, author, message);
            });
        }
        private void AddChannelPrivateMessage(string conversation, string mode, string author, string message)
        {
            message = Regex.Replace(message, @"\[.+?\]", "");

            if (mode != null)
            {
                author = "[" + mode + "]" + author;
            }
            if (!_channelPmTabs.ContainsKey(conversation))
            {
                AddChannelPmTab(conversation);
            }
            ChatPanels[conversation].Text += "[" + DateTime.Now.ToLongTimeString() + "] " + author + ": " + message + Environment.NewLine;
        }

        public void Client_PrivateMessage(string conversation, string mode, string author, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                AddPrivateMessage(conversation, mode, author, message);
            });
        }
        private void AddPrivateMessage(string conversation, string mode, string author, string message)
        {
            message = Regex.Replace(message, @"\[.+?\]", "");

            if (mode != null)
            {
                author = "[" + mode + "]" + author;
            }
            if (!_pmTabs.ContainsKey(conversation))
            {
                AddPmTab(conversation);
            }
            ChatPanels[conversation].Text += "[" + DateTime.Now.ToLongTimeString() + "] " + author + ": " + message + Environment.NewLine;
        }

        public void Client_EmoteMessage(string mode, string author, int emoteId)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                AddEmoteMessage(mode, author, emoteId);
            });
        }
        private void AddEmoteMessage(string mode, string author, int emoteId)
        {
            if (mode != null)
            {
                author = "[" + mode + "]" + author;
            }
            ChatPanels["Local"].Text += "[" + DateTime.Now.ToLongTimeString() + "] " + author + " is " + ChatEmotes.GetDescription(emoteId) + Environment.NewLine;
        }
        private void AddChannelTab(string tabName)
        {
            Button tabCloseButton = Device.Android == Device.RuntimePlatform ? new Button { Text = "Close", FontSize = 8 } : new Button { Text = "Close" };
            Button sendButton = Device.Android == Device.RuntimePlatform ? new Button { Text = "Send", FontSize = 8 } : new Button { Text = "Send" };
            ChatPanel = Device.Android == Device.RuntimePlatform ? new Editor { HeightRequest = 130, FontSize = 8 } : new Editor { HeightRequest = 130 };
            InputEntry = Device.Android == Device.RuntimePlatform ? new Entry { WidthRequest = 50, FontSize = 8, Placeholder = $"Send message({tabName})" } : new Entry { WidthRequest = 50, Placeholder = "Send message" };
            var grid = new Grid { RowSpacing = 1, ColumnSpacing = 1 };
            grid.HorizontalOptions = LayoutOptions.FillAndExpand;
            grid.RowDefinitions = new RowDefinitionCollection
            {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
            };
            var SL = new StackLayout
            {
                Children =
                {
                    ChatPanel
                },
                
            };
            var stL = new StackLayout
            {
                Children =
                {
                    InputEntry,
                    sendButton,
                    tabCloseButton,
                },
                VerticalOptions = LayoutOptions.Center,
            };
            grid.Children.Add(SL, 0, 0);
            grid.Children.Add(stL, 0, 1);

            ScrollView view = new ScrollView { Content = grid };
            var tab = new ContentPage
            {
                Title = "#C",
                Content = view
            };
            tabCloseButton.Clicked += TabCloseButton_Clicked;
            _channelTabs[tabName] = tab;
            TextInputs[tabName] = InputEntry;
            ChatPanels[tabName] = ChatPanel;
            TabControl.Children.Add(tab);
            sendButton.Clicked += SendButton_Clicked;
        }
        private void CloseChannelTab(string channelName)
        {
            if (!_channelTabs.ContainsKey(channelName))
            {
                return;
            }
            if (_bot.Game != null && _bot.Game != null && _bot.Game.IsMapLoaded && _bot.Game.Channels.Any(e => e.Name == channelName))
            {
                _bot.Game.CloseChannel(channelName);
            }
            else
            {
                RemoveChannelTab(channelName);
            }
        }
        private void RemoveChannelTab(string tabName)
        {
            TabControl.Children.Remove(_channelTabs[tabName]);
            _channelTabs.Remove(tabName);
        }
        private void AddChannelPmTab(string tabName)
        {
            Button tabCloseButton = Device.Android == Device.RuntimePlatform ? new Button { Text = "Close", FontSize = 8 } : new Button { Text = "Close" };
            Button sendButton = Device.Android == Device.RuntimePlatform ? new Button { Text = "Send", FontSize = 8 } : new Button { Text = "Send" };
            ChatPanel = Device.Android == Device.RuntimePlatform ? new Editor { HeightRequest = 130, FontSize = 8 } : new Editor { HeightRequest = 130 };
            InputEntry = Device.Android == Device.RuntimePlatform ? new Entry { WidthRequest = 50, FontSize = 8, Placeholder = $"Send message({tabName})" } : new Entry { WidthRequest = 50, Placeholder = "Send message" };
            var grid = new Grid { RowSpacing = 1, ColumnSpacing = 1 };
            grid.HorizontalOptions = LayoutOptions.FillAndExpand;
            grid.RowDefinitions = new RowDefinitionCollection
            {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
            };
            var SL = new StackLayout
            {
                Children =
                {
                    ChatPanel
                }
            };
            var stL = new StackLayout
            {
                Children =
                    {
                        InputEntry,
                        sendButton,
                        tabCloseButton,
                    },
                VerticalOptions = LayoutOptions.Center,
            };
            grid.Children.Add(SL);
            Grid.SetRowSpan(SL, 4);
            grid.Children.Add(stL, 0, 1);
            ScrollView view = new ScrollView { Content = grid };
            var tab = new ContentPage
            {
                Title = "#CP",
                Content = view
            };
            tabCloseButton.Clicked += TabCloseButton_Clicked;
            _channelPmTabs[tabName] = tab;
            TextInputs[tabName] = InputEntry;
            ChatPanels[tabName] = ChatPanel;
            TabControl.Children.Add(tab);
            sendButton.Clicked += SendButton_Clicked;
        }
        private void CloseChannelPmTab(string channelName)
        {
            if (!_channelPmTabs.ContainsKey(channelName))
            {
                return;
            }
            RemoveChannelPmTab(channelName);
        }

        private void RemoveChannelPmTab(string tabName)
        {
            TabControl.Children.Remove(_channelPmTabs[tabName]);
            _channelPmTabs.Remove(tabName);
        }
        private void AddPmTab(string tabName)
        {
            Button tabCloseButton = Device.Android == Device.RuntimePlatform ? new Button { Text = "Close", FontSize = 8 } : new Button { Text = "Close" };
            Button sendButton = Device.Android == Device.RuntimePlatform ? new Button { Text = "Send", FontSize = 8 } : new Button { Text = "Send" };
            ChatPanel = Device.Android == Device.RuntimePlatform ? new Editor { HeightRequest = 130, FontSize = 8 } : new Editor { HeightRequest = 130 };
            InputEntry = Device.Android == Device.RuntimePlatform ? new Entry { WidthRequest = 50, FontSize = 8, Placeholder = $"Send message({tabName})" } : new Entry { WidthRequest = 50, Placeholder = "Send message" };
            var grid = new Grid { RowSpacing = 1, ColumnSpacing = 1 };
            grid.HorizontalOptions = LayoutOptions.FillAndExpand;
            grid.RowDefinitions = new RowDefinitionCollection
            {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
            };
            var SL = new StackLayout
            {
                Children =
                {
                    ChatPanel
                }
            };
            var stL = new StackLayout
            {
                Children =
                {
                    InputEntry,
                    sendButton,
                    tabCloseButton,
                },
                VerticalOptions = LayoutOptions.Center,
            };
            grid.Children.Add(SL);
            grid.Children.Add(stL, 0, 1);
            ScrollView view = new ScrollView { Content = grid };
            var tab = new ContentPage
            {
                Title = "#P",
                Content = view
            };
            tabCloseButton.Clicked += TabCloseButton_Clicked;
            _pmTabs[tabName] = tab;
            TextInputs[tabName] = InputEntry;
            ChatPanels[tabName] = ChatPanel;
            TabControl.Children.Add(tab);
            sendButton.Clicked += SendButton_Clicked;
        }

        private void TabCloseButton_Clicked(object sender, EventArgs e)
        {
            if (_bot.Game != null && _bot.Game.IsMapLoaded)
            {
                ContentPage tab = TabControl.CurrentPage as ContentPage;
                string tabName = tab.Title.Replace("#", "");
                if (_pmTabs.ContainsKey(tabName))
                    ClosePmTab(tabName);
                else if (_channelTabs.ContainsKey(tabName))
                    CloseChannelTab(tabName);
                else if (_channelPmTabs.ContainsKey(tabName))
                    CloseChannelPmTab(tabName);
            }
        }

        private void ClosePmTab(string pmName)
        {
            if (!_pmTabs.ContainsKey(pmName))
            {
                return;
            }
            if (_bot.Game != null && _bot.Game != null && _bot.Game.IsMapLoaded && _bot.Game.Conversations.Contains(pmName))
            {
                _bot.Game.CloseConversation(pmName);
            }
            RemovePmTab(pmName);
        }

        private void RemovePmTab(string tabName)
        {
            TabControl.Children.Remove(_pmTabs[tabName]);
            _pmTabs.Remove(tabName);
        }
    }
}