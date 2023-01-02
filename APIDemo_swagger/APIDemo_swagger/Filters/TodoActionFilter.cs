using APIDemo_swagger.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APIDemo_swagger.Filters
{
    public class TodoActionFilter : IActionFilter // 利用actionfilter全域寫log檔
    {
        private readonly IWebHostEnvironment _env;
        public TodoActionFilter(IWebHostEnvironment env)
        {
            _env = env;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            string rootRoot = _env.ContentRootPath + @"\Log\";

            if (!Directory.Exists(rootRoot))
            {
                Directory.CreateDirectory(rootRoot);
            }

            var employeeid = context.HttpContext.User.FindFirst("EmployeeId");
            var path = context.HttpContext.Request.Path;
            var method = context.HttpContext.Request.Method;

            string text = "結束: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " path:" + path + " method:" + method + " " + employeeid + "\n";

            File.AppendAllText(rootRoot + DateTime.Now.ToString("yyyyMMdd") + ".txt", text);

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string rootRoot = _env.ContentRootPath + @"\Log\";

            if (!Directory.Exists(rootRoot))
            {
                Directory.CreateDirectory(rootRoot);
            }

            var employeeid = context.HttpContext.User.FindFirst("EmployeeId");
            var path = context.HttpContext.Request.Path;
            var method = context.HttpContext.Request.Method;

            string text = "開始: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " path:" + path + " method:" + method + " " + employeeid + "\n";

            File.AppendAllText(rootRoot + DateTime.Now.ToString("yyyyMMdd") + ".txt", text);
        }
    }
}
