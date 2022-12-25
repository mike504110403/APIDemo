using APIDemo_swagger.Dtos;
using APIDemo_swagger.Models;
using System.ComponentModel.DataAnnotations;

namespace APIDemo_swagger.Abstracts
{
    public abstract class TodoListEditDtoAbstract : IValidatableObject // 抽象化 讓其他dto繼承
    {
        [Required] // 資料模型驗證
        public string Name { get; set; }
        public bool Enable { get; set; }
        [Range(1, 3)]
        public int Orders { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<UploadFileDto>? UploadFiles { get; set; }

        public TodoListEditDtoAbstract()
        {
            UploadFiles = new List<UploadFileDto>();
        }

        // 實作驗證邏輯，若只有此dto用到，沒必要拉出去做標籤
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // [TodoName]
            TodoContext _todoContext = (TodoContext)validationContext.GetService(typeof(TodoContext)); // 取得資料庫連線物件service

            var findName = from a in _todoContext.TodoLists
                           where a.Name == Name
                           select a;

            if (this.GetType() == typeof(TodoListPutDto)) // 如果抓到的類別為更新的dto
            {
                var dtoUpdate = (TodoListPutDto)this;
                findName = findName.Where(a => a.TodoId != dtoUpdate.TodoId); // 如果是更新 排除自己跟自己相同TodoID原始的那筆 這樣就不會被排
            }

            if (findName.FirstOrDefault() != null)
            {
                yield return new ValidationResult("已存在相同的待辦事項", new string[] { "Name" });
            }

            yield return ValidationResult.Success;


            // [StartEndTime]
            if (StartTime >= EndTime)
            {
                yield return new ValidationResult("起始時間不可大於結束時間", new string[] { "Time" });
            }

            yield return ValidationResult.Success;
        }

    }
}
