using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;
using System.ComponentModel.DataAnnotations;

namespace APIDemo_swagger.ValidationAttributes
{
    public class TodoNameAttribute: ValidationAttribute // name是否已存在判斷
    {
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            TodoContext _todoContext = (TodoContext)validationContext.GetService(typeof(TodoContext)); // 取得資料庫連線物件service

            var name = (string)value;

            var findName = from a in _todoContext.TodoLists
                           where a.Name == name
                           select a;

            var dto = validationContext.ObjectInstance; // 抓整個類別

            if (dto.GetType() == typeof(TodoListPutDto)) // 如果抓到的類別為更新的dto
            {
                var dtoUpdate = (TodoListPutDto)dto;
                findName = findName.Where(a => a.TodoId != dtoUpdate.TodoId); // 如果是更新 排除自己跟自己相同TodoID原始的那筆 這樣就不會被排
            }

            if (findName.FirstOrDefault() != null)
            {
                return new ValidationResult("已存在相同的待辦事項");
            }

            return ValidationResult.Success;
        }
    }
}
