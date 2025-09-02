namespace HorsesForCourses.MVC.Models.Courses;

public class CreateCourseViewModel
{
    public string Name { get; set; } = string.Empty;

    public DateOnly Startdate { get; set; }

    public DateOnly Enddate { get; set; }

    public CreateCourseViewModel() { }

    public CreateCourseViewModel(string name, DateOnly startdate, DateOnly enddate)
    {
        Name = name;
        Startdate = startdate;
        Enddate = enddate;
    }
}