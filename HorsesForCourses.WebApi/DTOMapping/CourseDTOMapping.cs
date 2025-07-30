using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;

public class CourseDTOMapping
{

    public static Course DTO_To_Course(CourseDTO dto)
    {
        var timeslotlist = TimeslotDTOMapping.DTOList_To_TimeslotList(dto.Planning);
        var course = new Course(dto.Name, dto.Start, dto.End);
        course.OverWriteRequirements(dto.Requirements);
        course.OverWriteCourseMoment(timeslotlist);

        return course;
    }

    public static CourseDTO Course_To_DTO(Course course)
    {
        var dtotimeslots = TimeslotDTOMapping.TimeslotList_To_DTOList(course.Planning);
        return new CourseDTO(course.CourseName, course.StartDate, course.EndDate, course.RequiredCompetencies, dtotimeslots);
    }

    public static List<Course> DTOList_To_CourseList(List<CourseDTO> dtolist)
    {
        List<Course> courses = new();
        foreach (var dto in dtolist)
        {
            courses.Add(DTO_To_Course(dto));
        }
        return courses;
    }

    public static List<CourseDTO> CourseList_To_DTOList(List<Course> courses)
    {
        List<CourseDTO> dtolist = new();
        foreach (var course in courses)
        {
            dtolist.Add(Course_To_DTO(course));
        }
        return dtolist;
    }
}