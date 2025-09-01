using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.MVC.Models;
using HorsesForCourses.Service;
using HorsesForCourses.WebApi;
using System.Threading.Tasks;

namespace HorsesForCourses.MVC.Controllers;

public class CoachMVCController : Controller
{
    private readonly ICoachService _service;

    public CoachMVCController(ICoachService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var result = await _service.GetById(id);
        if (result == null)
            return NotFound();
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int size = 5, CancellationToken ct = default)
    {
        var result = await _service.GetAll(page, size, ct);
        if (result == null)
            return NotFound();
        return View(result);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string name, string email)
    {
        var result = await _service.AddCoach(name, email);
        if (result == null)
            return NotFound();
        return View(result);
    }
}
