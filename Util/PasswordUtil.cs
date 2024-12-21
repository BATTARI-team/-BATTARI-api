using System.Security.Cryptography;
using System.Text;

namespace webUserLoginTest.Util;

public class PasswordUtil
{
    /// <summary>
    /// 引数pepperから計算した実効ソルトとパスワードからパスワードハッシュを計算します
    /// </summary>
    /// <param name="salt"></param>
    /// <param name="pass"></param>
    /// <param name="pepper"></param>
    /// <returns></returns>
    public static byte[] GetPasswordHashFromPepper(byte[] salt, string pass, string pepper)
    {
        var actualPasswordSalt = Encoding.Unicode.GetString(salt) + pepper;
        using (var sha256 = SHA256.Create())
        {
            var passHash = sha256.ComputeHash(Encoding.Unicode.GetBytes(actualPasswordSalt + pass));
            return passHash;
        }
    }
    /// <summary>
    /// データベースに保存するパスワードソルトを計算します
    /// </summary>
    /// <param name="変化する値">ユーザーの作成日などがいいとにかく変化する値</param>
    /// <returns></returns>
    public static byte[] GetInitialPasswordSalt(string 変化する値)
    {
        using (var hmac = SHA256.Create())
        {
            var passwordSaltOnDatabase = hmac.ComputeHash(Encoding.Unicode.GetBytes(変化する値));
            return passwordSaltOnDatabase;
        }
    }
    /// <summary>
    /// パスワードからハッシュ値を計算します
    /// </summary>
    /// <param name="pass"></param>
    /// <returns></returns>
    public static byte[] GetNormalPasswordHash(string pass)
    {
        var hmac = new HMACSHA256();
        var passwordByte = Encoding.Unicode.GetBytes(pass);
        return hmac.ComputeHash(passwordByte);
    }

    public static bool CompareHash(byte[] hash1, byte[] hash2)
    {
        if (hash1.Length != hash2.Length)
        {
            return false;
        }
        for (int i = 0; i < hash1.Length; i++)
        {
            if (hash1[i] != hash2[i])
            {
                return false;
            }
        }
        return true;
    }
    public static void PutByte(byte[] str)
    {
        //文字列補完式
        Console.WriteLine($" \"{Encoding.GetEncoding(Encoding.Unicode.EncodingName).GetString(str)}\"のバイトシーケンスは");
        foreach (var VARIABLE in str)
        {
            Console.WriteLine(VARIABLE);
        }
        /*
         * バイトシーケンス:8桁の二進数の連続
         * 文字セット：使用する文字それぞれに数字を割り振っている
         */
    }

    public static void PutByteWithString(byte[] str)
    {
        //文字列補完式
        Console.WriteLine($" \"{Encoding.Unicode.GetString(str)}\"のバイトシーケンスは");
        for (int i = 0; i < str.Length; i += 2)//Unicodeは2バイト使うっぽい
        {
            Console.WriteLine($"{Encoding.Unicode.GetString(new byte[] { str[i], str[i + 1] })}：{str[i] + str[i + 1]}");
        }
    }
    /// <summary>
    /// ハッシュコードをコンソールに出力します
    /// </summary>
    /// <param name="str"></param>
    public static void PutHash(byte[] str)
    {

        //16進数の文字列に変換
        Console.WriteLine(BitConverter.ToString(str).Replace("-", ""));
    }
}