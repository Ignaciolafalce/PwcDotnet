﻿using Microsoft.AspNetCore.Identity;

namespace PwcDotnet.Application.Common.Auth.IdentityEntities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
