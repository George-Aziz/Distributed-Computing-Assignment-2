// Filename: BlockchainServerImplementation.cs
// Project:  DC Assignment 2 (Practical 8)
// Purpose:  Blockchain server implementation
// Author:   George Aziz (19765453)
// Date:     16/05/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain_Classes
{
    public class BlockchainServerImplementation : IBlockChainServer
    {
        public void AddBlock(Block block)
        {
            Blockchain.AddBlock(block);
        }

        public float GetBalance(uint walletID)
        {
            return Blockchain.GetBalance(walletID);
        }

        public List<Block> GetChain()
        {
            return Blockchain.GetChain();
        }

        public int GetCount()
        {
            return Blockchain.GetChainCount();
        }

        public void ReceiveNewTransaction(Transaction transaction)
        {
            Transactions.CreateTransaction(transaction);
        }

        public void UpdateChain(List<Block> newChain)
        {
            Blockchain.UpdateChain(newChain);
        }
    }
}
