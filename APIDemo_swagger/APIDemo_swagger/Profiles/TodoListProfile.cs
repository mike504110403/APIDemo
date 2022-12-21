using AutoMapper;
using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;

namespace APIDemo_swagger.Profiles
{
    public class TodoListProfile : Profile
    {
        public TodoListProfile() // Automapper 設定檔
        {
            CreateMap<TodoList, TodoListSelectDto>()
                .ForMember(
                dest => dest.InsertEmployeeName, // 目的地欄位
                opt => opt.MapFrom(src => src.InsertEmployee.Name + "(" + src.InsertEmployeeId + ")") // 來源
                )
                .ForMember(
                dest => dest.UpdateEmployeeName,
                opt => opt.MapFrom(src => src.UpdateEmployee.Name + "(" + src.UpdateEmployeeId + ")")
                );
            CreateMap<TodoListPostDto, TodoList>();

        }
    }
}
