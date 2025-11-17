using System.ComponentModel.DataAnnotations;

namespace Thuong_Mai_Dien_Tu.ViewModels
{
    public class LoginVM
    {
        [Display(Name ="Ten dang nhap")]
        [Required(ErrorMessage ="Chưa nhập tên đăng nhập")]
        [MaxLength(20,ErrorMessage ="Toi da 20 ki tu")]
        public string UserName { get; set; }

        [Display(Name = "Mat khau")]
        [Required(ErrorMessage = "Chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
