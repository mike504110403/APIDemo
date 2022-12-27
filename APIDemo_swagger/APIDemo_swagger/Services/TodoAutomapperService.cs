using APIDemo_swagger.Dtos;
using APIDemo_swagger.Interfaces;
using APIDemo_swagger.Models;
using APIDemo_swagger.Parameters;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace APIDemo_swagger.Services
{
    public class TodoAutomapperService: ITodoListService // 實作介面 // IOC server注入 目前情境不需要
    {
        public string type => "automapper";
        private readonly TodoContext _todoContext; // service
        private readonly IMapper _iMapper; // AutoMapper
        public TodoAutomapperService(TodoContext todoContext, IMapper mapper)
        {
            _todoContext = todoContext;
            _iMapper = mapper;
        }

        public List<TodoListSelectDto> Selectdb(TodoSelectParameters value)
        {
            var result = _todoContext.TodoLists
                        .Include(a => a.UpdateEmployee)
                        .Include(a => a.InsertEmployee)
                        .Select(a => a);

            if (!string.IsNullOrWhiteSpace(value.name))
            {
                result = result.Where(a => a.Name.Contains(value.name)); // 撈出包含name的
            }

            if (value.enable != null)
            {
                result = result.Where(a => a.Enable == value.enable);
            }

            if (value.insertTime != null)
            {
                result = result.Where(a => a.InsertTime.Date == value.insertTime); // 撈出DateTime符合的
            }

            if (value.minOrder != null && value.maxOrder != null)
            {
                result = result.Where(a => a.Orders >= value.minOrder && a.Orders <= value.maxOrder); // 撈出介於minOrder-maxOrder
            }

            // automapper 自動映射
            return _iMapper.Map<IEnumerable<TodoListSelectDto>>(result).ToList();
        }
    }
}
