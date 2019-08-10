using System;
using System.Collections.Generic;

namespace Church.API.Models
{
    public partial class Contributor
    {
        public Contributor()
        {
            Contribution = new HashSet<Contribution>();
            ContributorLoan = new HashSet<ContributorLoan>();
        }

        public int ContributorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FamilyName { get; set; }
        public sbyte? Status { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateChanged { get; set; }

        public virtual ICollection<Contribution> Contribution { get; set; }
        public virtual ICollection<ContributorLoan> ContributorLoan { get; set; }
    }
}
