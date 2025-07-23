using HorsesForCourses.Core;

public class InMemoryCourseRepository
{
    private readonly List<Course> _courses = new();

    public void SaveCourse(Course course)
    {
        int index = _courses.FindIndex(c => c.CourseName == course.CourseName && c.StartDate == course.StartDate && c.EndDate == course.EndDate);
        if (index >= 0)
        {
            _courses[index] = course;
        }
        else
        {
            _courses.Add(course);
        }
    }

    public Course? GetByCourseName(string name)
    {
        return _courses.FirstOrDefault(c => c.CourseName == name);
    }
}