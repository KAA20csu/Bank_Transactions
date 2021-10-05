using System;
using System.Collections.Generic;
using System.IO;

namespace Ex
{
    class Searcher
    {
        public static Dictionary<Transaction, int> HistoryOfTransactions { get; } = new Dictionary<Transaction, int>();
        static void Main()
        {
            var information = File.ReadAllLines("..\\..\\..\\..\\file.txt");
            Transaction.GetTransactList(information);
            Transaction.CountLastSum();
            Console.WriteLine("Введите дату транзакции в формате yyyy-mm-dd: ");
            string date = Console.ReadLine();
            Console.WriteLine("Введите время транзакции: ");
            string time = Console.ReadLine();
            SearchForTransaction(date, time);
        }
        static void SearchForTransaction(string date, string time)
        {
            foreach(var transaction in HistoryOfTransactions.Keys)
            {
                if(transaction.Date == date)
                {
                    if(transaction.Time == time)
                    {
                        Console.WriteLine(HistoryOfTransactions[transaction]);
                    }
                }
            }
        }
        
    }
    class Transaction
    {
        public static List<Transaction> Transactions { get; private set; } = new List<Transaction>();
        public string Date { get; }
        public string Time { get; }
        public int Sum { get; }
        public string Cmnd { get; }
        public static int CurrentSumAfterTransaction { get; set; }
        public Transaction(string date, string time, int sum, string cmnd)
        {
            Date = date;
            Time = time;
            Sum = sum;
            Cmnd = cmnd;
        }
        public static void GetTransactList(string[] information)
        {
            foreach (var transaction in information)
            {
                if (transaction.Contains("|"))
                    Transactions.Add(ParseTransact(transaction));
                else CurrentSumAfterTransaction = int.Parse(transaction);
            }
        }
        static Transaction ParseTransact(string str)
        {
            var infoAbtTrnsct = str.Split(new char[] { '|', ' '}, StringSplitOptions.RemoveEmptyEntries);
            return new Transaction
            (
                infoAbtTrnsct[0], infoAbtTrnsct[1],

                int.Parse(infoAbtTrnsct.Length == 4 ? infoAbtTrnsct[2] : null),

                infoAbtTrnsct.Length == 4 ? infoAbtTrnsct[3] : infoAbtTrnsct[2]
            );
        }
        public static void CountLastSum()
        {
            foreach(var transaction in Transactions)
            {
                switch (transaction.Cmnd.Trim())
                {
                    case "in":
                        new In().Cmnd(transaction.Sum, transaction);
                        break;
                    case "out":
                        new Out().Cmnd(transaction.Sum, transaction);
                        break;
                }
            }
            
        }

    }
    abstract class Command
    {
        public abstract void Cmnd(int sum, Transaction trnsc);
    }
    class In : Command
    {
        public override void Cmnd(int sum, Transaction trnsc)
        {
            Transaction.CurrentSumAfterTransaction += sum;
            Searcher.HistoryOfTransactions.Add(trnsc, Transaction.CurrentSumAfterTransaction);
        }
    }
    class Out : Command
    {
        public override void Cmnd(int sum, Transaction trnsc)
        {
            Transaction.CurrentSumAfterTransaction -= sum;
            Searcher.HistoryOfTransactions.Add(trnsc, Transaction.CurrentSumAfterTransaction);
        }
    }
    //class Revert : Command
    //{
    //    public override void Cmnd(int sum)
    //    {
    //        Searcher.StartSum += sum;
    //    }
    //}
}