using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Repository.Entities
{
    [Table("ServiceLog")]
    public class ServiceLog
    {
        [Key]
        [Column("id")]
        public Int64 id { get; set; }

        [Column("serviceId")]
        public Int64 serviceId { get; set; }

        [Column("text")]
        public string text { get; set; }

        [Column("date")]
        public long date { get; set; }
    }
}