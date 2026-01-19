using Android.Media;

namespace BoleBiljart.Navigation
{

    public partial class AuthNavBar : ContentView
    {
        public AuthNavBar()
        {
            InitializeComponent();
            InitializeNavigation();
        }

        private void InitializeNavigation()
        {
            BtnUserStats.Clicked += async (_, _) => await Shell.Current.GoToAsync("//UserStats");
            BtnGameEditor.Clicked += async (_, _) => await Shell.Current.GoToAsync("//GameEditor");
            BtnGameHistory.Clicked += async (_, _) => await Shell.Current.GoToAsync("//GameHistory");
        }
        public static readonly BindableProperty ActiveTabProperty =
            BindableProperty.Create(
                nameof(ActiveTab),
                typeof(string),
                typeof(AuthNavBar),
                default(string),
                propertyChanged: OnActiveTabChanged);

        public string ActiveTab
        {
            get => (string)GetValue(ActiveTabProperty);
            set => SetValue(ActiveTabProperty, value);
        }

        private static void OnActiveTabChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is AuthNavBar authNavBar && newValue is string tabName)
                authNavBar.SetActiveTab(tabName);
        }


        public void SetActiveTab(string tabName)
        {
            Color activeColor = Colors.White;
            if (App.Current.Resources.TryGetValue("CustomPurpleLight", out var colorvalue))
            {
                activeColor = (Color)colorvalue;
            }

            SetButtonIconColor(BtnUserStats, Colors.DarkGray);
            SetButtonIconColor(BtnGameEditor, Colors.DarkGray);
            SetButtonIconColor(BtnGameHistory, Colors.DarkGray);

            switch (tabName)
            {
                case "UserStats": SetButtonIconColor(BtnUserStats, activeColor); break;
                case "GameEditor": SetButtonIconColor(BtnGameEditor, activeColor); break;
                case "GameHistory": SetButtonIconColor(BtnGameHistory, activeColor); break;
            }
        }

        private void SetButtonIconColor(Button button, Color color)
        {
            if (button.ImageSource is FontImageSource fontImageSource)
            {
                fontImageSource.Color = color;
            }
        }
    }
}