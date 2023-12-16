using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_ImportOrderDetail")]
    public class ImportOrderDetail
    {
        public ImportOrderDetail()
        {
            // Khởi tạo thuộc tính có sẵn nếu cần
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ProductName { get; set; } // Tên sản phẩm

        public int ProductAttributeId { get; set; } // // Khóa ngoại liên kết chi tiết sản phẩm

        

        public int Quantity { get; set; } // Số lượng

        public decimal TotalPrice { get; set; } // Thành tiền
        public decimal ImportPrice { get; set; } // Thành tiền

        public int ImportOrderId { get; set; } // Khóa ngoại liên kết với phiếu nhập
        public virtual ImportOrder ImportOrder { get; set; }
        public virtual ProductAttributes ProductAttributes { get; set; }
    }
}