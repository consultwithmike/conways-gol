using GameOfLife;

namespace Tests;

public class GameTests
{
    [Fact]
    public void Game_InitialState_IsValid()
    {
        var sut = new Game(InitialGameStates.Glider);

        // initial state
        sut.Next();

        Assert.Equal(5, sut.Population);

        Assert.True(sut.GetCell(4, 3)!.Alive);
        Assert.True(sut.GetCell(5, 4)!.Alive);
        Assert.True(sut.GetCell(3, 5)!.Alive);
        Assert.True(sut.GetCell(4, 5)!.Alive);
        Assert.True(sut.GetCell(5, 5)!.Alive);
    }

    [Fact]
    public void Game_NextState_IsValid()
    {
        var sut = new Game(InitialGameStates.Glider);

        // initial state
        sut.Next();

        // next state
        sut.Next();

        Assert.Equal(3, sut.Population);

        Assert.True(sut.GetCell(4, 5)!.Alive);
        Assert.True(sut.GetCell(5, 4)!.Alive);
        Assert.True(sut.GetCell(5, 5)!.Alive);

        // next state
        sut.Next();

        Assert.Equal(4, sut.Population);

        Assert.True(sut.GetCell(4, 4)!.Alive);
        Assert.True(sut.GetCell(4, 5)!.Alive);
        Assert.True(sut.GetCell(5, 4)!.Alive);
        Assert.True(sut.GetCell(5, 5)!.Alive);
    }
}
