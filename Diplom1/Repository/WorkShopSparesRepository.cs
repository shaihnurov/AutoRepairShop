using Diplom1.MVVM.Model.WorkShop;
using System;
using System.Collections.ObjectModel;
using System.Data;
using Diplom1.MVVM.Model.Interface;
using System.Data.SqlClient;
using Diplom1.MVVM.Model;
using System.Collections.Generic;

namespace Diplom1.Repository
{
    public class WorkShopSparesRepository : RepositoryBase, IWorkShopSparesRepository
    {
        public void AddSpares(WorkShopSparesModel wssModel)
        {
            try
            {
                using var connection = GetConnection();
                using var command = new SqlCommand();
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [WorkShopSpares] (WorkShopId, SparesId, AmountSpares, Articul) VALUES (@WorkShopId, @SparesId, @AmountSpares, @Articul)";
                command.Parameters.Add("@WorkShopId", SqlDbType.NVarChar).Value = wssModel.WorkShopId;
                command.Parameters.Add("@SparesId", SqlDbType.NVarChar).Value = wssModel.SparesId;
                command.Parameters.Add("@AmountSpares", SqlDbType.NVarChar).Value = wssModel.Amount;
                command.Parameters.Add("@Articul", SqlDbType.NVarChar).Value = wssModel.Articul;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"* Ошибка при добавлении товара для пользователя: {ex.Message}");
            }
        }
        public ObservableCollection<WorkShopSparesModel> GetWorkShopSpares(string workShopId)
        {
            ObservableCollection<WorkShopSparesModel> shops = [];
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT ws.SparesId, ws.AmountSpares, s.Name, s.Articul, s.Make, s.Image, s.Price FROM dbo.[WorkShopSpares] ws JOIN dbo.Spares s ON ws.SparesId = s.Id WHERE ws.WorkShopId = @workShopId";
                command.Parameters.AddWithValue("@workShopId", workShopId);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    WorkShopSparesModel shop = new()
                    {
                        SparesId = reader["SparesId"].ToString(),
                        Amount = reader["AmountSpares"].ToString(),
                        Name = reader["Name"].ToString(),
                        Articul = reader["Articul"].ToString(),
                        Make = reader["Make"].ToString(),
                        Image = (byte[])reader["Image"],
                        Price = Convert.ToDecimal(reader["Price"])
                    };
                    shops.Add(shop);
                }
            }
            return shops;
        }
        public ObservableCollection<WorkShopSparesModel> GetSparesToAmountZero(string workShopId)
        {
            ObservableCollection<WorkShopSparesModel> shops = [];
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT ws.SparesId, ws.AmountSpares, ws.WorkShopId, s.Name, s.Articul, s.Make, s.Image, s.Price FROM dbo.[WorkShopSpares] ws JOIN dbo.Spares s ON ws.SparesId = s.Id WHERE ws.WorkShopId = @workShopId AND ws.AmountSpares = 0";
                command.Parameters.AddWithValue("@workShopId", workShopId);
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    WorkShopSparesModel shop = new()
                    {
                        SparesId = reader["SparesId"].ToString(),
                        WorkShopId = reader["WorkShopId"].ToString(),
                        Amount = reader["AmountSpares"].ToString(),
                        Name = reader["Name"].ToString(),
                        Articul = reader["Articul"].ToString(),
                        Make = reader["Make"].ToString(),
                        Image = (byte[])reader["Image"],
                        Price = Convert.ToDecimal(reader["Price"])
                    };
                    shops.Add(shop);
                }
            }
            return shops;
        }
        public void IncreaseAmount(string sparesId, string workshopId, string articul)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();
            command.Connection = connection;
            command.CommandText = "SELECT COUNT(*) FROM dbo.WorkShopSpares WHERE SparesId = @sparesId AND WorkShopId = @workShopId";
            command.Parameters.AddWithValue("@sparesId", sparesId);
            command.Parameters.AddWithValue("@workShopId", workshopId);
            int count = Convert.ToInt32(command.ExecuteScalar());

            if (count == 0)
            {
                var newSparesModel = new WorkShopSparesModel
                {
                    WorkShopId = workshopId,
                    SparesId = sparesId,
                    Amount = "1",
                    Articul = articul,
                };
                AddSpares(newSparesModel);
            }
            else
            {
                command.CommandText = "UPDATE dbo.WorkShopSpares SET AmountSpares = AmountSpares + 1 WHERE SparesId = @sparesId AND WorkShopId = @workShopId";
                command.ExecuteNonQuery();
            }
        }
        public void AddDelivery(string idWorkShop, string sparesId, string amount)
        {
            try
            {
                using var connection = GetConnection();
                using var command = new SqlCommand();
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [dbo].[Delivery] (IdWorkShop, Date, DateTimeDelivery, SparesId, Amount) VALUES (@IdWorkShop, @Date, @DateTimeDelivery, @SparesId, @Amount)";

                command.Parameters.Add("@IdWorkShop", SqlDbType.Int).Value = idWorkShop;
                command.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@DateTimeDelivery", SqlDbType.DateTime).Value = GetEstimatedDeliveryDate();
                command.Parameters.Add("@SparesId", SqlDbType.Int).Value = sparesId;
                command.Parameters.Add("@Amount", SqlDbType.Int).Value = amount;

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при добавлении записи в Delivery: {ex.Message}");
            }
        }
        public static DateTime GetEstimatedDeliveryDate()
        {
            Random rnd = new();
            int deliveryDays = rnd.Next(3, 8);
            return DateTime.Now.AddDays(deliveryDays);
        }
        public ObservableCollection<DeliveryModel> GetDeliveries(string idWorkShop)
        {
            ObservableCollection<DeliveryModel> deliveries = [];
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = @"SELECT d.Id, d.IdWorkShop, d.Date, d.DateTimeDelivery, d.SparesId, d.Amount, s.Name, s.Make, s.Articul, s.Price, s.Image FROM dbo.[Delivery] d JOIN dbo.Spares s ON d.SparesId = s.Id WHERE d.IdWorkShop = @IdWorkShop";
                command.Parameters.AddWithValue("@IdWorkShop", idWorkShop);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var delivery = new DeliveryModel
                    {
                        Id = reader["Id"].ToString(),
                        IdWorkShop = reader["IdWorkShop"].ToString(),
                        Date = Convert.ToDateTime(reader["Date"]),
                        DateDelivery = Convert.ToDateTime(reader["DateTimeDelivery"]),
                        SparesId = reader["SparesId"].ToString(),
                        Amount = reader["Amount"].ToString(),
                        Name = reader["Name"].ToString(),
                        Make = reader["Make"].ToString(),
                        Articul = reader["Articul"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Image = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : (byte[])reader["Image"],
                    };
                    deliveries.Add(delivery);
                }
            }
            return deliveries;
        }
        public void DeleteDeliveies(string idDelivery)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();
            command.Connection = connection;
            command.CommandText = "DELETE FROM dbo.Delivery WHERE Id = @idDelivery";
            command.Parameters.AddWithValue("@idDelivery", idDelivery);
            command.ExecuteNonQuery();
        }
        public void CheckDeliveriesForExactTime(IEnumerable<DeliveryModel> deliveries, string idWorkShop)
        {
            foreach (var delivery in deliveries)
            {
                if (delivery.DateDelivery.Date == DateTime.Now.Date)
                {
                    IncreaseAmount(delivery.SparesId, idWorkShop, delivery.Articul);
                    DeleteDeliveies(delivery.Id);
                }
            }
        }
        public void DecreaseSparesAmount(string sparesId, string workShopId)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();
            command.Connection = connection;
            command.CommandText = "SELECT AmountSpares FROM dbo.WorkShopSpares WHERE WorkShopId = @workShopId AND SparesId = @sparesId";
            command.Parameters.AddWithValue("@workShopId", workShopId);
            command.Parameters.AddWithValue("@sparesId", sparesId);

            int currentAmount = Convert.ToInt32(command.ExecuteScalar());

            if (currentAmount > 0)
            {
                command.CommandText = "UPDATE dbo.WorkShopSpares SET AmountSpares = AmountSpares - 1 WHERE WorkShopId = @workShopId AND SparesId = @sparesId";
                command.ExecuteNonQuery();
            }
            else
            {
                throw new Exception("Товар закончился");
            }
        }
        public void DeleteWorkShopSpares(string sparesId, string workShopId)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand();
            connection.Open();
            command.Connection = connection;
            command.CommandText = "DELETE FROM dbo.WorkShopSpares WHERE WorkShopId = @workShopId AND SparesId = @sparesId";
            command.Parameters.AddWithValue("@workShopId", workShopId);
            command.Parameters.AddWithValue("@sparesId", sparesId);
            command.ExecuteNonQuery();
        }
    }
}