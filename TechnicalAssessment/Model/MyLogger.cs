using System;
using System.Collections.Generic;

namespace TechnicalAssessment.Model;

public partial class MyLogger
{
    public int Id { get; set; }

    public DateTime? LogDate { get; set; }

    public string? Originator { get; set; }

    public string? FileName { get; set; }

    public string? Status { get; set; }
}
