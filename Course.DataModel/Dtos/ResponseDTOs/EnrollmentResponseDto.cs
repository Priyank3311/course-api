namespace Course.DataModel.Dtos.ResponseDTOs;

public class EnrollmentResponseDto
{
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public int Credits { get; set; }
    public string Department { get; set; } = null!;
    public bool IsCompleted { get; set; }
}
