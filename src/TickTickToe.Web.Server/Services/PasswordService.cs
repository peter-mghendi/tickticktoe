using System.Security.Cryptography;
using Konscious.Security.Cryptography;

namespace TickTickToe.Web.Server.Services;

public class PasswordService: IPasswordService
{
    public byte[] HashPassword(byte[] password, byte[] salt)
    {
        using var argon2 = new Argon2id(password);
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = 4;
        argon2.Iterations = 2;
        argon2.MemorySize = 1024;
        return argon2.GetBytes(32);
    }

    public bool VerifyHash(byte[] password, byte[] salt, byte[] hash) =>
        HashPassword(password, salt).SequenceEqual(hash);

    public byte[] GenerateSalt(int length = 32)
    {
        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[length];
        rng.GetBytes(buffer);
        return buffer;
    }
}