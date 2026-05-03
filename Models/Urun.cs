namespace dotnet_store.Models;
public class Urun
{
    public int Id { get; set; }
    public string UrunAdi { get; set; } = null!;
    public double Fiyat { get; set; }
    public string? Resim { get; set; }
    public string? Aciklama { get; set; }
    public bool Aktif { get; set; }
    public bool Anasayfa { get; set; }

    public int KategoriId { get; set; } // foreign key kategori ile ilişki kuruyoruz. bu urun hangi kategoriye ait? isimlendirme kuralı: {EntityName}Id
    public Kategori Kategori { get; set; } = null!; // navigation property

}
