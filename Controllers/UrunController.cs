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
    public async Task<ActionResult> Create(UrunCreateModel model)
    {
        var fileName = Path.GetRandomFileName() + ".jpg"; //random bir isim oluşturduk çünkü aynı isimde dosya yüklenirse eski dosyanın üzerine yazılabilir 
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName); //wwwroot/img/1.jpeg kaydedilecek dosya yolu
        using (var stream = new FileStream(path, FileMode.Create)) //dosya oluşturma işlemi
        {
            await model.Resim!.CopyToAsync(stream); //dosya upload işlemi
        }

        var urun = new Urun
        {
            UrunAdi = model.UrunAdi,
            Aciklama = model.Aciklama,
            Fiyat = model.Fiyat,
            Aktif = model.Aktif,
            Anasayfa = model.Anasayfa,
            KategoriId = model.KategoriId,
            Resim = fileName //upload işlemi
        };
        _context.Urunler.Add(urun);
        _context.SaveChanges();
        
        return RedirectToAction("Index"); //işlem tamamlandıktan sonra index sayfasına yönlendirdim böylece eklediğimiz ürünü görebiliriz
    }

    public ActionResult Edit(int id)
    {
        var urun = _context.Urunler.Select(u => new UrunEditModel
        {
            Id = u.Id,
            UrunAdi = u.UrunAdi,
            Aciklama = u.Aciklama,
            Fiyat = u.Fiyat,
            Aktif = u.Aktif,
            Anasayfa = u.Anasayfa,
            KategoriId = u.KategoriId,
            ResimAdi = u.Resim
        }).FirstOrDefault(u => u.Id == id);

        ViewBag.Kategoriler = _context.Kategoriler.ToList();

        return View(urun);
    }

    [HttpPost]
    public async Task<ActionResult> Edit(int id,UrunEditModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction("index");
        }
        var urun = _context.Urunler.FirstOrDefault(u => u.Id == model.Id);
        if (urun != null)
        {
            if (model.ResimDosyasi != null) //eğer kullanıcı yeni resim yüklemek isterse
            {
                var fileName = Path.GetRandomFileName() + Path.GetExtension(model.ResimDosyasi.FileName); //random bir isim oluşturduk çünkü aynı isimde dosya yüklenirse eski dosyanın üzerine yazılabilir 
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName); //wwwroot/img/1.jpeg kaydedilecek dosya yolu
                using (var stream = new FileStream(path, FileMode.Create)) //dosya oluşturma işlemi
                {
                    await model.ResimDosyasi.CopyToAsync(stream); //dosya upload işlemi
                }
                urun.Resim = fileName; //yeni resim yüklenirse eski resim adı yerine yeni resim adı kaydedilir
            }

            urun.UrunAdi = model.UrunAdi;
            urun.Aciklama = model.Aciklama;
            urun.Fiyat = model.Fiyat;
            urun.Aktif = model.Aktif;
            urun.Anasayfa = model.Anasayfa;
            urun.KategoriId = model.KategoriId;
            
            _context.SaveChanges();

            TempData["Mesaj"] = $"{urun.UrunAdi} adlı ürün güncellendi.";
            return RedirectToAction("Index");
        }
        
        return View(model);

    }

}