using Generators;

namespace SourceGenerators.Sample;

[GenerateDto]
public class Student
{
    public Student(string Name, int Age, string? Address, string? Phone, Course[] Courses)
    {
        this.Name = Name;
        this.Age = Age;
        this.Address = Address;
        this.Phone = Phone;
        this.Courses = Courses;
    }

    public string Name { get; init; }
    public int Age { get; init; }
    public string? Address { get; init; }
    public string? Phone { get; init; }
    public Course[] Courses { get; init; }

    public void Deconstruct(out string Name, out int Age, out string? Address, out string? Phone, out Course[] Courses)
    {
        Name = this.Name;
        Age = this.Age;
        Address = this.Address;
        Phone = this.Phone;
        Courses = this.Courses;
    }
}

[GenerateDto]
public record Course(string Name, int Credits, string? Description, Teacher? Teacher);

[GenerateDto]
public record Teacher(string Name, string? Address, string? Phone, string? Email);