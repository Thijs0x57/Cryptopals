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
            else Console.WriteLine("Challenge 1 result: " + result);
        }
        private static void challengeTwo()
        {
            byte[] input = HexStringToHex("1c0111001f010100061a024b53535009181c");
            byte[] xorKey = HexStringToHex("686974207468652062756c6c277320657965");
            byte[] result = XorEqualBuffers(input, xorKey);
            string resultString = ByteArrayToString(result);
            if (resultString != "746865206b696420646f6e277420706c6179".ToUpper()) Console.WriteLine("something went wrong");
            else Console.WriteLine("Challenge 2 result:\n\thex: " + resultString + "\n\tascii: " + Encoding.Default.GetString(result));
            
        }

        public static byte[] XorEqualBuffers(byte[] input, byte[] xorKey)
        {
            byte[] result = new byte[input.Length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = (byte) (input[i] ^ xorKey[i]);
            }

            return result;
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

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }
    }
}
