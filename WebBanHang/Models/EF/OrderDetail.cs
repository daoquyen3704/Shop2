using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHang.Models.EF
{
    [Table("tb_orderdetail")]
    public class OrderDetail : CommonAbstract
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]

        //[Column(Order = 0)]
        public int Id { get; set; }
        public int OrderId { get; set; }
        //[Key, Column(Order = 1)]

        public int ProductId { get; set; }

        public decimal Price { get; set; }

        public int Quanlity { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }

    }
}