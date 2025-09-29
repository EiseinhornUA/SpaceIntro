using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class UIDGenerator
{
    public static string Generate6CharHash(string userID)
    {
        using (var md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(userID));

            uint value = BitConverter.ToUInt32(hashBytes, 0);
            string base36 = Base36Encode(value);

            return base36.Substring(0, 6);
        }
    }

    private static string Base36Encode(uint value)
    {
        const string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
        string result = "";
        do
        {
            result = chars[(int)(value % 36)] + result;
            value /= 36;
        } while (value > 0);

        return result.PadLeft(6, '0');
    }
}
