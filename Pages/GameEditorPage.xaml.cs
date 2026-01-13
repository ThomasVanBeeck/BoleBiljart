
using BoleBiljart.Viewmodels;

namespace BoleBiljart.Pages
{
    public partial class GameEditorPage : ContentPage
    {
        public GameEditorPage(GameEditorViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}