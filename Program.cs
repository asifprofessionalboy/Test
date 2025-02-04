using Microsoft.AspNetCore.Authentication;
using SmeterReceiver.Classes.DBModel;
using SmeterReceiver.DataAccess;
using SmeterReceiver.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ISmeterDataAccess, SmeterDataAccess>();
builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthMiddleware>("BasicAuthentication", null);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
//app.UseMiddleware<SharedSecretAuthorizationMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
