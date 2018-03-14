using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public static class ByteExtensions
    {
        /// <summary>
        /// Convert byte array to base 64 string
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] self)
        {
            if (self == null)
            {
                return null;
            }

            return Convert.ToBase64String(self);
        }

        /// <summary>
        /// Calculate MD5 for byte array
        /// </summary>
        /// <param name="self">byte array</param>
        /// <returns>null or hash values</returns>
        public static byte[] ToMd5Hash(this byte[] self)
        {
            if (self == null || self.Length == 0)
            {
                return null;
            }

            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(self);
            }
        }

        /// <summary>
        /// Calculate MD5 for byte array
        /// </summary>
        /// <param name="self">byte array</param>
        /// <returns>null or base 64 of MD5</returns>
        public static string ToBase64Md5Hash(this byte[] self)
        {
            if (self == null || self.Length == 0)
            {
                return null;
            }

            return self.ToMd5Hash()
                .ToBase64String();
        }
    }
}
