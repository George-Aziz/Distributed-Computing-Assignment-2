// Filename: BlockchainController.cs
// Project:  DC Assignment 2 (Practical 7)
// Purpose:  Controller for Blockchain Web API
// Author:   George Aziz (19765453)
// Date:     16/05/2021

using Blockchain_Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blockchain.Controllers
{
    public class BlockchainController : ApiController
    {
        /// <summary>
        /// Retrieves Blockchain
        /// </summary>
        /// <returns>Blockchain list</returns>
        [Route("api/Blockchain/GetChain")]
        [HttpGet]
        public List<Block> GetChain()
        {
            return Models.Blockchain.GetChain();
        }

        /// <summary>
        /// Retrieves Blockchain count
        /// </summary>
        /// <returns>Count of Blockchain list</returns>
        [Route("api/Blockchain/GetCount")]
        [HttpGet]
        public int GetCount()
        {
            return Models.Blockchain.GetChainCount();
        }

        /// <summary>
        /// Adds a new block into blockchain
        /// </summary>
        /// <param name="block">New block to be added</param>
        [Route("api/Blockchain/AddBlock")]
        [HttpPost]
        public void AddBlock(Block block)
        {
            Models.Blockchain.AddBlock(block);
        }

        /// <summary>
        /// Retrieves balance of wallet
        /// </summary>
        /// <param name="walletID">Wallet that will be checked</param>
        /// <returns>Balance of found wallet</returns>
        [Route("api/Blockchain/GetBalance/{walletID}")]
        [HttpGet]
        public float GetBalance(uint walletID)
        {
            return Models.Blockchain.GetBalance(walletID);
        }
    }
}