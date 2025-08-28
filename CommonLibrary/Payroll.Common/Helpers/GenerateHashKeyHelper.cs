using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.Helpers
{
    public static class GenerateHashKeyHelper
    {
        /// <summary>
        /// Definition :- SHA-512 is not an encryption algorithm but a cryptographic hash function.
        ///            :- Which generates a fixed-size (512-bit or 64-byte) hash value from an input.
        ///            :- Ensuring that any small change in the input results in a drastically different hash value.
        ///            :- Note :-  you cannot reverse the process to retrieve the original input. 
        /// </summary>
        /// <param name="input">string type as input parameter accept.</param>
        /// <returns>String hash key.</returns>
        public static string HashKey(string input)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
