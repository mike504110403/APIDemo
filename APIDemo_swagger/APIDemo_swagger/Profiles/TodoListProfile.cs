using AutoMapper;
using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;

namespace APIDemo_swagger.Profiles
{
    public class TodoListProfile : Profile
    {
        public TodoListProfile() // Automapper 設定檔
        {
            // Get
            CreateMap<TodoList, TodoListSelectDto>()
                .ForMember(
                dest => dest.InsertEmployeeName, // 目的地欄位
                opt => opt.MapFrom(src => src.InsertEmployee.Name + "(" + src.InsertEmployeeId + ")") // 來源
                )
                .ForMember(
                dest => dest.UpdateEmployeeName,
                opt => opt.MapFrom(src => src.UpdateEmployee.Name + "(" + src.UpdateEmployeeId + ")")
                )
                .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => src.Name + "(use automapper)")
                );
            // Post
            CreateMap<TodoListPostDto, TodoList>().ForMember(
                dest => dest.UpdateTime, // 目的地欄位
                opt => opt.MapFrom(src => DateTime.Now) // 系統指定值
                ).ForMember(
                dest => dest.InsertTime, // 目的地欄位
                opt => opt.MapFrom(src => DateTime.Now) // 系統指定值
                )
                .ForMember(
                dest => dest.UpdateEmployeeId,
                opt => opt.MapFrom(src => Guid.Parse("00000000-0000-0000-0000-000000000001"))
                ).ForMember(
                dest => dest.InsertEmployeeId,
                opt => opt.MapFrom(src => Guid.Parse("00000000-0000-0000-0000-000000000001"))
                );

            // Put
            CreateMap<TodoListPutDto, TodoList>().ForMember(
                dest => dest.UpdateTime, // 目的地欄位
                opt => opt.MapFrom(src => DateTime.Now) // 系統指定值
                ) 
                .ForMember(
                dest => dest.UpdateEmployeeId,
                opt => opt.MapFrom(src => Guid.Parse("00000000-0000-0000-0000-000000000001"))
                );

        }
    }
}
