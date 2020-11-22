using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Repository.Entities
{
    [Table("Group")]
    public class Group
    {
        [Key]
        [Column("id")]
        public Int64 id { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("description")]
        public string description { get; set; }
    }
}