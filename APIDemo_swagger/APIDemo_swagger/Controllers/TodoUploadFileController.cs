using Microsoft.AspNetCore.Mvc;
using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;
using AutoMapper;
using Microsoft.OpenApi.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIDemo_swagger.Controllers
{
    [Route("api/Todo/{TodoId}/UploadFile")]
    [ApiController]
    public class TodoUploadFileController : ControllerBase
    {
        //========================= 利用DI注入關聯物件 API結束後自動釋放資源 ================================//
        private readonly TodoContext _todoContext; // 唯讀   
        private readonly IMapper _iMapper; // AutoMapper
        public TodoUploadFileController(TodoContext todoContext, IMapper iMapper) // 另用建構子存至唯讀
        {
            _todoContext = todoContext;
            _iMapper = iMapper;
        }
        //========================= 利用DI注入關聯物件 API結束後自動釋放資源 ================================//

        // 撈該TodoID全部UploadFile
        // GET: api/<TodoUploadFileController>
        [HttpGet]
        public ActionResult<IEnumerable<UploadFileDto>> Get(Guid TodoId)
        {
            if (!_todoContext.TodoLists.Any(a => a.TodoId == TodoId)) // 如果找不到任一筆符合的TodoId
            {
                return NotFound("找不到該事項");
            }

            var result = from a in _todoContext.UploadFiles   // DB更新? 
                         where a.TodoId == TodoId
                         select ItemToDto(a);

            if (result == null || result.Count() == 0)
            {
                return NotFound("找不到檔案");
            }

            return Ok(result);
        }

        // 撈該TodoId一筆UploadFileId
        // GET api/<TodoUploadFileController>/UploadFileId
        [HttpGet("{UploadFileId}")]
        public ActionResult<UploadFileDto> Get(Guid TodoId, Guid UploadFileId)
        {
            if (!_todoContext.TodoLists.Any(a => a.TodoId == TodoId)) // 如果找不到任一筆符合的TodoId
            {
                return NotFound("找不到該事項");
            }

            var result = (from a in _todoContext.UploadFiles
                         where a.TodoId == TodoId && a.UploadFileId == UploadFileId
                         select ItemToDto(a)).SingleOrDefault();

            if (result == null)
            {
                return NotFound("找不到檔案");
            }

            return Ok(result);
        }

        // 新增該TodoId一筆uploadfile
        // POST api/<TodoUploadFileController>
        [HttpPost]
        public string Post(Guid TodoId, [FromBody] UploadFilePostDto value)
        {
            if (!_todoContext.TodoLists.Any(a => a.TodoId == TodoId)) // 如果找不到任一筆符合的TodoId
            {
                return "找不到該事項";
            }

            UploadFile insert = new UploadFile // 轉型
            {
                Name = value.Name,
                Src = value.Src,
                TodoId = TodoId
            };

            _todoContext.UploadFiles.Add(insert);
            _todoContext.SaveChanges();

            return TodoId + " uploadfile 已新增!";
        }

        // Automapper Dto 自動映射
        // 新增該TodoId一筆uploadfile
        // POST api/<TodoUploadFileController>
        [HttpPost("Automapper")]
        public string PostAutomapper(Guid TodoId, [FromBody] UploadFile value)
        {
            if (!_todoContext.TodoLists.Any(a => a.TodoId == TodoId)) // 如果找不到任一筆符合的TodoId
            {
                return "找不到該事項";
            }

            var map = _iMapper.Map<UploadFile>(value);
            map.TodoId = TodoId;

            _todoContext.UploadFiles.Add(map);
            _todoContext.SaveChanges();

            return TodoId + " uploadfile 已新增!";
        }

        // PUT api/<TodoUploadFileController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TodoUploadFileController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }


        //=========================== 函式化 ========================================//

        // Dto函式
        private static UploadFileDto ItemToDto(UploadFile a)
        {
            return new UploadFileDto
            {
                Name = a.Name,
                Src = a.Src,
                TodoId = a.TodoId,
                UploadFileId = a.UploadFileId
            };
        }

        //=========================== 函式化 ========================================//
    }
}
