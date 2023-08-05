using SourceGenerators.Sample;

var course = new Course("C# 9", 5, "Learn C# 9", new Teacher("John Doe", "123 Main St", "555-555-5555", "email"));
var student = new Student("Jane Doe", 20, "123 Main St", "555-555-5555", new[] { course });

Console.WriteLine($"Student: {student.Name}"); 
Console.WriteLine($"Course: {course.Name}");

var studentDto = (StudentDto)student;
Console.WriteLine("Dto: " + studentDto);

var studentAgain = (Student)studentDto;
Console.WriteLine("Model: " + studentAgain);