using System.ComponentModel.DataAnnotations;

namespace APIDemo_swagger.Dtos
{
    public class TodoListPutDto // 更新用轉型Dto
    {
        public Guid TodoId { get; set; }// 更新需主key
        [Required] // 資料模型驗證
        public string Name { get; set; }
        public bool Enable { get; set; }
        [Range(1, 3)]
        public int Orders { get; set; }
        public List<UploadFilePostDto> UploadFiles { get; set; }

        public TodoListPutDto()
        {
            UploadFiles = new List<UploadFilePostDto>();
        }


    }
}
