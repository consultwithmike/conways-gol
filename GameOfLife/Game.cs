namespace GameOfLife;

public class Game : IGame
{
    private const int GridWidth = 12;
    private const int GridHeight = 12;

    private InitialGameStates _initialGameState;
    private IList<Cell> _cellsList = [];
    private int[,] _cellsArray = new int[12, 12];

    public int Id { get; set; } = -1;

    public int Generation { get; set; } = -1;

    public int Population => _cellsList.Count(c => c.Alive);

    public IEnumerable<Cell> Cells
    {
        get { return _cellsList; }
        set
        {
            _cellsList = value.OrderBy(c => c.Y).ThenBy(c => c.X).ToList();
            _cellsArray = new int[12, 12];

            for (int i = 0; i < _cellsList.Count; i++)
            {
                var cell = _cellsList[i];
                _cellsArray[cell.X, cell.Y] = i;
            }
        }
    }

    public Game() : this(InitialGameStates.Random) { }

    public Game(InitialGameStates initialGameState)
    {
        _initialGameState = initialGameState;
    }

    /// <summary>
    /// Creates a new generation of the game state
    /// </summary>
    public IGame Next(int iterations = 0)
    {
        if (iterations <= 1)
        {
            return NextGeneration();
        }

        for (int i = 0; i < iterations; i++)
        {
            NextGeneration();
        }

        return this;
    }

    public Cell? GetCell(int x, int y)
    {
        if (x < 0 || x == GridWidth || y < 0 || y == GridHeight)
        {
            return null;
        }

        return _cellsList[_cellsArray[x, y]];
    }

    public IEnumerable<Cell> GetNeighbors(Cell cell)
    {
        int x = cell.X, y = cell.Y;
        var cells = new List<Cell>();

        var addNeighbor = () =>
        {
            var neighbor = GetCell(x, y);
            if (neighbor is not null) { cells.Add(neighbor); }
        };

        x--;
        y--;
        addNeighbor();

        x++;
        addNeighbor();

        x++;
        addNeighbor();

        x -= 2;
        y++;
        addNeighbor();

        x += 2;
        addNeighbor();

        x -= 2;
        y++;
        addNeighbor();

        x++;
        addNeighbor();

        x++;
        addNeighbor();

        return cells;
    }

    public IGame Reset()
    {
        Generation = -1;
        return Next();
    }

    private void Initialize()
    {
        _cellsList.Clear();
        _cellsArray = new int[12, 12];

        // create a cell for every square on the grid
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                _cellsList.Add(new Cell(x, y));
                _cellsArray[x, y] = _cellsList.Count - 1;
            }
        }

        switch (_initialGameState)
        {
            case InitialGameStates.Random:
                var random = new Random(25618742);
                foreach (var cell in _cellsList)
                {
                    var next = random.Next(1, int.MaxValue);
                    cell.Alive = next % 2 == 0;
                }

                break;
            case InitialGameStates.Glider:
                // initialize the board with an expected space ship pattern
                GetCell(4, 3)!.Alive = true;
                GetCell(5, 4)!.Alive = true;
                GetCell(3, 5)!.Alive = true;
                GetCell(4, 5)!.Alive = true;
                GetCell(5, 5)!.Alive = true;

                break;
        }
    }

    private Game NextGeneration()
    {
        if (Generation == -1)
        {
            Generation++;
            Initialize();

            return this;
        }

        Generation++;

        foreach (var cell in _cellsList)
        {
            cell.Next(this);
        }

        return this;
    }
}

public interface IGame
{
    int Id { get; }

    int Generation { get; }

    int Population { get; }

    IEnumerable<Cell> Cells { get; }

    IGame Next(int iterations = 0);

    Cell? GetCell(int x, int y);

    IEnumerable<Cell> GetNeighbors(Cell cell);

    IGame Reset();
}
