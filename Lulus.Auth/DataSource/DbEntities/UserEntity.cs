using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace Lulus.Auth.DataSource.DbEntities
{
    public class UserEntity: IdentityUser
    {
        public string NickName { get; set; } = String.Empty;
        public string Signature { get; set; }
        public GenderEnum Gender { get; set; }
        public DateTime BirthDay { get; set; }
        /// <summary>
        /// 所在地信息,省|市|区|详细
        /// </summary>
        public string Address { get; set; } = String.Empty;
        public string Avatar { get; set; }
    }
    /// <summary>
    /// 性别枚举
    /// </summary>
    public enum GenderEnum
    {
        Boy = 0,
        Girl = 1,
        Secret = 2,
    }
}
