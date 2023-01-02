using APIDemo_swagger.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace APIDemo_swagger.Filters
{
    public class FileLimitAttribute : Attribute, IResourceFilter
    {
        public long size = 100000;
        public void OnResourceExecuted(ResourceExecutedContext context) // 出去前驗證
        {
            // throw new NotImplementedException();
        }

        public void OnResourceExecuting(ResourceExecutingContext context) // 進來時驗證
        {
            var files = context.HttpContext.Request.Form.Files;

            foreach (var temp in files)
            {
                if (temp.Length > size) // 檔案大小驗證
                {
                    context.Result = new JsonResult(new RetrunJson()
                    {
                        Data = "",
                        HttpCode = 400,
                        ErrorMessage = "檔案太大囉"
                    });
                }

                if (Path.GetExtension(temp.FileName) != ".mp4") // 副檔名驗證
                {
                    context.Result = new JsonResult(new RetrunJson()
                    {
                        Data = "",
                        HttpCode = 400,
                        ErrorMessage = "只允許上傳mp4"
                    });

                }
            }
        }
    }
}
