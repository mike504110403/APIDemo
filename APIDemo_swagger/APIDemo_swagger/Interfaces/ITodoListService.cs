using APIDemo_swagger.Dtos;
using APIDemo_swagger.Parameters;

namespace APIDemo_swagger.Interfaces
{
    public interface ITodoListService // 定義介面 // IOC server注入 目前情境不需要
    {
        string type { get; } // 注入server type
        List<TodoListSelectDto> Selectdb(TodoSelectParameters value);
    }
}
