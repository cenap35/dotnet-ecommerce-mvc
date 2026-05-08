namespace dotnet_store.Controllers;

using dotnet_store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class KategoriController : Controller
{
    private readonly DataContext _context;
    public KategoriController(DataContext context)
    {
        _context = context;
    }


    //localhot:/Kategori
    public ActionResult Index()
    {
        var kategoriler = _context.Kategoriler.Select(k => new KategoriGetModel
        {
            Id = k.Id,
            KategoriAdi = k.KategoriAdi,
            Url = k.Url,
            UrunSayisi = k.Uruns.Count
        }).ToList();
        return View(kategoriler);
    }

    public ActionResult Create()
    {
        return View(); //boş bir model döndürüyoruz böylece formu doldurmak için kullanabiliriz
    }

    [HttpPost]
    public ActionResult Create(KategoriCreateModel model)
    {   
        if (ModelState.IsValid)
        {
            var kategori = new Kategori
        {
            KategoriAdi = model.KategoriAdi,
            Url = model.Url
        };
        _context.Kategoriler.Add(kategori);
        _context.SaveChanges();
        
        return RedirectToAction("Index"); //işlem tamamlandıktan sonra index sayfasına yönlendirdim böylece eklediğimiz kategoriyi görebiliriz
        }
        return View(model); //model geçerli değilse tekrar formu doldurmak için aynı modeli döndürüyoruz
    }

    public ActionResult Edit(int id)
    {
        var kategori = _context.Kategoriler.Select(k => new KategoriEditModel
        {
            Id = k.Id,
            KategoriAdi = k.KategoriAdi,
            Url = k.Url
        }).FirstOrDefault(k => k.Id == id);
        if (kategori == null)
        {
            return RedirectToAction("Index");
        }
        return View(kategori);
    }

    [HttpPost]
    public ActionResult Edit(int Id, KategoriEditModel model)
    {
        if (Id != model.Id)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
        var kategori = _context.Kategoriler.FirstOrDefault(k => k.Id == Id);
        if (kategori!= null)
        {
            kategori.KategoriAdi = model.KategoriAdi;
            kategori.Url = model.Url;
            _context.SaveChanges(); //update kaydediyoruz

            TempData["Mesaj"] = $"{kategori.KategoriAdi} adlı kategori güncellendi.";

            return RedirectToAction("Index");
        }
    }
        return View(model);
    }
    
    // Remove işlemi için iki action oluşturuyoruz biri silme onayı göstermek için diğeri ise silme işlemini gerçekleştirmek için
    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return RedirectToAction("Index");
        }
        var kategori = _context.Kategoriler.FirstOrDefault(k => k.Id == id);
        if (kategori != null)
        {
            return View(kategori);
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
        var kategori = _context.Kategoriler.FirstOrDefault(k => k.Id == id);
        if (kategori != null)
        {
            _context.Kategoriler.Remove(kategori);
            _context.SaveChanges();

            TempData["Mesaj"] = $"{kategori.KategoriAdi} adlı kategori silindi.";
        }
        return RedirectToAction("Index");
    }
}