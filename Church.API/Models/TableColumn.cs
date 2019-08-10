using System;
using System.Collections.Generic;

namespace Church.API.Models
{
    public partial class TableColumn
    {
        public TableColumn()
        {
            ColumnValueDesc = new HashSet<ColumnValueDesc>();
        }

        public int TableColumnId { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public sbyte? Status { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateChanged { get; set; }

        public virtual ICollection<ColumnValueDesc> ColumnValueDesc { get; set; }
    }
}
