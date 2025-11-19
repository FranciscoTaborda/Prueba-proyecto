// --- CÓDIGO CORREGIDO ---

using GymTrainerGuide.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymTrainerGuide.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<string>, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Objetivo> Objetivos { get; set; }
        public DbSet<NivelExperiencia> NivelExperiencia { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Ejercicio> Ejercicios { get; set; }
        public DbSet<Rutina> Rutinas { get; set; }
        public DbSet<RutinaEjercicio> RutinaEjercicios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rutina>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rutinas)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ejercicio>()
                .HasOne(e => e.Equipo)
                .WithMany(eq => eq.Ejercicios)
                .HasForeignKey(e => e.EquipoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
