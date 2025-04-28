using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _04._23._25Homework.Data
{
    public class AdRepository
    {
        private readonly string _connectionString;

        public AdRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User Login(string email, string password)
        {
            var user = UserByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isMatch ? user : null;
        }

        public void NewUser(User user, string password)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            user.PasswordHash = hash;

            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();

            cmd.CommandText = "INSERT INTO Users VALUES (@email, @password, @name)";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@password", user.PasswordHash);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public User UserByEmail(string email)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();

            cmd.CommandText = "SELECT * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);

            connection.Open();
            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new User
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Email = (string)reader["Email"],
                    PasswordHash = (string)reader["PasswordHash"]
                    
                };
            }
            return null;
        }

        public void NewAd(SimpleAd ad)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();

            cmd.CommandText = "INSERT INTO Ads VALUES (@title, @phoneNumber, @description, @userId)";
            cmd.Parameters.AddWithValue("@title", ad.Title);
            cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@description", ad.Description);
            cmd.Parameters.AddWithValue("@userId", ad.UserId);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public List<SimpleAd> GetAds(int userId = 0)
        {
            List<SimpleAd> ads = new();

            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();

            cmd.CommandText = "SELECT * FROM Ads";

            if(userId != 0)
            {
                cmd.CommandText += " WHERE UserId = @userId";
                cmd.Parameters.AddWithValue("@userId", userId);
            }

            connection.Open();
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ads.Add(new SimpleAd
                {
                    Id = (int)reader["Id"],
                    Title = (string)reader["Title"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Description = (string)reader["Description"],
                    UserId = (int)reader["UserId"]
                });
            }

            return ads;
        }

        public void DeleteAd(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();

            cmd.CommandText = "DELETE FROM Ads WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            cmd.ExecuteNonQuery();
        }

    }
}
