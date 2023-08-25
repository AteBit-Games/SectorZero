/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using Runtime.BehaviourTree;
using UnityEngine;

namespace Runtime.Utils
{
    public static class OperationUtils
    {
        public static bool Compare(float a, float b, CompareMethod cm, float floatingPoint) {
            return cm switch
            {
                CompareMethod.EqualTo => Mathf.Abs(a - b) <= floatingPoint,
                CompareMethod.GreaterThan => a > b,
                CompareMethod.LessThan => a < b,
                CompareMethod.GreaterOrEqualTo => a >= b,
                CompareMethod.LessOrEqualTo => a <= b,
                _ => true
            };
        }

        public static bool Compare(int a, int b, CompareMethod cm) {
            return cm switch
            {
                CompareMethod.EqualTo => a == b,
                CompareMethod.GreaterThan => a > b,
                CompareMethod.LessThan => a < b,
                CompareMethod.GreaterOrEqualTo => a >= b,
                CompareMethod.LessOrEqualTo => a <= b,
                _ => true
            };
        }
        
        public static IEnumerator Delay(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }

    public static class EncryptionUtils
    {
        private static readonly byte [] ivBytes = new byte [16];
        private static readonly byte [] keyBytes = new byte [16];

        private static void GenerateIVBytes()
        {
            var rnd = new System.Random();
            rnd.NextBytes(ivBytes);
        }

        private const string NameOfGame = "SctrZer0";

        private static void GenerateKeyBytes()
        {
            var sum = 0;
            foreach (var curChar in NameOfGame) sum += curChar;
            var rnd = new System.Random(sum);
            rnd.NextBytes( keyBytes );
        }
     
        public static string EncryptAES( string data )
        {
            GenerateIVBytes();
            GenerateKeyBytes();
     
            SymmetricAlgorithm algorithm = Aes.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor( keyBytes, ivBytes );
            byte[] inputBuffer = Encoding.Unicode.GetBytes(data);
            byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
     
            string ivString = Encoding.Unicode.GetString(ivBytes);
            string encryptedString = Convert.ToBase64String(outputBuffer);
            return ivString + encryptedString;
        }
     
        public static string DecryptAES(string text)
        {
            GenerateIVBytes();
            GenerateKeyBytes();
     
            int endOfIVBytes = ivBytes.Length / 2;
            string ivString = text.Substring(0, endOfIVBytes);
            byte[] extractedivBytes = Encoding.Unicode.GetBytes(ivString);
            string encryptedString = text.Substring(endOfIVBytes);
     
            SymmetricAlgorithm algorithm = Aes.Create();
            ICryptoTransform transform = algorithm.CreateDecryptor(keyBytes, extractedivBytes);
            byte[] inputBuffer = Convert.FromBase64String( encryptedString );
            byte[] outputBuffer = transform.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
     
            string decryptedString = Encoding.Unicode.GetString(outputBuffer);
            return decryptedString;
        }
    }
}