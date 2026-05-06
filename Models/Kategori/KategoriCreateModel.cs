using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class KategoriCreateModel
{
    [Required] //xorunlu alan
    [StringLength(100)] //makssimum 100 karakter olabilir
    [Display(Name = "Kategori Adı")]
    public string KategoriAdi { get; set; } = null!;

    
    [Display(Name = "URL")]
    [StringLength(30)]
    [Required]
    public string Url { get; set; } = null!;
}