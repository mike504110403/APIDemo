using APIDemo_swagger.Dtos;
using APIDemo_swagger.Interfaces;
using APIDemo_swagger.Models;
using APIDemo_swagger.Parameters;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APIDemo_swagger.Services
{
    public class TodoLinqService: ITodoListService // 實作介面 // IOC server注入 目前情境不需要
    {
        private readonly TodoContext _todoContext; // service

        public string type => "fun";

        public TodoLinqService(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }

        // 撈資料判斷 參數化查詢邏輯 連線db取得資料
        public List<TodoListSelectDto> Selectdb(TodoSelectParameters value)
        {
            var result = _todoContext.TodoLists
                .Include(a => a.UpdateEmployee)
                .Include(a => a.InsertEmployee)
                .Include(a => a.UploadFiles)
                .Select(a => a);

            if (!string.IsNullOrWhiteSpace(value.name)) // 撈出包含name的
            {
                result = result.Where(a => a.Name.Contains(value.name));
            }

            if (value.enable != null) // 撈出enable符合的
            {
                result = result.Where(a => a.Enable == value.enable);
            }

            if (value.insertTime != null) // 撈出DateTime符合的
            {
                result = result.Where(a => a.InsertTime.Date == value.insertTime);
            }

            if (value.minOrder != null && value.maxOrder != null) // 撈出order符合的
            {
                result = result.Where(a => a.Orders >= value.minOrder && a.Orders <= value.maxOrder); // 撈出介於minOrder-maxOrder
            }

            return result.ToList().Select(a => ItemToDto(a)).ToList();

        }

        // Dto函式化
        private static TodoListSelectDto ItemToDto(TodoList a)
        {
            List<UploadFileDto> updto = new List<UploadFileDto>();

            foreach (var temp in a.UploadFiles)
            {
                UploadFileDto up = new UploadFileDto() // 逐筆轉換子資料
                {
                    Name = temp.Name,
                    Src = temp.Src,
                    TodoId = temp.TodoId,
                    UploadFileId = temp.UploadFileId
                };
                updto.Add(up);
            }

            return new TodoListSelectDto
            {
                Enable = a.Enable,
                InsertEmployeeName = a.InsertEmployee.Name,
                InsertTime = a.InsertTime,
                Name = a.Name +"(use fun)",
                Orders = a.Orders,
                TodoId = a.TodoId,
                UpdateEmployeeName = a.UpdateEmployee.Name,
                UpdateTime = a.UpdateTime,
                UploadFiles = updto
            };
        }
    }
}
