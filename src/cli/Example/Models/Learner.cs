using System;

namespace Example;

public class Learner
{
    public Guid Identifier { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Department[] Departments { get; set; }
}
