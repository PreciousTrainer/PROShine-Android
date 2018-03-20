using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PROShine
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InventoryView : ContentPage
	{
        public ListView ItemView;
		public InventoryView ()
		{
			InitializeComponent ();
            Title = "Items";
            var itemDataTemplate = new DataTemplate(() =>
            {
                var grid = new Grid();
                var IdLabel = new Label { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
                var NameLabel = new Label { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
                var QuantityLabel = new Label { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };
                var ScopeLabel = new Label { FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)) };

                IdLabel.SetBinding(Label.TextProperty, "Id");
                NameLabel.SetBinding(Label.TextProperty, "Name");
                QuantityLabel.SetBinding(Label.TextProperty, "Quantity");
                ScopeLabel.SetBinding(Label.TextProperty, "Scope");

                grid.Children.Add(IdLabel);
                grid.Children.Add(NameLabel, 1, 0);
                grid.Children.Add(QuantityLabel, 2, 0);
                grid.Children.Add(ScopeLabel, 3, 0);

                return new ViewCell { View = grid };
            });
            ItemView = new ListView { ItemTemplate = itemDataTemplate, Margin = new Thickness(0, 20, 0, 0) };
            Content = new StackLayout
            {
                Margin = new Thickness(20),
                Children = {
                   ItemView
                }
            };
        }
	}
}