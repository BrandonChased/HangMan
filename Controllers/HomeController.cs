using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HangMan.Models;

namespace HangMan.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewBag.Error = HttpContext.Session.GetString("Error");
        HttpContext.Session.SetString("ShowWord", "");
        HttpContext.Session.SetString("Win", "No");
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost("StartGame")]
    public IActionResult StartGame(string Word)
    {
        if (Word == null)
        {
            HttpContext.Session.SetString("Error", "Please enter a Word");
            return RedirectToAction("Index");
        }
        else if (Word.Contains(" "))
        {
            HttpContext.Session.SetString("Error", "Word cannot have any spaces");
            return RedirectToAction("Index");
        }
        else if (Word.Length < 3)
        {
            HttpContext.Session.SetString("Error", "Word must be at least 3 characters");
            return RedirectToAction("Index");
        }
        HttpContext.Session.SetString("Error", "");
        HttpContext.Session.SetString("Word", Word.ToUpper());

        string emptyGuesses = "";

        for (int i = 0; i < Word.Length; i++)
        {
            emptyGuesses += "_";
        }

        HttpContext.Session.SetString("Hint", emptyGuesses);
        HttpContext.Session.SetInt32("GuessCount", 0);
        HttpContext.Session.SetString("ImgUrl", "/img/" + HttpContext.Session.GetInt32("GuessCount") + ".png");
        HttpContext.Session.SetString("UsedLetters", "");

        return RedirectToAction("Game");
    }

    [HttpGet("Game")]
    public IActionResult Game()
    {
        ViewBag.Word = HttpContext.Session.GetString("Word");
        ViewBag.Hint = HttpContext.Session.GetString("Hint");
        ViewBag.Url = HttpContext.Session.GetString("ImgUrl");
        ViewBag.Win = HttpContext.Session.GetString("Win");
        ViewBag.UsedLetters = HttpContext.Session.GetString("UsedLetters");
        ViewBag.ShowWord = HttpContext.Session.GetString("ShowWord");
        return View();
    }

    [HttpPost("Guess")]
    public IActionResult Guess(char Guess)
    {
        if (HttpContext.Session.GetInt32("GuessCount") == 5)
        {

            HttpContext.Session.SetString("ShowWord", HttpContext.Session.GetString("Word"));
            HttpContext.Session.SetString("ImgUrl", "/img/6.png");
            HttpContext.Session.SetString("Win", "Yes");
            return RedirectToAction("Game");
        }

        char upperGuess = char.ToUpper(Guess);
        string correctWord = HttpContext.Session.GetString("Word");
        string hint = HttpContext.Session.GetString("Hint");
        string newHint = "";

        if (HttpContext.Session.GetString("UsedLetters").Contains(upperGuess))
        {
            return RedirectToAction("Game");
        }

        if (correctWord.Contains(upperGuess))
        {
            for (int i = 0; i < correctWord.Length; i++)
            {
                if (correctWord[i] == upperGuess && hint[i] == '_')
                {
                    newHint += upperGuess;
                }
                else if (hint[i] != '_')
                {
                    newHint += hint[i];
                }
                else
                {
                    newHint += "_";
                }
            }

            if (newHint == correctWord)
            {
                HttpContext.Session.SetString("ImgUrl", "/img/win.png");
                HttpContext.Session.SetString("Win", "Yes");
            }
            HttpContext.Session.SetString("Hint", newHint);
        }
        else
        {
            int? oldCount = HttpContext.Session.GetInt32("GuessCount");
            HttpContext.Session.SetInt32("GuessCount", (int)oldCount + 1);
            HttpContext.Session.SetString("ImgUrl", "/img/" + HttpContext.Session.GetInt32("GuessCount") + ".png");
            HttpContext.Session.SetString("Win", "No");
            string oldLetters = HttpContext.Session.GetString("UsedLetters");
            HttpContext.Session.SetString("UsedLetters", oldLetters += upperGuess);
        }


        return RedirectToAction("Game");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
