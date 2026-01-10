using Microsoft.EntityFrameworkCore;
using Nesdesign.Models;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Nesdesign
{
    public class DatabaseHandler
    {
    public static bool offersLoaded = false;
        public void ConnectToDatabase()
        {
            using (var db = new OffersDbContext())
            {
                db.Database.EnsureCreated();
               
            }
        }

        public static async Task AddRecordAsync<T>(T entity) where T : class
        {
            using (var db = new OffersDbContext())
            {
                try
                {
                    await db.Set<T>().AddAsync(entity);
                    await db.SaveChangesAsync();

                } catch
                {
                    MessageBox.Show("Taka oferta juz istnieje w bazie", "Wystąpił bład");
                }
            }
        }


        public static async Task UpdateRecordAsync<T>(T entity) where T : class
        {
            using (var db = new OffersDbContext())
            {
                db.Set<T>().Update(entity);
                await db.SaveChangesAsync();
            }
        }


        public static async Task<List<T>> GetAllRecordsAsync<T>() where T : class
        {
            using (var db = new OffersDbContext())
            {
                return await db.Set<T>().ToListAsync();
         
            }
        }


        public static async Task<List<Offer>> GetOffersAsync()
        {   
            return await GetAllRecordsAsync<Offer>();
        }

        public static async Task<List<Client>> GetClientsAsync()
        {
            return await GetAllRecordsAsync<Client>();
        }


        public static async Task DeleteRecordAsync<T>(T entity) where T : class
        {
            using (var db = new OffersDbContext())
            {
                db.Set<T>().Remove(entity);
                await db.SaveChangesAsync();
            }
        }


    }
}
