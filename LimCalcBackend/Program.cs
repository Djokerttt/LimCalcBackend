using System;
using System.Collections.Generic;

class Program
{
    // Splits a string to a list of monomials, using - or + as separators. Omits +, while leaving -
    static List<string> SplitToMonomials(string input)
    {
        List<string> monomials = new List<string>(); // List to store monomials
        int startIndex = 0;

        for (int i = 1; i < input.Length; i++)
        {
            if (input[i] == '+' || input[i] == '-') // Check for '+' or '-' character
            {
                string segment = input.Substring(startIndex, i - startIndex); // Extract monomial
                monomials.Add(segment); // Add monomial to the list
                if (input[i] == '+')
                {
                    startIndex = i + 1; // Update start index for next segment
                }
                else
                {
                    startIndex = i; // Update start index for next segment
                }
            }
        }

        // Add last monomial after the last '+' or '-' character (if any)
        string lastSegment = input.Substring(startIndex);
        monomials.Add(lastSegment);

        return monomials; // Return the list of monomials
    }

    // Checks is X lacks coefficient, uses 1 in that case
    static List<string> fixCoefficients(List<string> input)
    {
        // Clone the input
        List<string> output = new List<string>(input.ConvertAll(x => x));

        // Loop over the items 
        for (int i = 0; i < output.Count; i++)
        {
            string str = output[i];

            if (str.StartsWith("-x"))
            {
                // Replace "-x" with "-1x"
                output[i] = "-1" + str.Substring(1);
            }
            else if (str.StartsWith("x"))
            {
                // Replace "x" with "1x"
                output[i] = "1" + str;
            }
        }

        return output;
    }

    // Check if some X's lack powers, and use ^1 in that case
    static List<string> fixPow(List<string> input)
    {
        // Clone the input
        List<string> output = new List<string>(input.ConvertAll(x => x));

        // Loop over the items
        for (int i = 0; i < output.Count; i++)
        {
            string str = output[i];

            // Check if it has X but has no ^
            if (str.Contains("x") && !str.Contains("^"))
            {
                string newString = String.Concat(str, "^1");
                output[i] = newString;
            }
        }

        return output;
    }

    // Finds the highest number after ^, and returns it
    static int findHighestPower(List<string> input)
    {
        // Iterate over the input
        int highestPower = 1;
        foreach (string str in input)
        {
            // Find the '^' in the string
            int index = str.IndexOf('^');

            // If we found it
            if (index != -1)
            {
                // Store the currentPower as a string to compare it to the highestPower
                string currentPowerStr = str.Substring(index + 1);
                int currentPower = int.Parse(currentPowerStr);
                if (currentPower > highestPower)
                {
                    highestPower = currentPower;
                }
            }
        }
        return highestPower;
    }

    // 1. Find the powers which match the highest one exactly
    // 2. Turn those items from "3x^5" into "3"
    // 3. Turn all other items in 0
    static string finalizeDivision(List<string> input, int inputPow)
    {
        // Create a list to hold the output strings
        List<string> outputStrings = new List<string>();

        // Loop trough each string in the input list
        foreach (string str in input)
        {
            // Find the index of the ^ symbol in the string
            int index = str.IndexOf('^');

            // If it's found
            if (index != -1)
            {
                // Extract the substring after the ^ symbol
                string numString = str.Substring(index + 1);

                // Parse the substring as int
                int currentPower = int.Parse(numString);

                if (currentPower == inputPow)
                {
                    if (str.StartsWith("-"))
                    {
                        outputStrings.Add(str.Substring(0, index - 1));
                    }
                    else
                    {
                        outputStrings.Add("+" + str.Substring(0, index - 1));
                    }

                    // Go to the next string
                    continue;
                }
            }

            // If there is no ^ symbol or the power doesn't match, add -0 or +0 to the output list
            if (str.StartsWith("-"))
            {
                outputStrings.Add("-0");
            }
            else
            {
                outputStrings.Add("+0");
            }
        }

        // Turn the list of strings into one string
        string separator = "";
        string output = string.Join(separator, outputStrings);

        // Remove leading +, if present
        if (output.StartsWith("+"))
        {
            string outputTrimmed = output.TrimStart('+');
            return outputTrimmed;
        }

        return output;
    }

    static int result(string expression)
    {

        // Split the expression to monomials with + and - as delimiters
        string[] parts = expression.Split(new[] { "+", "-" }, StringSplitOptions.RemoveEmptyEntries);
        int resultInt = 0;

        // Loop over each monomial and add/substract it's value to the result
        foreach (string part in parts)
        {
            int value = int.Parse(part);
            if (expression.IndexOf(part) == 0)
            {
                resultInt += value;
            }
            else if (expression[expression.IndexOf(part) - 1] == '+')
            {
                resultInt += value;
            }
            else if (expression[expression.IndexOf(part) - 1] == '-')
            {
                resultInt -= value;
            }
        }

        return resultInt;
    }

    static void Main(string[] args)
    {
        // Deny user if there are less or more than 2 strings as an input
        if (args.Length != 2)
        {
            Console.WriteLine("Please provide 2 strings as arguments.");
            return;
        }
        string numerator = args[0];
        string denominator = args[1];

        // Divide numerator to segments (monomials)
        List<string> numeratorSplit = SplitToMonomials(numerator);
        List<string> denominatorSplit = SplitToMonomials(denominator);

        // Check if some X's lack coefficients, and add 1 in that case
        numeratorSplit = fixCoefficients(numeratorSplit);
        denominatorSplit = fixCoefficients(denominatorSplit);

        // Check if some X's lack powers, and use ^1 in that case
        numeratorSplit = fixPow(numeratorSplit);
        denominatorSplit = fixPow(denominatorSplit);

        // Find the highest power in the lists
        int highestPower = findHighestPower(numeratorSplit);
        int highestPowerDenominator = findHighestPower(denominatorSplit);
        if (highestPowerDenominator > highestPower)
        {
            highestPower = highestPowerDenominator;
        }

        // !!! IMPORTANT !!!
        // THIS IS THE FIRST OUTPUT OF THE BACKEND, WILL BE USED TO PROCESS THE SOLVING STEPS BY FRONTEND
        Console.WriteLine(highestPower);
        Console.WriteLine("");

        // 1. Find the powers which match the highest one exactly
        // 2. Turn those items from "3x^5" into "3"
        // 3. Turn all other items in 0
        string numeratorDiv = finalizeDivision(numeratorSplit, highestPower);
        string denominatorDiv = finalizeDivision(denominatorSplit, highestPower);

        // !!! IMPORTANT !!!
        // THIS IS THE SECOND OUTPUT OF THE BACKEND, WILL BE USED TO PROCESS THE SOLVING STEPS BY FRONTEND
        Console.WriteLine(numeratorDiv);
        Console.WriteLine(denominatorDiv);
        Console.WriteLine("");

        int numeratorResult = result(numeratorDiv);
        int denominatorResult = result(denominatorDiv);
        Console.WriteLine(numeratorResult);
        Console.WriteLine(denominatorResult);
    }
}

