using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringUtilities 
{
    public static string[] SplitString(string input, string splitString, bool canHaveMultipleSplitString = true)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new string[] { };
        }
        if (string.IsNullOrEmpty(splitString))
        {
            return new string[] { input };
        }
        if (canHaveMultipleSplitString)
        {
            return input.Split(splitString, StringSplitOptions.None);
        }
        else
        {
            var splitedString = input.Split(splitString, StringSplitOptions.None);
            if (splitedString.Length > 1)
            {
                var text = splitedString[0];
                for (int i = 1; i < splitedString.Length-2; i++)
                {
                    text += splitString + splitedString[i];
                }
                return new [] { text, splitedString[^1] };
            }
            return splitedString;
        }
    }
}
