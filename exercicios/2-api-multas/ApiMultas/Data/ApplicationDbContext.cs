using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiMultas.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Agentes> Agentes { get; set; }

        public DbSet<Condutores> Condutores { get; set; }

        public DbSet<Multas> Multas { get; set; }

        public DbSet<Viaturas> Viaturas { get; set; }
    }
}
