// Filename: Blockchain.cs
// Project:  DC Assignment 2 (Practical 8)
// Purpose:  Blockchain Model with all tasks related
// Author:   George Aziz (19765453)
// Date:     17/05/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain_Classes
{
    public class Blockchain
    {
        private static List<Block> _blockchain = new List<Block>();

        /// <summary>
        /// Ceates genesis block on initialisation of Blockchain
        /// </summary>
        static Blockchain()
        {
            Block initBlock = new Block()
            {
                BlockID = 0,
                WalletIDFrom = 0,
                WalletIDTo = 0,
                Amount = float.PositiveInfinity,
                Offset = 0,
                PrevHash = "",
                Hash = ""
            };

            initBlock = GenerateHash(initBlock);
            _blockchain.Add(initBlock);
        }

        /// <summary>
        /// Retrieves count of the blockchain
        /// </summary>
        /// <returns>Blockchain count</returns>
        public static int GetChainCount()
        {
            return _blockchain.Count;
        }

        /// <summary>
        /// Retrieves the block chain
        /// </summary>
        /// <returns></returns>
        public static List<Block> GetChain()
        {
            return _blockchain;
        }

        public static void UpdateChain(List<Block> newChain)
        {
            _blockchain = newChain;
        }

        /// <summary>
        /// Adds a new block to the block chain
        /// </summary>
        /// <param name="block">The block to be added</param>
        public static void AddBlock(Block block)
        {
            if (ValidateBlock(block))
            {
                _blockchain.Add(block);
            }
        }

        /// <summary>
        /// Checks Wallet ID balance
        /// </summary>
        /// <param name="walletID">ID of wallet to check balance</param>
        /// <returns></returns>
        public static float GetBalance(uint walletID)
        {
            float balance = 0;
            if (walletID == 0) //Genesis Block (Infinite Amount of $$$)
            {
                balance = float.PositiveInfinity;
            }
            else
            {
                foreach (Block block in _blockchain) //Goes through each block adding/deducting balance accordingly
                {
                    if (block.WalletIDFrom == walletID) //If wallet ID has sent money, it will deduct
                    {
                        balance -= block.Amount;
                    }
                    if (block.WalletIDTo == walletID) //If wallet has received money, it will add to balance
                    {
                        balance += block.Amount;
                    }
                }
            }
            return balance;
        }

        /// <summary>
        /// Validates provided block
        /// </summary>
        /// <param name="block">Block to be validated</param>
        private static bool ValidateBlock(Block block)
        {
            bool valid = false;
            //Validation for block includes checking ID is larger than largest ID, checking balance of money from & new block,
            //hash of of previous block is valid and hash of new block starts with '12345'
            if (block.BlockID > GetLargestBlockID() && GetBalance(block.WalletIDFrom) >= block.Amount &&
               block.BlockID >= 0 && block.WalletIDFrom >= 0 && block.WalletIDFrom >= 0 && block.Offset >= 0
               && block.Offset % 5 == 0 && block.Amount >= 0 && block.PrevHash == _blockchain.Last().Hash &&
               block.Hash.StartsWith("12345") && block.Hash == CheckHash(block)
              )
            {
                valid = true;
            }
            return valid;
        }

        /// <summary>
        /// Gets largest Block ID for verification when adding a new block
        /// </summary>
        private static uint GetLargestBlockID()
        {
            uint largestID = 0;

            foreach (Block block in _blockchain)
            {
                if (block.BlockID > largestID)
                {
                    largestID = block.BlockID;
                }
            }
            return largestID;
        }

        /// <summary>
        /// Generates a new hash by brute force. Ensures that the hash starts with 12345, and if not, increases offset by 1
        /// </summary>
        /// <param name="block">The block that will have a new hash</param>
        /// <returns>Block with new hash</returns>
        private static Block GenerateHash(Block block)
        {
            SHA256 sha256 = SHA256.Create();
            Block returnBlock = block;

            while (!returnBlock.Hash.StartsWith("12345"))
            {
                returnBlock.Offset += 5;

                byte[] bytes = Encoding.UTF8.GetBytes(returnBlock.ToHashString());
                byte[] hashArray = sha256.ComputeHash(bytes);
                string newHash = BitConverter.ToUInt64(hashArray, 0).ToString();
                returnBlock.Hash = newHash;
            }

            return returnBlock;
        }

        /// <summary>
        /// Computes hash for block and returns the hash to verify if the hash is correct
        /// </summary>
        /// <param name="block">The block that is being checked</param>
        /// <returns>The computed hash of the block</returns>
        private static string CheckHash(Block block)
        {
            SHA256 sha256 = SHA256.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(block.ToHashString());
            byte[] hashArray = sha256.ComputeHash(bytes);
            string hash = BitConverter.ToUInt64(hashArray, 0).ToString();
            return hash;
        }
    }
}
