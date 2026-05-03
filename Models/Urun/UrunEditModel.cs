namespace dotnet_store.Models;
public class UrunEditModel
{
    public int Id { get; set; }
    public string UrunAdi { get; set; } = null!;
    public double Fiyat { get; set; }
    public string? ResimAdi { get; set; }
    public IFormFile? ResimDosyasi { get; set; } //edit işlemi yaparken yeni resim yüklemek isteyebilir kullanıcı bu yüzden IFormFile tipinde bir property ekliyoruz. eğer kullanıcı yeni resim yüklemek istemezse Resim null olacaktır ve eski resim adıyla devam edeceğiz.
    public string? Aciklama { get; set; }
    public bool Aktif { get; set; }
    public bool Anasayfa { get; set; }

    public int KategoriId { get; set; } // foreign key kategori ile ilişki kuruyoruz. bu urun hangi kategoriye ait? isimlendirme kuralı: {EntityName}Id


}
