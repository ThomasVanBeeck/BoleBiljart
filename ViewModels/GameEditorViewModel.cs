
using BoleBiljart.Models;
using BoleBiljart.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BoleBiljart.Viewmodels
{
    [QueryProperty(nameof(OriginalGameModel), "SelectedGame")]
    [QueryProperty(nameof(IsNewGame), "IsNewGame")]

    public partial class GameEditorViewModel : ObservableObject
    {
        private readonly GameService _gameService;
        private readonly GlobalLookupService _globalLookupService;
        private readonly UserService _userService;

        private IObservable<Models.GlobalLookup> _globalLookupObservable;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        [ObservableProperty]
        private Models.GlobalLookup _gLookup = null!;

        [ObservableProperty]
        private Game? originalGameModel;

        [ObservableProperty]
        private bool isNewGame = true;

        [ObservableProperty]
        private string _saveStatus = null!;


        [ObservableProperty]
        private Game tempGameModel = new();


        //private IObservable<Models.User> _p1Observable;
        //private IObservable<Models.User> _p2Observable;

        private Models.User _player1 = null!;
        private Models.User _player2 = null!;

        private List<Models.User> _playerlist = new List<Models.User>();

        public GameEditorViewModel(GameService gameService,
            GlobalLookupService globalLookupService,
            UserService userService)
        {
            _gameService = gameService;
            _globalLookupService = globalLookupService;
            _userService = userService;
            _globalLookupObservable = _globalLookupService.GetByUidAsync("singleton");

            // aanmaken vd singleton in db, code 1x uitvoeren in development
            //Models.GlobalLookup glookup = new();
            //_globalLookupService.PostAsync(glookup);

            var subscription = _globalLookupObservable
                .Where(gl => gl.Uid == "singleton")
                .Subscribe(gl => GLookup = gl);
            _disposables.Add(subscription);
        }

        [RelayCommand]
        private async Task Submit()
        {
            if (tempGameModel.Player1Username == null || tempGameModel.Player2Username == null)
            {
                SaveStatus = "Vul de spelersnamen in.";
                return;
            }

            else if (tempGameModel.Player1Username != null && !GLookup.Usernames.ContainsKey(tempGameModel.Player1Username))
            {
                SaveStatus = "Naam van speler 1 is niet correct.";
                return;
            }

            else if (tempGameModel.Player2Username != null && !GLookup.Usernames.ContainsKey(tempGameModel.Player2Username))
            {
                SaveStatus = "Naam van speler 2 is niet correct.";
                return;
            }

            SaveStatus = "Bezig met verwerken...";

            // Stap 1: Spelers ophalen
            if (!await GetPlayers()) return;

            // Stap 3: User statistieken bijwerken
            if (!await SaveUserChanges()) return;

            // Stap 2: Game data verwerken en opslaan
            if (!await SaveGameChanges()) return;


            // Stap 4: Status en/of navigeren bij succes
            SaveStatus = "Opslaan succesvol voltooid.";
            // Even pauze voor verder navigeren
            await Task.Delay(200);
            await Shell.Current.GoToAsync("//GameHistory");
        }

        [RelayCommand]
        private async Task<bool> SaveGameChanges()
        {
            TempGameModel.Player1Id = _player1.Uid;
            TempGameModel.Player2Id = _player2.Uid;

            TempGameModel.YearMonth = $"{TempGameModel.Datetime.Year}-{TempGameModel.Datetime.Month}"; 
            TempGameModel.YearMonthDay = $"{TempGameModel.Datetime.Day}-{TempGameModel.Datetime.Month}-{TempGameModel.Datetime.Year}";

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
                    // check of user records ongedaan gemaakt moeten worden


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

                            // check records PLAYER 1

                        if (OriginalGameModel.Key == p.HighrunRecordGameKey &&
                                OriginalGameModel.Player1HighRun > TempGameModel.Player1HighRun &&
                                OriginalGameModel.Player1HighRun == p.HighrunRecord)
                        {
                            // TODO: ophalen record door alle voorgaande games te checken op Highrun waarde
                        }

                        if (OriginalGameModel.Key == p.ScoreRecordGameKey &&
                            OriginalGameModel.Player1Score > TempGameModel.Player1Score &&
                            OriginalGameModel.Player1Score == p.ScoreRecord)
                        {
                            // TODO: ophalen record door alle voorgaande games te checken op Score waarde
                        }
                    }
                    else
                    {

                        scoreOriginalGame = OriginalGameModel.Player2Score;
                        highrunOriginalGame = OriginalGameModel.Player2HighRun;
                        hasWonOriginalGame = (OriginalGameModel.Player2Score >= OriginalGameModel.Player1Score);

                            // check records PLAYER 2

                        if (OriginalGameModel.Key == p.HighrunRecordGameKey &&
                            OriginalGameModel.Player2HighRun > TempGameModel.Player2HighRun &&
                            OriginalGameModel.Player2HighRun == p.HighrunRecord)
                        {
                            // TODO: ophalen record door alle voorgaande games te checken op Highrun waarde
                        }

                        if (OriginalGameModel.Key == p.ScoreRecordGameKey &&
                            OriginalGameModel.Player2Score > TempGameModel.Player1Score &&
                            OriginalGameModel.Player2Score == p.ScoreRecord)
                        {
                            // TODO: ophalen record door alle voorgaande games te checken op Score waarde
                        }
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
                Debug.Write("just before user saving..");
                try
                {
                    // Bepaal wie de referentie-speler is voor deze vergelijking
                    var reference = (p == _player1) ? _player1 : _player2;
                    string label = (p == _player1) ? "PLAYER 1" : "PLAYER 2";

                    Debug.WriteLine($"\n>>>> COMPARING {label} <<<<");
                    Debug.WriteLine($"Property       | Current (p) | Reference");
                    Debug.WriteLine($"---------------|-------------|----------");
                    Debug.WriteLine($"Games Played:  | {p.GamesPlayed,-11} | {reference.GamesPlayed}");
                    Debug.WriteLine($"Games Won:     | {p.GamesWon,-11} | {reference.GamesWon}");
                    Debug.WriteLine($"Games Lost:    | {p.GamesLost,-11} | {reference.GamesLost}");
                    Debug.WriteLine($"Highrun Avg:   | {p.HighrunAvg,-11} | {reference.HighrunAvg}");
                    Debug.WriteLine($"Highrun Record:| {p.HighrunRecord,-11} | {reference.HighrunRecord}");
                    Debug.WriteLine($"Score Avg:     | {p.ScoreAvg,-11} | {reference.ScoreAvg}");
                    Debug.WriteLine($"Score Record:  | {p.ScoreRecord,-11} | {reference.ScoreRecord}");
                    Debug.WriteLine($">>>> END {label} <<<<\n");


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
            await Shell.Current.GoToAsync("//GameHistory");
        }

        [RelayCommand]
        private async Task Delete()
        {
            SaveStatus = "Spel wordt verwijderd...";
            return;
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

                TargetScore = gameModel.TargetScore,
                YearMonth = gameModel.YearMonth,
                YearMonthDay = gameModel.YearMonthDay
            };
        }
    }
}