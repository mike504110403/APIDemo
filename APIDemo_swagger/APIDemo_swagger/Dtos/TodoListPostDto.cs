using APIDemo_swagger.Abstracts;
using APIDemo_swagger.Models;
using APIDemo_swagger.ValidationAttributes;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace APIDemo_swagger.Dtos
{
    // [StartEndTime] // 起始結束時間驗證
    // [Test(Tvalue = "321")] // 標籤傳值
    // 新增用轉型Dto
    public class TodoListPostDto: TodoListEditDtoAbstract // 繼承該介面 於此類別實作驗證功能 
    {

    }
}
