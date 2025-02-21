using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// **1. Veritabanı Bağlantısını Yapılandır**
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// **2. Controller Desteğini Ekle**
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Döngüsel referansları engelle ve JSON'u optimize et
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// **3. CORS Politikalarını Tanımla**
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// **4. Swagger (API Dokümantasyonu) Ayarları**
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RPG Game API",
        Version = "v1",
        Description = "RPG oyun API'si ile karakter, oyuncu, görev ve eşya yönetimi",
        Contact = new OpenApiContact
        {
            Name = "Kadir",
            Email = "kadir@example.com",
            Url = new Uri("https://github.com/kadir")
        }
    });

    // Swagger'da JWT Authentication desteği :
    /*
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Lütfen 'Bearer [token]' formatında giriniz",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
    */
});

var app = builder.Build();

// **5. Ortam (Development/Production) Kontrolleri**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RPG Game API v1");
        c.RoutePrefix = string.Empty; // Swagger'ı ana dizinde çalıştır
    });
}

// **6. Middleware (Orta Katman) Ayarları**
app.UseCors("AllowAllOrigins"); // CORS politikalarını uygula
app.UseHttpsRedirection(); // HTTPS yönlendirmesini aktif et
app.UseAuthorization(); // Yetkilendirme Middleware'i aktif et

// **7. API Controller'ları Kullan**
app.MapControllers();

// **8. Uygulamayı Başlat**
app.Run();
