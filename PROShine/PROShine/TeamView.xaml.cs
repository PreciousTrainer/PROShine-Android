using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using PROBot;
using PROProtocol;
using Xamarin.Forms.Xaml;

namespace PROShine
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TeamView : ContentPage
    {
        public ListView PokemonsListView;
        private BotClient _bot;
        public List<Pokemon> Team;
        private int selectedInt = -1;
        public TeamView(BotClient bot)
        {
            _bot = bot;
            PokemonsListView = new ListView();
            Team = new List<Pokemon>();
            PokemonsListView.ItemSelected += PokemonsListView_ItemSelected;
            PokemonsListView.ItemTapped += PokemonsListView_ItemTapped;


            InitializeComponent();
            Title = "Team";
            Content = new ScrollView { Content = PokemonsListView };
        }

        private void PokemonsListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var grid = new Grid { RowSpacing = 1, ColumnSpacing = 1 };
            grid.HorizontalOptions = LayoutOptions.FillAndExpand;
            grid.RowDefinitions = new RowDefinitionCollection
            {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
            };
            selectedInt = (PokemonsListView.ItemsSource as System.Collections.IList).IndexOf(e.Item);
            var content1 = new StackLayout
            {
                Children =
                {
                    new Label{ Text = "Name: " + Team[selectedInt].Name , FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label{ Text = "Level: " + Team[selectedInt].Level.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
                    new Label{ Text = "Nature: " + Team[selectedInt].Nature.Name, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
                    new Label{ Text = "Gender: " + Team[selectedInt].Gender, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
                    new Label{ Text = "OT: " + Team[selectedInt].OriginalTrainer, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) }
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var lbl = new StackLayout
            {
                Children = {
                    new Label { Text = string.Format("Ivs:"), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
                    new Label { Text = string.Format("Evs:"), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var eviv1 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "ATK: " + Team[selectedInt].IV.Attack.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label {Text = "ATK: " + Team[selectedInt].EV.Attack.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var eviv2 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "HP: " + Team[selectedInt].IV.Health.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label {Text = "HP: " + Team[selectedInt].EV.Health.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var eviv3 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "SPD: " + Team[selectedInt].IV.Speed.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label {Text = "SPD: " + Team[selectedInt].EV.Speed.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var eviv4 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "DEF: " + Team[selectedInt].IV.Defence.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label {Text = "DEF: " + Team[selectedInt].EV.Defence.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var eviv5 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "SpDEF: " + Team[selectedInt].IV.SpDefence.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label {Text = "SpDEF: " + Team[selectedInt].EV.SpDefence.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var eviv6 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "SpATK: " + Team[selectedInt].IV.SpAttack.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label {Text = "SpATK: " + Team[selectedInt].EV.SpAttack.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            Label moveName = new Label { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
            foreach (var move in Team[selectedInt].Moves)
            {
                moveName.Text += move.Name + "-" + move.PP + "\n";
            }
            Button ChangeToUp = new Button { WidthRequest = Device.Android == Device.RuntimePlatform ? 60 : 120, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), Text = Team[selectedInt].Uid == 1 ? "Swap Down" : "Swap Up" };
            Button btnGoBack = new Button { WidthRequest = Device.Android == Device.RuntimePlatform ? 60 : 120, Text = "Go Back", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
            var content3 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "Moves:", FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    moveName,
                    ChangeToUp,
                    btnGoBack
                },
            
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            btnGoBack.Clicked += BtnGoBack_Clicked;
            ChangeToUp.Clicked += ChangeToUp_Clicked;
            grid.Children.Add(content1);
            grid.Children.Add(lbl, 0, 1);
            grid.Children.Add(eviv1, 0, 2);
            grid.Children.Add(eviv2, 0, 3);
            grid.Children.Add(eviv3, 0, 4);
            grid.Children.Add(eviv4, 0, 5);
            grid.Children.Add(eviv5, 0, 6);
            grid.Children.Add(eviv6, 0, 7);
            grid.Children.Add(content3, 0, 8);
            ScrollView view = new ScrollView { Content = grid };
            Content = view;
        }

        private void ChangeToUp_Clicked(object sender, EventArgs e)
        {
            if (selectedInt < 0 || selectedInt > Team.Count)
                return;

            Pokemon sourcePokemon = Team[selectedInt];
            Pokemon destinationPokemon = Team[selectedInt].Uid == 1 ? Team[selectedInt + 1] : Team[selectedInt - 1];
            _bot.Game.SwapPokemon(sourcePokemon.Uid, destinationPokemon.Uid);
            Content = new StackLayout
            {
                Children =
                {
                    PokemonsListView
                }
            };
            selectedInt = -1;
        }

        private void BtnGoBack_Clicked(object sender, EventArgs e)
        {
            Content = new StackLayout
            {
                Children =
                {
                    PokemonsListView
                }
            };
            selectedInt = -1;
        }

        private void PokemonsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var grid = new Grid { RowSpacing = 1, ColumnSpacing = 1 };
            grid.HorizontalOptions = LayoutOptions.FillAndExpand;
            grid.RowDefinitions = new RowDefinitionCollection
            {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
            };
            var lbl = new StackLayout
            {
                Children = {
                    new Label { Text = string.Format("Ivs:"), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
                    new Label { Text = string.Format("Evs:"), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            selectedInt = (PokemonsListView.ItemsSource as System.Collections.IList).IndexOf(e.SelectedItem);
            var eviv1 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "ATK: " + Team[selectedInt].IV.Attack.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label {Text = "ATK: " + Team[selectedInt].EV.Attack.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var content1 = new StackLayout
            {
                Children =
                {
                    new Label{ Text = "Name: " + Team[selectedInt].Name, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
                    new Label{ Text = "Level: " + Team[selectedInt].Level.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
                    new Label{ Text = "Nature: " + Team[selectedInt].Nature.Name, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
                    new Label{ Text = "Gender: " + Team[selectedInt].Gender, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) },
                    new Label{ Text = "OT: " + Team[selectedInt].OriginalTrainer, FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label)) }
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var eviv2 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "HP: " + Team[selectedInt].IV.Health.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label {Text = "HP: " + Team[selectedInt].EV.Health.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var eviv3 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "SPD: " + Team[selectedInt].IV.Speed.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label {Text = "SPD: " + Team[selectedInt].EV.Speed.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var eviv4 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "DEF: " + Team[selectedInt].IV.Defence.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label {Text = "DEF: " + Team[selectedInt].EV.Defence.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var eviv5 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "SpDEF: " + Team[selectedInt].IV.SpDefence.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label {Text = "SpDEF: " + Team[selectedInt].EV.SpDefence.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            var eviv6 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "SpATK: " + Team[selectedInt].IV.SpAttack.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    new Label {Text = "SpATK: " + Team[selectedInt].EV.SpAttack.ToString(), FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                },
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            Label moveName = new Label { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
            foreach (var move in Team[selectedInt].Moves)
            {
                moveName.Text += move.Name + "-" + move.PP + "\n";
            }
            Button ChangeToUp = new Button { WidthRequest = Device.Android == Device.RuntimePlatform ? 60 : 120, FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)), Text = Team[selectedInt].Uid == 1 ? "Swap Down" : "Swap Up" };
            Button btnGoBack = new Button { WidthRequest = Device.Android == Device.RuntimePlatform ? 60 : 120, Text = "Go Back", FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
            var content3 = new StackLayout
            {
                Children =
                {
                    new Label {Text = "Moves:", FontSize = Device.GetNamedSize (NamedSize.Micro, typeof(Label))},
                    moveName,
                    ChangeToUp,
                    btnGoBack
                },

                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15
            };
            btnGoBack.Clicked += BtnGoBack_Clicked;
            ChangeToUp.Clicked += ChangeToUp_Clicked;
            grid.Children.Add(content1);
            grid.Children.Add(lbl, 0, 1);
            grid.Children.Add(eviv1, 0, 2);
            grid.Children.Add(eviv2, 0, 3);
            grid.Children.Add(eviv3, 0, 4);
            grid.Children.Add(eviv4, 0, 5);
            grid.Children.Add(eviv5, 0, 6);
            grid.Children.Add(eviv6, 0, 7);
            grid.Children.Add(content3, 0, 8);
            ScrollView view = new ScrollView { Content = grid };
            Content = view;
        }
    }
}