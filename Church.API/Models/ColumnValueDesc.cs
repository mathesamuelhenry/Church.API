using System;
using System.Collections.Generic;

namespace Church.API.Models
{
    public partial class ColumnValueDesc
    {
        public int ColumnValueDescId { get; set; }
        public int TableColumnId { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public sbyte? Status { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateChanged { get; set; }

        public virtual TableColumn TableColumn { get; set; }
    }
}
