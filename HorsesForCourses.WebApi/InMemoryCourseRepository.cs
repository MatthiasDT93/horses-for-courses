using HorsesForCourses.Core;

public class InMemoryCourseRepository
{
    private int nextid = 1;
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
            course.AssignId(nextid);
            _courses.Add(course);
            nextid++;
        }
    }

    public Course? GetById(int id)
    {
        return _courses.FirstOrDefault(c => c.Id == id);
    }

    public int GenerateNewId()
    {
        return nextid++;
    }
}