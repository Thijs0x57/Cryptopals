﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CryptopalsSetOne
{
    static class Program
    {
        public static bool debug = false;
        static void Main(string[] args)
        {
            ChallengeOne();
            Console.WriteLine();
            if (debug) Console.ReadKey();
            ChallengeTwo();
            Console.WriteLine();
            if (debug) Console.ReadKey();
            ChallengeThree();
            Console.WriteLine();
            if (debug) Console.ReadKey();
            ChallengeFour();
            Console.WriteLine();
            if (debug) Console.ReadKey();
            ChallengeFive();
            Console.WriteLine();
            if (debug) Console.ReadKey();
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
            char[] alphabetScores = new char[327];

            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(@"4.txt");
            while ((line = file.ReadLine()) != null)
            {
                Tuple<Tuple<string, double>,char> t = decodeHexString(line);
                decodedLines[counter] = t.Item1.Item1;
                lineScores[counter] = t.Item1.Item2;
                alphabetScores[counter] = t.Item2;
                if (debug) Console.WriteLine("\ncounter: " + counter);
                counter++;
            }

            double maxScore = lineScores.Max();
            int maxIndex = lineScores.ToList().IndexOf(maxScore);

            file.Close();
            if (debug) Console.WriteLine("There were {0} lines.", counter);

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
            byte[] line = Encoding.ASCII.GetBytes("Burning 'em, if you ain't quick and nimble\nI go crazy when I hear a cymbal");
            string key = "ICE";
            string result = ByteArrayToString(XorBytesWithRepeatingKey(line, key));
            if (result != "0b3637272a2b2e63622c2e69692a23693a2a3c6324202d623d63343c2a26226324272765272a282b2f20430a652e2c652a3124333a653e2b2027630c692b20283165286326302e27282f".ToUpper())
            {
                Console.WriteLine("Something went wrong!");
            }
            Console.WriteLine("Challenge 5 result: " + result);
        }

        private static void ChallengeSix()
        {
            string encoded = File.ReadAllText("6.txt");
            if (debug) Console.WriteLine("encoded with base64:\n" + encoded);
            byte[] cipherText = DecodeBase64(encoded);
            Dictionary<int, int> keySizeResults = new Dictionary<int, int>();
            // 1. "Let KEYSIZE be the guessed length of the key; try values from 2 to (say) 40."
            for (var keySize = 2; keySize <= 40; keySize++)
            {
                // 3. "For each KEYSIZE, take the first KEYSIZE worth of bytes, 
                // and the second KEYSIZE worth of bytes, and find the edit distance between them.
                // Normalize this result by dividing by KEYSIZE."
                var hammingDistance = 0;
                var numberOfHams = 0;

                for (int i = 1; i < cipherText.Length / keySize; i++)
                {
                    // take the amount of bytes the key is long
                    // this means that a keySize of 2 will take the first 2 bytes
                    // The .Take method returns the specified part of an array
                    // The .skip method makes sure we don't use the same piece of the text twice 
                    // it does so by multiplying the inumerator with the keySize
                    var firstKeySizeBytes = cipherText.Skip(keySize * (i - 1)).Take(keySize);
                    var secondKeySizeBytes = cipherText.Skip(keySize * i).Take(keySize);

                    hammingDistance += computeHammingDistance(firstKeySizeBytes.ToArray(), secondKeySizeBytes.ToArray());
                    numberOfHams++;
                }

                var normalizedDistance = hammingDistance / numberOfHams / keySize;
                keySizeResults.Add(keySize, normalizedDistance);
            }

            KeyValuePair<int, int> smallestDistance = keySizeResults.First();
            // go through the dictionary and get the smallest normilzed distance (since this is likely the key size)
            foreach (KeyValuePair<int, int> kvp in keySizeResults)
            {
                if (kvp.Value < smallestDistance.Value) smallestDistance = kvp;
                if (debug) Console.WriteLine("Keysize = {0} \t normalizedDistance = {1}", kvp.Key, kvp.Value);
            }
            Console.WriteLine("smallest distance: {0}, keySize: {1}", smallestDistance.Value, smallestDistance.Key);

            // 5. "Now that you probably know the KEYSIZE: break the ciphertext into blocks of KEYSIZE length."
            var blocksOfKeySize = cipherText.CreateMatrix(smallestDistance.Key);
            // 6. "Now transpose the blocks: make a block that is the first byte of every block,
            // and a block that is the second byte of every block, and so on."
            var transposedBlocks = blocksOfKeySize.Transpose();
            // 7. "Solve each block as if it was single-character XOR. You already have code to do this."
            var bruteForceResults = transposedBlocks
                .Select(x => decodeHexString(ByteArrayToString(x)))
                .ToList();
            var iterator = 0;
            foreach (var result in bruteForceResults)
            {
                if(debug)Console.WriteLine("brute force result {0}: {1}", iterator, result);
                iterator++;
            }

            // 8. "For each block, the single-byte XOR key that produces the best looking histogram is the repeating-key
            // XOR key byte for that block. Put them together and you have the key."
            var fullKey = string.Empty;
            foreach (var result in bruteForceResults)
            {
                string key = null;
                double currentHighestScore = 0;

                //foreach (var attempt in result)
                for(int i = 0; i < 2; i++)
                {
                    var rating = EnglishRating(result.Item1.Item1);
                    if (currentHighestScore < rating)
                    {
                        key = result.Item2.ToString();
                        currentHighestScore = rating;
                    }
                }

                fullKey += key;
            }
            Console.WriteLine("full key: \"{0}\"\n", fullKey);
            byte[] decodedResult = XorBytesWithRepeatingKey(cipherText, fullKey);
            Console.WriteLine("decoded message: " + Encoding.Default.GetString(decodedResult));
        }

        public static byte[] XorBytesWithRepeatingKey(byte[] toBeXorred, string key)
        {
            int keyLength = key.Length;
            byte[] result = new byte[toBeXorred.Length];
            for (int i = 0; i < toBeXorred.Length; i++)
            {
                // To repeat the key, we use the iterator and modulo it against the keylenght. This will return the index of the right key
                // example: 0 % 3 = 0 ('I'), 1 % 3 = 1 ('C'), 2 % 3 = 2 ('E'), 3 % 3 = 0 ('I'), etc.
                char charKey = key.ElementAt(i % keyLength);
                byte b = (XorOneByte(toBeXorred[i], charKey));
                if (debug) Console.WriteLine("char key: " + charKey + "\tresult byte: " + b);
                result[i] = b;
            }
            return result;
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

        public static int computeHammingDistance(byte[] input, byte[] other)
        {
            int result = 0;
            if (input.Length != other.Length)
            {
                Console.WriteLine("to compute hamming distance, the length needs to be the same.");
            }
            if (debug) Console.WriteLine("input: " + ByteArrayToString(input) + "\nother: " + ByteArrayToString(other));

            for (int i = 0; i < input.Length; i++)
            {
                // Xor the values (to see if they're different)
                var value = input[i] ^ other[i];
                while (value != 0)
                {
                    result++;
                    if (debug) Console.WriteLine("original value: " + value + "\tvalue -1: " + (value - 1) + "\tAND: " + (value & (value - 1)));
                    // not sure what is happening here
                    // value = value & value - 1; 
                    value &= value - 1;
                }
                if (debug) Console.WriteLine();
            }
            return result;
        }

        public static T[][] CreateMatrix<T>(this IEnumerable<T> source, int size)
        {
            var taken = 0;
            var output = new List<T[]>();
            var enumeratedSource = source.ToArray();

            while (taken < enumeratedSource.Length)
            {
                output.Add(enumeratedSource.Skip(taken).Take(size).ToArray());
                taken += size;
            }

            return output.ToArray();
        }

        public static T[][] Transpose<T>(this T[][] source)
        {
            var transposedBlocks = new List<List<T>>();

            for (var i = 0; i <= source.Length; i++)
            {
                foreach (var block in source.ToList())
                {
                    if (i < block.Length)
                    {
                        if (transposedBlocks.ElementAtOrDefault(i) == null)
                            transposedBlocks.Add(new List<T>());

                        transposedBlocks[i].Add(block[i]);
                    }
                }
            }

            return transposedBlocks.Select(x => x.ToArray()).ToArray();
        }

        public static double EnglishRating(string text)
        {
            var chars = text.ToUpper().GroupBy(c => c).Select(g => new { g.Key, Count = g.Count() });

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

        public static Tuple<Tuple<string, double>,char> decodeHexString(string encodedString)
        {
            char[] alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789: ".ToCharArray();
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

            return Tuple.Create(Tuple.Create(decodedArray[maxIndex], score[maxIndex]), alphabet[maxIndex]);
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

        public static byte[] DecodeBase64(string input)
        {
            return Convert.FromBase64String(input);
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
