using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;
using APIDemo_swagger.Parameters;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIDemo_swagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        //========================= 利用DI注入關聯物件 API結束後自動釋放資源 ================================//
        private readonly TodoContext _todoContext; // 唯讀   
        private readonly IMapper _iMapper; // AutoMapper
        public TodoController(TodoContext todoContext, IMapper iMapper) // 另用建構子存至唯讀
        {
            _todoContext = todoContext;
            _iMapper = iMapper;
        }
        //========================= 利用DI注入關聯物件 API結束後自動釋放資源 ================================//


        //================================================= 取得資料 =================================================================//
        //========================== GET LINQ - 將資料庫以物件存取 ============================================//
        // GET api/<TodoController>  
        // 關鍵字搜尋 傳參數有預設:直接傳預設FromQuery;用類別預設FromBody。因此須特別標示【FromQuery】
        [HttpGet]
        public IActionResult Get([FromQuery] TodoSelectParameters value)  // 回應狀態 // 若要回資料格式 => IEnumerable<TodoListSelectDto> 
        {
            var result = _todoContext.TodoLists.
                Include(a => a.UpdateEmployee).
                Include(a => a.InsertEmployee).
                Include(a => a.UploadFiles).
                Select(a => a);

            if (!string.IsNullOrWhiteSpace(value.name)) // 撈出包含name的
            {
                result = result.Where(a => a.Name.Contains(value.name)); 
            }

            if (value.enable != null) // 撈出enable符合的
            {
                result = result.Where(a => a.Enable == value.enable);
            }

            if (value.insertTime != null) // 撈出DateTime符合的
            {
                result = result.Where(a => a.InsertTime.Date == value.insertTime); 
            }

            if (value.minOrder != null && value.maxOrder != null) // 撈出order符合的
            {
                result = result.Where(a => a.Orders >= value.minOrder && a.Orders <= value.maxOrder); // 撈出介於minOrder-maxOrder
            }

            if (result == null || result.Count() <= 0) // 如果找不到資料
            {
                return NotFound("找不到資源"); // 找不到資源狀態
            }

            return Ok(result.ToList().Select(a => ItemToDto(a)));
        }
        // GET api/<TodoController>/5
        [HttpGet("{id}")]
        public ActionResult<TodoListSelectDto> Get(Guid id) // 回應狀態 - 實作 // 若要回資料格式 => TodoListSelectDto
        {
            var result = (from a in _todoContext.TodoLists
                          where a.TodoId == id
                          select a)
                          .Include(a => a.UpdateEmployee)
                          .Include(a => a.InsertEmployee)
                          .Include(a => a.UploadFiles)
                          .SingleOrDefault(); // singleordefault => 找不到資料就會為空
            if (result == null)
            {
                return NotFound("找不到Id為'" + id + "'的資料");
                // Response.StatusCode = 404;
            }
            return Ok(ItemToDto(result));
        }
        //========================== GET LINQ - 將資料庫以物件存取 ============================================//
        //====================== SQL 撈資料 ======================================//
        // sql語法撈
        [HttpGet("GetSQL")]
        public IEnumerable<TodoList> GetSQL(string? name)
        {
            string sql = "select * from todolist where 1=1";

            if (!string.IsNullOrWhiteSpace(name))
            {
                sql = sql + "and name like N'%" + name + "%'";   // Sql injection => name = ';update [TodoList] set name = N'去上課' where [TodoId] = '' --
            }
            var result = _todoContext.TodoLists.FromSqlRaw(sql); // 下sql語法

            return result;
        }
        // sql語法撈Dto欄位
        [HttpGet("GetSQLDto")]
        public IEnumerable<TodoListSelectDto> GetSQLDto(string? name)
        {
            string sql = @"SELECT [TodoId], a.[Name], [InsertTime], [UpdateTime], [Enable], [Orders]
                        , b.Name as InsertEmployeeName, c.Name as UpdateEmployeeName
                        FROM [TodoList] a
                        join Employee b on a.InsertEmployeeId = b.EmployeeId
                        join Employee c on a.UpdateEmployeeId = c.EmployeeId 
                        where 1=1";

            if (!string.IsNullOrWhiteSpace(name))
            {
                sql = sql + "and name like N'%" + name + "%'";   
            }
            var result = _todoContext.TodoListSelectDto.FromSqlRaw(sql); // 下sql語法

            return result;
        }
        //====================== SQL 撈資料 ======================================//
        //====================== Automapper Dto 自動映射======================================//
        [HttpGet("Automapper")] 
        public IEnumerable<TodoListSelectDto> GetAutoMapper([FromQuery] TodoSelectParameters value)
        { 
            var result = _todoContext.TodoLists.
                Include(a => a.UpdateEmployee).
                Include(a => a.InsertEmployee).
                Select(a => a);

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
            var map = _iMapper.Map<IEnumerable<TodoListSelectDto>>(result);

            return map;
        }
        [HttpGet("Automapper/{id}")]
        public TodoListSelectDto GetAutoMapper(Guid id)
        {
            var result = (from a in _todoContext.TodoLists
                          where a.TodoId == id
                          select a)
                          .Include(a => a.UpdateEmployee)
                          .Include(a => a.InsertEmployee).SingleOrDefault();

            return _iMapper.Map<TodoListSelectDto>(result);
        }
        //====================== Automapper Dto 自動映射 ======================================//
        //====================== api引數吃法展示 ======================================//
        // GET api/<TodoController>/from/{id}
        // 有預設吃route 無預設吃query
        [HttpGet("From/{id}")]
        public dynamic GetFrom([FromRoute] string id, [FromQuery] string id2, [FromForm] string id4) // [FromBody]string id3,
        {
            List<dynamic> result = new List<dynamic>();

            result.Add(id);
            result.Add(id2);
            // result.Add(id3);
            result.Add(id4);

            return result;
        }
        //====================== api引數吃法 ======================================//
        //================================================= 取得資料 =================================================================//


        //================================================= 新增資料 =================================================================//
        //============================ 同時新增父子資料 ======================================//
        // 有設外鍵情況下同時新增父子資料 只有新增父資料情況有異常 -> 待解
        // POST api/<TodoController>
        [HttpPost]
        public void Post([FromBody] TodoListPostDto value) // IActionResult回應狀態
        {
            // 轉型以給定uploadfiles
            List<UploadFile> upl = new List<UploadFile>();

            foreach (var temp in value.UploadFiles)
            {
                UploadFile up = new UploadFile()
                {
                    Name = temp.Name,
                    Src = temp.Src
                };
                upl.Add(up);
            };

            TodoList insert = new TodoList // 轉型
            {
                // 使用者能決定的欄位
                Name = value.Name,
                Enable = value.Enable,
                Orders = value.Orders,
                // 系統決定的欄位
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"), // 還沒做 先寫死
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UploadFiles = upl // 子資料 (db已做外鍵關聯 Uploadfile內的todoid不用給 會自動吃父資料的)
            };
            _todoContext.TodoLists.Add(insert);
            _todoContext.SaveChanges();

            // return CreatedAtAction(nameof(Get), new { TodoId = insert.TodoId} , insert);

        }

        // 未設外鍵情況下同時新增父子資料
        // POST api/<TodoController>/nofk
        [HttpPost("nofk")]
        public void Postnofk([FromBody] TodoListPostDto value)
        {
            // 新增父資料
            TodoList insert_f = new TodoList // 轉型
            {
                // 使用者能決定的欄位
                Name = value.Name,
                Enable = value.Enable,
                Orders = value.Orders,
                // 系統決定的欄位
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"), // 還沒做 先寫死
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001")
            };

            _todoContext.TodoLists.Add(insert_f);
            _todoContext.SaveChanges(); // 先存父資料 下方存子資料才取的到TodoId

            // 新增子資料
            foreach (var temp in value.UploadFiles)
            {
                UploadFile insert_c = new UploadFile // 轉型
                {
                    Name = temp.Name,
                    Src = temp.Src,
                    TodoId = insert_f.TodoId
                    // UploadFileId = temp.UploadFileId // 此處才建立upliadfile id此時自動生成 無須給定 且 body給值也不用給UploadFileId
                };
                _todoContext.UploadFiles.Add(insert_c);
            };

            _todoContext.SaveChanges();


        }
        //============================ 同時新增父子資料 ======================================//
        //====================== Automapper Dto 自動映射======================================//
        // 有設外鍵情況下同時新增父子資料
        // POST api/<TodoController>
        [HttpPost("AutoMapper")]
        public void PostAutoMapper([FromBody] TodoList value)
        {
            var map = _iMapper.Map<TodoList>(value);

            // 系統決定的欄位 先寫死
            map.InsertTime = DateTime.Now;
            map.UpdateTime = DateTime.Now;
            map.InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // 還沒做 先寫死
            map.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            // map.UploadFiles = upl; // automapper連同子資料憶起自動轉

            _todoContext.TodoLists.Add(map);
            _todoContext.SaveChanges();
        }
        //====================== Automapper Dto 自動映射 ======================================//
        //============================ SQL 新增資料 ======================================//
        // POST api/<TodoController>/postSQL
        [HttpPost("postSQL")]
        public void PostSQL([FromBody] TodoListPostDto value)
        {
            var name = new SqlParameter("name", value.Name); // 語法轉成單純字串 避免sql injection --> 參數化

            string sql = @"INSERT INTO [dbo].[TodoList]
                            ([Name],[InsertTime],[UpdateTime],[Enable],[Orders],[InsertEmployeeId],[UpdateEmployeeId]) 
                            VALUES
                            (@name,'"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"','"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            +"','"+value.Enable+"',"+value.Orders+",'00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000001') ";
           
            _todoContext.Database.ExecuteSqlRaw(sql, name); // 直接送sql
        }
        //============================ SQL 新增資料 ======================================//

        //================================================= 新增資料 =================================================================//

        // PUT api/<TodoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TodoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        //=========================== 函式化 ========================================//

        // Dto函式 轉型資料 -> 只顯示需要顯示的欄位
        private static TodoListSelectDto ItemToDto(TodoList a)
        {
            List<UploadFileDto> updto = new List<UploadFileDto>();

            foreach (var temp in a.UploadFiles)
            {
                UploadFileDto up = new UploadFileDto() // 逐筆轉換子資料
                {
                    Name = temp.Name,
                    Src = temp.Src,
                    TodoId = temp.TodoId,
                    UploadFileId = temp.UploadFileId
                };
                updto.Add(up);
            }

            return new TodoListSelectDto
            {
                Enable = a.Enable,
                InsertEmployeeName = a.InsertEmployee.Name,
                InsertTime = a.InsertTime,
                Name = a.Name,
                Orders = a.Orders,
                TodoId = a.TodoId,
                UpdateEmployeeName = a.UpdateEmployee.Name,
                UpdateTime = a.UpdateTime,
                UploadFiles = updto
            };
        }

        //=========================== 函式化 ========================================//

    }
}
