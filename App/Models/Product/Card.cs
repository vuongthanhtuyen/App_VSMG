using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Product
{
    [Table("Card")]
    public class Card
    {

        [Key]
        public int Id { get; set; }

        // Người mua
        [Display(Name = "Người mua")]
        public string? AuthorId { set; get; }


        [ForeignKey("AuthorId")]
        [Display(Name = "Người mua")]
        public AppUser? Author { set; get; }


        [Display(Name = "Số lượng")]
        [Range(0, int.MaxValue, ErrorMessage = "Nhập giá trị từ {1}")] // nhập số nguyên từ 
        public int quantity { set; get; }


        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public ProductModel Product { get; set; }
    }
}
