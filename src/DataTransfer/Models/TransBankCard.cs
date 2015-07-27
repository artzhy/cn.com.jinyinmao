namespace DataTransfer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TransBankCard")]
    public partial class TransBankCard
    {
        public int? AddingTime { get; set; }

        public int? Args { get; set; }

        [StringLength(25)]
        public string BankCardNo { get; set; }

        [StringLength(20)]
        public string BankName { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string Cellphone { get; set; }

        [StringLength(20)]
        public string CityName { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Dispaly { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Verified { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VerifiedByYilian { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? VerifiedTime { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int WithdrawAmount { get; set; }
    }
}
