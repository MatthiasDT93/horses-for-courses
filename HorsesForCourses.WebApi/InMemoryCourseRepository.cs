using HorsesForCourses.Core;

public class InMemoryCourseRepository
{
    private readonly List<Course> _courses = new();

    public IReadOnlyList<Course> Courses => _courses;

    public void SaveCourse(Course course)
    {
        int index = _courses.FindIndex(c => c.Id == course.Id);
        if (index >= 0)
        {
            _courses[index] = course;
        }
        else
        {
            _courses.Add(course);
        }
    }

    public Course? GetById(Guid id)
    {
        return _courses.FirstOrDefault(c => c.Id == id);
    }
}