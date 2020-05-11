using System;
using System.Numerics;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection.Metadata.Ecma335;

namespace ListDivisors
{
    class Program
    {
        static void Main(string[] args)
        {
            List<BigInteger> primes = new List<BigInteger>();
            List<BigInteger> primescop = new List<BigInteger>();
            BigInteger value = 1;

            List<BigInteger> primefact = new List<BigInteger>();

            while (value != 0)
            {
                bool succeeded = false;
                while (!succeeded)
                {
                    Console.WriteLine("Please enter your number. (0 to exit)");
                    if (BigInteger.TryParse(Console.ReadLine(), out value))
                    {
                        succeeded = true;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid number\n");
                    }
                }

                if (value != 0)
                {
                    primes = ReadtxtBigIntegeroList(value);
                    primescop = new List<BigInteger>();

                    for (int i = 0; i < primes.Count; i++)
                    {
                        primescop.Add(primes[i]);
                    }

                    if (primes[primes.Count - 1] < value)
                    {
                        AddPrimesUntil(primes, value);
                    }

                    foreach (BigInteger p in primes)
                    {
                        BigInteger valcop = value;
                        while (valcop % p == 0)
                        {
                            primefact.Add(p);
                            valcop /= p;
                        }
                    }

                    primefact.Sort();

                    List<List<BigInteger>> exponent = new List<List<BigInteger>>();
                    exponent.Add(new List<BigInteger>() { primefact[0], 1 });

                    for (int i = 1; i < primefact.Count; i++)
                    {
                        if (primefact[i] == exponent[exponent.Count - 1][0])
                        {
                            exponent[exponent.Count - 1][1]++;
                        }
                        else
                        {
                            exponent.Add(new List<BigInteger>() { primefact[i], 1 });
                        }
                    }

                    List<BigInteger> allfact = new List<BigInteger>();

                    for (int i = 0; i < primefact.Count; i++)
                    {
                        allfact.Add(primefact[i]);
                    }

                    for (int i = 0; i < allfact.Count; i++)
                    {
                        for (int j = i + 1; j < allfact.Count; j++)
                        {
                            if (allfact[i] == allfact[j])
                            {
                                allfact.RemoveAt(j);
                                j--;
                            }
                        }
                    }

                    for (int i = 0; i < allfact.Count - 1; i++)
                    {
                        for (int j = i; j < allfact.Count; j++)
                        {
                            if (value % (allfact[i] * allfact[j]) == 0)
                            {
                                int counter = 0;
                                for (int a = 0; a < allfact.Count; a++)
                                {
                                    if (allfact[a] == allfact[i] * allfact[j])
                                    {
                                        counter++;
                                    }
                                }
                                if (counter == 0)
                                {
                                    allfact.Add((allfact[i] * allfact[j]));
                                }
                            }
                        }
                    }

                    allfact.Sort();

                    Console.Write($"\nPrime factorization: {value} = {exponent[0][0]}^{exponent[0][1]}");
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
                    Console.Write($"All divisors: 1, {allfact[0]}");
                    for (int i = 1; i < allfact.Count; i++)
                    {
                        Console.Write($", {allfact[i]}");
                    }
                    Console.WriteLine("\n\n");
                    Console.ReadKey();
                    SaveNewPrimes(primes, primescop);
                    Console.Clear();
                }
            }
        }

        static List<BigInteger> ReadtxtBigIntegeroList()
        {
            List<BigInteger> primes = new List<BigInteger>();       //Create list
            {
                if (!File.Exists("res_divisibility/primes.txt"))     //Check if file exists
                {
                    if (!Directory.Exists("res_divisibility"))       //Check if directory exists
                    {
                        Directory.CreateDirectory("res_divisibility");
                    }
                    File.Create("res_divisibility/primes.txt").Close();
                    File.WriteAllText("res_divisibility/primes.txt", "2\n3");       //Write 2 in file to prevent count = 0 errors in code
                }
                string primestxt = File.ReadAllText("res_divisibility/primes.txt");      //read file
                string[] tempstrarr = new string[] { "" };      //create string array
                tempstrarr = primestxt.Split('\n');      //Split text from file into string array
                foreach (string prime in tempstrarr)     //parse all strings to ints 
                {
                    BigInteger temp = new BigInteger();
                    if (BigInteger.TryParse(prime, out temp))
                    {
                        primes.Add(BigInteger.Parse(prime));
                    }
                }
            }
            return primes;
        }

        static List<BigInteger> ReadtxtBigIntegeroList(BigInteger limit)
        {
            List<BigInteger> primes = new List<BigInteger>() { 2, 3 };       //Create list
            if (!File.Exists("res_divisibility/primes.txt"))     //Check if file exists
            {
                if (!Directory.Exists("res_divisibility"))       //Check if directory exists
                {
                    Directory.CreateDirectory("res_divisibility");
                }
                File.Create("res_divisibility/primes.txt").Close();
                File.WriteAllText("res_divisibility/primes.txt", "2\n3");       //Write 2 in file to prevent count = 0 errors in code
            }

            string line = "";
            StreamReader file = new StreamReader("res_divisibility/primes.txt");        //Read line by line until limit is found
            if (limit > 3)
            {
                for (int i = 0; (line = file.ReadLine()) != null && primes[i] < limit; i++)
                {
                    if (line != "2" && line != "3")
                    {
                        primes.Add(BigInteger.Parse(line));
                    }
                }
            }
            file.Close();
            return primes;
        }

        static void SaveNewPrimes(List<BigInteger> primes, List<BigInteger> primescop)
        {
            Console.WriteLine("Saving, do not turn off the program.");

            string totxt = "";

            int countOld = 0;

            string line = "";
            StreamReader file = new StreamReader("res_divisibility/primes.txt");
            while ((line = file.ReadLine()) != null)
            {
                countOld++;
            }
            file.Close();

            for (int i = countOld + 2; i < primes.Count; i++)      //add rest of primes with separators
            {
                totxt += $"\n{primes[i]}";
            }

            bool success = false;

            while (!success)
            {
                try
                {
                    File.AppendAllText("res_divisibility/primes.txt", totxt);
                    success = true;
                }
                catch (Exception error) { }
            }
        }

        static List<BigInteger> AddPrimesUntil(List<BigInteger> primes, int wantedAmount)
        {
            while (wantedAmount > primes[primes.Count - 1])     //Repeat Until Done
            {
                BigInteger test = primes[primes.Count - 1];
                bool found = false;

                while (!found)
                {
                    test += 2;
                    if (IsPrime(test, primes))       //If new prime is found, add to list
                    {
                        primes.Add(test);
                        found = true;
                    }
                }
            }
            return primes;
        }

        static List<BigInteger> AddPrimesUntil(List<BigInteger> primes, BigInteger wantedAmount)
        {
            while (wantedAmount > primes[primes.Count - 1])     //Repeat Until Done
            {
                BigInteger test = primes[primes.Count - 1];
                bool found = false;

                while (!found)
                {
                    test += 2;
                    if (IsPrime(test, primes))       //If new prime is found, add to list
                    {
                        primes.Add(test);
                        found = true;
                    }
                }
            }
            return primes;
        }

        static bool IsPrime(BigInteger test, List<BigInteger> primes)
        {
            if (primes[primes.Count - 1] < (test + 1) / 2)     //If not enough primes to check against add primes
            {
                primes = AddPrimesUntil(primes, (int)((test + 1) / 2));
            }

            for (int i = 0; i < primes.Count; i++)      //check divisibility
            {
                if (test % primes[i] == 0)
                {
                    if (test == primes[i])
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
