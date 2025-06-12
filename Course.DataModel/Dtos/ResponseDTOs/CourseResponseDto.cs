namespace Course.DataModel.Dtos.ResponseDTOs;

public class CourseResponseDto
{
    public int Id { get; set; }
    public string CourseName { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int Credits { get; set; }
    public string Department { get; set; } = null!;
}
