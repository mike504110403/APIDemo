namespace APIDemo_swagger.Dtos
{
    public class UploadFilePostDto
    {
        // public Guid? UploadFileId { get; set; } // 系統給定
        public string? Name { get; set; }
        public string? Src { get; set; }
        // public Guid TodoId { get; set; } // 可取父單的
    }
}
