// Filename: Transaction.cs
// Project:  DC Assignment 2 (Practical 8)
// Purpose:  Transaction Struct
// Author:   George Aziz (19765453)
// Date:     17/05/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain_Classes
{
    public class Transaction
    {
        public uint WalletIDFrom;
        public uint WalletIDTo;
        public float Amount;
        public bool Processed;
    }
}
