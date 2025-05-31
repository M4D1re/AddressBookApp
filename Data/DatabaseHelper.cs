using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using AddressBookApp.Models;
using Microsoft.Data.Sqlite;

namespace AddressBookApp.Data
{
    public static class DatabaseHelper
    {
        private static readonly string DbFile = "AddressBook.db";
        private static readonly string ConnectionString = $"Data Source={DbFile};";

        public static void InitializeDatabase()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var createContactsTable = @"
                CREATE TABLE IF NOT EXISTS Contacts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FirstName TEXT NOT NULL,
                    LastName TEXT NOT NULL,
                    PhoneNumber TEXT NOT NULL,
                    Category TEXT NOT NULL
                );";

            var createAdminTable = @"
                CREATE TABLE IF NOT EXISTS AdminSettings (
                    Id INTEGER PRIMARY KEY,
                    PasswordHash TEXT NOT NULL
                );";

            using var cmd1 = new SqliteCommand(createContactsTable, connection);
            cmd1.ExecuteNonQuery();

            using var cmd2 = new SqliteCommand(createAdminTable, connection);
            cmd2.ExecuteNonQuery();
        }

        public static List<Contact> GetContacts()
        {
            var contacts = new List<Contact>();
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var query = "SELECT * FROM Contacts";
            using var cmd = new SqliteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                contacts.Add(new Contact
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    PhoneNumber = reader["PhoneNumber"].ToString(),
                    Category = reader["Category"].ToString()
                });
            }
            return contacts;
        }

        public static void AddContact(Contact contact)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var query = "INSERT INTO Contacts (FirstName, LastName, PhoneNumber, Category) VALUES (@FirstName, @LastName, @PhoneNumber, @Category)";
            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@FirstName", contact.FirstName);
            cmd.Parameters.AddWithValue("@LastName", contact.LastName);
            cmd.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
            cmd.Parameters.AddWithValue("@Category", contact.Category);
            cmd.ExecuteNonQuery();
        }

        public static void UpdateContact(Contact contact)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var query = "UPDATE Contacts SET FirstName = @FirstName, LastName = @LastName, PhoneNumber = @PhoneNumber, Category = @Category WHERE Id = @Id";
            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", contact.Id);
            cmd.Parameters.AddWithValue("@FirstName", contact.FirstName);
            cmd.Parameters.AddWithValue("@LastName", contact.LastName);
            cmd.Parameters.AddWithValue("@PhoneNumber", contact.PhoneNumber);
            cmd.Parameters.AddWithValue("@Category", contact.Category);
            cmd.ExecuteNonQuery();
        }

        public static void DeleteContact(int id)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var query = "DELETE FROM Contacts WHERE Id = @Id";
            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public static bool IsAdminPasswordSet()
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var query = "SELECT PasswordHash FROM AdminSettings WHERE Id = 1";
            using var cmd = new SqliteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                string hash = reader["PasswordHash"]?.ToString();
                return !string.IsNullOrEmpty(hash);
            }
            return false;
        }

        public static void SetAdminPassword(string password)
        {
            string hash = HashPassword(password);

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var query = "INSERT OR REPLACE INTO AdminSettings (Id, PasswordHash) VALUES (1, @PasswordHash)";
            using var cmd = new SqliteCommand(query, connection);
            cmd.Parameters.AddWithValue("@PasswordHash", hash);
            cmd.ExecuteNonQuery();
        }

        public static bool ValidateAdminPassword(string password)
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var query = "SELECT PasswordHash FROM AdminSettings WHERE Id = 1";
            using var cmd = new SqliteCommand(query, connection);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                string storedHash = reader["PasswordHash"].ToString();
                return storedHash == HashPassword(password);
            }
            return false;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
