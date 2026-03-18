using Microsoft.AspNetCore.Identity;

namespace AnketPortal.API.Models
{
    public class AppRole : IdentityRole
    {
        // İleride rollere özel ekstra bir özellik eklemek istersek (örneğin Rol Açıklaması) 
        // buraya özellik (property) ekleyebiliriz. Şimdilik standart IdentityRole özelliklerini miras alıyoruz.
        public string? Description { get; set; }
    }
}