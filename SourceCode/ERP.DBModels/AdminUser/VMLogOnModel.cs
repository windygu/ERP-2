using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models.AdminUser
{
    public class VMLogOnModel
    {
        [Required(ErrorMessage = "用户名必填")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密码必填")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        // TODO: 开发阶段不验证
        //[Required(ErrorMessage = "验证码必填")]
        [Display(Name = "验证码")]
        public string ValidateCode { get; set; }

        [Display(Name = "保存账号信息")]
        public bool RememberMe { get; set; }
    }
}
