using LinqProviderMSSQL.DataBaseTestEntity;
using LinqProviderMSSQL.QueryProvider;
using LinqProviderMSSQL.Services;

// Server=localhost;Database=MSLinqTestDatabase;Trusted_Connection=True;TrustServerCertificate=True
using var dataService = new MSDataService("Server=localhost;Database=MSLinqTestDatabase;Trusted_Connection=True;TrustServerCertificate=True");

var persons = new MSEntitySet<Person>(dataService);

var data = persons.Where(p =>
    p.Salary < 501 && p.LastName == "Rogers"
    || p.Id == 1
    || p.Email.StartsWith("Brenda") && p.Salary == 501
    || p.Email.Contains("Andrjak")
    || p.Email.EndsWith("gmail.com"))
    .ToList();

foreach (var user in data)
{
    Console.WriteLine($"Id: {user.Id}, FirstName: {user.FirstName}, LastName: {user.LastName}, Salary: {user.Salary}");
}

