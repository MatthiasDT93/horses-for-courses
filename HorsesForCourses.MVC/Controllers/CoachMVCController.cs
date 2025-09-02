using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.MVC.Models;
using HorsesForCourses.Service;
using HorsesForCourses.WebApi;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Console;
using HorsesForCourses.MVC.Models.Coaches;

namespace HorsesForCourses.MVC.Controllers;

public class CoachMVCController : Controller
{
    private readonly ICoachService _service;

    public CoachMVCController(ICoachService service)
    {
        _service = service;
    }

    // public ActionResult Lookup()
    // {
    //     return View();
    // }

    // [HttpPost]
    // public ActionResult Lookup(int id)
    // {
    //     return RedirectToAction("Details", new { id = id });
    // }

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




    [HttpGet]
    public IActionResult CreateCoachForm()
    {
        return View(new CreateCoachViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCoachForm(string txtname, string txtemail)
    {
        var result = await _service.AddCoach(txtname, txtemail);
        if (result == null)
            return View(new CreateCoachViewModel(txtname, txtemail));
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> EditCoachSkillsForm(int id)
    {
        var coach = await _service.GetById(id);
        var model = new EditCoachSkillsViewModel(id, string.Join(", ", coach.Skills), coach.Name);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCoachSkillsForm(int txtid, string txtskillsinput)
    {
        var txtskills = txtskillsinput?
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .ToList() ?? new List<string>();
        var result = await _service.ModifySkills(txtskills, txtid);
        if (!result)
            return View(new EditCoachSkillsViewModel(txtid, txtskillsinput!));
        return RedirectToAction(nameof(Index));
    }
}
