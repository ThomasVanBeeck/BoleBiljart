
using BoleBiljart.Models;
using BoleBiljart.Pages;
using BoleBiljart.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Reactive.Linq;

namespace BoleBiljart.Viewmodels
{
    [QueryProperty(nameof(OriginalGameModel), "SelectedGame")]
    [QueryProperty(nameof(IsNewGame), "IsNewGame")]

    public partial class GameEditorViewModel : ObservableObject
    {

        private readonly GameService _gameService;
        private readonly UserService _userService;

        [ObservableProperty]
        private Game? originalGameModel;

        [ObservableProperty]
        private bool isNewGame = true;

        [ObservableProperty]
        private string _saveStatus = null!;


        [ObservableProperty]
        private Game tempGameModel = new();

        public GameEditorViewModel(GameService gameService, UserService userService)
        {
            _gameService = gameService;
            _userService = userService;
        }

        //private IObservable<Models.User> _p1Observable;
        //private IObservable<Models.User> _p2Observable;

        private Models.User _player1 = null!;
        private Models.User _player2 = null!;

        private List<Models.User> _playerlist = new List<Models.User>();

        [RelayCommand]
        private async Task Submit()
        {
            SaveStatus = "Bezig met verwerken...";

            // Stap 1: Spelers ophalen
            if (!await GetPlayers()) return;

            // Stap 2: Game data opslaan
            if (!await SaveGameChanges()) return;

            // Stap 3: User statistieken bijwerken
            if (!await SaveUserChanges()) return;

            // Alles gelukt? Dan pas terug navigeren
            SaveStatus = "Opslaan succesvol voltooid.";
        }

        [RelayCommand]
        private async Task<bool> SaveGameChanges()
        {
            SaveStatus = "Save Game changes start...";
            try
            {
                if (IsNewGame)
                    await _gameService.PostAsync(TempGameModel);
                else
                    await _gameService.UpdateAsync(TempGameModel);
                return true;
            }
            catch (Exception ex)
            {
                SaveStatus = $"Fout bij opslaan: {ex}";
                return false;
            }
        }

        private async Task<bool> GetPlayers()
        {
            SaveStatus = "Get players start...";
            try
            {
                _player1 = await _userService.GetByUsernameAsync(TempGameModel.Player1Username).FirstAsync();
                _player2 = await _userService.GetByUsernameAsync(TempGameModel.Player2Username).FirstAsync();
                _playerlist = new List<Models.User> { _player1, _player2 };
                return true;
            }
            catch (Exception ex)
            {
                SaveStatus = $"Fout bij ophalen spelers: {ex}";
                return false;
            }
        }

