using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Chords_site.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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