using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HorsesForCourses.MVC.Models;
using HorsesForCourses.Service;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Console;
using HorsesForCourses.MVC.Models.Courses;
using System.Text.Json;

namespace HorsesForCourses.MVC.Controllers;

public class CourseMVCController : Controller
{
    private readonly ICourseService _service;

    public CourseMVCController(ICourseService service)
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


    [HttpGet]
    public IActionResult CreateCourseForm()
    {
        return View(new CreateCourseViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCourseForm(string txtcoursename, DateOnly txtstart, DateOnly txtend)
    {
        var result = await _service.AddCourse(txtcoursename, txtstart, txtend);
        if (result == null)
            return View(new CreateCourseViewModel(txtcoursename, txtstart, txtend));
        return RedirectToAction(nameof(Index));
    }


    [HttpGet]
    public async Task<IActionResult> EditSkillsMenu(int id)
    {
        var course = await _service.GetById(id);
        if (course == null)
            return NotFound();
        var model = new EditCourseSkillsViewModel
        {
            CourseId = course.Id,
            CourseName = course.Name,
            Skills = course.Skills.ToList()
        };
        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSkills(EditCourseSkillsViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var success = await _service.ModifySkills(model.Skills, model.CourseId);
        if (!success)
            return NotFound();

        return RedirectToAction(nameof(Details), new { id = model.CourseId });
    }


    [HttpGet]
    public async Task<IActionResult> EditTimeSlotsMenu(int id)
    {
        var course = await _service.GetById(id);
        var model = new EditCourseTimeslotsViewModel(course.Id, course.Name, course.timeslots);
        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCourseTimeslotsForm(EditCourseTimeslotsViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _service.ModifyTimeSlots(model.NewSlots, model.Id);
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }
}
