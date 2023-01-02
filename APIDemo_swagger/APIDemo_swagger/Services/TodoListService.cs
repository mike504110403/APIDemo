using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;
using APIDemo_swagger.Parameters;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace APIDemo_swagger.Services
{
    public class TodoListService
    {
        private readonly TodoContext _todoContext; // service
        private readonly IMapper _mapper; // automapper
        private readonly IHttpContextAccessor _httpContextAccessor; // httpcontext
        public TodoListService(TodoContext todoContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _todoContext = todoContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
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

        // 撈資料判斷 依id取得單筆資料 連線db取得資料
        public TodoListSelectDto SelectOnedb(Guid id)
        {
            var result = (from a in _todoContext.TodoLists
                          where a.TodoId == id
                          select a)
                          .Include(a => a.UpdateEmployee)
                          .Include(a => a.InsertEmployee)
                          .Include(a => a.UploadFiles)
                          .SingleOrDefault(); // singleordefault => 找不到資料就會為空

            if (result != null)
            {
                return ItemToDto(result); // 回傳null值會錯
            }

            return null;

        }

        // 撈資料判斷 sql語法 連線db取得資料
        public IEnumerable<TodoList> Selectsqldb(string? name)
        {
            string sql = "select * from todolist where 1=1";

            if (!string.IsNullOrWhiteSpace(name))
            {
                sql = sql + "and name like N'%" + name + "%'";   // Sql injection => name = ';update [TodoList] set name = N'去上課' where [TodoId] = '' --
            }
            var result = _todoContext.TodoLists.FromSqlRaw(sql); // 下sql語法

            return result;
        }

        // 撈資料判斷 sql語法 連線db取得資料
        public IEnumerable<TodoListSelectDto> Selectsqldtodb(string? name)
        {
            string sql = @"SELECT [TodoId], a.[Name], [InsertTime], [UpdateTime], [Enable], [Orders]
                        , b.Name as InsertEmployeeName, c.Name as UpdateEmployeeName
                        FROM [TodoList] a
                        join Employee b on a.InsertEmployeeId = b.EmployeeId
                        join Employee c on a.UpdateEmployeeId = c.EmployeeId 
                        where 1=1";

            if (!string.IsNullOrWhiteSpace(name))
            {
                sql = sql + "and name like N'%" + name + "%'";
            }
            var result = _todoContext.TodoListSelectDto.FromSqlRaw(sql); // 下sql語法

            return result;
        }

        // Automapper自動映射 參數化查詢邏輯 連線db取得資料
        public IEnumerable<TodoListSelectDto> AutomapperSelectdb(TodoSelectParameters value)
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

            // automapper 自動映射
            return _mapper.Map<IEnumerable<TodoListSelectDto>>(result);
        }

        // Automapper自動映射 依id取得單筆資料 連線db取得資料
        public TodoListSelectDto AutomapperSelectOnedb(Guid id)
        {
            var result = (from a in _todoContext.TodoLists
                         where a.TodoId == id
                         select a)
                         .Include(a => a.UpdateEmployee)
                         .Include(a => a.InsertEmployee)
                         .SingleOrDefault();

            if (result != null)
            {
                return _mapper.Map<TodoListSelectDto>(result); // 回傳null值會錯
            }

            return null;
        }

        // 有外鍵情況下 同時新增父子資料 連線db新增資料
        public TodoList Postdb(TodoListPostDto value)
        {
            // 取得使用者資訊(登入狀態)
            //var Claim = _httpContextAccessor.HttpContext.User.Claims.ToList(); // cookie方式認證 取得登入時宣告的使用者資訊
            //var employeeid = Claim.Where(a => a.Type == "EmployeeId").First().Value; // employeeid需在claims內宣告為其中一個type才能取
            var employeeid = _httpContextAccessor.HttpContext.User.FindFirstValue("EmployeeId"); // 另一個取得方式
            // var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);

            // 轉型以給定uploadfiles
            List <UploadFile> upl = new List<UploadFile>();

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
                InsertEmployeeId = Guid.Parse(employeeid),
                UpdateEmployeeId = Guid.Parse(employeeid),
                UploadFiles = upl // 子資料 (db已做外鍵關聯 Uploadfile內的todoid不用給 會自動吃父資料的)
            };
            _todoContext.TodoLists.Add(insert);
            _todoContext.SaveChanges();

            // return CreatedAtAction(nameof(Get), new { TodoId = insert.TodoId} , insert);

            return insert;
        }

        // 有外鍵情況下 同時新增父子資料 連線db新增資料
        public void PostNofkdb(TodoListPostDto value)
        {
            // 新增父資料
            TodoList insert_f = new TodoList // 轉型
            {
                // 使用者能決定的欄位
                Name = value.Name,
                Enable = value.Enable,
                Orders = value.Orders,
                // 系統決定的欄位
                InsertTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"), // 還沒做 先寫死
                UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001")
            };

            _todoContext.TodoLists.Add(insert_f);
            _todoContext.SaveChanges(); // 先存父資料 下方存子資料才取的到TodoId

            // 新增子資料
            foreach (var temp in value.UploadFiles)
            {
                UploadFile insert_c = new UploadFile // 轉型
                {
                    Name = temp.Name,
                    Src = temp.Src,
                    TodoId = insert_f.TodoId
                    // UploadFileId = temp.UploadFileId // 此處才建立upliadfile id此時自動生成 無須給定 且 body給值也不用給UploadFileId
                };
                _todoContext.UploadFiles.Add(insert_c);
            };

            _todoContext.SaveChanges();
        }

        // Automapper自動映射 連線db新增資料
        public void AutomapperPostdb(TodoListPostDto value)
        {
            var map = _mapper.Map<TodoList>(value); // 自動映射(系統給定值及其他參數寫在TodoListProfile)

            // 系統決定的欄位 先寫死 -> 移至TodoListProfile
            //map.InsertTime = DateTime.Now;
            //map.UpdateTime = DateTime.Now;
            //map.InsertEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // 還沒做 先寫死
            //map.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            // map.UploadFiles = upl; // automapper連同子資料憶起自動轉

            _todoContext.TodoLists.Add(map);
            _todoContext.SaveChanges();
        }

        // sql語法 連線db新增資料
        public void Postsql(TodoListPostDto value)
        {
            var name = new SqlParameter("name", value.Name); // 語法轉成單純字串 避免sql injection --> 參數化 其餘參數化防injection待補

            string sql = @"INSERT INTO [dbo].[TodoList]
                            ([Name],[InsertTime],[UpdateTime],[Enable],[Orders],[InsertEmployeeId],[UpdateEmployeeId]) 
                            VALUES
                            (@name,'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            + "','" + value.Enable + "'," + value.Orders + ",'00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000001') ";

            _todoContext.Database.ExecuteSqlRaw(sql, name);
        }

        // 查詢、更新邏輯 連線db更新資料
        public int Putdb(Guid id, TodoListPutDto value)
        {
            // _todoContext.Entry(value).State = EntityState.Modified; // 不同寫法 自動匹配更新

            //_todoContext.TodoLists.Update(value); // 實務上不會直接將輸入值丟進去
            //_todoContext.SaveChanges();

            // 運用Dto轉型傳進來的值

            // 先找要更新的那筆
            // var update = _todoContext.TodoLists.Find(id);  // Find()內需填主key
            var update = (from a in _todoContext.TodoLists
                          where a.TodoId == id
                          select a).SingleOrDefault(); // id為主key 所以才能下single

            if (update != null) // 找的到資料再針對該筆做更新
            {
                // 系統決定欄位給值
                update.UpdateTime = DateTime.Now;
                update.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // 還沒做 先寫死
                // 傳入給值
                update.Name = value.Name;
                update.Orders = value.Orders;
                update.Enable = value.Enable;

                //  or內建函式自動匹配
                // _todoContext.TodoLists.Update(update).CurrentValues.SetValues(value);
                // _todoContext.SaveChanges();
            }

            return _todoContext.SaveChanges();
        }

        // 路由不帶id的更新 查詢、更新邏輯 連線db更新資料 
        public int Putnroutedb(TodoListPutDto value)
        {
            // 先找要更新的那筆
            var update = (from a in _todoContext.TodoLists
                          where a.TodoId == value.TodoId
                          select a).SingleOrDefault(); // id為主key 所以才能下single

            if (update != null) // 找的到資料再針對該筆做更新
            {
                // 系統決定欄位給值
                update.UpdateTime = DateTime.Now;
                update.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // 還沒做 先寫死
                // 傳入給值
                update.Name = value.Name;
                update.Orders = value.Orders;
                update.Enable = value.Enable;
            }

            return _todoContext.SaveChanges();
        }

        // Automapper自動映射 查詢、更新邏輯 連線db更新資料
        public int AutomapperPutdb(Guid id, TodoListPutDto value)
        {
            // 先找要更新的那筆
            var update = (from a in _todoContext.TodoLists
                          where a.TodoId == id
                          select a).SingleOrDefault(); // id為主key 所以才能下single

            if (update != null) // 找的到資料再針對該筆做更新
            {
                _mapper.Map(value, update); // 自動映射(系統給定值及其他參數寫在TodoListProfile)
            }
            return _todoContext.SaveChanges();
        }

        // 查詢邏輯 連線刪除資料
        public int Deletedb(Guid id)
        {
            // 先找該筆資料，因有外鍵，把相關子資料也找出來
            var delete = (from a in _todoContext.TodoLists
                          where a.TodoId == id
                          select a)
                          .Include(c => c.UploadFiles)
                          .SingleOrDefault(); // id為主key 所以才能下single

            if (delete != null)
            {
                _todoContext.TodoLists.Remove(delete);
            }

            return _todoContext.SaveChanges();
        }

        // 查詢邏輯 連線刪除資料 無外鍵情況下
        public int Deletenofkdb(Guid id)
        {
            // 先找該筆資料
            var delete_f = (from a in _todoContext.TodoLists
                            where a.TodoId == id
                            select a).SingleOrDefault(); // id為主key 所以才能下single

            // 找到相關子資料
            var delete_c = from a in _todoContext.UploadFiles
                           where a.TodoId == id
                           select a;

            if (delete_c != null)
            {
                // 清子資料
                _todoContext.UploadFiles.RemoveRange(delete_c); // RemoveRange -> 刪除多筆
                _todoContext.SaveChanges();
            }

            if (delete_f != null)
            {
                _todoContext.TodoLists.Remove(delete_f);
            }

            return _todoContext.SaveChanges();
        }

        // 查詢邏輯 連線刪除資料 指定刪除多筆資料
        public int Deletenumdb(string ids)
        {
            // 反序列字串
            List<Guid> deleteList = JsonConvert.DeserializeObject<List<Guid>>(ids);

            // 先找指定資料
            var delete = (from a in _todoContext.TodoLists
                          where deleteList.Contains(a.TodoId)
                          select a).Include(c => c.UploadFiles);

            _todoContext.TodoLists.RemoveRange(delete);

            return _todoContext.SaveChanges();
        }

        // 查詢邏輯 連線部分更新指定資料
        public int Patchdb(Guid id, JsonPatchDocument value)
        {
            // 先找要更新的那筆
            var update = (from a in _todoContext.TodoLists
                          where a.TodoId == id
                          select a).SingleOrDefault(); // id為主key 所以才能下single

            if (update != null) // 找的到資料再針對該筆做更新
            {
                // 系統決定欄位給值
                update.UpdateTime = DateTime.Now;
                update.UpdateEmployeeId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // 還沒做 先寫死

                value.ApplyTo(update); // 局部更新
            }

            return _todoContext.SaveChanges();
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
