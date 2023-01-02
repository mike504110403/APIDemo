using APIDemo_swagger.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace APIDemo_swagger.Filters
{
    public class TodoAuthorizationFilter : Attribute, IAuthorizationFilter // 測試用 驗證filter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool tokenFlag = context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues outValue); // 取request header，取得到丟給outVlue、給true;取不到給false

            var ignore = (from a in context.ActionDescriptor.EndpointMetadata
                         where a.GetType() == typeof(AllowAnonymousAttribute) // 借用[AllowAnonymous]標籤 以避開全域設定
                         select a).FirstOrDefault();

            if (ignore == null) // 沒有忽略的才進權限驗證
            {
                if (tokenFlag)
                {
                    if (outValue != "123")
                    {
                        context.Result = new JsonResult(new RetrunJson()
                        {
                            Data = "test1",
                            HttpCode = 401,
                            ErrorMessage = "取值錯誤"
                        });
                    }
                }
                else
                {
                    context.Result = new JsonResult(new RetrunJson()
                    {
                        Data = "test2",
                        HttpCode = 401,
                        ErrorMessage = "取不到值"
                    });
                }
            }
        }
    }
}
