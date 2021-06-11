// Filename: Transactions.cs
// Project:  DC Assignment 2 (Practical 8)
// Purpose:  Transactions List tasks
// Author:   George Aziz (19765453)
// Date:     17/05/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain_Classes
{
    public class Transactions
    {
        private static readonly Queue<Transaction> _transactions = new Queue<Transaction>();

        public static Queue<Transaction> GetTransactions()
        {
            return _transactions;
        }

        public static void CreateTransaction(Transaction transaction)
        {
            _transactions.Enqueue(transaction);
        }

        /*public static void FinishTransaction(Transaction transaction)
        {
            foreach(Transaction curTransaction in _transactions)
            {
                if(curTransaction == transaction)
                {
                    curTransaction.Processed = true;
                }
            }
        }*/
        
    }
}
