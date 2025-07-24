namespace HorsesForCourses.Core;

public enum States
{
    PENDING = 1,
    CONFIRMED = 2,
    FINALISED = 3
}

public class Course
{
    public Guid Id { get; private set; }
    public string CourseName { get; set; }

    private States Status;

    public List<string> RequiredCompetencies { get; }

    public List<Timeslot> Planning { get; }

    public DateOnly StartDate { get; }

    public DateOnly EndDate { get; }

    public Coach? coach;

    public Course(string name, DateOnly start, DateOnly end)
    {
        CourseName = name;
        Status = States.PENDING;
        Planning = new();
        RequiredCompetencies = new();
        coach = null;
        StartDate = start;
        EndDate = end;
        Id = Guid.NewGuid();
    }



    public void AddRequirement(string req)
    {
        if (Status != States.FINALISED)
        {
            if (!RequiredCompetencies.Contains(req)) { RequiredCompetencies.Add(req); }
            else
                throw new Exception("This required competence is already added.");
        }
        else
            throw new Exception("Course has been finalised and cannot be altered.");
    }

    public void RemoveRequirement(string req)
    {
        if (Status != States.FINALISED)
        {
            if (!RequiredCompetencies.Remove(req)) { throw new Exception("This course does not have this requirement."); }
        }
        else
            throw new Exception("Course has been finalised and cannot be altered.");
    }

    public void AddCourseMoment(Timeslot slot)
    {
        if (Status != States.FINALISED)
        {
            if (!Planning.Any(p => p.Overlap(slot))) { Planning.Add(slot); }
            else
                throw new Exception("There is overlap between the time slots.");
        }
        else
            throw new Exception("Course has been finalised and cannot be altered.");
    }

    public void RemoveCourseMoment(Timeslot slot)
    {
        if (Status != States.FINALISED)
        {
            if (!Planning.Remove(slot)) { throw new Exception("This is not yet planned in."); }
        }
        else
            throw new Exception("Course has been finalised and cannot be altered.");
    }

    public void ConfirmCourse()
    {
        if (Status != States.PENDING) throw new Exception($"Cannot confirm a course that's not in the PENDING state, current state is: {Status}.");
        if (Planning.Count == 0) throw new Exception("Cannot confirm a course that does not have a planning yet.");

        Status = States.CONFIRMED;
    }

    public void AddCoach(Coach coach)
    {
        if (Status != States.FINALISED)
        {
            if (Status == States.PENDING) throw new Exception("Course needs to be CONFIRMED before adding a coach.");
            if (!coach.IsCompetent(RequiredCompetencies)) throw new Exception("The coach does not meet the requirements for teaching this course.");
            if (Planning.Intersect(coach.bookings).Any()) throw new Exception("The coach's schedule does not match the planning of the course.");

            this.coach = coach;
            coach.BookIn(Planning);
            Status = States.FINALISED;
        }
        else
            throw new Exception("Course has been finalised and cannot be altered.");
    }
}
