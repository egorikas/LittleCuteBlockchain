using System;
using System.Security.Cryptography;
using System.Text;

namespace LittleCuteBlockchain.Core
{
    public class Block
    {
        //First
        public int Index { get; }
        public string Hash { get; }
        public string PreviousHash { get; }
        public DateTime TimeStamp { get; }
        public string Data { get; }

        //Second
        public Block(
            int index,
            string previousHash,
            string data,
            DateTime timeStamp
        )
        {
            Index = index;
            PreviousHash = previousHash;
            TimeStamp = timeStamp;
            Data = data;
            Hash = CalculateHash();
        }

        //Third
        public string CalculateHash()
        {
            var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.Default.GetBytes($"{Index} {PreviousHash} {TimeStamp} {Data}"));
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }
}