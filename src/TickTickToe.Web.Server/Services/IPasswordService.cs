namespace TickTickToe.Web.Server.Services;

public interface IPasswordService
{
    public byte[] HashPassword(byte[] password, byte[] salt);
    public bool VerifyHash(byte[] password, byte[] salt, byte[] hash);
    public byte[] GenerateSalt(int length = 32);
}