using dotnet_store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        var urunler = _context.Urunler.Select(i => new UrunGetModel
        {
            Id = i.Id,
            UrunAdi = i.UrunAdi,
            Fiyat = i.Fiyat,
            Resim = i.Resim,
            Aktif = i.Aktif,
            Anasayfa = i.Anasayfa,
            KategoriAdi = i.Kategori.KategoriAdi
        }).ToList();

        ViewBag.Kategoriler = new SelectList(_context.Kategoriler.ToList(), "Id", "KategoriAdi");

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
        if (model.Resim == null || model.Resim!.Length == 0)
        {
            ModelState.AddModelError("Resim", "Lütfen bir resim dosyası seçin.");
        }

        if (ModelState.IsValid)
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
                Fiyat = model.Fiyat ?? 0, //modelde fiyat nullable olduğu için null gelme ihtimaline karşı 0 verdim
                Aktif = model.Aktif,
                Anasayfa = model.Anasayfa,
                KategoriId = (int)model.KategoriId!,
                Resim = fileName //upload işlemi
            };
            _context.Urunler.Add(urun);
            _context.SaveChanges();
            return RedirectToAction("Index"); //işlem tamamlandıktan sonra index sayfasına yönlendirdim böylece eklediğimiz ürünü görebiliriz
        }

        ViewBag.Kategoriler = _context.Kategoriler.ToList();
        return View(model);

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
    public async Task<ActionResult> Edit(int id, UrunEditModel model)
    {
        if (id != model.Id)
        {
            return RedirectToAction("index");
        }

        var urun = _context.Urunler.FirstOrDefault(u => u.Id == model.Id);
        if (urun == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            model.ResimAdi = urun.Resim;
            ViewBag.Kategoriler = _context.Kategoriler.ToList();
            return View(model);
        }

        if (model.Resim != null && model.Resim.Length > 0) //eğer kullanıcı yeni resim yüklemek isterse
        {
            var fileName = Path.GetRandomFileName() + Path.GetExtension(model.Resim.FileName); //random bir isim oluşturduk çünkü aynı isimde dosya yüklenirse eski dosyanın üzerine yazılabilir 
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName); //wwwroot/img/1.jpeg kaydedilecek dosya yolu
            using (var stream = new FileStream(path, FileMode.Create)) //dosya oluşturma işlemi
            {
                await model.Resim.CopyToAsync(stream); //dosya upload işlemi
            }
            urun.Resim = fileName; //yeni resim yüklenirse eski resim adı yerine yeni resim adı kaydedilir
        }

        urun.UrunAdi = model.UrunAdi;
        urun.Aciklama = model.Aciklama;
        urun.Fiyat = model.Fiyat ?? 0;
        urun.Aktif = model.Aktif;
        urun.Anasayfa = model.Anasayfa;
        urun.KategoriId = (int)model.KategoriId!;

        await _context.SaveChangesAsync();

        TempData["Mesaj"] = $"{urun.UrunAdi} adlı ürün güncellendi.";
        return RedirectToAction("Index");

    }

    //remove işlemi için get ve post olmak üzere iki action oluşturduk çünkü kullanıcı silme işlemi yaparken önce onay sayfasına yönlendirilecek ve burada silmek istediğinden emin olacak eğer onay verirse post action'ı çalışacak ve ürün silinecek
    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var urun = _context.Urunler.FirstOrDefault(u => u.Id == id);
        if (urun != null)
        {
            return View(urun);
        }
        return RedirectToAction("Index");
    }


    [HttpPost]
    public ActionResult DeleteConfirm(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var urun = _context.Urunler.FirstOrDefault(u => u.Id == id);
        if (urun != null)
        {
            _context.Urunler.Remove(urun);
            _context.SaveChanges();

            TempData["Mesaj"] = $"{urun.UrunAdi} adlı ürün silindi.";
        }
        return RedirectToAction("Index");
    }
}
