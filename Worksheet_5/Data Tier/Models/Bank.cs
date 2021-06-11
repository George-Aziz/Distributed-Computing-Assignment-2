// Filename: Bank.cs
// Project:  DC Assignment 2 (Practical 4)
// Purpose:  Data Tier Model For Bank DLL Access
// Author:   George Aziz (19765453)
// Date:     06/05/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BankDB;

namespace Data_Tier.Models
{
    public class Bank
    {
        private BankDB.BankDB bankData;
        private static Bank instance = null;
        private TransactionAccessInterface transAccess;
        private UserAccessInterface userAccess;
        private AccountAccessInterface accAccess;

        //Gets current instance of class since it is a singleton
        public static Bank Get()
        {
            if(instance == null)
            {
                instance = new Bank();
            }
            return instance;
        }

        //Creates BankData as well as creates new instances of each interface class
        private Bank()
        {
            bankData = new BankDB.BankDB();
            transAccess = bankData.GetTransactionInterface();
            userAccess = bankData.GetUserAccess();
            accAccess = bankData.GetAccountInterface();
        }

        //Gets BankData Class
        public BankDB.BankDB GetBankData()
        {
            return bankData;
        }

        //Gets Transaction Interface Access
        public TransactionAccessInterface GetTransAccess()
        {
            return transAccess;
        }

        //Gets User Interface Access
        public UserAccessInterface GetUserAccess()
        {
            return userAccess;
        }

        //Gets Account Interface Access
        public AccountAccessInterface GetAccAccess()
        {
            return accAccess;
        }



    }
}