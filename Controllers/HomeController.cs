using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;

namespace MvcMovie.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Items()
    {
        HttpClient httpClient = new();
        var response = await httpClient.GetAsync("http://api-app:3000/items");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var items = JsonSerializer.Deserialize<List<Item>>(content);

        return View(items);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
