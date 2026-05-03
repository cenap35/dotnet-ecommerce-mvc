using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using dotnet_store.Models;

namespace dotnet_store.Controllers;

public class HomeController : Controller
{
   // Dependecy Injection => DI
    private readonly DataContext _context;
    public HomeController(DataContext context)
    {
        _context = context;
    }

    public ActionResult Index()
    {
        // anasayfa true ve aktif olan urunleri getir
        var urunler = _context.Urunler.Where(u => u.Anasayfa == true && u.Aktif == true).ToList();
        ViewData["Kategoriler"] = _context.Kategoriler.ToList();
        return View(urunler);
    
    }
}
