using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SohatNotebook.Authentication.Configruation;
using SohatNotebook.DataService.Data;
using SohatNotebook.DataService.IConfiguration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Update the JWt config from settings
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiVersioning(options =>
{
	//Provides to the client the different Api versions that we have
	options.ReportApiVersions = true;

	// this will allow the api to automatically provide a default version
	options.AssumeDefaultVersionWhenUnspecified = true;

	options.DefaultApiVersion = ApiVersion.Default;

});

builder.Services.AddAuthentication(option =>
{
	option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
	option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
	.AddJwtBearer(jwt =>
	{
		var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]);

		jwt.SaveToken = true;
		jwt.TokenValidationParameters = new TokenValidationParameters 
		{ 
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(key),
			ValidateIssuer = false, // ToDo Update
			ValidateAudience = false, // ToDo Update
			RequireExpirationTime = false, // ToDo Update
			ValidateLifetime = true,
		};
	});

builder.Services.AddDefaultIdentity<IdentityUser>(options 
		=> options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
