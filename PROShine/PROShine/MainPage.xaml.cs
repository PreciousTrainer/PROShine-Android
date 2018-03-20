using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using PROBot;
using PROProtocol;
using System.Text.RegularExpressions;
using Plugin.FilePicker.Abstractions;
using Plugin.FilePicker;
using System.IO;
using System.Threading;
#if __iOS__
using UIKit;
#endif
#if __ANDROID__
using Android.Views;
#endif

namespace PROShine
{
    public partial class MainPage : TabbedPage
    {
        Button btnLogin;
        Entry txtUsername;
        Entry txtPassword;
        Entry txtServer;
        private int _queuePosition;
        DateTime _lastQueueBreakPointTime;
        int? _lastQueueBreakPoint;
        Label StatusText;
        Label MapNameText;
        Label PlayerPositionText;
        Label MoneyText;
        Label PokeTimeText;
        private Switch AutoEvolveSwitch;
        private List<string> Logs;

        private TeamView Team;
        private ChatView Chat;
        private PlayersView Players;
        private InventoryView Inventories;
        public BotClient Bot { get; private set; }
        private Grid grid;
        DateTime _refreshPlayers;
        public int _refreshPlayersDelay;
        private Editor LogTexts;
        Button btnBot;
        Button btnStopBot;
        private Button btnLoadScript;
        /// <summary>
        /// Stupid Android UI or I am so stupid.
        /// </summary>
        class InventoryItemsLabelName
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Quantity { get; set; }
            public string Scope { get; set; }
        }
        public MainPage()
        {
            Thread.CurrentThread.Name = "UI Thread";
            Bot = new BotClient(App.LanguageXMLFile);
            Bot.ClientChanged += Bot_ClientChanged;
            Bot.StateChanged += Bot_StateChanged;
            Bot.ConnectionOpened += Bot_ConnectionOpened;
            Bot.ConnectionClosed += Bot_ConnectionClosed;
            Bot.MessageLogged += Bot_LogMessage;
            Bot.PokemonEvolver.StateChanged += Bot_PokemonEvolverStateChanged;

            Team = new TeamView(Bot);
            Chat = new ChatView(Bot);
            Players = new PlayersView(Bot);
            Inventories = new InventoryView();

            InitializeComponents();

            Task.Run(() => UpdateClients());

            _refreshPlayers = DateTime.UtcNow;
            _refreshPlayersDelay = 5000;
            _refreshPlayersDelay = CurrentPage == Players ? 200 : 5000;

            LogMessage("Running PROShine by Silv3r. Ported to android by PreciousTrainer.");
            btnLogin.Clicked += BtnLogin_Clicked;
            btnLoadScript.Clicked += LoadScript_Clicked;
            AutoEvolveSwitch.Toggled += AutoEvolveSwitch_Toggled;
            btnStopBot.Clicked += BtnBot_Clicked1;
            btnBot.Clicked += BtnBot_Clicked;
        }
        private void InitializeComponents()
        {
            InventoryItemsLabelName lblNames = new InventoryItemsLabelName { Id = "Id", Name = "Name", Quantity = "Quantity", Scope = "Scope" };
            List<InventoryItemsLabelName> lol = new List<InventoryItemsLabelName>();
            lol.Add(lblNames);
            Inventories.ItemView.ItemsSource = lol;
            PlayerPositionText = new Label { Text = "Player Position: (0, 0)", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
            MapNameText = new Label { Text = "Map Name: ??", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
            StatusText = new Label { Text = "Status: Offline", TextColor = Color.OrangeRed, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
            MoneyText = new Label { Text = "Poke$: 0", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
            PokeTimeText = new Label { Text = "Game Time: 0:00", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), Margin = 2 };
            btnLogin = new Button
            {
                Text = "Login",
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("77D065"),
            };
            txtUsername = new Entry { Placeholder = "Username" };
            txtPassword = new Entry { Placeholder = "Password", IsPassword = true };
            txtServer = new Entry { Placeholder = "Server" };
            Logs = new List<string>();
            btnBot = new Button { WidthRequest = Device.Android == Device.RuntimePlatform ? 60 : 120, TextColor = Color.White, Text = "Start", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), BackgroundColor = Color.FromHex("2C88C6") };
            btnStopBot = new Button { WidthRequest = Device.Android == Device.RuntimePlatform ? 60 : 120, TextColor = Color.White, Text = "Stop", IsEnabled = false, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), BackgroundColor = Color.FromHex("2C88C6") };
            AutoEvolveSwitch = new Switch { Margin = -5, };

            Label header = new Label { Text = "Auto Evolve:", Margin = 2, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
            LogTexts = new Editor { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), HeightRequest = 130 };
            AutoEvolveSwitch.IsToggled = Bot.PokemonEvolver.IsEnabled;
            btnLoadScript = new Button { WidthRequest = Device.Android == Device.RuntimePlatform ? 60 : 120, TextColor = Color.White, Text = "Load Script", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), BackgroundColor = Color.FromHex("2C88C6") };
            grid = new Grid { RowSpacing = 1, ColumnSpacing = 1 };
            grid.HorizontalOptions = LayoutOptions.FillAndExpand;
            grid.RowDefinitions = new RowDefinitionCollection
            {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
            };
            var content1 = new StackLayout
            {
                Children =
                {
                    StatusText,
                    MapNameText,
                    PlayerPositionText,
                    MoneyText,
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var content2 = new StackLayout
            {
                Children =
                {
                    PokeTimeText,
                    header,
                    AutoEvolveSwitch,
                },
                Orientation = StackOrientation.Horizontal,
            };
            var content3 = new StackLayout
            {
                Children =
                {
                    btnLoadScript,
                    btnBot,
                    btnStopBot
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            grid.Children.Add(content1);
            Grid.SetColumnSpan(content1, 1);
            grid.Children.Add(content2, 0, 1);
            Grid.SetColumnSpan(content2, 1);
            grid.Children.Add(content3, 0, 2);
            Grid.SetColumnSpan(content3, 1);
            grid.Children.Add(LogTexts, 0, 3);

            var profilePage = new ContentPage
            {
                Title = "Login",
                Content = new StackLayout
                {
                    Spacing = 20,
                    Padding = 50,
                    VerticalOptions = LayoutOptions.Center,
                    Children = {
                    txtUsername,
                    txtPassword,
                    txtServer,
                    btnLogin
                    }
                }
            };
            var settingsPage = new ContentPage
            {
                Title = "Bot",
                Content = grid
            };         

            var mainPage = new TabbedPage { Children = { profilePage, settingsPage, Team, Chat, Players, Inventories } };
            Children.Add(mainPage);          
        }
        private void Wake_Lock(object sender, ToggledEventArgs e)
        {
            if (e.Value) {
#if __iOS__
            App.LockScreeniOS?.Invoke();
#endif
#if __ANDROID__
            App.LockScreenAndroid?.Invoke();
#endif
            }
            else
            {
#if __iOS__
            App.UnlockiOS?.Invoke();
#endif
#if __ANDROID__
            App.UnlockAndroid?.Invoke();
#endif
            }
        }
        private void BtnBot_Clicked1(object sender, EventArgs e)
        {
            lock (Bot)
            {
                Bot.Stop();
                Bot.CancelInvokes();
                btnBot.Text = "Start";
                btnStopBot.IsEnabled = false;
            }
        }

        private void BtnBot_Clicked(object sender, EventArgs e)
        {
            lock (Bot)
            {
                if (!Bot.Game.IsConnected || Bot.Game is null) return;
                if (Bot.Running == BotClient.State.Stopped && btnBot.Text == "Start")
                {
                    Bot.Start();
                }
                else if (Bot.Running == BotClient.State.Started || (Bot.Running == BotClient.State.Paused && btnBot.Text == "Paused"))
                {
                    Bot.Pause();
                }
            }
        }

        private void Bot_StateChanged(BotClient.State state)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                string stateText;
                if (state == BotClient.State.Started)
                {
                    stateText = "started";
                    btnBot.Text = "Pause";
                    btnStopBot.IsEnabled = true;
                }
                else if (state == BotClient.State.Paused)
                {
                    stateText = "paused";
                    btnBot.Text = "Start";
                    btnStopBot.IsEnabled = true;
                }
                else
                {
                    stateText = "stopped";
                    btnBot.Text = "Start";
                    btnStopBot.IsEnabled = false;
                }
                LogMessage("Bot " + stateText);
            });
        }
        private async void LoadScript_Clicked(object sender, EventArgs e)
        {
            try
            {
#if __MOBILE__

#else
                LogMessage("Please make sure your scripts are in this program's folder. Thanks");
#endif
                //nuget: 
                //var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                FileData x = await CrossFilePicker.Current.PickFile();
                //var filePath = Path.Combine(documentsPath, x.FilePath);
                if (x != null)
                {
                    LoadScript(x.FilePath);
                }

            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
#if __MOBILE__
                
#else
                LogMessage("Please make sure your scripts are in this program's folder. Thanks");
#endif
            }
        }
        private void LoadScript(string fileName)
        {
            if (!fileName.EndsWith(".lua") && !fileName.EndsWith(".txt")) return;
            try
            {
                lock (Bot)
                {
                    Bot.LoadScript(fileName);
                    LogMessage(string.Format("Script \"{0}\" by \"{1}\" successfully loaded", Bot.Script.Name, Bot.Script.Author));
                    if (!string.IsNullOrEmpty(Bot.Script.Description))
                    {
                        LogMessage(Bot.Script.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("Could not load script {0}: " + Environment.NewLine + "{1}", fileName, ex.Message));
            }
        }
        private void AutoEvolveSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            lock (Bot)
            {
                Bot.PokemonEvolver.IsEnabled = e.Value;
            }
        }

        private void UpdateClients()
        {
            lock (Bot)
            {
                if (Bot.Game != null)
                {
                    Bot.Game.Update();
                }
                Bot.Update();
            }
            Task.Delay(1).ContinueWith((previous) => UpdateClients());
        }
        private void Bot_PokemonEvolverStateChanged(bool value)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (AutoEvolveSwitch.IsToggled == value) return;
                AutoEvolveSwitch.IsToggled = value;
            });
            
        }
        
        private void BtnLogin_Clicked(object sender, EventArgs e)
        {
            try
            {
                bool shouldLogin = false;
                lock (Bot)
                {
                    if (Bot.Game == null || !Bot.Game.IsConnected)
                    {
                        shouldLogin = true;
                    }
                    else
                    {
                        Logout();
                        btnLogin.Text = "Login";
                    }
                }
                if (shouldLogin)
                {
                    Login(txtUsername.Text, txtPassword.Text, txtServer.Text);
                }
            }catch (Exception ex)
            {
                LogMessage(ex.Message);
            }
        }
        private void LogMessage(string message)
        {
            Logs.Add(message);
            LogTexts.Text += "[" + DateTime.UtcNow.ToLongTimeString() + "]: " + message + Environment.NewLine;
        }
        private void Logout()
        {
            LogMessage("Logging out...");
            lock (Bot)
            {
                Bot.Logout(false);
            }
        }
        private void Bot_ConnectionClosed()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _lastQueueBreakPoint = null;
                btnLogin.IsEnabled = true;
                StatusText.Text = "Status: Offline";
                StatusText.TextColor = Color.OrangeRed;
                Team.Team = new List<Pokemon>();
                Team.PokemonsListView.ItemsSource = null;
                Team.PokemonsListView.ItemsSource = null;
                Team.PokemonsListView.BeginRefresh();
                Team.PokemonsListView.EndRefresh();
                Players.PlayerListView.ItemsSource = null;
                Players.PlayerListView.BeginRefresh();
                Players.PlayerListView.EndRefresh();
                Inventories.ItemView.BeginRefresh();
                InventoryItemsLabelName lblNames = new InventoryItemsLabelName { Id = "Id", Name = "Name", Quantity = "Quantity", Scope = "Scope" };
                List<InventoryItemsLabelName> lol = new List<InventoryItemsLabelName>();
                lol.Add(lblNames);
                Inventories.ItemView.ItemsSource = lol;
                Inventories.ItemView.EndRefresh();
                Players.Content = new StackLayout
                {
                    Children =
                    {
                        Players.PlayerListView
                    }
                };
                Team.Content = new StackLayout {
                    Children =
                    {
                        Team.PokemonsListView
                    }
                };
                btnLogin.Text = "Login";
            });
        }
        private void Bot_ConnectionOpened()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (Bot)
                {
                    if (Bot.Game != null)
                    {
                        LogMessage("Logging in....");
                        Team.Team = new List<Pokemon>();
                        Team.PokemonsListView.ItemsSource = null;
                        Team.PokemonsListView.BeginRefresh();
                        Team.PokemonsListView.EndRefresh();
                        Players.PlayerListView.ItemsSource = null;
                        Players.PlayerListView.BeginRefresh();
                        Players.PlayerListView.EndRefresh();
                        Players.Content = new StackLayout
                        {
                            Children =
                            {
                                Players.PlayerListView
                            }
                        };
                        Team.Content = new StackLayout
                        {
                            Children =
                            {
                                Team.PokemonsListView
                            }
                        };
                    }
                }
            });
        }
        private void Bot_LogMessage(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                LogMessage(message);
            });
        }
        private void Client_LoggedIn()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _lastQueueBreakPoint = null;
                LogMessage("Connected and logged in to the server!");
                StatusText.Text = "Status: Online";
                StatusText.TextColor = Color.SeaGreen;
                btnLogin.IsEnabled = true;
                btnLogin.Text = "Logout";
            });
        }
        private void Client_AuthenticationFailed(AuthenticationResult reason)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                string message = "";
                switch (reason)
                {
                    case AuthenticationResult.AlreadyLogged:
                        message = "Already logged in";
                        break;
                    case AuthenticationResult.Banned:
                        message = "You are banned from PRO";
                        break;
                    case AuthenticationResult.EmailNotActivated:
                        message = "Email not activated";
                        break;
                    case AuthenticationResult.InvalidPassword:
                        message = "Invalid password";
                        break;
                    case AuthenticationResult.InvalidUser:
                        message = "Invalid username";
                        break;
                    case AuthenticationResult.InvalidVersion:
                        message = "Outdated client, please wait for an update";
                        break;
                    case AuthenticationResult.Locked:
                    case AuthenticationResult.Locked2:
                        message = "Server locked for maintenance";
                        break;
                    case AuthenticationResult.OtherServer:
                        message = "Already logged in on another server";
                        break;
                }
                LogMessage("Authentication failed: " + message);
            });
        }
        private void Bot_ClientChanged()
        {
            lock (Bot)
            {
                if (Bot.Game != null)
                {
                    Bot.Game.LoggedIn += Client_LoggedIn;
                    Bot.Game.AuthenticationFailed += Client_AuthenticationFailed;
                    Bot.Game.QueueUpdated += Client_QueueUpdated;
                    Bot.Game.PositionUpdated += Client_PositionUpdated;
                    Bot.Game.PokemonsUpdated += Client_PokemonsUpdated;
                    Bot.Game.InventoryUpdated += Client_InventoryUpdated;
                    Bot.Game.BattleStarted += Client_BattleStarted;
                    Bot.Game.BattleMessage += Client_BattleMessage;
                    Bot.Game.BattleEnded += Client_BattleEnded;
                    Bot.Game.DialogOpened += Client_DialogOpened;

                    Bot.Game.ChatMessage += Chat.Client_ChatMessage;
                    Bot.Game.ChannelMessage += Chat.Client_ChannelMessage;
                    Bot.Game.EmoteMessage += Chat.Client_EmoteMessage;
                    Bot.Game.ChannelSystemMessage += Chat.Client_ChannelSystemMessage;
                    Bot.Game.ChannelPrivateMessage += Chat.Client_ChannelPrivateMessage;
                    Bot.Game.PrivateMessage += Chat.Client_PrivateMessage;
                    Bot.Game.LeavePrivateMessage += Chat.Client_LeavePrivateMessage;
                    Bot.Game.RefreshChannelList += Chat.Client_RefreshChannelList;

                    Bot.Game.SystemMessage += Client_SystemMessage;
                    Bot.Game.InvalidPacket += Client_InvalidPacket;
                    Bot.Game.PokeTimeUpdated += Client_PokeTimeUpdated;
                    Bot.Game.PlayerAdded += Client_PlayerAdded;
                    Bot.Game.PlayerUpdated += Client_PlayerUpdated;
                    Bot.Game.PlayerRemoved += Client_PlayerRemoved;
                }
            }
        }
        private void Client_PlayerAdded(PlayerInfos player)
        {
            if (_refreshPlayers < DateTime.UtcNow)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Players.RefreshView();
                });
                _refreshPlayers = DateTime.UtcNow.AddMilliseconds(_refreshPlayersDelay);
            }
        }

        private void Client_PlayerUpdated(PlayerInfos player)
        {
            if (_refreshPlayers < DateTime.UtcNow)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Players.RefreshView();
                });
                _refreshPlayers = DateTime.UtcNow.AddMilliseconds(_refreshPlayersDelay);
            }
        }

        private void Client_PlayerRemoved(PlayerInfos player)
        {
            if (_refreshPlayers < DateTime.UtcNow)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Players.RefreshView();
                });               
                _refreshPlayers = DateTime.UtcNow.AddMilliseconds(_refreshPlayersDelay);
            }
        }
        private void Client_BattleStarted()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                StatusText.Text = "Status: In battle";
                StatusText.TextColor = Color.Blue;
            });
        }
        private void Client_BattleMessage(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                message = Regex.Replace(message, @"\[.+?\]", "");
                LogMessage(message);
            });
        }
        private void Client_BattleEnded()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                StatusText.Text = "Online";
                StatusText.TextColor = Color.SeaGreen;
                Bot.Game.AskForPokedex();
            });
        }
        private void Client_DialogOpened(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                LogMessage(message);
            });
        }
        private void Client_PositionUpdated(string map, int x, int y)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                MapNameText.Text = "Map Name: " + map;
                PlayerPositionText.Text = string.Format("Position: {0}, {1}", x, y);
            });
        }
        private void Client_SystemMessage(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                LogMessage(message);
            });
        }
        private void Client_InvalidPacket(string packet, string error)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                LogMessage("Received Invalid Packet: " + error + ": " + packet);
            });
        }
        private void Client_PokeTimeUpdated(string pokeTime, string weather)
        {
            lock (Bot)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (Bot.Game != null)
                    {
                        PokeTimeText.Text = "Game Time: " + pokeTime;
                        DateTime dt = Convert.ToDateTime(Bot.Game.PokemonTime);
                    }
                });
            }
        }
        private void Client_PokemonsUpdated()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                lock (Bot)
                {
                    if (Bot.Game != null)
                    {
                        List<Pokemon> teamSource;
                        teamSource = Bot.Game.Team.ToList();
                        List<string> teamNames = new List<string>();
                        foreach(var s in teamSource)
                        {
                            teamNames.Add(s.Name);
                        }
                        Team.Team = teamSource;
                        Team.PokemonsListView.ItemsSource = teamNames;
                    }
                }
            });
        }
        private void Client_InventoryUpdated()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                string money;
                IList<InventoryItem> items;
                lock (Bot)
                {
                    money = Bot.Game.Money.ToString("#,##0");
                    items = Bot.Game.Items.ToArray();
                }
                MoneyText.Text = "Poke$: " + money;
                InventoryItemsLabelName lblNames = new InventoryItemsLabelName { Id = "Id", Name = "Name", Quantity = "Quantity", Scope = "Scope" };
                List<InventoryItemsLabelName> lol = new List<InventoryItemsLabelName>();
                lol.Add(lblNames);
                foreach(var item in items)
                {
                    InventoryItemsLabelName values = new InventoryItemsLabelName { Id = item.Id.ToString(), Name = item.Name, Quantity = item.Quantity.ToString(), Scope = item.Scope.ToString() };
                    lol.Add(values);
                }
                Inventories.ItemView.ItemsSource = lol;
            });
        }
        private void Client_QueueUpdated(int position)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (_queuePosition != position)
                {
                    _queuePosition = position;
                    TimeSpan? queueTimeLeft = null;
                    if (_lastQueueBreakPoint != null && position < _lastQueueBreakPoint)
                    {
                        queueTimeLeft = TimeSpan.FromTicks((DateTime.UtcNow - _lastQueueBreakPointTime).Ticks / (_lastQueueBreakPoint.Value - position) * position);
                    }
                    StatusText.Text = "In Queue" + " (" + position + ")";
                    if (queueTimeLeft != null)
                    {
                        StatusText.Text += " ";
                        if (queueTimeLeft.Value.Hours > 0)
                        {
                            StatusText.Text += queueTimeLeft.Value.ToString(@"hh\:mm\:ss");
                        }
                        else
                        {
                            StatusText.Text += queueTimeLeft.Value.ToString(@"mm\:ss");
                        }
                        StatusText.Text += " left";
                    }
                    StatusText.TextColor = Color.DarkBlue;
                    if (_lastQueueBreakPoint == null)
                    {
                        _lastQueueBreakPoint = position;
                        _lastQueueBreakPointTime = DateTime.UtcNow;
                    }
                }
            });
        }
        private void Login(string userName, string passWord = "", string server = "", string script = "")
        {
            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(passWord) || string.IsNullOrEmpty(server)) {
                btnLogin.Text = "Login";
                return;
            }
            Account account = new Account(userName);
            passWord = passWord.Replace(" ", string.Empty);
            userName = userName.Replace(" ", string.Empty);
            lock (Bot)
            {
                account.Password = passWord;
                account.Server = server;
                btnLogin.IsEnabled = false;
                Bot.Login(account);
            }
        }
    }
}
