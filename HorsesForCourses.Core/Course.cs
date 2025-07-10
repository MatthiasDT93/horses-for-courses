namespace HorsesForCourses.Core;

public enum States
{
    PENDING = 1,
    CONFIRMED = 2,
    FINALISED = 3
}

public class Course
{
    public string CourseName { get; set; }

    public States Status;

    public List<string> RequiredCompetencies { get; set; }

    public List<CourseTime> Planning { get; set; }

    public CourseTime Duration { get; set; }

    public Coach? coach { get; set; }

    public Course(string name, DateTime start, DateTime end)
    {
        CourseName = name;
        Status = States.PENDING;
        Planning = new();
        RequiredCompetencies = new();
        coach = null;
        Duration = CourseTime.From(start, end);
    }



    public void AddRequirements(string req)
    {
        if (Status != States.FINALISED)
        {
            if (!RequiredCompetencies.Contains(req))
            {
                RequiredCompetencies.Add(req);
            }
            else
            {
                throw new Exception("This required competence is already added.");
            }
        }
        else
        {
            throw new Exception("Course has been finalised and cannot be altered.");
        }
    }

    public void RemoveRequirement(string req)
    {
        if (Status != States.FINALISED)
        {
            if (!RequiredCompetencies.Remove(req))
            {
                throw new Exception("This course does not have this requirement.");
            }
        }
        else
        {
            throw new Exception("Course has been finalised and cannot be altered.");
        }
    }

    public void AddCourseMoment(CourseTime ct)
    {
        if (Status != States.FINALISED)
        {
            if (!Planning.Contains(ct))
            {
                var start = ct.StartTime;
                var end = ct.EndTime;
                while (end <= Duration.EndTime)
                {
                    Planning.Add(CourseTime.From(start, end));
                    start.AddDays(7.0);
                    end.AddDays(7.0);
                }
            }
            else
            {
                throw new Exception("This is already planned in.");
            }
        }
        else
        {
            throw new Exception("Course has been finalised and cannot be altered.");
        }
    }

    public void RemoveCourseMoment(CourseTime ct)
    {
        if (Status != States.FINALISED)
        {
            if (Planning.Contains(ct))
            {
                var start = ct.StartTime;
                var end = ct.EndTime;
                while (end <= Duration.EndTime)
                {
                    Planning.Remove(CourseTime.From(start, end));
                    start.AddDays(7.0);
                    end.AddDays(7.0);
                }
            }
            else
            {
                throw new Exception("This is not yet planned in.");
            }
        }
        else
        {
            throw new Exception("Course has been finalised and cannot be altered.");
        }
    }

    public void ConfirmCourse()
    {
        if (Status == States.PENDING) throw new Exception("Cannot confirm a course that's not in the PENDING state.");
        if (Planning.Count != 0) throw new Exception("Cannot confirm a course that does not have a planning yet.");

        Status = States.CONFIRMED;
    }

    public void AddCoach(Coach coach)
    {
        if (this.coach != null) throw new Exception("A coach is already planned in for this course.");
        if (Status != States.CONFIRMED) throw new Exception("Course needs to be CONFIRMED before adding a coach.");
        if (!coach.IsCompetent(RequiredCompetencies)) throw new Exception("The coach does not meet the requirements for teaching this course.");

        this.coach = coach;
        Status = States.FINALISED;
    }
}
