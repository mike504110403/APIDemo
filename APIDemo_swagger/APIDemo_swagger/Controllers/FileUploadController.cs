using APIDemo_swagger.Filters;
using APIDemo_swagger.Models;
using APIDemo_swagger.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIDemo_swagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly TodoContext _todoContext;
        public FileUploadController(IWebHostEnvironment env, TodoContext todoContext)
        {
            _env = env;
            _todoContext = todoContext;
        }

        // Post: api/<FileUploadController>        
        [HttpPost]
        public void Post(IFormFile file1) // 上傳單一檔案
        {
            string rootRoot = _env.ContentRootPath + @"\wwwroot\";

            if (file1.Length > 0) 
            {
                string fileName = file1.FileName;

                using (var stream = System.IO.File.Create(rootRoot + fileName)) // 開啟檔名為fileName的檔案
                {
                    file1.CopyTo(stream); // 儲存至路徑stream
                }
            }
        }

        // Post: api/<FileUploadController>
        [FileLimit(size = 1)]
        [HttpPost("{id}")]
        public void Post(List<IFormFile> files, Guid id) // 上傳複數檔案
        {
            string rootRoot = _env.ContentRootPath + @"\wwwroot\UploadFiles\" + id + "\\";

            if (!Directory.Exists(rootRoot))
            {
                Directory.CreateDirectory(rootRoot);
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    string fileName = file.FileName;

                    using (var stream = System.IO.File.Create(rootRoot + fileName)) // 開啟檔名為fileName的檔案
                    {
                        file.CopyTo(stream); // 儲存至路徑stream

                        var insert = new UploadFile
                        {
                            Name = file.Name,
                            Src = "/UploadFiles/" +id +"/" + fileName,
                            TodoId = id
                        };

                        _todoContext.UploadFiles.Add(insert);
                    }
                }
            }

            _todoContext.SaveChanges();
        }

    }
}
