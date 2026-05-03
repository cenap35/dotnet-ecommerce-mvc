using dotnet_store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers;

public class UrunController : Controller
{
    // Dependecy Injection => DI
    private readonly DataContext _context;
    public UrunController(DataContext context)
    {
        _context = context;
    }

    public ActionResult Index()
    {
        var urunler = _context.Urunler.Select(i =>  new UrunGetModel
        {
                Id = i.Id,
                UrunAdi = i.UrunAdi,
                Fiyat = i.Fiyat,
                Resim = i.Resim,
                Aktif = i.Aktif,
                Anasayfa = i.Anasayfa,
                KategoriAdi = i.Kategori.KategoriAdi
            }).ToList();
        
        return View(urunler);
    }
    
    public ActionResult List(string url, string q)
    {
        var urunler = _context.Urunler.Where(u => u.Aktif).AsQueryable();

        if (!string.IsNullOrEmpty(url))
        {
            urunler = urunler.Where(u => u.Kategori.Url == url);
        }
         if (!string.IsNullOrEmpty(q))
        {
            urunler = urunler.Where(u => u.UrunAdi.ToLower().Contains(q.ToLower())); 
        }

        return View(urunler.ToList());

    }

    public ActionResult Details(int id)
    {   
        // id'ye göre urun getir
        var urun = _context.Urunler.FirstOrDefault(u => u.Id == id);

        //benzr urunler için kategori id'si aynı olan ve aktif olan ürünleri getir.
        if (urun == null)
        {
            return RedirectToAction("Index", "Home");  // urun bulunamazsa liste sayfasına yönlendir
        }
        ViewBag.BenzerUrunler = _context.Urunler.Where(u => u.Aktif && u.KategoriId == urun.KategoriId && u.Id != id).Take(4).ToList(); //burada urun olmazsa sıkıntı çıkakr ondan yukarıda kontrol yapabiliriz
        
        return View(urun);
    }

    public ActionResult Create()
    {
        // ViewData["Kategoriler"] = _context.Kategoriler.ToList();
        ViewBag.Kategoriler = _context.Kategoriler.ToList(); //ViewBag ile de gönderebiliriz 
        return View();
    
    }

    [HttpPost]
    public ActionResult Create(UrunCreateModel model)
    {

        var urun = new Urun
        {
            UrunAdi = model.UrunAdi,
            Aciklama = model.Aciklama,
            Fiyat = model.Fiyat,
            Aktif = model.Aktif,
            Anasayfa = model.Anasayfa,
            KategoriId = model.KategoriId,
            Resim = "1.jpeg" //upload işlemi yapmadığımız için şimdilik resim kısmını sabit tuttum ama ilerleyen zamanlarda upload işlemi yaparak dinamik hale getirebiliriz
        };
        _context.Urunler.Add(urun);
        _context.SaveChanges();
        
        return RedirectToAction("Index"); //işlem tamamlandıktan sonra index sayfasına yönlendirdim böylece eklediğimiz ürünü görebiliriz
    }
}