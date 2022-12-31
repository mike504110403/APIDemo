using APIDemo_swagger.Abstracts;
using APIDemo_swagger.ModelBinder;
using APIDemo_swagger.Models;
using APIDemo_swagger.ValidationAttributes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace APIDemo_swagger.Dtos
{
    public class TodoListPostUpDto // 將傳值類別轉型，不然fromform同時新增檔案與資料要傳的資料格式太亂
    {
        [ModelBinder(BinderType = typeof(FormDateJsonBinder))] // 自己做擴充方法[FormDateJsonBinder]來轉型// FromForm 不會轉類別，因此要用字串轉型程類別
        public TodoListPostDto TodoList { get; set; }
        public IFormFileCollection files { get; set; }
    }
}
