using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Chords_site.Services;
using System.Text;
using Chords_site.Data;
using Chords_site.Data;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;




var builder = WebApplication.CreateBuilder(args);

Batteries.Init();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.
builder.Services.AddControllers();

// ��������� JWT ��������������
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKey")), // ��� ��������� ����
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});



// ���������� ������ ������� JWT
builder.Services.AddSingleton<JwtService>(new JwtService("YourSecretKey", "YourRefreshSecretKey", 15, 7));

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication(); // �������� ��������������
app.UseAuthorization();  // �������� �����������

app.MapControllers();

app.Run();