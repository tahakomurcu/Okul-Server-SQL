using App.Api.Data.Entities;
using Bogus;

namespace App.Api.Data
{
    public class DbSeed
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            var faker = new Faker<StudentEntity>("tr")
                .RuleFor(s => s.Number, f => f.Random.Number(1,100))
                .RuleFor(s => s.Name, f => f.Name.FirstName())
                .RuleFor(s => s.Surname, f => f.Name.LastName())
                .RuleFor(s => s.Class, f => f.PickRandom(new[] {"A", "B", "C", "D", "E"}))
                .RuleFor(s => s.Grade, f => f.Random.Number(0, 100));

            var students = faker.Generate(20);

            await context.Students.AddRangeAsync(students);
            await context.SaveChangesAsync();
        }
    }
}
