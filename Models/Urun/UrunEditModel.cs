using System.ComponentModel.DataAnnotations;

namespace dotnet_store.Models;
public class UrunEditModel : UrunModel //inheritance => kalıtım
{
    public int Id { get; set; }
    public string? ResimAdi { get; set; }

}
