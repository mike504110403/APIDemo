using System;
using System.Collections.Generic;

namespace APIDemo_swagger.Models;

public partial class Division
{
    public Guid DivisionId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; } = new List<Employee>();
}
