using System;
using System.Collections.Generic;

namespace DormAPI.Database.EF.Scaffolding;

public partial class DocumentsPath
{
    public int DocumentPathId { get; set; }

    public string? DocumentPath { get; set; }

    public string? DocumentName { get; set; }

    public string? DocumentType { get; set; }

    public int? StudentId { get; set; }

    public virtual Student? Student { get; set; }
}
