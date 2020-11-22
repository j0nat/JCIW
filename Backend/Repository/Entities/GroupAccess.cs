using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Repository.Entities
{
    [Table("GroupAccess")]
    public class GroupAccess
    {
        [Key]
        [Column("id")]
        public Int64 id { get; set; }

        [Column("groupid")]
        public Int64 groupid { get; set; }

        [Column("accountId")]
        public Int64 accountId { get; set; }
    }
}