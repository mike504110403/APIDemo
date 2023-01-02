using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace APIDemo_swagger.Filters
{
    public class TodoAuthorizationFilter2 : Attribute, IAuthorizationFilter // 測試用 驗證filter
    {
        public string Roles = "";
        public TodoAuthorizationFilter2() { }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            TodoContext _todoContext = (TodoContext)context.HttpContext.RequestServices.GetService(typeof(TodoContext)); // 要給值只能用這個方式注入 不能用建構子

            //var role =(from a in _todoContext.Roles
            //          where a.name == Roles
            //          select a).FirstOrDefault();

            //if (role == null)
            //{
            //    context.Result = new JsonResult(new RetrunJson()
            //    {
            //        Data = Roles,
            //        HttpCode = 401,
            //        ErrorMessage = "沒有登入"
            //    });
            //}
        }
    }
}