        private async Task<bool> SaveUserChanges()
        {
            SaveStatus = "SaveUserChanges start...";
            if (_player1 == null || _player2 == null || _player1.Username == null || _player2.Username == null)
            {
                SaveStatus = "Selecteer alle spelers voor opslaan.";
                return false;
            }
            foreach (var p in _playerlist)
            {
                int scoreCurrentGame = 0;
                int highrunCurrentGame = 0;
                bool hasWonThisGame;
                if (p == _player1)
                {

                    scoreCurrentGame = TempGameModel.Player1Score;
                    highrunCurrentGame = TempGameModel.Player1HighRun;
                    hasWonThisGame = (TempGameModel.Player1Score >= TempGameModel.Player2Score);

                }
                else
                {

                    scoreCurrentGame = TempGameModel.Player2Score;
                    highrunCurrentGame = TempGameModel.Player2HighRun;
                    hasWonThisGame = (TempGameModel.Player2Score >= TempGameModel.Player1Score);

                }

                // ** edit van een spel **
                if (!IsNewGame && OriginalGameModel != null)
                {

                    
                    int scoreOriginalGame = 0;
                    // belangrijk: momenteel is gelijkspel ook gerekend als winst
                    // normaal speel je gewoon door tot iemand wint en wijzig je eventueel totaal aantal beurten
                    // dus gelijkspel komt normaal niet voor
                    bool hasWonOriginalGame;
                    int highrunOriginalGame = 0;
                    if (p == _player1)
                    {

                        scoreOriginalGame = OriginalGameModel.Player1Score;
                        highrunOriginalGame = OriginalGameModel.Player1HighRun;
                        hasWonOriginalGame = (OriginalGameModel.Player1Score >= OriginalGameModel.Player2Score);
                    }
                    else
                    {

                        scoreOriginalGame = OriginalGameModel.Player2Score;
                        highrunOriginalGame = OriginalGameModel.Player2HighRun;
                        hasWonOriginalGame = (OriginalGameModel.Player2Score >= OriginalGameModel.Player1Score);
                    }
                    // avg score + avg highrun herberekenen
                    // avg herberekenen door originele invoer eerst te verminderen van de avg (= recalculated)
                    // vervolgens nieuwe ingestelde waarde gebruiken om tot nieuwe avg te komen
                    if (p.GamesPlayed > 1)
                    {
                        float recalculatedScoreAvg = (p.ScoreAvg * p.GamesPlayed - scoreOriginalGame) / (p.GamesPlayed - 1);
                        p.ScoreAvg = (recalculatedScoreAvg * (p.GamesPlayed - 1) + scoreCurrentGame) / p.GamesPlayed;

                        float recalculatedHighrunAvg = (p.HighrunAvg * p.GamesPlayed - highrunOriginalGame) / (p.GamesPlayed - 1);
                        p.HighrunAvg = (recalculatedHighrunAvg * (p.GamesPlayed - 1) + highrunCurrentGame) / p.GamesPlayed;

                    }
                    // indien kleiner dan is dat een fout en moet het aantal games played terug 1 zijn
                    // we zijn een spel aan het editen, dus er is er zeker één
                    else if (p.GamesPlayed <= 1)
                    {
                        p.ScoreAvg = scoreCurrentGame;
                        p.HighrunAvg = highrunCurrentGame;
                        p.GamesPlayed = 1;
                    }

                    // games won/lost herberekenen bij edit
                    if (hasWonOriginalGame != hasWonThisGame)
                    {
                        if (hasWonThisGame)
                        {
                            p.GamesWon += 1;
                            p.GamesLost -= 1;
                        }
                        else
                        {
                            p.GamesWon -= 1;
                            p.GamesLost += 1;
                        }
                    }

                }

                // ** bij NIEUW spel **
                else
                {
                    // nieuw spel dus aantal gespeelde spellen verhogen
                    p.GamesPlayed += 1;

                    // avg score herberekenen
                    p.ScoreAvg = (p.ScoreAvg * (p.GamesPlayed - 1) + scoreCurrentGame) / p.GamesPlayed;

                    // avg highrun (hoogste reeks) herberekenen
                    p.HighrunAvg = (p.HighrunAvg * (p.GamesPlayed - 1) + highrunCurrentGame) / p.GamesPlayed;

                    // toevoegen van win of loss
                    if (hasWonThisGame) p.GamesWon++;
                    else p.GamesLost++;
                }

                // checken of records moeten bijgewerkt worden
                if (p.ScoreRecord < scoreCurrentGame) p.ScoreRecord = scoreCurrentGame;
                if (p.HighrunRecord < highrunCurrentGame) p.HighrunRecord = highrunCurrentGame;

                // effectief bijwerken van users in db
                try
                {

                    await _userService.UpdateAsync(p);
                }
                catch (Exception ex)
                {
                    SaveStatus = $"Fout tijdens bijwerken gebruikersgegevens: {ex}";
                    return false;
                }
            }   // einde foreach loop
            return true;
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.Navigation.PopAsync();
        }

        partial void OnOriginalGameModelChanged(Game? value)
        {
            if (value != null)
            {
                Initialize(value);
            }
        }

        public void Initialize(Game gameModel)
        {

            OriginalGameModel = gameModel;
            TempGameModel = new Game
            {
                Key = gameModel.Key,

                Datetime = gameModel.Datetime,

                HasWhiteBall = gameModel.HasWhiteBall,
                HasOpeningShot = gameModel.HasOpeningShot,

                Player1Id = gameModel.Player1Id,
                Player1HighRun = gameModel.Player1HighRun,
                Player1Username = gameModel.Player1Username,
                Player1Score = gameModel.Player1Score,

                Player2Id = gameModel.Player2Id,
                Player2HighRun = gameModel.Player2HighRun,
                Player2Score = gameModel.Player2Score,
                Player2Username = gameModel.Player2Username,

                PlayerIds = [.. gameModel.PlayerIds],

                TargetScore = gameModel.TargetScore,
                YearMonth = gameModel.YearMonth,
                YearMonthDay = gameModel.YearMonthDay
            };
        }
    }
}
