using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models
{
    public class UserUpdateViewModel
    {
        [Display(Name ="Email: ")]
        [Required(ErrorMessage = "Email alanı gereklidir")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir email adresi giriniz")]
        public string Email { get; set; }
        [Display(Name = "Telefon: ")]
        public string PhoneNumber { get; set; }
        public string PictureUrl { get; set; }
        [Display(Name = "İsim: ")]
        [Required(ErrorMessage = "İsim alanı gereklidir")]
        public string Name { get; set; }
        [Display(Name = "Soyisim: ")]
        [Required(ErrorMessage = "Soyisim alanı gereklidir")]
        public string SurName { get; set; }
        public IFormFile Picture { get; set; }


    }
}
