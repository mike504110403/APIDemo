using APIDemo_swagger.Models;

namespace APIDemo_swagger.Dtos
{
    public class UploadFileDto
    {
        public Guid? UploadFileId { get; set; } // 系統給定Id
        public string? Name { get; set; }
        public string? Src { get; set; }
        public Guid TodoId { get; set; }
    }
}
