using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptopalsSetOne
{
    class Program
    {
        static void Main(string[] args)
        {
            challengeOne();
            challengeTwo();
            Console.ReadKey();
        }


        private static void challengeOne()
        {
            string result = HexString2B64String("49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d");
            if (result != "SSdtIGtpbGxpbmcgeW91ciBicmFpbiBsaWtlIGEgcG9pc29ub3VzIG11c2hyb29t") Console.WriteLine("something went wrong");
            else Console.WriteLine("challenge 1 result: " + result);
        }
        private static void challengeTwo()
        {
            
        }

        public static string HexString2B64String(string input)
        {
            // Convert Hex to base64
            return Convert.ToBase64String(HexStringToHex(input));
        }

        //This method takes hex in a string and return an byte array
        public static byte[] HexStringToHex(string inputHex)
        {
            // The ascii representation of bytes (in hex) takes 2 characters we divide the length of the string by 2
            var resultantArray = new byte[inputHex.Length / 2];
            for (var i = 0; i < resultantArray.Length; i++)
            {
                // Take the substring starting at the right byte and take the 2 characters at that place and convert it to an actual byte
                resultantArray[i] = Convert.ToByte(inputHex.Substring(i * 2, 2), 16);
            }
            return resultantArray;
        }
    }
}
