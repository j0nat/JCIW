using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Repository.Entities
{
    [Table("account")]
    public class Account
    {
        [Key]
        [Column("id")]
        public Int64 id { get; set; }

        [Column("name")]
        public string username { get; set; }

        [Column("password")]
        public string password { get; set; }

        [Column("firstname")]
        public string firstname { get; set; }

        [Column("lastname")]
        public string lastname { get; set; }

        [Column("session")]
        public string session { get; set; }
    }
}