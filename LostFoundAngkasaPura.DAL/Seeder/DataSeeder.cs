using LostFoundAngkasaPura.DAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostFoundAngkasaPura.DAL.Seeder
{
    public class DataSeeder
    {
        private readonly LostFoundDbContext lostFoundDbContext;

        public DataSeeder()
        {
        }

        public void Seed()
        {
            if (!lostFoundDbContext.admin.Any())
            {
                var admin = new Admin()
                {
                    
                   Access = "SuperAdmin",
                   ActiveFlag = true,
                   CreatedBy = "Seeder",
                   CreatedDate = DateTime.Now,
                   Email = "raihanr090@gmail.com",
                   LastUpdatedBy = "Seeder",
                   LastUpdatedDate = DateTime.Now,
                   Password = "$2b$10$WP4uGTm0Yam84Y70sCSTCuAVL34OXr2ejK9inWpC8uszD1JARUPRi",
                   RefreshToken = "LEwtsmCEUR7W3GXZ7PA6bWNUwaIkrKuY65plee5ekOvXpyrdWBk06Eh0Uk2sqpQwZA+5yYL+SckQA1Pp8OKO4Q==",
                   Unit = "Developer",
                   Name ="Raihan"
                };
                lostFoundDbContext.admin.Add(admin);
                lostFoundDbContext.SaveChanges();
            }
        }
    }
}
