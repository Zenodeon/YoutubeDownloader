using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YoutubeDownloader.Classes.Decrypt;

namespace YoutubeDownloader.Classes
{
    static class Decrypter
    {
        public static void Decrypt(JObject signatureCipher)
        {
            Debug.SaveFile(signatureCipher.ToString(), "signatureCipher");

            var s = signatureCipher["s"].ToString();


        }


    }
}
