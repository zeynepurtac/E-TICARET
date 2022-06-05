using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace E_Commerce.Areas.Admin.Models
{
    public class User
    {
        public short UserId { get; set; }
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "char(100)")]
        [DataType(DataType.EmailAddress)]
        public string UserEMail { get; set; }

        [Required(ErrorMessage = "Lütfen Şifrenizi Giriniz.")]
        [Column(TypeName = "char(64)")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        [NotMapped]
        [Compare("UserPassword", ErrorMessage = "şifreler uyuşmuyor")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        //----------- Authorization(Yetkilendirme) ---------------------//
        //-------------   Kullanıcı  -------------//
        [Required]
        public bool ViewUsers { get; set; }
        [Required]
        public bool CreateUser { get; set; }
        [Required]
        public bool DeleteUser { get; set; }
        [Required]
        public bool EditUser { get; set; }
        //-------------     Satıcı    -------------//
        [Required]
        public bool ViewSellers { get; set; }
        [Required]
        public bool CreateSeller { get; set; }
        [Required]
        public bool DeleteSeller { get; set; }
        [Required]
        public bool EditSeller { get; set; }
        //-------------    Kategoriler    ------------//
        [Required]
        public bool ViewCategories { get; set; }
        [Required]
        public bool CreateCategory { get; set; }
        [Required]
        public bool DeleteCategory { get; set; }
        [Required]
        public bool EditCategory { get; set; }
        //-------------    Ürün   ------------//
        [Required]
        public bool DeleteProduct { get; set; }
        [Required]
        public bool EditCProduct { get; set; }

    }

}
