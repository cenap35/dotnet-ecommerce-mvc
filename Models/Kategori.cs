namespace dotnet_store.Models;

//Entity class/ veritabanında Kategoriler tablosuna karşılık gelecek sınıf
public class Kategori
{
    public int Id { get; set; }
    public string KategoriAdi { get; set; } = null!;
    public string Url { get; set; } = null!;

    public List<Urun> Uruns { get; set; } = new(); //navigation property/  burada Kategori ile Urun arasında bir ilişki kuruyoruz. Kategori birden fazla urun içerebilir birden çoka ilişki kuruyoruz. isimlendirme kuralı: {EntityName}s 
}