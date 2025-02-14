using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

ConfigurationAuthentication(builder);

builder.Services
	.AddControllers()
	.ConfigureApiBehaviorOptions(options =>
	{
		options.SuppressModelStateInvalidFilter = true;
	});

ConfigurationService(builder);

var app = builder.Build();

ConfigurationEmail(app);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

void ConfigurationAuthentication(WebApplicationBuilder builder)
{
	builder.Services.AddAuthentication(x =>
	{
		x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	}).AddJwtBearer(x =>
	{
		x.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(key),
			ValidateIssuer = false,
			ValidateAudience = false
		};
	});
}

void ConfigurationService(WebApplicationBuilder builder)
{
	builder.Services.AddDbContext<BlogDataContext>();
	builder.Services.AddTransient<TokenService>();
	builder.Services.AddTransient<EmailService>();
}

void ConfigurationEmail(WebApplication app)
{
	Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");
	Configuration.ApiKey = app.Configuration.GetValue<string>("ApiKey");
	Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");

	var smtp = new Configuration.SmtpConfiguration();
	app.Configuration.GetSection("Smtp").Bind(smtp);
	Configuration.Smtp = smtp;
}