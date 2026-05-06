using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;

public class KategoriEditModel
{
    public int Id { get; set; }

    [Required] //xorunlu alan
    [StringLength(30)] //makssimum 100 karakter olabilir
    [Display(Name = "Kategori Adı")]
    public string KategoriAdi { get; set; } = null!;
    
    [StringLength(30)]
    [Required]
    [Display(Name = "URL")]
    public string Url { get; set; } = null!;
}