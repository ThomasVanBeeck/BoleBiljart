using BoleBiljart.Pages;
using BoleBiljart.Services;
using BoleBiljart.Viewmodels;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using Microsoft.Extensions.Logging;
using UraniumUI;

namespace BoleBiljart
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    //fonts.AddMaterialSymbolsFonts();
                    fonts.AddFontAwesomeIconFonts();

                });

            builder.Services.AddSingleton<AuthStateService>();

            builder.Services.AddSingleton(new FirebaseAuthClient(
                new FirebaseAuthConfig
                {
                    ApiKey = "AIzaSyA4BBfkvTwrKWYuj79ZF9sL-EzkEzyZCxE",
                    AuthDomain = "bolebiljart.firebaseapp.com",
                    Providers = new FirebaseAuthProvider[]
                    {
                                new EmailProvider()
                    }
                }));

            builder.Services.AddSingleton(sp =>
            {
                var authState = sp.GetRequiredService<AuthStateService>();
                return new FirebaseClient("https://bolebiljart-default-rtdb.europe-west1.firebasedatabase.app/",
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = async () => await authState.GetIdTokenAsync()
                });
            });

            builder.Services.AddTransient<GameEditorPage>();
            builder.Services.AddTransient<GameEditorViewModel>();

            builder.Services.AddTransient<GameHistoryPage>();
            builder.Services.AddTransient<GameHistoryViewModel>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();

            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<RegisterViewModel>();

            builder.Services.AddTransient<UserStatsPage>();
            builder.Services.AddTransient<UserStatsViewModel>();

            //builder.Services.AddSingleton<AppShell>();
            //builder.Services.AddSingleton<AppShellViewModel>();

            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<GameService>();
            builder.Services.AddSingleton<AvatarService>();
            builder.Services.AddSingleton<GlobalLookupService>();


#if DEBUG
            builder.Logging.AddDebug();
#endif


            return builder.Build();
        }
    }
}