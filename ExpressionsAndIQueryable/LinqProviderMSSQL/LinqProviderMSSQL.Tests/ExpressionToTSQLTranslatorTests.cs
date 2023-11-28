using LinqProviderMSSQL.DataBaseTestEntity;
using LinqProviderMSSQL.QueryProvider;
using LinqProviderMSSQL.Services;
using System.Linq.Expressions;

namespace LinqProviderMSSQL.Tests;

public class ExpressionToTSQLTranslatorTests
{
    [Test]
    public void TestIntBinary()
    {
        var translator = new ExpressionToTSQLTranslator();
        Expression<Func<TestData, bool>> expression
            = employee => employee.Id == 10;

        string translated = translator.Translate(expression);
        Assert.That(translated, Is.EqualTo("[Id] IN (10)"));
    }

    [Test]
    public void TestIntBinaryReverse()
    {
        var translator = new ExpressionToTSQLTranslator();
        Expression<Func<TestData, bool>> expression
            = employee => 10 == employee.Id;

        string translated = translator.Translate(expression);
        Assert.That(translated, Is.EqualTo("[Id] IN (10)"));
    }

    [Test]
    public void TestStringBinary()
    {
        var translator = new ExpressionToTSQLTranslator();
        Expression<Func<TestData, bool>> expression
            = employee => employee.Name == "10";

        string translated = translator.Translate(expression);
        Assert.That(translated, Is.EqualTo("[Name] IN ('10')"));
    }

    [Test]
    public void TestIntGreaterThan()
    {
        var translator = new ExpressionToTSQLTranslator();
        Expression<Func<TestData, bool>> expression
            = employee => employee.Id > 10;

        string translated = translator.Translate(expression);
        Assert.That(translated, Is.EqualTo("[Id] > 10"));
    }

    [Test]
    public void TestIntLessThan()
    {
        var translator = new ExpressionToTSQLTranslator();
        Expression<Func<TestData, bool>> expression
            = employee => employee.Id < 10;

        string translated = translator.Translate(expression);
        Assert.That(translated, Is.EqualTo("[Id] < 10"));
    }

    [Test]
    public void TestBinaryEqualsQueryable()
    {
        var translator = new ExpressionToTSQLTranslator();
        Expression<Func<IQueryable<TestData>, IQueryable<TestData>>> expression
            = query => query.Where(e => e.Name == "Andrjak");

        string translated = translator.Translate(expression);
        Assert.That(translated, Is.EqualTo("WHERE [Name] IN ('Andrjak')"));
    }

    [Test]
    public void TestBinaryEqualsQueryableWithAnd()
    {
        var translator = new ExpressionToTSQLTranslator();
        Expression<Func<IQueryable<TestData>, IQueryable<TestData>>> expression
            = query => query.Where(e => e.Name == "Andrjak" && e.Id > 10 && e.Id < 100);

        string translated = translator.Translate(expression);
        Assert.That(translated, Is.EqualTo("WHERE [Name] IN ('Andrjak') AND [Id] > 10 AND [Id] < 100"));
    }

    [Test]
    public void TestStartWith()
    {
        var translator = new ExpressionToTSQLTranslator();
        Expression<Func<IQueryable<TestData>, IQueryable<TestData>>> expression
            = query => query.Where(e => e.Name.StartsWith("Andrjak") && e.Id > 10 && e.Id < 100);

        string translated = translator.Translate(expression);
        Assert.That(translated, Is.EqualTo("WHERE [Name] LIKE 'Andrjak%' AND [Id] > 10 AND [Id] < 100"));
    }

    [Test]
    public void TestContains()
    {
        var translator = new ExpressionToTSQLTranslator();
        Expression<Func<IQueryable<TestData>, IQueryable<TestData>>> expression
            = query => query.Where(e => e.Name.Contains("Andrjak") && e.Id > 10 && e.Id < 100);

        string translated = translator.Translate(expression);
        Assert.That(translated, Is.EqualTo("WHERE [Name] LIKE '%Andrjak%' AND [Id] > 10 AND [Id] < 100"));
    }

    [Test]
    public void TestEndWith()
    {
        var translator = new ExpressionToTSQLTranslator();
        Expression<Func<IQueryable<TestData>, IQueryable<TestData>>> expression
            = query => query.Where(e => e.Name.EndsWith("Andrjak") && e.Id > 10 && e.Id < 100);

        string translated = translator.Translate(expression);
        Assert.That(translated, Is.EqualTo("WHERE [Name] LIKE '%Andrjak' AND [Id] > 10 AND [Id] < 100"));
    }

    [Test]
    public void IntegrationTestOnRealDB()
    {
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

        Assert.IsTrue(data.Count == 5);
    }

    public class TestData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}