using System;
using System.Collections.Generic;

namespace APIDemo_swagger.Models;

public partial class UploadFile
{
    public Guid UploadFileId { get; set; }

    public string Name { get; set; } = null!;

    public string Src { get; set; } = null!;

    public Guid TodoId { get; set; }

    public virtual TodoList? Todo { get; set; } = null!;
}
