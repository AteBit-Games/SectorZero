/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;

namespace Runtime.Utils
{
    public static class StringUtils
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly Dictionary<string, string> SplitCaseCache = new(StringComparer.Ordinal);
        
        public static string SplitCamelCase(this string s) {
            if (string.IsNullOrEmpty(s)) return s;
            if (SplitCaseCache.TryGetValue(s, out var result)) return result;

            result = s;
            var underscoreIndex = result.IndexOf('_');
            if (underscoreIndex <= 1) 
            {
                result = result[(underscoreIndex + 1)..];
            }
            result = Regex.Replace(result, "(?<=[a-z])([A-Z])", " $1").CapitalizeFirst().Trim();
            return SplitCaseCache[s] = result;
        }
        
        public static string CapitalizeFirst(this string s) 
        {
            if (string.IsNullOrEmpty(s)) return s; 
            return s.First().ToString().ToUpper() + s[1..];
        }
        
        public static string CapLength(this string s, int max) 
        {
            if (string.IsNullOrEmpty(s) || s.Length <= max || max <= 3) return s; 
            var result = s[..(Mathf.Min(s.Length, max) - 3)];
            result += "...";
            return result;
        }
        
        public static string GetCapitals(this string s) 
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;

            var result = s.Where(char.IsUpper).Aggregate("", (current, c) => current + c.ToString());
            result = result.Trim();
            return result;
        }
        
        public static string FormatError(this string input) 
        {
            return $"<color=#ff6457>* {input} *</color>";
        }
        
        public static string GetAlphabetLetter(int index) 
        {
            if (index < 0) return null;
            return index >= Alphabet.Length ? index.ToString() : Alphabet[index].ToString();
        }
    }
}