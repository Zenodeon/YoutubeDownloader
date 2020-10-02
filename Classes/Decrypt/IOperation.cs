using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YoutubeDownloader.Classes.Decrypt
{
    internal interface IOperation
    {
        string Decipher(string input);
    }

    internal static class CipherOperationExtensions
    {
        public static string Decipher(this IEnumerable<IOperation> operations, string input) =>
            operations.Aggregate(input, (acc, op) => op.Decipher(acc));

    }
}
