using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models
{
    public class AplicationDbContext : MyDbContext
    {
        public DbSet<CatUsuario> Usuarios {get; set;}

    }
}