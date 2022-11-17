using Microsoft.AspNetCore.Identity;
using System;

namespace SystemEnterprise.Api.Models.Users
{
    public class ApplicationUserToken : IdentityUserToken<Guid>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
