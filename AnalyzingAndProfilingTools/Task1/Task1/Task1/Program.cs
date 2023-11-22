// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Security.Cryptography;

var timer = new Stopwatch();
timer.Start();
GeneratePasswordHashUsingSalt("12345678qwerty", Guid.NewGuid().ToByteArray());
timer.Stop();
Console.WriteLine($"ms: {timer.ElapsedMilliseconds} | ticks: {timer.ElapsedTicks}");

var timerNew = new Stopwatch();
timerNew.Start();
// On my device it's 5 times faster, SHA256 more optimaized then default SHA1
GeneratePasswordHashUsingSaltNew("12345678qwerty", Guid.NewGuid().ToByteArray());
timerNew.Stop();
Console.WriteLine($"ms: {timerNew.ElapsedMilliseconds} | ticks: {timerNew.ElapsedTicks}");

string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
{
    var iterate = 10000;
    var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
    byte[] hash = pbkdf2.GetBytes(20);

    byte[] hashBytes = new byte[36];
    Array.Copy(salt, 0, hashBytes, 0, 16);
    Array.Copy(hash, 0, hashBytes, 16, 20);

    var passwordHash = Convert.ToBase64String(hashBytes);

    return passwordHash;
}

string GeneratePasswordHashUsingSaltNew(string passwordText, byte[] salt)
{
    var iterate = 10000;
    var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate, HashAlgorithmName.SHA256);
    byte[] hash = pbkdf2.GetBytes(20);

    byte[] hashBytes = new byte[36];
    Array.Copy(salt, 0, hashBytes, 0, 16);
    Array.Copy(hash, 0, hashBytes, 16, 20);

    var passwordHash = Convert.ToBase64String(hashBytes);

    return passwordHash;
}

