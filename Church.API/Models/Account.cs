using System;
using System.Collections.Generic;

namespace Church.API.Models
{
    public partial class Account
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BankName { get; set; }
        public DateTime? AccountEndDate { get; set; }
        public decimal InitialBalance { get; set; }
        public sbyte? Status { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateChanged { get; set; }
    }
}
