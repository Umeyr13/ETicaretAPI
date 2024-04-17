using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Helpers
{
    static public class CustomEncoders
    {
        public static string UrlEncode(this string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return WebEncoders.Base64UrlEncode(bytes);//http protokolüne uygun hale dönüştürdük. tırnak vs den arındı şifre
        }
        public static string UrlDecode(this string value)
        {
            byte[] bytes = WebEncoders.Base64UrlDecode(value);//geri şifreyi çözdürdük
            return Encoding.UTF8.GetString(bytes);//şifressi çözülmüş token ı geri verdik
        }
    }
}
