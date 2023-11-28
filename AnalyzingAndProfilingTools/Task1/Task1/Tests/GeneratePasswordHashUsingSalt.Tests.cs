using System.Diagnostics;

namespace Tests;

public class Tests
{
    [Test]
    public void CompareStopwatch()
    {
        var stopwatch1 = new Stopwatch();
        stopwatch1.Start();

        for (var i = 100; i > 0; i--)
        {
            var hash = Program.GeneratePasswordHashUsingSalt("12345678qwerty", Guid.NewGuid().ToByteArray());
        }

        stopwatch1.Stop();
        Console.WriteLine(stopwatch1.Elapsed.ToString());

        var stopwatch2 = new Stopwatch();
        stopwatch2.Start();
        for (var i = 100; i > 0; i--)
        {
            var hash = Program.GeneratePasswordHashUsingSaltNew("12345678qwerty", Guid.NewGuid().ToByteArray());
        }
        stopwatch2.Stop();
        Console.WriteLine(stopwatch2.Elapsed.ToString());

        Assert.IsTrue(stopwatch1.Elapsed > stopwatch2.Elapsed);
    }
}
