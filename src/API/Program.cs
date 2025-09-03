using System.Globalization;
using API.SwaggerConfig;
using TQM.Backoffice.Application;
using TQM.Backoffice.Application.DTOs.Mail;
using TQM.BackOffice.Identity;
using TQM.BackOffice.Persistence;
using TQM.BackOffice.RESTApi;

var builder = WebApplication.CreateBuilder(args);

// Using the GetValue<type>(string key) method
//var configValue = builder.Configuration.GetValue<string>("Authentication:CookieAuthentication:LoginPath");

// or using the index property (which always returns a string)
//var configValue = builder.Configuration["Authentication:CookieAuthentication:LoginPath"];
var configValue = builder.Configuration["Env"];
var mailSection = builder.Configuration.GetSection("MailSettings");

// var cultureInfo = new CultureInfo("en-US");

// CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
// CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.ConfigureApplicationServices();
builder.Services.ConfigurePersistenceServices();
builder.Services.Configure<MailDTOs.MailSettings>(mailSection);


builder.Services.ConfigureIdentityServices(builder.Configuration);
builder.Services.ConfigureRESTApiServices();


builder.Services.AddCors(o =>
    {
        o.AddDefaultPolicy(builder => builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwaggerServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

if (configValue.ToUpper().Trim() != "PRO")
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.DefaultModelsExpandDepth(-1); c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); });
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
