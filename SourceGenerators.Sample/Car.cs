using Generators;

namespace SourceGenerators.Sample;

[IncrementalGenerateDto]
public class Car
{
    public Car(Guid id, string name, DateOnly releaseDate, Engine engine)
    {
        Id = id;
        Name = name;
        ReleaseDate = releaseDate;
        Engine = engine;
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public DateOnly ReleaseDate { get; init; }
    public Engine Engine { get; init; }
}

[IncrementalGenerateDto]
public record Engine(Guid Id, string Version);