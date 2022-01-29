using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReservationSystem.Data;
using ReservationSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddCors(options =>
                {
                    options.AddPolicy("MyPolicy",
                        builder =>
                            {
                                builder.SetIsOriginAllowed(origin => true)
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials();
                            });
                });

            bool clientValidationEnabled = Configuration.GetValue<bool>("ClientValidationEnabled");
            services.AddControllersWithViews()
                .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = clientValidationEnabled);

            services.AddScoped<CustomerService>();

            services.AddAutoMapper(cfg => { cfg.CreateMap<Areas.Admin.Models.SittingCategory.CreateSC, Data.SittingCategory>(); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "area-route",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });

            CreateRoles(serviceProvider);
            SeedAdmin(serviceProvider);

            //Problem: can't call async task in Configure
            //await SeedAdmin(serviceProvider, cxt);
        }

        public void CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "Employee", "Member" };
            foreach (var roleName in roleNames)
            {
                Task<bool> roleExists = roleManager.RoleExistsAsync(roleName);
                roleExists.Wait();
                if (!roleExists.Result)
                {
                    Task<IdentityResult> result = roleManager.CreateAsync(new IdentityRole(roleName));
                    result.Wait();
                }
            }
        }

        public void SeedAdmin(IServiceProvider serviceProvider)
        {
            var cxt = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();


            var user = userManager.FindByNameAsync("admin@a.com").Result;
            if (user == null)
            {

                var admin = new IdentityUser { UserName = "admin@a.com", Email = "admin@a.com" };
                var result = userManager.CreateAsync(admin, "Admin@123").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(admin, "Admin").Wait();
                    var emp = new Data.Employee
                    {
                        EmpFName = "admin",
                        EmpLName = "admin",
                        EmpEmail = admin.Email,
                        EmpPhone = null,
                        IdentityUserId = admin.Id
                    };
                    cxt.Employees.Add(emp);
                    cxt.SaveChangesAsync().Wait();
                }

            }
        }
    }
}
