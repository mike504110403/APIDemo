using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;
using System.ComponentModel.DataAnnotations;

namespace APIDemo_swagger.ValidationAttributes
{
    public class TestAttribute: ValidationAttribute // 標籤傳值
    {
        private string _tvalue;
        public string Tvalue = "de";
        public TestAttribute(string tvalue = "de") // 給預設值
        {
            _tvalue = tvalue;
        }
        protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
        {
            var st = (TodoListPostDto)value;


            return new ValidationResult(Tvalue, new string[] { "_tvalue" });

        }
    }
}
