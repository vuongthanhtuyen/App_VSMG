using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Models.Contacts
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [Required(ErrorMessage = "Phải nhập {0}")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }


        [StringLength(100)]
        [Required(ErrorMessage = "Phải nhập {0}")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        public DateTime DateSent { get; set; }
        [Phone(ErrorMessage = "Nhập sai định dạng")]
        [StringLength(50)]
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }

        //[Column(TypeName ="nvarchar")]
        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Message { get; set; }

    }
}
