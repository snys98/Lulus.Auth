using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Lulus.Auth.Models
{
    public class Role:IdentityRole
    {
        [Display(Name = "角色描述")]
        [StringLength(50, ErrorMessage = "{0}不能超过50个字符")]
        public string Description { get; set; }
    }
}
