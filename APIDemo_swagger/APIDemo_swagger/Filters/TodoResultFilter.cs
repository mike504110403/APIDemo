using APIDemo_swagger.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APIDemo_swagger.Filters
{
    public class TodoResultFilter : IResultFilter // 統一API回傳格式
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            // throw new NotImplementedException();
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            var contextResult = context.Result as ObjectResult;

            if (context.ModelState.IsValid)
            {
                context.Result = new JsonResult(new RetrunJson()
                {
                    Data = contextResult.Value
                });
            }
            else
            {
                context.Result = new JsonResult(new RetrunJson()
                {
                    Error = contextResult.Value
                });
            }
            
        }
    }
}
