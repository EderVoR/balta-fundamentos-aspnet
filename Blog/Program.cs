using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);

ConfigurationAuthentication(builder);
ConfigureMvc(builder);
ConfigurationService(builder);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

ConfigurationEmail(app);

app.UseAuthentication();
app.UseAuthorization();
app.UseResponseCompression();
app.UseStaticFiles();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

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
	builder.Services.AddDbContext<BlogDataContext>(option =>
		option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
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

void ConfigureMvc(WebApplicationBuilder builder)
{
	builder.Services.AddResponseCompression(option =>
	{
		option.Providers.Add<GzipCompressionProvider>();
	});
	builder.Services.Configure<GzipCompressionProviderOptions>(options =>
	{
		options.Level = CompressionLevel.Optimal;
	});

	builder.Services
	.AddControllers()
	.ConfigureApiBehaviorOptions(options =>
	{
		options.SuppressModelStateInvalidFilter = true;
	})
	.AddJsonOptions(x =>
	{
		x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
		x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
	});
}