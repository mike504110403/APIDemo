using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;
using APIDemo_swagger.Parameters;
using APIDemo_swagger.Services;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using System;
using JsonPatchDocument = Microsoft.AspNetCore.JsonPatch.JsonPatchDocument;
using System.Collections.Generic;
using APIDemo_swagger.ModelBinder;
using Microsoft.AspNetCore.Authorization;
using APIDemo_swagger.Filters;
using Microsoft.AspNetCore.Authorization.Infrastructure;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIDemo_swagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // 身分驗證
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _todoContext; //   
        private readonly TodoListService _todoListService; // 取得資料服務注入
        private readonly IMapper _iMapper; // AutoMapper
        private readonly IWebHostEnvironment _env;

        public TodoController(TodoContext todoContext, IMapper iMapper, TodoListService todoListService, IWebHostEnvironment env) // 另用建構子存至唯讀
        {
            _todoContext = todoContext;
            _iMapper = iMapper;
            _todoListService = todoListService;
            _env = env;
        }

        //================================================= 取得資料 =================================================================//

        // GET api/<TodoController>
        // [Authorize] // 身分驗證
        // [Authorize(Roles = "select")]
        [TodoAuthorizationFilter2(Roles = "aaa")]        
        [HttpGet]
        public IActionResult Get([FromQuery] TodoSelectParameters value)  // 關鍵字搜尋 傳參數有預設:直接傳預設FromQuery;用類別預設FromBody。因此須特別標示【FromQuery】
        {
            var result = _todoListService.Selectdb(value); // 取得資料

            if (result == null || result.Count() <= 0) // 如果找不到資料
            {
                return NotFound("找不到資源"); // 找不到資源狀態
            }

            return Ok(result);
        }

        // GET api/<TodoController>/5
        [HttpGet("{id}")]
        public ActionResult<TodoListSelectDto> Get(Guid id) // 回應狀態 - 實作 // 若要回資料格式 => TodoListSelectDto
        {
            var result = _todoListService.SelectOnedb(id); // 取得單筆資料

            if (result == null) // 如果找不到資料 
            {
                return NotFound("找不到Id為'" + id + "'的資料");
            }

            return Ok(result);
        }

        // GET api/<TodoController>/Automapper
        // [Authorize(Roles = "automapper")]
        [HttpGet("Automapper")] 
        public IEnumerable<TodoListSelectDto> GetAutoMapper([FromQuery] TodoSelectParameters value)
        { 
            return _todoListService.AutomapperSelectdb(value);
        }

        // GET api/<TodoController>/Automapper/id
        [HttpGet("Automapper/{id}")]
        public ActionResult<TodoListSelectDto> GetAutoMapper(Guid id)
        {
            var result = _todoListService.AutomapperSelectOnedb(id); // 取得單筆資料

            if (result == null) // 如果找不到資料 
            {
                return NotFound("找不到Id為'" + id + "'的資料");
            }

            return Ok(result);
        }

        // GET api/<TodoController>/GetSQL
        [HttpGet("GetSQL")]
        public IEnumerable<TodoList> GetSQL(string? name) //sql語法撈 
        {
            return _todoListService.Selectsqldb(name);
        }

        // GET api/<TodoController>/GetSQLDto
        [HttpGet("GetSQLDto")]
        public IEnumerable<TodoListSelectDto> GetSQLDto(string? name) // sql語法撈 Dto轉型秀特定欄位 // 參數查詢有問題 -> 待解
        {
            return _todoListService.Selectsqldtodb(name);
        }

        //================================================= 新增資料 =================================================================//
         
        // POST api/<TodoController>
        [HttpPost]
        public IActionResult Post([FromBody] TodoListPostDto value) // 有設外鍵情況下同時新增父子資料 只有新增父資料情況有異常 -> 待解
        {
            var insert = _todoListService.Postdb(value); // 新增資料
            // return CreatedAtAction(nameof(Get), new { TodoId = insert.TodoId} , insert);
            return Ok();
        }

        // POST api/<TodoController>/up
        [HttpPost("up")]
        public void PostUp([FromForm] TodoListPostUpDto value) // 一次請求，同時新增備忘錄及檔案 
        { 
            // TodoList _value = JsonConvert.DeserializeObject<TodoList>(value); // 寫成模型繫結擴充方法

            TodoList insert = new TodoList()
            {
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001")
            };

            _todoContext.TodoLists.Add(insert).CurrentValues.SetValues(value.TodoList);
            _todoContext.SaveChanges();

            string rootRoot = _env.ContentRootPath + @"\wwwroot\UploadFiles\" + insert.TodoId + "\\";

            if (!Directory.Exists(rootRoot))
            {
                Directory.CreateDirectory(rootRoot);
            }

            foreach (var file in value.files)
            {
                if (file.Length > 0)
                {
                    string fileName = file.FileName;

                    using (var stream = System.IO.File.Create(rootRoot + fileName)) // 開啟檔名為fileName的檔案
                    {
                        file.CopyTo(stream); // 儲存至路徑stream

                        var insert_file = new UploadFile
                        {
                            Name = file.Name,
                            Src = "/UploadFiles/" + insert.TodoId + "/" + fileName,
                            TodoId = insert.TodoId
                        };

                        _todoContext.UploadFiles.Add(insert_file);
                    }
                }
            }

            _todoContext.SaveChanges();
        }

        // POST api/<TodoController>/nofk
        [HttpPost("nofk")]
        public void Postnofk([FromBody] TodoListPostDto value) // 未設外鍵情況下同時新增父子資料
        {
            _todoListService.PostNofkdb(value); // 新增資料
        }

        // POST api/<TodoController>/AutoMapper
        [HttpPost("AutoMapper")]
        public void PostAutoMapper([FromBody] TodoListPostDto value) // 有設外鍵情況下同時新增父子資料 // 同時新增父子資料情況下異常 -> 待解
        {
            _todoListService.AutomapperPostdb(value);
        }

        // POST api/<TodoController>/postSQL
        [HttpPost("postSQL")]
        public void PostSQL([FromBody] TodoListPostDto value)
        {
            _todoListService.Postsql(value);
        }

        //================================================= 更新資料 =================================================================//

        // PUT api/<TodoController>/id
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] TodoListPutDto value)
        {
            if (id != value.TodoId)
            {
                return BadRequest();
            }
            if (_todoListService.Putdb(id, value) == 0)
            {
                return NotFound();
            }
            return NoContent();
        }

        // PUT api/<TodoController>
        [HttpPut]
        public IActionResult Put([FromBody] TodoListPutDto value) //路由不帶id的更新 - 不符合restful API
        {
            if (_todoListService.Putnroutedb(value) == 0)
            {
                return NotFound();
            }
            return NoContent();
        }

        // PUT api/<TodoController>/AutoMapper/id
        [HttpPut("AutoMapper/{id}")]
        public IActionResult PutAutoMapper(Guid id, [FromBody] TodoListPutDto value)
        {
            if (id != value.TodoId)
            {
                return BadRequest();
            }
            if (_todoListService.AutomapperPutdb(id, value) == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        //================================================= 取代資料 =================================================================//
        [HttpPatch("{id}")]
        public IActionResult Patch(Guid id, [FromBody] JsonPatchDocument value)
        {
            if (_todoListService.Patchdb(id, value) == 0)
            {
                return NotFound("找不到該筆資料");
            }
            return NoContent();
        }

        //================================================= 刪除資料 =================================================================//

        // DELETE api/<TodoController>/id
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            if (_todoListService.Deletedb(id) == 0)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE api/nofk/<TodoController>/nofk/id
        [HttpDelete("nofk/{id}")]
        public IActionResult NofkDelete(Guid id) // 同時刪除多筆資料、無外鍵下刪除子資料
        {
            if (_todoListService.Deletenofkdb(id) == 0) // 找不到資料
            {
                return NotFound("無該筆資料");
            }
            return NoContent();
        }

        // DELETE api/list/<TodoController>/5
        [HttpDelete("list/{ids}")]
        public IActionResult Delete(string ids) // 同時刪除多筆指定資料
        {
            if (_todoListService.Deletenumdb(ids) == 0) // 找不到資料
            {
                return NotFound("無該筆資料");
            }
            return NoContent();
        }

    }
}
