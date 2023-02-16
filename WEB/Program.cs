using RabbitMQ.Client;
using BLL.QueueServices;
using DAL.Context;
using Microsoft.AspNetCore.Identity;
using WEB.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddSingleton<ConnectionFactory>(x => new ConnectionFactory { 
	Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")),
});
builder.Services.AddTransient<RabbitMQClientService>();
builder.Services.AddTransient<RabbitMQClientPublisher>();
builder.Services.AddTransient<RabbitMQClientSubscriber>();

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddIdentity<AppUser, AppRole>(actions => {
		// password
		actions.Password.RequiredLength = 8;
		actions.Password.RequireDigit = true;
		actions.Password.RequireUppercase = true;
		actions.Password.RequireLowercase = true;
		actions.Password.RequireNonAlphanumeric = false;
		actions.Password.RequiredUniqueChars = 0;

		// user
		actions.User.RequireUniqueEmail = true;

		// lockout
		actions.Lockout.MaxFailedAccessAttempts = 5;
		actions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
	}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(config =>
{
	// cookie
	CookieBuilder cookie = new CookieBuilder();
	cookie.Name = "AppAuth";
	cookie.MaxAge = TimeSpan.FromHours(12);
	cookie.HttpOnly = true;
	cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
	cookie.SameSite = SameSiteMode.Lax;

	// config
	config.Cookie = cookie;
	config.AccessDeniedPath = "/User/Login";
	config.LoginPath = "/User/Login";
	config.SlidingExpiration = true;
	config.ExpireTimeSpan = TimeSpan.FromHours(12);
});

builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapAreaControllerRoute("Areas", "Service", "/service/{controller}/{action=Index}");
app.MapHub<ExcelHub>("/ExcelHub");

app.Run();
