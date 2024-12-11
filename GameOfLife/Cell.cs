namespace GameOfLife;

public class Cell
{
    public Cell(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; }

    public int Y { get; set; }

    public bool Alive { get; set; }

    public void Next(Game game)
    {
        var neighbors = game.GetNeighbors(this);
        var liveNeighbors = neighbors.Count(n => n.Alive);

        if (Alive && (liveNeighbors < 2 || liveNeighbors > 3))
        {
            Alive = false;
        }
        else if (!Alive)
        {
            Alive = liveNeighbors == 3;
        }
    }

    public override string ToString() => $"({X},{Y})";
}
