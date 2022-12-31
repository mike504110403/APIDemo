using APIDemo_swagger.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace APIDemo_swagger.ModelBinder
{
    public class FormDateJsonBinder : IModelBinder // 模型繫結擴充方法，用來解決主程式傳值時多做的轉型
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;

            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }
            
            // 模型繫結擴充方法
            try
            {
                object result = JsonConvert.DeserializeObject(value, bindingContext.ModelType); // 傳進來甚麼型別就處理甚麼型別

                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (Exception)
            {
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}
