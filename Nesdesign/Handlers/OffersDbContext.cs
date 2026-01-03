using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nesdesign
{
    public class OffersDbContext : DbContext
    {
        private static string _dbPath => Path.Combine(SettingsManager.Instance.GetValue("BASE_PATH"),"nesdesign.db");
        public DbSet<Models.Offer> Offers { get; set; } 
        public DbSet<Models.Client> Clients { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {

            options.UseSqlite("Data Source=" + _dbPath);
        }


    }
}
