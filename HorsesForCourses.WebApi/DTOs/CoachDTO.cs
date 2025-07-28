using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;

public class CoachDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> Competencies { get; set; }
    public List<Booking> Bookings { get; set; }


    public CoachDTO(string name, string mail, List<string> comp, List<Booking> bookings)
    {
        Name = name;
        Email = mail;
        Competencies = comp;
        Bookings = bookings;
    }

}