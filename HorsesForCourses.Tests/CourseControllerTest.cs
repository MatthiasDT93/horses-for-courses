

using HorsesForCourses.WebApi.Controllers;

public class CourseControllerTest
{
    public InMemoryCourseRepository courserepo { get; set; }
    public InMemoryCoachRepository coachrepo { get; set; }

    public CourseController controller { get; set; }

    public CourseControllerTest()
    {
        courserepo = new();
        coachrepo = new();
        controller = new CourseController(courserepo, coachrepo);
    }


    [Fact]
    public void Adding_A_Course_Works()
    {

    }


}