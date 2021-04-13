using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebDstu.Database;
using WebDstu.Models;

namespace WebDstu
{
    public class Startup
    {

        public static Dictionary<string, DSTUSaved> saveds = new Dictionary<string, DSTUSaved>();
        public static List<int> deletes = new List<int>();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

                //Bot bot = new Bot();
            //bot.StartListen();


            using (DatabaseContext db = new DatabaseContext())
            {
                //saveds = db.Saved.ToList();
                List<DSTUSaved> list = db.Saved.ToList();


                int index = 1;
                foreach (var item in list)
                {
                    saveds.Add(item.Action, item);
                }
            }

            Startup.saveds = Startup.saveds.OrderBy(pair => pair.Value.SortId).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }
}
