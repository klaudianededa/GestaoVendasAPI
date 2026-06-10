using Microsoft.EntityFrameworkCore;
using GestaoVendasAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews() //Além de processar dados, prepare-se para renderizar arquivos de interface (Views)
    .AddJsonOptions(options =>
    {
        // Ensina o tradutor de JSON a ignorar loops infinitos de relacionamentos
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Busca a string de conexão que você configurou no appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Definimos a versão do MySQL manualmente (substituindo o AutoDetect)
// Se você instalou o MySQL recentemente, provavelmente é a versão 8.0 ou 8.x
var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

// 3. Injeta o AppDbContext usando a versão fixa
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles(); //Pode liberar o envio de arquivos de design (CSS), scripts (JS) e imagens para o navegador". Sem isso, o Bootstrap 5 nunca vai carregar.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
