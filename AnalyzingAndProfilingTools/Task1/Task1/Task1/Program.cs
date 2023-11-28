// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Security.Cryptography;

public class Program
{
    private static void Main(string[] args)
    {
        var timer = new Stopwatch();
        timer.Start();
        GeneratePasswordHashUsingSalt("12345678qwerty", Guid.NewGuid().ToByteArray());
        timer.Stop();
        Console.WriteLine($"ms: {timer.ElapsedMilliseconds} | ticks: {timer.ElapsedTicks}");

        var timerNew = new Stopwatch();
        timerNew.Start();
        GeneratePasswordHashUsingSaltNew("12345678qwerty", Guid.NewGuid().ToByteArray());
        timerNew.Stop();
        Console.WriteLine($"ms: {timerNew.ElapsedMilliseconds} | ticks: {timerNew.ElapsedTicks}");
    }

    public static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
    {
        // The code works fast
        var iterate = 10000;
        var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate); // This can be achieved by changing the hashing algorithm, but more advanced algorithms require more computing resources
        byte[] hash = pbkdf2.GetBytes(20); // Most of the time is spent executing the hashing algorithm

        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        var passwordHash = Convert.ToBase64String(hashBytes);

        return passwordHash;
    }

    public static string GeneratePasswordHashUsingSaltNew(string passwordText, byte[] salt)
    {
        var iterate = 10000;
        var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
        byte[] hash = pbkdf2.GetBytes(20);

        // In this case, you can replace copying within one array with Linq, since passwords are not large in size.
        // And creating a new object in this case does not greatly affect performance.
        // This looks a little better than copying the array in place.
        var passwordHash = Convert.ToBase64String(salt.Concat(hash).ToArray());

        return passwordHash;
    }
}