using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_ImportOrder")]
    public class ImportOrder: CommonAbstract
    {

        public ImportOrder()
        {
            this.ImportOrderDetail = new HashSet<ImportOrderDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Type { get; set; }// kiểu là phân biệt nhập hay xuất hàng : 1 là nhập, 0 là xuất
        public string Note { get; set; } // Ghi chú
        public int TotalProducts { get; set; } // Tổng số hàng

        public decimal TotalAmount { get; set; } // Tổng số tiền
        public virtual ICollection<ImportOrderDetail> ImportOrderDetail { get; set; }
    }
}