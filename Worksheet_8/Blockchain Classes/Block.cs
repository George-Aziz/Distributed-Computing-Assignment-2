// Filename: Block.cs
// Project:  DC Assignment 2 (Practical 8)
// Purpose:  Block Struct
// Author:   George Aziz (19765453)
// Date:     17/05/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain_Classes
{
    public class Block
    {
        public uint BlockID { get; set; }
        public uint WalletIDFrom { get; set; }
        public uint WalletIDTo { get; set; }
        public float Amount { get; set; }
        public uint Offset { get; set; }
        public string PrevHash { get; set; }
        public string Hash { get; set; }


        public string ToHashString()
        {
            return BlockID.ToString() + WalletIDFrom.ToString() + WalletIDTo.ToString() + Amount.ToString() + Offset.ToString() + PrevHash.ToString();
        }
    }
}
