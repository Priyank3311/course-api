using FluentValidation;

namespace Course.DataModel.Dtos.RequestDTOs;

public class CourseRequestDto
{
    public string CourseName { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int Credits { get; set; }
    public string Department { get; set; } = null!;
}
public class CourseRequestValidation : AbstractValidator<CourseRequestDto>
{
    public CourseRequestValidation()
    {
        RuleFor(x => x.CourseName).NotEmpty();
        RuleFor(x => x.Credits).GreaterThan(0);
        RuleFor(x => x.Department).NotEmpty();
    }
}