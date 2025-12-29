using Microsoft.Data.Sqlite;
using Nesdesign.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Nesdesign.Data
{
    public static class SqliteDatabase
    {
        private static string _dbPath => Path.Combine(FileHandler.BASE_PATH, "nesdesign.db");
        public static string DbPath => _dbPath;

        public static async Task InitializeAsync()
        {
            try
            {
                Debug.WriteLine($"[SqliteDatabase] InitializeAsync; DB path = {_dbPath}");
                Directory.CreateDirectory(FileHandler.BASE_PATH);

                await using var conn = new SqliteConnection($"Data Source={_dbPath}");
                await conn.OpenAsync();

                // Włącz wymuszanie FK na tym połączeniu
                await using (var pragmaCmd = conn.CreateCommand())
                {
                    pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
                    await pragmaCmd.ExecuteNonQueryAsync();
                }

                await using var cmd = conn.CreateCommand();
                // Clients z AUTOINCREMENT, Offers z FK
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Clients (
                        clientId INTEGER PRIMARY KEY AUTOINCREMENT,
                        nazwa TEXT,
                        NIP TEXT,
                        opis TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Offers (
                        offerId TEXT PRIMARY KEY,
                        fotoUrl TEXT,
                        description TEXT,
                        quantity INTEGER,
                        name TEXT,
                        clientId INTEGER,
                        status INTEGER,
                        closed INTEGER,
                        FOREIGN KEY(clientId) REFERENCES Clients(clientId)
                    );";
                await cmd.ExecuteNonQueryAsync();

                Debug.WriteLine($"[SqliteDatabase] InitializeAsync completed; file exists: {File.Exists(_dbPath)}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SqliteDatabase] InitializeAsync ERROR: {ex}");
                throw;
            }
        }

        public static async Task<List<Offer>> GetOffersAsync()
        {
            var list = new List<Offer>();
            await using var conn = new SqliteConnection($"Data Source={_dbPath}");
            await conn.OpenAsync();

            await using (var pragmaCmd = conn.CreateCommand())
            {
                pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
                await pragmaCmd.ExecuteNonQueryAsync();
            }

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT offerId, fotoUrl, description, quantity, name, clientId, status, closed FROM Offers;";
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                string photoPath = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                var o = new Offer
                {
                    offerId = reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                    description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    quantity = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                    name = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    clientId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    status = reader.IsDBNull(6) ? OfferStatus.UTWORZONA : (OfferStatus)reader.GetInt32(6),
                    Closed = !reader.IsDBNull(7) && reader.GetInt32(7) != 0
                };
                o.loadPhoto(photoPath);
                list.Add(o);
            }
            return list;
        }

        public static async Task<int> GetOffersCountAsync()
        {
            await using var conn = new SqliteConnection($"Data Source={_dbPath}");
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Offers;";
            var result = await cmd.ExecuteScalarAsync();
            int count = 0;
            if (result != null && int.TryParse(result.ToString(), out var c))
                count = c;
            Debug.WriteLine($"[SqliteDatabase] Offers count = {count}");
            return count;
        }

        public static async Task InsertOrUpdateOfferAsync(Offer o)
        {
            try
            {
                Debug.WriteLine($"[SqliteDatabase] InsertOrUpdateOfferAsync: {o?.offerId} clientId={o?.clientId} db='{_dbPath}'");
                await using var conn = new SqliteConnection($"Data Source={_dbPath}");
                await conn.OpenAsync();

                await using (var pragmaCmd = conn.CreateCommand())
                {
                    pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
                    await pragmaCmd.ExecuteNonQueryAsync();
                }

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    INSERT INTO Offers (offerId, fotoUrl, description, quantity, name, clientId, status, closed)
                    VALUES ($offerId, $fotoUrl, $description, $quantity, $name, $clientId, $status, $closed)
                    ON CONFLICT(offerId) DO UPDATE SET
                        fotoUrl = excluded.fotoUrl,
                        description = excluded.description,
                        quantity = excluded.quantity,
                        name = excluded.name,
                        clientId = excluded.clientId,
                        status = excluded.status,
                        closed = excluded.closed;
                ";
                cmd.Parameters.AddWithValue("$offerId", o.offerId ?? string.Empty);
                //cmd.Parameters.AddWithValue("$fotoUrl", (object?)o.fotoUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$description", (object?)o.description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$quantity", o.quantity);
                cmd.Parameters.AddWithValue("$name", (object?)o.name ?? DBNull.Value);

                // jeśli clientId == 0 traktujemy jako brak klienta => zapisujemy NULL
                if (o.clientId == 0)
                    cmd.Parameters.AddWithValue("$clientId", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("$clientId", o.clientId);

                cmd.Parameters.AddWithValue("$status", (int)o.status);
                cmd.Parameters.AddWithValue("$closed", o.Closed ? 1 : 0);

                await cmd.ExecuteNonQueryAsync();
                Debug.WriteLine($"[SqliteDatabase] InsertOrUpdateOfferAsync completed: {o?.offerId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SqliteDatabase] InsertOrUpdateOfferAsync FAILED for {o?.offerId}: {ex}");
                throw;
            }
        }

        public static async Task DeleteOfferAsync(string offerId)
        {
            try
            {
                await using var conn = new SqliteConnection($"Data Source={_dbPath}");
                await conn.OpenAsync();

                await using (var pragmaCmd = conn.CreateCommand())
                {
                    pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
                    await pragmaCmd.ExecuteNonQueryAsync();
                }

                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM Offers WHERE offerId = $offerId;";
                cmd.Parameters.AddWithValue("$offerId", offerId ?? string.Empty);
                await cmd.ExecuteNonQueryAsync();
                Debug.WriteLine($"[SqliteDatabase] DeleteOfferAsync deleted: {offerId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SqliteDatabase] DeleteOfferAsync ERROR for {offerId}: {ex}");
                throw;
            }
        }

        public static async Task<List<Client>> GetClientsAsync()
        {
            var list = new List<Client>();
            await using var conn = new SqliteConnection($"Data Source={_dbPath}");
            await conn.OpenAsync();

            await using (var pragmaCmd = conn.CreateCommand())
            {
                pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
                await pragmaCmd.ExecuteNonQueryAsync();
            }

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT clientId, nazwa, NIP, opis FROM Clients;";
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var c = new Client
                {
                    clientId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    nazwa = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    NIP = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    opis = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
                };
                list.Add(c);
            }
            return list;
        }

        public static async Task InsertOrUpdateClientAsync(Client c)
        {
            try
            {
                Debug.WriteLine($"[SqliteDatabase] InsertOrUpdateClientAsync start: clientId={c?.clientId} nazwa='{c?.nazwa}' db='{_dbPath}'");
                await using var conn = new SqliteConnection($"Data Source={_dbPath}");
                await conn.OpenAsync();

                await using (var pragmaCmd = conn.CreateCommand())
                {
                    pragmaCmd.CommandText = "PRAGMA foreign_keys = ON;";
                    await pragmaCmd.ExecuteNonQueryAsync();
                }

                if (c.clientId <= 0)
                {
                    // nowy klient - wstaw bez id, DB nadaje AUTOINCREMENT
                    await using var insertCmd = conn.CreateCommand();
                    insertCmd.CommandText = @"
                        INSERT INTO Clients (nazwa, NIP, opis)
                        VALUES ($nazwa, $nip, $opis);
                    ";
                    insertCmd.Parameters.AddWithValue("$nazwa", (object?)c.nazwa ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("$nip", (object?)c.NIP ?? DBNull.Value);
                    insertCmd.Parameters.AddWithValue("$opis", (object?)c.opis ?? DBNull.Value);
                    await insertCmd.ExecuteNonQueryAsync();

                    // pobierz nadane id przez funkcję SQLite
                    await using var idCmd = conn.CreateCommand();
                    idCmd.CommandText = "SELECT last_insert_rowid();";
                    var lastObj = await idCmd.ExecuteScalarAsync();
                    if (lastObj != null && long.TryParse(lastObj.ToString(), out var lastId))
                    {
                        c.clientId = (int)lastId;
                        Debug.WriteLine($"[SqliteDatabase] InsertOrUpdateClientAsync inserted new client id={c.clientId}");
                    }
                    else
                    {
                        Debug.WriteLine("[SqliteDatabase] InsertOrUpdateClientAsync: failed to read last_insert_rowid()");
                    }
                }
                else
                {
                    // aktualizacja istniejącego klienta
                    await using var updateCmd = conn.CreateCommand();
                    updateCmd.CommandText = @"
                        INSERT INTO Clients (clientId, nazwa, NIP, opis)
                        VALUES ($clientId, $nazwa, $nip, $opis)
                        ON CONFLICT(clientId) DO UPDATE SET
                            nazwa = excluded.nazwa,
                            NIP = excluded.NIP,
                            opis = excluded.opis;
                    ";
                    updateCmd.Parameters.AddWithValue("$clientId", c.clientId);
                    updateCmd.Parameters.AddWithValue("$nazwa", (object?)c.nazwa ?? DBNull.Value);
                    updateCmd.Parameters.AddWithValue("$nip", (object?)c.NIP ?? DBNull.Value);
                    updateCmd.Parameters.AddWithValue("$opis", (object?)c.opis ?? DBNull.Value);
                    await updateCmd.ExecuteNonQueryAsync();
                    Debug.WriteLine($"[SqliteDatabase] InsertOrUpdateClientAsync updated client id={c.clientId}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SqliteDatabase] InsertOrUpdateClientAsync ERROR for clientId={c?.clientId}: {ex}");
                throw;
            }
        }
    }
}