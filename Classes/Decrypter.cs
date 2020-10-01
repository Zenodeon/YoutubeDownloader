using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YoutubeDownloader.Classes
{
    static class Decrypter
    {
        public static void Decrypt(JObject signatureCipher)
        {
            Debug.SaveFile(signatureCipher.ToString(), "signatureCipher");

            //char[] s = signatureCipher["s"].ToString().ToCharArray();
            var s = signatureCipher["s"].ToString();
            s.Aggregate(s, (acc, op) => op.Decipher(acc));

           
        }
    }

    internal interface ICipherOperation
    {
        string Decipher(string input);
    }

    internal static class CipherOperationExtensions
    {
        public static string Decipher(this IEnumerable<ICipherOperation> operations, string input) =>
            operations.Aggregate(input, (acc, op) => op.Decipher(acc));
    }
}
