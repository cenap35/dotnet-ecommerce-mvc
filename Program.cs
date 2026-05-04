using dotnet_store.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

// app.MapStaticAssets(); //wwwroot klasöründeki dosyalara erişim sağlamak için kullanılır. Örneğin wwwroot/img/1.jpeg dosyasına erişmek için bu middleware'i kullanırız. Bu middleware'i kullanmazsak wwwroot klasöründeki dosyalara erişim sağlanamaz.
app.UseStaticFiles(); //mapstaticassets ile aynı işlemi yapar ancak mapstaticassets daha performanslıdır çünkü sadece statik dosyalara erişim sağlar. UseStaticFiles ise tüm istekleri kontrol eder ve statik dosya isteği olup olmadığını kontrol eder. neden kullandım resim upload da uyarı veriordu çalışıordu ama?

// urun/telefon
// urun/bilgisayar ...
app.MapControllerRoute(
    name: "urunler_by_kategori",
    pattern: "urunler/{url?}",
    defaults: new { controller = "Urun", action = "List" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
