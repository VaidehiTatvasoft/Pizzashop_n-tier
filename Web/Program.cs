using System.Text;
using Entity.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddProjectServices(builder.Configuration);

// Configure Database Connection
var conn = builder.Configuration.GetConnectionString("pizza_shopConnection");
builder.Services.AddDbContext<PizzaShopContext>(options => options.UseNpgsql(conn));

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var jwtKey = builder.Configuration["Jwt:key"];
if(string.IsNullOrEmpty(jwtKey)){
    throw new ArgumentNullException("Jwt:Key","Jwt Key is missing in appsettings.json");
}
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("AuthToken")) 
                {
                    context.Token = context.Request.Cookies["AuthToken"];
                }
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStatusCodePages(async context =>
   {
       if (context.HttpContext.Response.StatusCode == 404)
       {
           context.HttpContext.Response.Redirect("/Home/NotFound");
       }
       else if (context.HttpContext.Response.StatusCode == 401)
       {
           context.HttpContext.Response.Redirect("/Home/Unauthorized");
       }
   });
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); 


app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Accounts}/{action=Login}/{id?}");

app.Run();