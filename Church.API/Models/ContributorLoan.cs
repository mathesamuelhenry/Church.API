using System;
using System.Collections.Generic;

namespace Church.API.Models
{
    public partial class ContributorLoan
    {
        public int ContributorLoanId { get; set; }
        public int ContributorId { get; set; }
        public decimal LoanAmount { get; set; }
        public sbyte? Status { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateChanged { get; set; }

        public virtual Contributor Contributor { get; set; }
    }
}
