using System.Security.Cryptography;

namespace AnujTiles.NotificationEngine.Services;

public sealed class OtpService : IOtpService
{
    public string Generate(int length = 6)
    {
        if (length < 4 || length > 10)
        {
            throw new ArgumentOutOfRangeException(
                nameof(length),
                "OTP length must be between 4 and 10.");
        }

        Span<char> otp = stackalloc char[length];

        for (int i = 0; i < length; i++)
        {
            otp[i] = (char)(
                '0' +
                RandomNumberGenerator.GetInt32(0, 10));
        }

        return new string(otp);
    }
}