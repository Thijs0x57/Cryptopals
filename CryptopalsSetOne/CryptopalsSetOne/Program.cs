using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CryptopalsSetOne
{
    class Program
    {
        public static bool debug = false;
        static void Main(string[] args)
        {
            ChallengeOne();
            Console.WriteLine();
            ChallengeTwo();
            Console.WriteLine();
            ChallengeThree();
            Console.WriteLine();
            ChallengeFour();
            Console.WriteLine();
            ChallengeFive();
            Console.WriteLine();
            ChallengeSix();
            Console.ReadKey();
        }


        private static void ChallengeOne()
        {
            string result = HexString2B64String("49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d");
            if (result != "SSdtIGtpbGxpbmcgeW91ciBicmFpbiBsaWtlIGEgcG9pc29ub3VzIG11c2hyb29t") Console.WriteLine("something went wrong");
            else Console.WriteLine("Challenge 1 result: " + result);
        }
        private static void ChallengeTwo()
        {
            byte[] input = HexStringToHex("1c0111001f010100061a024b53535009181c");
            byte[] xorKey = HexStringToHex("686974207468652062756c6c277320657965");
            byte[] result = XorEqualBuffers(input, xorKey);
            string resultString = ByteArrayToString(result);
            if (resultString != "746865206b696420646f6e277420706c6179".ToUpper()) Console.WriteLine("something went wrong");
            else Console.WriteLine("Challenge 2 result:\n\thex: " + resultString + "\n\tascii: " + Encoding.Default.GetString(result));
        }

        private static void ChallengeThree()
        {
            // TODO: Make sure score it right, it seems wayyy too low (which hinders challenge 4)
            Console.WriteLine("Challenge 3 result: " + decodeHexString("1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736"));
        }

        private static void ChallengeFour()
        {
            int counter = 0;
            string line;
            // in these arrays we're going store the lines and the scores (i'm too lazy to make a nice data structure)
            string[] decodedLines = new string[327];
            double[] lineScores = new double[327];

            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(@"4.txt");
            while ((line = file.ReadLine()) != null)
            {
                Tuple<string,double> t = decodeHexString(line);
                decodedLines[counter] = t.Item1;
                lineScores[counter] = t.Item2;
                if(debug) Console.WriteLine("\ncounter: "+ counter);
                counter++;
            }

            double maxScore = lineScores.Max();
            int maxIndex = lineScores.ToList().IndexOf(maxScore);

            file.Close();
            if(debug) Console.WriteLine("There were {0} lines.", counter);

            if (debug)
            {
                for (int i = 0; i < decodedLines.Length; i++)
                {
                    Console.WriteLine("string: " + decodedLines[i] + "\tscore: " + lineScores[i]);
                }
            }

            Console.WriteLine("Challenge 4 result: " + decodedLines[maxIndex] + "\tscore: " + lineScores[maxIndex]);
        }

        private static void ChallengeFive()
        {

        }

        private static void ChallengeSix()
        {

        }

        public static byte XorOneByte(byte input, char key)
        {
            return (byte)(input ^ key);
        }

        public static byte[] XorEqualBuffers(byte[] input, byte[] xorKey)
        {
            byte[] result = new byte[input.Length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = (byte)(input[i] ^ xorKey[i]);
            }

            return result;
        }

        public static double EnglishRating(string text) {
            var chars = text.ToUpper().GroupBy(c => c).Select(g => new {g.Key, Count = g.Count()});

            double coefficient = 0;

            foreach (var c in chars)
            {
                if (LetterScore.TryGetValue(c.Key, out var freq))
                {
                    coefficient += Math.Sqrt(freq * c.Count / text.Length);
                }
            }
            return coefficient;
        }

        private static readonly Dictionary<char, double> LetterScore = new Dictionary<char, double>
        {
            {'E', 12.02}, {'T', 9.10}, {'A', 8.12}, {'O', 7.68}, {'I', 7.31}, {'N', 6.95},
            {'S', 6.28}, {'R', 6.02}, {'H', 5.92}, {'D', 4.32}, {'L', 3.98}, {'U', 2.88},
            {'C', 2.71}, {'M', 2.61}, {'F', 2.30}, {'Y', 2.11}, {'W', 2.09}, {'G', 2.03},
            {'P', 1.82}, {'B', 1.49}, {'V', 1.11}, {'K', 0.69}, {'X', 0.17}, {'Q', 0.11},
            {'J', 0.10}, {'Z', 0.07}, {' ', 0.19}
        };

        public static Tuple<string,double> decodeHexString(string encodedString)
        {
            char[] alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            double[] score = new double[alphabet.Length];
            string[] decodedArray = new string[alphabet.Length];
            // For every char of the alphabet, decode the given string.
            for (int c = 0; c < alphabet.Length; c++)
            {
                string xorredString = XorEncodedHexToString(encodedString, alphabet[c]);
                score[c] = EnglishRating(xorredString);
                xorredString = Regex.Replace(xorredString, @"\t|\n|\r", "");
                decodedArray[c] = xorredString;
            }
            double maxScore = score.Max();
            int maxIndex = score.ToList().IndexOf(maxScore);
            if (debug)
            {
                for (int i = 0; i < decodedArray.Length; i++)
                {
                    Console.WriteLine("string: " + decodedArray[i] + "\tscore: " + score[i]);
                }
            }

            return Tuple.Create(decodedArray[maxIndex], score[maxIndex]);
        }

        public static string XorEncodedHexToString(string input, char xorKey)
        {
            string result = "";
            byte[] hexEncoded = HexStringToHex(input);
            for (int i = 0; i < hexEncoded.Length; i++)
            {
                char found = Convert.ToChar(XorOneByte(hexEncoded[i], xorKey));
                result += found;
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
