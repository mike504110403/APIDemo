using APIDemo_swagger.Dtos;
using APIDemo_swagger.Interfaces;
using APIDemo_swagger.Parameters;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIDemo_swagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoIOCController : ControllerBase // IOC server注入 目前情境不需要
    {
        private readonly IEnumerable<ITodoListService> _todoListService; // 同時注入多個server
        public TodoIOCController(IEnumerable<ITodoListService> todoListService) // 取得服務注入 - 介面
        {
            _todoListService = todoListService;
        }

        // GET: api/<TodoIOCController>
        [HttpGet]
        public List<TodoListSelectDto> Get([FromQuery] TodoSelectParameters value)
        {
            ITodoListService _todo;
            // 因為注入了兩個server 此處依照Get傳進來([FromQuery])的type決定切哪支server
            if (value.type == "fun")
            {
                _todo = _todoListService.Where(a => a.type == "fun").Single();
            }
            else
            {
                _todo = _todoListService.Where(a => a.type == "automapper").Single();
            }

            return _todo.Selectdb(value);
        }

    }
}
