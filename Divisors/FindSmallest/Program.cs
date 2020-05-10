using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Divisibility_2._0
{
    class Program
    {
        static void Main(string[] args)
        {
            List<BigInteger> primes = ReadtxtBigIntegeroList();
            List<BigInteger> primescop = new List<BigInteger>();
            for(int i = 0; i < primes.Count; i++)
            {
                primescop.Add(primes[i]);
            }
            BigInteger testfor = 1;

            while (testfor != 0)
            {
                //Create Variables
                List<List<BigInteger>> allpos = new List<List<BigInteger>>();
                Console.WriteLine("What number would you like to test? (0 to exit)");
                testfor = BigInteger.Parse(Console.ReadLine());
                DateTime start = DateTime.Now; //Start timer
                BigInteger smallest = Power(2, testfor); //Largest smallest number possible as a solution is 2^(n-1)

                //Primetest (if testfor is prime then smallest sollution = 2^(testfor-1)
                bool isP = IsPrime(testfor, primes);

                if (testfor != 0 && testfor != 1 && !isP)
                {
                    //Find all divisors of testfor
                    List<BigInteger> divisors = new List<BigInteger>();
                    for (int i = (int)(testfor / 2); i > 1; i--)
                    {
                        if (testfor % i == 0)
                        {
                            divisors.Add(i);
                        }
                    }

                    //pair divisors
                    if (divisors.Count != 1)
                    {
                        int count = divisors.Count;

                        if (count % 2 != 0)
                        {
                            count--;
                        }

                        for (int i = 0; i < count / 2; i++)
                        {
                            allpos.Add(new List<BigInteger> { divisors[i], divisors[divisors.Count - 1 - i] });
                        }

                        if (divisors.Count % 2 != 0)
                        {
                            allpos.Add(new List<BigInteger> { divisors[((divisors.Count - 1) / 2)], divisors[((divisors.Count - 1) / 2)] });
                        }
                    }
                    else
                    {
                        allpos.Add(new List<BigInteger> { divisors[0], divisors[0] });
                    }

                    //find all combinations of splitting the divisors
                    for (int i = 0; i < allpos.Count; i++)      //Go over all groupings of divisors
                    {
                        List<List<List<BigInteger>>> temp = new List<List<List<BigInteger>>>();     //Create temporary 3D List
                        for (int j = 0; j < allpos[i].Count; j++)       //Go over each element of each grouping
                        {
                            temp.Add(new List<List<BigInteger>>());     //For each combination of constructions add a 2D List in temp
                            if (IsPrime(allpos[i][j], primes))
                            {
                                temp[j].Add(new List<BigInteger> { allpos[i][j] });     //If prime add self
                            }
                            else
                            {
                                for (int a = 2; a < allpos[i][j] / 2; a++)      //Check divisors of element of grouping
                                {
                                    if (allpos[i][j] % a == 0)
                                    {
                                        temp[j].Add(new List<BigInteger> { (BigInteger)a, allpos[i][j] / a });      //If divisor is found, add to temporary list, together with pair
                                    }
                                }
                            }
                        }
                        BigInteger count = 1;
                        foreach (List<List<BigInteger>> listpos in temp)        //Calculate amount of possible pairings
                        {
                            count *= listpos.Count;
                        }

                        List<List<BigInteger>> carryover = new List<List<BigInteger>>();    //Create carryover List
                        for (int j = 0; j < count; j++)     //Create carryoverlist for each of the possible combinations
                        {
                            carryover.Add(new List<BigInteger>());
                        }

                        foreach (List<List<BigInteger>> divpairs in temp)        //go over Lists of divisorpairs
                        {
                            for (int a = 0; a < divpairs.Count; a++)     //Go over divisorspairs
                            {
                                for (int b = (int)(a * (count / divpairs.Count)); b < (a + 1) * (count / divpairs.Count); b++)        //Add pairs to carryover according to needed frequency
                                {
                                    carryover[b].Add(divpairs[a][0]);
                                    if (divpairs[a].Count > 1)
                                    {
                                        carryover[b].Add(divpairs[a][1]);
                                    }
                                }
                            }
                        }

                        for (int j = 0; j < carryover.Count; j++)       //Go over each carryover list
                        {
                            carryover[j].Sort();      //Sort carryover list
                            carryover[j].Reverse();     //Reverse (we want large to small)
                            bool done = false;

                            for (int a = 0; a < allpos.Count && !done; a++)     //Go over allpos List
                            {
                                if (IsListEqual(carryover[j], allpos[a]))      //If equal delete carryover list
                                {
                                    carryover.RemoveAt(j);
                                    j--;                                //-- index to check all of carryover since an element was deleted
                                    done = true;
                                }
                            }
                        }

                        for (int j = 0; j < carryover.Count - 1; j++)       //Go over each carryover list
                        {
                            bool done = false;
                            for (int a = j + 1; a < carryover.Count && !done; a++)      //Go over all carryover lists after first
                            {
                                if (IsListEqual(carryover[j], carryover[a]))           //If lists are equal, delete earliest
                                {
                                    carryover.RemoveAt(j);
                                    j--;                                //-- index to check all of carryover since an element was deleted
                                    done = true;
                                }
                            }
                        }

                        foreach (List<BigInteger> possol in carryover)       //Add all remaining carryovers to list of possible solutions
                        {
                            allpos.Add(possol);
                        }
                    }

                    foreach (List<BigInteger> comb in allpos)      //Go over all possible solutions
                    {
                        BigInteger number = 1;

                        if (comb.Count > primes.Count)      //Check if enough prime numbers are present 
                        {
                            primes = AddPrimesUntil(primes, comb.Count);
                        }

                        for (int j = 0; j < comb.Count; j++)       //Calculate solution
                        {
                            number *= Power(primes[j], comb[j] - 1);
                        }

                        if (number < smallest)      //If solution is smaller than previous smallest replace smallest
                        {
                            smallest = number;
                        }
                    }
                }
                DateTime end = DateTime.Now;     //End timer
                if (isP)        //Display result
                {
                    Console.WriteLine($"{Power(2, testfor - 1)}");
                }
                else if (testfor > 1)
                {
                    Console.WriteLine($"{smallest}");
                }
                else if (testfor != 0)
                {
                    Console.WriteLine("1");
                }
                if (testfor != 0)
                {
                    Console.WriteLine($"Tested combinations: {allpos.Count}");      //Display amount of possible solutions checked
                    Console.WriteLine($"Time to find: {end - start}");      //Display calculation time
                    Console.WriteLine("\n\n");
                }
            }

            SaveNewPrimes(primes, primescop);      //Put generated primes in file
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
                    File.WriteAllText("res_divisibility/primes.txt", "2;3");       //Write 2 in file to prevent count = 0 errors in code
                }
                string primestxt = File.ReadAllText("res_divisibility/primes.txt");      //read file
                string[] tempstrarr = new string[] { "" };      //create string array
                tempstrarr = primestxt.Split(';');      //Split text from file into string array
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

        static void SaveNewPrimes(List<BigInteger> primes, List<BigInteger> primescop)
        {
            Console.WriteLine("Saving, do not turn off the program.");

            string totxt = "";

            for (int i = primescop.Count; i < primes.Count; i++)      //add rest of primes with separators
            {
                totxt += $";{primes[i]}";
            }
            File.AppendAllText("res_divisibility/primes.txt", totxt);
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

        static BigInteger Power(BigInteger x, BigInteger y)     //Math.Pow does not accept BigIntegers
        {
            BigInteger result = 1;
            for (int i = 0; i < y; i++)
            {
                result *= x;
            }
            return result;
        }

        static List<BigInteger> AddPrimesUntil(List<BigInteger> primes, int wantedAmount)
        {
            while (wantedAmount > primes.Count)     //Repeat Until Done
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

        static bool IsListEqual(List<BigInteger> List1, List<BigInteger> List2)
        {
            if (List1.Count == List2.Count)      //If length != => not equal
            {
                int counter = 0;
                for (int i = 0; i < List1.Count; i++)        //Compare elements
                {
                    if (List1[i] == List2[i])
                    {
                        counter++;              //If elements == => counter++
                    }
                }
                if (counter == List1.Count)      //If every element == => equal
                {
                    return true;
                }
            }
            return false;
        }
    }
}