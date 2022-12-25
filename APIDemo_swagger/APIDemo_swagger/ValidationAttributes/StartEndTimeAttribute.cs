using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;
using System.ComponentModel.DataAnnotations;

namespace APIDemo_swagger.ValidationAttributes
{
    public class StartEndTimeAttribute: ValidationAttribute // 起始時間結束時間判斷
    {
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            var st = (TodoListPostDto)value;

            if (st.StartTime >= st.EndTime)
            {
                return new ValidationResult("起始時間不可大於結束時間", new string[] { "time" });
            }

            return ValidationResult.Success;
        }
    }
}
