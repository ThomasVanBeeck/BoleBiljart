using BoleBiljart.Viewmodels;

namespace BoleBiljart.Pages
{
    public partial class UserStatsPage : ContentPage
    {
        public UserStatsPage(UserStatsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            //(BindingContext as UserStatsViewModel)?.LoadUser();
        }
    }
}