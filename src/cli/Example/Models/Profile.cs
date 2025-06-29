using System;
using System.Collections.Generic;

namespace Example;

public class Profile
{
    public Guid Identifier { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<Competency> Competencies { get; set; } = new List<Competency>();
}
