using System;
using System.Collections.Generic;

namespace Church.API.Models
{
    public partial class Contribution
    {
        public int ContributionId { get; set; }
        public int? AccountId { get; set; }
        public int? ContributorId { get; set; }
        public string ContributionName { get; set; }
        public string Category { get; set; }
        public int? TransactionType { get; set; }
        public int TransactionMode { get; set; }
        public decimal Amount { get; set; }
        public string CheckNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string Note { get; set; }
        public sbyte? Status { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateChanged { get; set; }

        public virtual Contributor Contributor { get; set; }
    }
}
