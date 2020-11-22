using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Repository.Entities
{
    [Table("module")]
    public class Module
    {
        [Key]
        [Column("id")]
        public Int64 id { get; set; }

        [Column("type")]
        public int type { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("version")]
        public string version { get; set; }

        [Column("path")]
        public string path { get; set; }

        [Column("enabled")]
        public int enabled { get; set; }
    }
}