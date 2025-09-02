using HorsesForCourses.Service;

namespace HorsesForCourses.MVC.Models.Courses;

public class EditCourseTimeslotsViewModel
{
    public int Id { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public List<TimeSlotDTO> NewSlots { get; set; } = [];

    public EditCourseTimeslotsViewModel() { }

    public EditCourseTimeslotsViewModel(int id, string coursename, List<TimeSlotDTO> newslots)
    {
        Id = id;
        CourseName = coursename;
        NewSlots = newslots;
    }
}