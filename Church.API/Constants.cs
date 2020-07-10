using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Church.API
{
    public class Constants
    {
        public enum TransactionType
        {
            Credit = 1,
            Debit = 2
        }

        public enum TransactionMode
        {
            Cash = 1,
            Check = 2,
            Online = 3,
            CreditDebitCard = 4
        }
    }
}
