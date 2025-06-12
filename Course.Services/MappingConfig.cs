using AutoMapper;
using Course.DataModel.Dtos.RequestDTOs;
using Course.DataModel.Dtos.ResponseDTOs;
using Course.DataModel.Entities;

namespace Course.Services;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Course.DataModel.Entities.Course, CourseResponseDto>().ReverseMap();
        CreateMap<Course.DataModel.Entities.Course, CourseRequestDto>().ReverseMap();

        CreateMap<Enrollment, EnrollmentResponseDto>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course!.Coursename))
                .ForMember(dest => dest.Credits, opt => opt.MapFrom(src => src.Course!.Credits))
                .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Course!.Department));

        CreateMap<User, AuthResponseDto>();
         
           CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.Passwordhash, opt => opt.Ignore());

            CreateMap<CourseRequestDto, DataModel.Entities.Course>();
    }
}
