namespace Course.DataModel.Dtos.ResponseDTOs;

public class StudentInCourseDto
{
    public int StudentId { get; set; }
    public string Username { get; set; } = null!;
    public bool IsCompleted { get; set; }
}
