using System.ComponentModel.DataAnnotations;

namespace duandian_test.Models
{
    public class EditUserViewModel
    {
        public EditUserViewModel() { }

        // Allow Initialization with an instance of ApplicationUser:
        public EditUserViewModel(ApplicationUser user)
        {
            this.UserName = user.UserName;           
            this.role = user.role;
            this.keshi = user.keshi;            
        }

        [Required]
        [Display(Name = "使用者账号")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "科室")]
        public string keshi { get; set; }

        [Required]
        [Display(Name = "角色")]
        public string role { get; set; }

    }




    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        //[Required]
        //[EmailAddress]
        //[Display(Name = "电子邮件")]
        //public string Email { get; set; }

        [Required(ErrorMessage = "用户名不能为空！")]
        [Display(Name = "用户名")]
        public string name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    //public class RegisterViewModel
    //{
    //    //[Required]
    //    //[EmailAddress]
    //    //[Display(Name = "电子邮件")]
    //    //public string Email { get; set; }

    //    [Required]
    //    [Display(Name = "使用者账号")]
    //    public string UserName { get; set; }

    //    [Required]
    //    [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "密码")]
    //    public string Password { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "确认密码")]
    //    [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
    //    public string ConfirmPassword { get; set; }
    //}




    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "使用者账号")]
        public string UserName { get; set; }

        //[Required]
        //[StringLength(100, ErrorMessage = "{0} 的长度至少必须为{2} 个字符。", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        //[Display(Name = "密码")]
        //public string Password { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "确认密码")]
        //[Compare("Password", ErrorMessage = "密码和确认密码不相符。")]
        //public string ConfirmPassword { get; set; }

        //[Required]
        //[Display(Name = "姓")]
        //public string FirstName { get; set; }

        //[Required]
        //[Display(Name = "名")]
        //public string LastName { get; set; }


        [Required]
        [Display(Name = "科室")]
        public string keshi { get; set; }


        [Required]
        [Display(Name = "角色")]
        public string role { get; set; }


        //[Required]
        //[Display(Name = "电子邮件箱")]
        //public string Email { get; set; }

        public ApplicationUser GetUser()
        {
            var user = new ApplicationUser()
            {
                UserName = this.UserName,
                //FirstName = this.FirstName,
                //LastName = this.LastName,
                //Email = this.Email,
                keshi = this.keshi,
                role = this.role,

            };
            return user;
        }
    }




    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }
    }
}
