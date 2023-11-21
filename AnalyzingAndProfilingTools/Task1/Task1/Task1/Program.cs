// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Security.Cryptography;

var timer = new Stopwatch();
timer.Start();
GeneratePasswordHashUsingSalt("12345678qwerty", Guid.NewGuid().ToByteArray());
timer.Stop();
Console.WriteLine($"ms: {timer.ElapsedMilliseconds} | ticks: {timer.ElapsedTicks}");

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

