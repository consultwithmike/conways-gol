using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.Controllers;

[ApiController]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> _logger;
    private readonly IGameRepository _gameRepository;

    public GameController(ILogger<GameController> logger, IGameRepository gameRepository)
    {
        _logger = logger;
        _gameRepository = gameRepository;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status200OK)]
    [Route("game/all")]
    public IActionResult All() => Ok(_gameRepository.GetGames());

    [HttpGet]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status200OK)]
    [Route("game/{id}")]
    public IActionResult GetGame(int id) => Ok(_gameRepository.GetGameById(id));

    [HttpGet]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status200OK)]
    [Route("game/new")]
    public IActionResult New() => Ok(_gameRepository.CreateGame());

    [HttpGet]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status404NotFound)]
    [Route("game/{id}/next")]
    public IActionResult Next(int id) => Next(id, 0);

    [HttpGet]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status404NotFound)]
    [Route("game/{id}/next/{iterations:int}")]
    public IActionResult Next(int id, int iterations)
    {
        var game = _gameRepository.GetGameById(id);
        if (game is null)
        {
            return NotFound();
        }

        game.Next(iterations);

        _gameRepository.Save();

        return Ok(game);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status404NotFound)]
    [Route("game/{id}/finalize")]
    public IActionResult Finalize(int id) => Finalize(id, 100);

    [HttpGet]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status404NotFound)]
    [Route("game/{id}/finalize/{maxIterations:int?}")]
    public IActionResult Finalize(int id, int maxIterations)
    {
        var game = _gameRepository.GetGameById(id);
        if (game is null)
        {
            return NotFound();
        }

        var iterations = 0;
        for (; iterations < maxIterations; iterations++)
        {
            var currentPopulation = game.Population;
            game.Next();

            if (currentPopulation == game.Population)
            {
                _gameRepository.Save();

                return Ok(game);
            }
        }

        _gameRepository.Save();

        return this.BadRequest($"The board did not finalize inside the max iterations of {maxIterations}.");
    }

    [HttpGet]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status404NotFound)]
    [Route("game/{id}/reset")]
    public IActionResult Reset(int id)
    {
        var game = _gameRepository.GetGameById(id);
        if (game is null)
        {
            return NotFound();
        }

        game.Reset();

        _gameRepository.Save();

        return Ok(game);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IGame), StatusCodes.Status201Created)]
    [Route("game/upload")]
    public IActionResult Upload(Game game)
    {
        var newGame = _gameRepository.UploadGame(game);
        return Created($"game/{newGame.Id}", newGame);
    }
}
