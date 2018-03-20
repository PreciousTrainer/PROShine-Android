using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PROBot;
using PROProtocol;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PROShine
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlayersView : ContentPage
	{
		private BotClient _bot;
		public ListView PlayerListView;
		public List<PlayerInfosView> listToDisplay = new List<PlayerInfosView>();
		public class PlayerInfosView
		{
			public int Distance { get; set; }
			public string Name { get; set; }
			public string Position { get; set; }
			public string Status { get; set; }
			public string Follower { get; set; }
			public string Guild { get; set; }
			public string LastSeen { get; set; }
		}
        public PlayersView (BotClient bot)
		{
			InitializeComponent ();
			_bot = bot;
			PlayerListView = new ListView { };
			PlayerListView.ItemTapped += PlayerListView_ItemTapped;
			PlayerListView.ItemSelected += PlayerListView_ItemSelected;

			Title = "Players";
			Content = new StackLayout
			{
				Children =
				{
					PlayerListView
				}
			};
            
		}

        private void PlayerListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var grid = new Grid { RowSpacing = 1, ColumnSpacing = 1 };
			grid.HorizontalOptions = LayoutOptions.FillAndExpand;
			grid.RowDefinitions = new RowDefinitionCollection
			{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
			};
			int selectedInt = (PlayerListView.ItemsSource as System.Collections.IList).IndexOf(e.SelectedItem);
			var content1 = new StackLayout
			{
				Children =
				{
					new Label{ Text = "Name: " + listToDisplay[selectedInt].Name, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
					new Label{ Text = "Follower: " + listToDisplay[selectedInt].Follower, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) }
				},
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 15
			};
			var content2 = new StackLayout
			{
				Children =
				{
					new Label{ Text = "Distance: " + listToDisplay[selectedInt].Distance, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
					new Label{ Text = "Position: " + listToDisplay[selectedInt].Position, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))}
				},
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 15
			};
			var content3 = new StackLayout
			{
				Children =
				{
					new Label{ Text = "Guild: " + listToDisplay[selectedInt].Guild, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
					new Label{ Text = "Status: " + listToDisplay[selectedInt].Status, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) }
				},
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 15
			};
			Button btnGoback = new Button { Text = "Go Back", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
			Button btnFriend = new Button { Text = "Ignore/Unignore", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
			Button btnMessage = new Button { Text = "PM", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
			Button btnIgnore = new Button { Text = "Friend/Unfriend", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
			btnIgnore.Clicked += BtnIgnore_Clicked;
			btnFriend.Clicked += BtnFriend_Clicked;
			btnGoback.Clicked += PlayersView_Clicked;
			btnMessage.Clicked += BtnMessage_Clicked;

			var content4 = new StackLayout
			{
				Children =
				{
					new Label{ Text = "Last Seen: " + listToDisplay[selectedInt].LastSeen, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
					btnMessage,
					btnFriend,
					btnIgnore,
					btnGoback
				},
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 15
			};
			grid.Children.Add(content1, 0, 0);
			grid.Children.Add(content2, 0, 1);
			grid.Children.Add(content3, 0, 2);
			grid.Children.Add(content4, 0, 3);
			Content = grid;
		}

		private void BtnFriend_Clicked(object sender, EventArgs e)
		{
			int selectedInt = -1;
			selectedInt = (PlayerListView.ItemsSource as System.Collections.IList).IndexOf(PlayerListView.SelectedItem);
			if (selectedInt < 0)
				return;
			var player = listToDisplay[selectedInt];
			lock (_bot)
			{
				_bot.Game.SendFriendToggle(player.Name);
			}
		}

		private void BtnMessage_Clicked(object sender, EventArgs e)
		{
			int selectedInt = -1;
			selectedInt = (PlayerListView.ItemsSource as System.Collections.IList).IndexOf(PlayerListView.SelectedItem);
			if (selectedInt < 0)
				return;
			var player = listToDisplay[selectedInt];
			lock (_bot)
			{
				if (!_bot.Game.Conversations.Contains(player.Name))
				{
					_bot.Game.SendStartPrivateMessage(player.Name);
				}
			}
		}

		private void BtnIgnore_Clicked(object sender, EventArgs e)
		{
			int selectedInt = -1;
			selectedInt = (PlayerListView.ItemsSource as System.Collections.IList).IndexOf(PlayerListView.SelectedItem);
			if (selectedInt < 0)
				return;
			var player = listToDisplay[selectedInt];
			lock (_bot)
			{
				_bot.Game.SendIgnoreToggle(player.Name);
			}
		}

		private void PlayersView_Clicked(object sender, EventArgs e)
		{
			Content = new StackLayout
			{
				Children =
				{
					PlayerListView
				}
			};
		}

		private void PlayerListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			var grid = new Grid { RowSpacing = 1, ColumnSpacing = 1 };
			grid.HorizontalOptions = LayoutOptions.FillAndExpand;
			grid.RowDefinitions = new RowDefinitionCollection
			{
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
					new RowDefinition { Height = GridLength.Auto },
			};
			int selectedInt = (PlayerListView.ItemsSource as System.Collections.IList).IndexOf(e.Item);
			var content1 = new StackLayout
			{
				Children =
				{
					new Label{ Text = "Name: " + listToDisplay[selectedInt].Name, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
					new Label{ Text = "Follower: " + listToDisplay[selectedInt].Follower, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) }
				},
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 15
			};
			var content2 = new StackLayout
			{
				Children =
				{
					new Label{ Text = "Distance: " + listToDisplay[selectedInt].Distance, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
					new Label{ Text = "Position: " + listToDisplay[selectedInt].Position, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) }
				},
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 15
			};
			var content3 = new StackLayout
			{
				Children =
				{
					new Label{ Text = "Guild: " + listToDisplay[selectedInt].Guild , FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
					new Label{ Text = "Status: " + listToDisplay[selectedInt].Status, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) }
				},
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 15
			};
			Button btnGoback = new Button { Text = "Go Back", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
			Button btnFriend = new Button { Text = "Ignore/Unignore", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
			Button btnMessage = new Button { Text = "PM", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
			Button btnIgnore = new Button { Text = "Friend/Unfriend", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
			btnIgnore.Clicked += BtnIgnore_Clicked;
			btnFriend.Clicked += BtnFriend_Clicked;
			btnGoback.Clicked += PlayersView_Clicked;
			btnMessage.Clicked += BtnMessage_Clicked;

			var content4 = new StackLayout
			{
				Children =
				{
					new Label{ Text = "Last Seen: " + listToDisplay[selectedInt].LastSeen, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
					btnMessage,
					btnFriend,
					btnIgnore,
					btnGoback
				},
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 15
			};
			grid.Children.Add(content1, 0, 0);
			grid.Children.Add(content2, 0, 1);
			grid.Children.Add(content3, 0, 2);
			grid.Children.Add(content4, 0, 3);
			Content = grid;
		}

		public void RefreshView()
		{
			lock (_bot)
			{
				if (_bot.Game != null && _bot.Game.IsMapLoaded && _bot.Game.Players != null)
				{
					IEnumerable<PlayerInfos> playersList = _bot.Game.Players.Values.OrderBy(e => e.Added);
					List<string> playerNames = new List<string>();
					foreach (PlayerInfos player in playersList)
					{
						string petName = "";
						if (player.PokemonPetId < PokemonNamesManager.Instance.Names.Length)
						{
							petName = PokemonNamesManager.Instance.Names[player.PokemonPetId];
							if (player.IsPokemonPetShiny)
							{
								petName = "(s)" + petName;
							}
						}
						listToDisplay.Add(new PlayerInfosView
						{
							Distance = _bot.Game.DistanceTo(player.PosX, player.PosY),
							Name = player.Name,
							Position = "(" + player.PosX + ", " + player.PosY + ")",
							Status = player.IsAfk ? "AFK" : (player.IsInBattle ? "BATTLE" : ""),
							Follower = petName,
							Guild = player.GuildId.ToString(),
							LastSeen = (DateTime.UtcNow - player.Updated).Seconds.ToString() + "s"
						});
						playerNames.Add(player.Name);
					}
					PlayerListView.ItemsSource = playerNames;
					PlayerListView.BeginRefresh();
					PlayerListView.EndRefresh();
				}
			}
		}
	}
}