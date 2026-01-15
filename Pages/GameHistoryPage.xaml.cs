

using BoleBiljart.Viewmodels;

namespace BoleBiljart.Pages
{
	public partial class GameHistoryPage : ContentPage
	{
		public GameHistoryPage (GameHistoryViewModel vm)
		{
			BindingContext = vm;
			InitializeComponent();
		}
	}
}