using System.ComponentModel.DataAnnotations;

namespace APIDemo_swagger.Dtos
{
    public class TodoListPostDto // 新增用轉型Dto
    {
        // public Guid TodoId { get; set; } // 新增無須Id欄位
        [Required] // 資料模型驗證
        public string Name { get; set; }
        // public DateTime InsertTime { get; set; } // 系統時間
        // public DateTime UpdateTime { get; set; } // 系統時間
        public bool Enable { get; set; }
        [Range(1, 3)]
        public int Orders { get; set; }
        // public string? InsertEmployeeName { get; set; } // 系統給定
        // public string? UpdateEmployeeName { get; set; } // 系統給定
        public ICollection<UploadFileDto>? UploadFiles { get; set; }


    }
}
