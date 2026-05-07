using System.ComponentModel.DataAnnotations;
namespace dotnet_store.Models;
public class UrunModel
{
    [Required(ErrorMessage = "Ürün adı zorunludur.")]
    [StringLength(50, ErrorMessage = "Ürün adı en fazla 50 karakter olabilir.")]
    [Display(Name = "Ürün Adı")]
    public string UrunAdi { get; set; } = null!;
    
    [Required(ErrorMessage = "Fiyat zorunludur.")]
    [Display(Name = "Fiyat")]
    [Range(0, 100000, ErrorMessage = "Fiyat 0 ile 100000 arasında olmalıdır.")]
    public double? Fiyat { get; set; }
    
    
    [Display(Name = "Ürün Resmi")]
    public IFormFile? Resim { get; set; }

    [Required(ErrorMessage = "Açıklama zorunludur.")]
    [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
    [Display(Name = "Açıklama")]
    public string? Aciklama { get; set; }
    public bool Aktif { get; set; }
    public bool Anasayfa { get; set; }

    [Required(ErrorMessage = "Kategori zorunludur.")]
    [Display(Name = "Kategori")]
    public int? KategoriId { get; set; }

}