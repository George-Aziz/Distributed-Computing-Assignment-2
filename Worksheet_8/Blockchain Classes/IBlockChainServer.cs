// Filename: IBlockChainServer.cs
// Project:  DC Assignment 2 (Practical 8)
// Purpose:  Blockchain server implementation
// Author:   George Aziz (19765453)
// Date:     16/05/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Blockchain_Classes
{
    [ServiceContract]
    public interface IBlockChainServer
    {
        [OperationContract]
        List<Block> GetChain();

        [OperationContract]
        int GetCount();

        [OperationContract]
        void AddBlock(Block block);

        [OperationContract]
        float GetBalance(uint walletID);

        [OperationContract]
        void ReceiveNewTransaction(Transaction transaction);

        [OperationContract]
        void UpdateChain(List<Block> newChain);
    }
}
