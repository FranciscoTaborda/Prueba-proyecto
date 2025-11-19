using GymTrainerGuide.Api.Helpers;
using GymTrainerGuide.Shared.Entities;
using GymTrainerGuide.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymTrainerGuide.Api.Data
{
    public class SeedDb
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(ApplicationDbContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedDbAsync()
        {

            await _context.Database.MigrateAsync();


            await CheckNivelExperienciaAsync();
            await CheckObjetivoAsync();
            await CheckEquipoAsync();
            await CheckEjercicioAsync();


            await CheckRolesAsync();
            await CheckUserAsync(
                nombre: "Francisco Taborda",
                edad: 25,
                genero: Genero.Masculino,
                correo: "Francisco@gmail.com",
                tipoUsuario: TipoUsuario.Usuario,
                nivelExperienciaId: 2,
                objetivoId: 1,
                password: "Contraseña123"
            );

            await CheckUserAsync(
                nombre: "Entrenador",
                edad: 35,
                genero: Genero.Masculino,
                correo: "Entrenador@gmail.com",
                tipoUsuario: TipoUsuario.Entrenador,
                nivelExperienciaId: 3,
                objetivoId: 2,
                password: "Contraseña123"
            );

            await CheckUserAsync(
                nombre: "Admin",
                edad: 40,
                genero: Genero.Femenino,
                correo: "Admin@gmail.com",
                tipoUsuario: TipoUsuario.Admin,
                nivelExperienciaId: 1,
                objetivoId: 3,
                password: "Contraseña123" 
            );

            await CheckRutinaAsync();
        }

        private async Task CheckRolesAsync()
        {

            await _userHelper.CheckRoleAsync(TipoUsuario.Usuario.ToString());
            await _userHelper.CheckRoleAsync(TipoUsuario.Entrenador.ToString());
            await _userHelper.CheckRoleAsync(TipoUsuario.Admin.ToString());
        }

        private async Task<User> CheckUserAsync(string nombre, int edad, Genero genero, string correo, TipoUsuario tipoUsuario, int nivelExperienciaId, int objetivoId, string password)
        {

            var user = await _userHelper.GetUserAsync(correo);
            if (user == null)
            {
                // ...
                user = new User
                {
                    Nombre = nombre,
                    Edad = edad,
                    Genero = genero,
                    Correo = correo,
                    UserName = correo,
                    Email = correo,
                    TipoUsuario = tipoUsuario,
                    NivelExperienciaId = nivelExperienciaId,
                    ObjetivoId = objetivoId,
                };
                // ...


                var result = await _userHelper.AddUserAsync(user, password);
                if (result.Succeeded)
                {
                    await _userHelper.AddUserToRoleAsync(user, tipoUsuario.ToString());
                }
                else
                {

                    Console.WriteLine($"Error al crear el usuario {correo}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            return user;
        }

        private async Task CheckNivelExperienciaAsync()
        {
            if (!_context.NivelExperiencia.Any())
            {
                _context.NivelExperiencia.AddRange(new List<NivelExperiencia>
                {
                    new NivelExperiencia { Nivel = "Principiante" },
                    new NivelExperiencia { Nivel = "Intermedio" },
                    new NivelExperiencia { Nivel = "Avanzado" }
                });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckObjetivoAsync()
        {
            if (!_context.Objetivos.Any())
            {
                _context.Objetivos.AddRange(new List<Objetivo>
                {
                    new Objetivo { Nombre = "Pérdida de Peso", Descripcion = "Quemar grasa y reducir el peso corporal." },
                    new Objetivo { Nombre = "Ganancia Muscular", Descripcion = "Aumentar la masa muscular y la fuerza." },
                    new Objetivo { Nombre = "Mejora de Resistencia", Descripcion = "Incrementar la capacidad cardiovascular y la resistencia." }
                });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckEquipoAsync()
        {
            if (!_context.Equipos.Any())
            {
                _context.Equipos.AddRange(new List<Equipo>
                {
                    new Equipo { Nombre = "Mancuernas", Descripcion = "Pesas de mano libres." },
                    new Equipo { Nombre = "Máquina de Cable", Descripcion = "Máquina con poleas para ejercicios de resistencia." },
                    new Equipo { Nombre = "Peso Corporal", Descripcion = "No se requiere equipo adicional." }
                });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckEjercicioAsync()
        {
            if (!_context.Ejercicios.Any())
            {

                _context.Ejercicios.AddRange(new List<Ejercicio>
                {
                    new Ejercicio
                    {
                        Nombre = "Sentadilla",
                        Descripcion = "Ejercicio compuesto para piernas y glúteos.",
                        GrupoMuscular = "Piernas",
                        EquipoId = 3
                    },
                    new Ejercicio
                    {
                        Nombre = "Press de Hombros",
                        Descripcion = "Ejercicio para hombros con mancuernas.",
                        GrupoMuscular = "Hombros",
                        EquipoId = 1
                    },
                    new Ejercicio
                    {
                        Nombre = "Remo en Máquina",
                        Descripcion = "Ejercicio para espalda con máquina de cable.",
                        GrupoMuscular = "Espalda",
                        EquipoId = 2
                    }
                });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckRutinaAsync()
        {
            if (!_context.Rutinas.Any())
            {

                var userAna = await _userHelper.GetUserAsync("ana.garcia@gym.com");
                if (userAna != null)
                {
                    var rutina = new Rutina
                    {
                        Nombre = "Rutina de Principiante",
                        FechaCreacion = DateTime.Now,
                        UserId = userAna.Id
                    };
                    _context.Rutinas.Add(rutina);
                    await _context.SaveChangesAsync();

                    var ejercicios = await _context.Ejercicios.Take(3).ToListAsync();
                    if (ejercicios.Count == 3)
                    {
                        _context.RutinaEjercicios.AddRange(new List<RutinaEjercicio>
                        {
                            new RutinaEjercicio { RutinaId = rutina.Id, EjercicioId = ejercicios[0].Id, Series = 3, Repeticiones = 12 },
                            new RutinaEjercicio { RutinaId = rutina.Id, EjercicioId = ejercicios[1].Id, Series = 3, Repeticiones = 10 },
                            new RutinaEjercicio { RutinaId = rutina.Id, EjercicioId = ejercicios[2].Id, Series = 3, Repeticiones = 10 }
                        });
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
