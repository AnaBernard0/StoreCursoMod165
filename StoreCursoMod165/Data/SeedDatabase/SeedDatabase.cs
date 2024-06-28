using Microsoft.AspNetCore.Identity;
using static StoreCursoMod165.StoreCursoMod165Constants.USERS;

namespace StoreCursoMod165.Data.SeedDatabase
{
    public class SeedDatabase
    {
        public static void Seed(ApplicationDbContext context,
                                UserManager<IdentityUser> userManager,
                                RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager).Wait(); //modo tarefa
            SeedUsers(userManager).Wait();
        }

        private static async Task SeedUsers(UserManager<IdentityUser> userManager)
        {

            var dbAdministrador = await userManager.FindByNameAsync(StoreCursoMod165Constants.USERS.ADMINISTRADOR.USERNAME);

            if (dbAdministrador == null)
            {
                IdentityUser userAdmin = new IdentityUser()
                {
                    UserName = StoreCursoMod165Constants.USERS.ADMINISTRADOR.USERNAME
                };

                var result = await userManager.CreateAsync(userAdmin, StoreCursoMod165Constants.USERS.ADMINISTRADOR.PASSWORD);

                if (result.Succeeded == true)
                {
                    dbAdministrador = await userManager.FindByNameAsync(StoreCursoMod165Constants.USERS.ADMINISTRADOR.USERNAME);
                    await userManager.AddToRoleAsync(dbAdministrador!, StoreCursoMod165Constants.ROLES.ADMINISTRADOR);
                }
            }


            var dbVendedor = await userManager.FindByNameAsync(StoreCursoMod165Constants.USERS.VENDEDOR.USERNAME);

            if (dbVendedor == null)
            {
                IdentityUser userDriver = new IdentityUser()
                {
                    UserName = StoreCursoMod165Constants.USERS.VENDEDOR.USERNAME
                };

                var result = await userManager.CreateAsync(userDriver, StoreCursoMod165Constants.USERS.VENDEDOR.PASSWORD);

                if (result.Succeeded == true)
                {
                    dbVendedor = await userManager.FindByNameAsync(StoreCursoMod165Constants.USERS.VENDEDOR.USERNAME);
                    await userManager.AddToRoleAsync(dbVendedor!, StoreCursoMod165Constants.ROLES.VENDEDOR);
                }
            }

            var dbLogistica = await userManager.FindByNameAsync(StoreCursoMod165Constants.USERS.LOGISTICA.USERNAME);

            if (dbLogistica == null)
            {
                IdentityUser userDriver = new IdentityUser()
                {
                    UserName = StoreCursoMod165Constants.USERS.LOGISTICA.USERNAME
                };

                var result = await userManager.CreateAsync(userDriver, StoreCursoMod165Constants.USERS.LOGISTICA.PASSWORD);

                if (result.Succeeded == true)
                {
                    dbLogistica = await userManager.FindByNameAsync(StoreCursoMod165Constants.USERS.LOGISTICA.USERNAME);
                    await userManager.AddToRoleAsync(dbLogistica!, StoreCursoMod165Constants.ROLES.LOGISTICA);
                }
            }

        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roleAdministrador = await roleManager.RoleExistsAsync(StoreCursoMod165Constants.ROLES.ADMINISTRADOR);

            if (!roleAdministrador)
            {
                var administradorRole = new IdentityRole
                {
                    Name = StoreCursoMod165Constants.ROLES.ADMINISTRADOR
                };
                await roleManager.CreateAsync(administradorRole);
            }

            var roleVendedor = await roleManager.RoleExistsAsync(StoreCursoMod165Constants.ROLES.VENDEDOR);

            if (!roleVendedor)
            {
                var vendedorRole = new IdentityRole
                {
                    Name = StoreCursoMod165Constants.ROLES.VENDEDOR
                };

                await roleManager.CreateAsync(vendedorRole);
            }


            var roleLogistica = await roleManager.RoleExistsAsync(StoreCursoMod165Constants.ROLES.LOGISTICA);

            if (!roleLogistica)
            {
                var logisticaRole = new IdentityRole
                {
                    Name = StoreCursoMod165Constants.ROLES.LOGISTICA
                };

                await roleManager.CreateAsync(logisticaRole);
            }
        }
    }
}
