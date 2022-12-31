using APIDemo_swagger.Dtos;
using APIDemo_swagger.Interfaces;
using APIDemo_swagger.Models;
using APIDemo_swagger.Parameters;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace APIDemo_swagger.Services
{
    public class TodoListAsyncService // 非同步寫法
    {
        private readonly TodoContext _todoContext; // service
        public TodoListAsyncService(TodoContext todoContext)
        {
            _todoContext = todoContext;
        }

        // 撈資料判斷 參數化查詢邏輯 連線db取得資料
        public async Task<List<TodoListSelectDto>> Selectdb(TodoSelectParameters value)
        {
            var result = _todoContext.TodoLists
                        .Include(a => a.UpdateEmployee)
                        .Include(a => a.InsertEmployee)
                        .Select(a => a);

            if (!string.IsNullOrWhiteSpace(value.name))
            {
                result = result.Where(a => a.Name.Contains(value.name)); // 撈出包含name的
            }

            if (value.enable != null)
            {
                result = result.Where(a => a.Enable == value.enable);
            }

            if (value.insertTime != null)
            {
                result = result.Where(a => a.InsertTime.Date == value.insertTime); // 撈出DateTime符合的
            }

            if (value.minOrder != null && value.maxOrder != null)
            {
                result = result.Where(a => a.Orders >= value.minOrder && a.Orders <= value.maxOrder); // 撈出介於minOrder-maxOrder
            }

            var temp = await result.ToListAsync();

            return temp.Select(a => ItemToDto(a)).ToList();
        }

        // 有外鍵情況下 同時新增父子資料 連線db新增資料
        public async Task<TodoList> Postdb(TodoListPostDto value)
        {
            // 轉型以給定uploadfiles
            List<UploadFile> upl = new List<UploadFile>();

            foreach (var temp in value.UploadFiles)
            {
                UploadFile up = new UploadFile()
                {
                    Name = temp.Name,
                    Src = temp.Src
                };
                upl.Add(up);
            };

            TodoList insert = new TodoList // 轉型
            {
                // 使用者能決定的欄位
                Name = value.Name,
                Enable = value.Enable,
                Orders = value.Orders,
                // 系統決定的欄位
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"), // 還沒做 先寫死
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                UploadFiles = upl // 子資料 (db已做外鍵關聯 Uploadfile內的todoid不用給 會自動吃父資料的)
            };
            _todoContext.TodoLists.Add(insert);
            await _todoContext.SaveChangesAsync();

            return insert;
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
                Name = a.Name,
                Orders = a.Orders,
                TodoId = a.TodoId,
                UpdateEmployeeName = a.UpdateEmployee.Name,
                UpdateTime = a.UpdateTime,
                UploadFiles = updto
            };
        }
    }
}
