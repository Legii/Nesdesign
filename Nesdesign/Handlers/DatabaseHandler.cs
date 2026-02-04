using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Nesdesign.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Nesdesign
{
    public class DatabaseHandler
    {


    public static bool offersLoaded = false;
        public async void ConnectToDatabase()
        {
            using (var db = new OffersDbContext())
            {
                db.Database.EnsureCreated();
                var connection = db.Database.GetDbConnection();
                Repeair();
                connection.Open();


            }
        }


        public static void Repeair()
        {
            string connectionString = "Data Source=" + OffersDbContext._dbPath;
            bool Altered = false;
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            // Check if the table exists
            var checkTableCmd = connection.CreateCommand();
            checkTableCmd.CommandText = @"
            SELECT COUNT(*) 
            FROM sqlite_master 
            WHERE type='table' AND name='Contractors';
        ";

            long tableCount = (long)checkTableCmd.ExecuteScalar()!;

            if (tableCount == 0)
            {
                Console.WriteLine("Table does not exist. Creating...");

                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = @"
                CREATE TABLE Contractors (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                );
            ";
                Altered = true;
                createTableCmd.ExecuteNonQuery();
                Console.WriteLine("Table created successfully!");
               
            }
            else
            {
                Console.WriteLine("Table already exists.");
            }
            string tableName = "Offers";
            string columnName = "ContractorId";
            var checkCmd = connection.CreateCommand();
            checkCmd.CommandText = $"PRAGMA table_info({tableName});";

            bool columnExists = false;

            using (var reader = checkCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var existingColumn = reader["name"].ToString();
                    if (string.Equals(existingColumn, columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        columnExists = true;
                        break;
                    }
                }
            }

            if (columnExists)
            {
                Console.WriteLine($"Column '{columnName}' already exists in '{tableName}'.");

            }
            else
            {

                // 2️⃣ Add column
                var alterCmd = connection.CreateCommand();
                alterCmd.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} INTEGER;";
                alterCmd.ExecuteNonQuery();

                Console.WriteLine($"Column '{columnName}' added to '{tableName}'.");
                Altered = true;
            }
            if (Altered)
                Application.Current.Shutdown();
            connection.Close();
            

        }

      

        public static async Task AddRecordAsync<T>(T entity) where T : class
        {
            using (var db = new OffersDbContext())
            {
                try
                {
                    await db.Set<T>().AddAsync(entity);
                    await db.SaveChangesAsync();

                } catch (Exception e)
                {
                    MessageBox.Show("Taki rekord już istnieje w bazie", "Wystąpił bład");
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

        public static async Task<List<Who>> GetContractorsAsync()
        {
            return await GetAllRecordsAsync<Who>();
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
