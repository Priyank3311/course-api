using FluentValidation;

namespace Course.DataModel.Dtos.RequestDTOs;

public class EnrollRequestDto
{
    public int CourseId { get; set; }
}
public class EnrollRequestValidation : AbstractValidator<EnrollRequestDto>
{
    public EnrollRequestValidation()
    {
        RuleFor(x => x.CourseId).GreaterThan(0);
    }
}
