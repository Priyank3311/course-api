namespace Course.DataModel.Dtos.ResponseDTOs;

public class StudentProfileDto
{
     public string Username { get; set; } = null!;
        public int TotalCredits { get; set; }
        public int EnrolledCourses { get; set; }
}


