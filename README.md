i have this permission in program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanWrite", policy => policy.Requirements.Add(new PermissionRequirement("AllowWrite")));
    options.AddPolicy("CanRead", policy => policy.Requirements.Add(new PermissionRequirement("AllowRead")));
    options.AddPolicy("CanModify", policy => policy.Requirements.Add(new PermissionRequirement("AllowModify")));
    options.AddPolicy("CanDelete", policy => policy.Requirements.Add(new PermissionRequirement("AllowDelete")));
});
