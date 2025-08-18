using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;

public class CoachDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> Competencies { get; set; }
    public List<BookingDTO> Bookings { get; set; }
    public List<Course> Courses { get; set; }

    public CoachDTO() { }
    public CoachDTO(string name, string mail, List<string> comp, List<BookingDTO> bookings, List<Course> courses)
    {
        Name = name;
        Email = mail;
        Competencies = comp;
        Bookings = bookings;
        Courses = courses;
    }

}