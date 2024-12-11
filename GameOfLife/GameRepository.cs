using System.Text.Json;

namespace GameOfLife;

public class GameRepository : IGameRepository
{
    private List<Game> _games;

    public GameRepository()
    {
        _games = JsonSerializer.Deserialize<List<Game>>(File.ReadAllText("data.json")) ?? [];
    }

    public IEnumerable<IGame> GetGames() => _games;

    public IGame? GetGameById(int id)
    {
        return _games.FirstOrDefault(g => g.Id == id);
    }

    public IGame CreateGame()
    {
        var game = new Game
        {
            Id = NextId()
        };

        _games.Add(game);

        Save();

        return game;
    }

    public IGame UploadGame(Game game)
    {
        if (game.Id <= 0)
        {
            game.Id = NextId();
        }

        if (game.Generation < 0)
        {
            game.Generation = 0;
        }

        _games.Add(game);

        Save();

        return game;
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(_games);

        File.WriteAllText("data.json", json);
    }

    private int NextId()
    {
        if (_games.Count == 0)
        {
            return 1;
        }

        return _games.Max(_game => _game.Id) + 1;
    }
}

public interface IGameRepository
{
    IEnumerable<IGame> GetGames();

    IGame? GetGameById(int id);

    IGame CreateGame();

    IGame UploadGame(Game game);

    void Save();
}