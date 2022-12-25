using APIDemo_swagger.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace APIDemo_swagger.Dtos
{
    public class TodoListPutDto: TodoListEditDtoAbstract // 更新用轉型Dto
    {
        public Guid TodoId { get; set; }// 更新需主key
    }
}
