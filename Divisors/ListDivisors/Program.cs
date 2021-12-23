using System;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ListDivisors
{
    class Program
    {
        static List<BigInteger> primes = new List<BigInteger>();
        static void Main(string[] args)
        {
            BigInteger value = 1;

            while (value != 0)
            {
                bool succeeded = false;
                while (!succeeded)
                {
                    Console.WriteLine("Please enter your number. (0 to exit)");
                    if (BigInteger.TryParse(Console.ReadLine(), out value))
                        succeeded = true;
                    else
                        Console.WriteLine("Please enter a valid number\n");
                }

                DateTime start = DateTime.Now;

                if (value < 1)
                    break;

                List<BigInteger> primefact = GetPrimeFactors(value);

                List<BigInteger[]> exponent = ConvertToExponents(primefact);

                List<BigInteger> allFact = FindAllDivisors(exponent, value);

                Console.WriteLine(DateTime.Now - start);

                Console.Write($"\nPrime factorization: {value} = {exponent[0][0]}" + (exponent[0][1] > 1 ? $"^{exponent[0][1]}" : ""));
                for (int i = 1; i < exponent.Count; i++)
                {
                    if (exponent[i][1] > 1)
                    {
                        Console.Write($" * {exponent[i][0]}^{exponent[i][1]}");
                    }
                    else
                    {
                        Console.Write($" * {exponent[i][0]}");
                    }
                }
                Console.Write($"\n{value} = {primefact[0]}");
                for (int i = 1; i < primefact.Count; i++)
                {
                    Console.Write($" * {primefact[i]}");
                }
                Console.ReadKey();
                Console.WriteLine();
                Console.Write($"All divisors: 1, {allFact[0]}");
                for (int i = 1; i < allFact.Count; i++)
                {
                    Console.Write($", {allFact[i]}");
                }
                Console.WriteLine("\n\n");
                Console.ReadKey();
                SaveNewPrimes();
                Console.Clear();
            }
        }

        /// <summary>
        /// Returns all the divisors of value
        /// </summary>
        /// <param name="exponents">The exponent formatted List of prime factors of value</param>
        /// <param name="value">The value of which to find the divisors</param>
        /// <returns>A List of divisors of value</returns>
        static List<BigInteger> FindAllDivisors(List<BigInteger[]> exponents, BigInteger value)
        {
            List<BigInteger> allFact = new List<BigInteger>();

            for(int i = 0; i < exponents.Count; i++)
                allFact.Add(exponents[i][0]);

            for (int i = 0; i < allFact.Count - 1; i++)
            {
                for (int j = i; j < allFact.Count; j++)
                {
                    if (value % (allFact[i] * allFact[j]) == 0 && !allFact.Contains(allFact[i] * allFact[j]))
                        allFact.Add((allFact[i] * allFact[j]));
                }
            }

            allFact.Sort();

            return allFact;
        }

        /// <summary>
        /// Converts a List of primefactors to a list of primes with their components
        /// </summary>
        /// <param name="primefactors">The List of prime factors</param>
        /// <returns>A list of primes with their exponents</returns>
        static List<BigInteger[]> ConvertToExponents(List<BigInteger> primefactors)
        {
            List<BigInteger[]> exponent = new List<BigInteger[]> {new BigInteger[]{ primefactors[0], 1 }};

            for (int i = 1; i < primefactors.Count; i++)
            {
                if (primefactors[i] == exponent.Last()[0])
                    exponent.Last()[1]++;
                else
                    exponent.Add(new BigInteger[]{ primefactors[i], 1 });
            }

            return exponent;
        }

        /// <summary>
        /// Returns a List of all the prime factors a BigInteger
        /// </summary>
        /// <param name="value">The BigInteger of which to seek the prime factors</param>
        /// <returns>A list of prime factors, sorted from small to large</returns>
        static List<BigInteger> GetPrimeFactors(BigInteger value)
        {
            if (primes.Count == 0 || primes.Last() < (value + 1) / 2)
                primes = ReadtxtBigIntegeroList((value + 1) / 2);

            if (primes.Last() < (value + 1) / 2)
                AddPrimesUntil((value + 1) / 2);

            List<BigInteger> primefact = new List<BigInteger>();
            BigInteger valcop = value;
            for (int i = 0; i < primes.Count && primes[i] <= valcop && primes[i] <= (value + 1) / 2; i++)
            {
                while (valcop % primes[i] == 0)
                {
                    primefact.Add(primes[i]);
                    valcop /= primes[i];
                }
            }

            if (primefact.Count == 0)
            {
                if (IsPrime(value))
                    primefact.Add(value);
            }

            primefact.Sort();

            return primefact;
        }

        /// <summary>
        /// Reads all primes smaller than the given limit
        /// </summary>
        /// <param name="limit">The largest allowed prime</param>
        /// <returns>A read out list of primes.</returns>
        static List<BigInteger> ReadtxtBigIntegeroList(BigInteger limit)
        {
            Console.WriteLine("Reading file...");
            List<BigInteger> primes = new List<BigInteger>();       //Create list
            if (!File.Exists("res_divisibility/primes.pn"))     //Check if file exists
            {
                if (!Directory.Exists("res_divisibility"))       //Check if directory exists
                    Directory.CreateDirectory("res_divisibility");

                File.Create("res_divisibility/primes.pn").Close();
            }

            FileStream file = new FileStream("res_divisibility/primes.pn", FileMode.OpenOrCreate, FileAccess.Read);        //Read primes
            while(file.Position < file.Length && (primes.Count() == 0 || primes.Last() < limit))
            {
                if (primes.Count == 6543)
                { }
                int length = ReadVariableLength(file);
                primes.Add(ReadVarLenBigInteger(file, length));
            }
            file.Close();
            return primes.Count == 0 ? new List<BigInteger>{2,3,5,7} : primes;
        }

        /// <summary>
        /// Reads all primes smaller than the given limit
        /// </summary>
        /// <param name="limit">The largest allowed prime</param>
        /// <param name="list"></param>
        /// <returns></returns>
        static List<BigInteger> ReadtxtBigIntegeroList(BigInteger limit, List<BigInteger> list)
        {
            Console.WriteLine("Reading file...");
            List<BigInteger> primes = new List<BigInteger>();       //Create list
            if (!File.Exists("res_divisibility/primes.pn"))     //Check if file exists
            {
                if (!Directory.Exists("res_divisibility"))       //Check if directory exists
                    Directory.CreateDirectory("res_divisibility");

                File.Create("res_divisibility/primes.pn").Close();
            }

            FileStream file = new FileStream("res_divisibility/primes.pn", FileMode.OpenOrCreate, FileAccess.Read);        //Read primes
            while (file.Position < file.Length && primes.Last() < limit)
            {
                int length = ReadVariableLength(file);
                primes.Add(ReadVarLenBigInteger(file, length));
            }
            file.Close();
            return primes.Count == 0 ? new List<BigInteger> { 2, 3, 5, 7 } : primes;
        }

        /// <summary>
        /// Reads out a viariable length integers length
        /// </summary>
        /// <param name="file">The FileStream from which to read</param>
        /// <returns>An Int32 with the length of the variable length integer</returns>
        static int ReadVariableLength(FileStream file)
        {
            BitArray bits = new BitArray(0);
            byte b = 0;
            while (b >> 7 == 0)
            {
                b = (byte)file.ReadByte();
                bits.Length += 7;
                bits.Or(VarLenByteToBitArray((byte)(b & 0b01111111), bits.Count));
            }
            return GetIntFromBitArray(Reverse(bits));
        }

        /// <summary>
        /// Turns a read Variable Length integer byte into a BitArray
        /// </summary>
        /// <param name="b">The byte to convert</param>
        /// <param name="length">The desired length of the BitArray</param>
        /// <returns>A BitArray representing the byte</returns>
        static BitArray VarLenByteToBitArray(byte b, int length)
        {
            BitArray bitArray = new BitArray(length);
            for (int i = 0; i < 7; i++)
            {
                bitArray.Set(length - (7 - i), (b&(1<<(6-i)))>>(6-i) == 1);
            }
            return bitArray;
        }

        /// <summary>
        /// Parses a BitArray to an Int32
        /// </summary>
        /// <param name="b">The BitArray, this should not be larger than 32 bits</param>
        /// <returns>An integer value</returns>
        /// <exception cref="ArgumentException">This error is thrown when the BitArray is longer than 32 bits</exception>
        static int GetIntFromBitArray(BitArray b)
        {
            if (b.Count > 32)
                throw new ArgumentException("BitArray should not be longer than 32 bits.");

            int[] ar = new int[1];
            b.CopyTo(ar, 0);
            return ar[0];
        }

        /// <summary>
        /// Reads a BigInteger of a given amount of bytes (Bigendian) from a file.
        /// </summary>
        /// <param name="file">The FileStream to read the BigInteger from</param>
        /// <param name="length">The amount of bytes used to save the BigInteger</param>
        /// <returns>A read out BigInteger</returns>
        static BigInteger ReadVarLenBigInteger(FileStream file, int length)
        {
            byte[] bytes = new byte[length];
            file.Read(bytes, 0, length);
            return new BigInteger(bytes, true, true);
        }

        /// <summary>
        /// Saves the currently known primes to "res_divisibility/primes.pn".
        /// </summary>
        static void SaveNewPrimes()
        {
            Console.WriteLine("Saving, do not turn off the program.");
            List<byte> bytes = new List<byte>();
            foreach(BigInteger prime in primes)
            {
                if(prime > 65000)
                {

                }
                byte[] p = prime.ToByteArray(true);
                byte[] l = ToByteArray(ToVariableLength(Trim(Reverse(new BitArray(BitConverter.GetBytes((uint)p.Length))))));
                bytes.AddRange(l);
                bytes.AddRange(p.Reverse());
            }

            if (!File.Exists("res_divisibility/primes.pn") || bytes.Count > new FileInfo("res_divisibility/primes.pn").Length)
            {
                File.WriteAllBytes("res_divisibility/primes.pn", bytes.ToArray());
            }

            Console.WriteLine("Saving done");
            Console.ReadLine();
        }

        /// <summary>
        /// Reverses the order of a BitArray
        /// </summary>
        /// <param name="array">BitArray to reverse</param>
        /// <returns>A reversed BitArray</returns>
        static BitArray Reverse(BitArray array)
        {
            int length = array.Length;
            int mid = (length / 2);

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }

            return array;
        }

        /// <summary>
        /// Trim the leading 0s from a BitArray
        /// </summary>
        /// <param name="bits">The BitArray to trim</param>
        /// <returns>The trimmed BitArray</returns>
        static BitArray Trim(BitArray bits)
        {
            for (int i = 0; i < bits.Count && bits[i]!=true; i++)
            {
                if (bits[i] == false)
                {
                    bits.RightShift(1);
                    bits.Length -= 1;
                    i--;
                }
            }
            return bits;
        }

        /// <summary>
        /// Convert a BitArray to a Variable Length number;
        /// </summary>
        /// <param name="bits">The input bit array</param>
        /// <returns>The variable length number bit array</returns>
        static BitArray ToVariableLength(BitArray bits)
        {
            int dif = 7 - bits.Count % 7;
            bits.Length += dif;
            bits.LeftShift(dif);
            bool[] varLength = new bool[bits.Count + bits.Count/7];
            for(int i = 0; i < bits.Count/7; i++)
            {
                varLength[i * 8] = i==(bits.Count/7)-1;
                for(int j = 1; j < 8; j++)
                {
                    varLength[i*8+j]=bits[i*7+j-1];
                }
            }
            return new BitArray(varLength);
        }

        /// <summary>
        /// Returns a byte[] of the BitArray
        /// </summary>
        /// <param name="bits">BitArray to convert</param>
        /// <returns>byte[] of the ByteArray</returns>
        /// <exception cref="InvalidCastException">Thrown when the BitArray is not divisible by 8</exception>
        static byte[] ToByteArray(BitArray bits)
        {
            if(bits.Count % 8 != 0)
                throw new InvalidCastException("A BitArray should have a Count divisible by 8 for conversion to a byte[]");

            List<byte> bytes = new List<byte>();
            for(int i = 0; i < bits.Count / 8; i++)
            {
                byte b = 0;
                for(int j = 0; j < 8; j++)
                    b |= (byte)((bits[i * 8 + j] ? 1 : 0) << (7 - j));
                bytes.Add(b);
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Returns a byte[] from the given BitArray, padded to fit the byte format
        /// </summary>
        /// <param name="bits">The BitArray to convert</param>
        /// <param name="padLeft">Add padding to the left or not</param>
        /// <param name="pad">Value to pad with</param>
        /// <returns>byte[] of the given BitArray, padded as asked</returns>
        static byte[] ToByteArray(BitArray bits, bool padLeft = true, bool pad = false)
        {
            int dif = 8 - bits.Count % 8;
            bits.Length += dif;
            if (padLeft)
            {
                bits.RightShift(dif);
                if(pad)
                {
                    BitArray comp = new BitArray(bits.Count, false);
                    comp[0] = true;
                    for (int i = 0; i < dif; i++)
                    {
                        bits.Or(comp.RightShift(i));
                    }
                }
            }
            else if(pad)
            {
                BitArray comp = new BitArray(bits.Count, false);
                comp[comp.Count - 1] = true;
                for (int i = 0; i < dif; i++)
                {
                    bits.Or(comp.LeftShift(i));
                }
            }

            List<byte> bytes = new List<byte>();
            for (int i = 0; i < bits.Count / 8; i++)
            {
                byte b = 0;
                for (int j = 0; j < 8; j++)
                    b |= (byte)((bits[i * 8 + j] ? 1 : 0) << (7 - j));
                bytes.Add(b);
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Adds primes until
        /// </summary>
        /// <param name="wantedSize">The smallest size the largest prime number in the list is allowed to have</param>
        /// <returns></returns>
        static List<BigInteger> AddPrimesUntil(int wantedSize)
        {
            Console.WriteLine("Adding primes...");
            while (wantedSize > primes.Last())     //Repeat Until Done
            {
                BigInteger test = primes.Last();
                bool found = false;

                while (!found)
                {
                    test += 2;
                    if (IsPrime(test))       //If new prime is found, add to list
                    {
                        primes.Add(test);
                        found = true;
                    }
                }
            }
            Console.WriteLine(primes.Count);
            return primes;
        }

        /// <summary>
        /// Adds primes until
        /// </summary>
        /// <param name="wantedSize">The smallest size the largest prime number in the list is allowed to have</param>
        static void AddPrimesUntil(BigInteger wantedSize)
        {
            Console.WriteLine("Adding primes...");
            while (wantedSize > primes.Last())     //Repeat Until Done
            {
                BigInteger test = primes.Last();
                bool found = false;

                while (!found)
                {
                    test += 2;
                    if (IsPrime(test))       //If new prime is found, add to list
                    {
                        primes.Add(test);
                        found = true;
                    }
                }
            }
            Console.WriteLine(primes.Count);
        }

        /// <summary>
        /// Returns wether a number is prime or not
        /// </summary>
        /// <param name="test">Number to check</param>
        /// <returns>Is prime</returns>
        static bool IsPrime(BigInteger test)
        {
            if (primes.Last() < (test + 1) / 2)     //If not enough primes to check against add primes
                AddPrimesUntil(((test + 1) / 2));

            for (int i = 0; i < primes.Count && primes[i] < (test+1)/2; i++)      //check divisibility
            {
                if (test % primes[i] == 0)
                    return test == primes[i];
            }
            return true;
        }
    }
}
