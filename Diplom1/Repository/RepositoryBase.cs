using System.Data.SqlClient;
using System.Windows;

namespace Diplom1.Repository
{
    public abstract class RepositoryBase
    {
        private readonly string _connectionString;
        public RepositoryBase()
        {
            try
            {
                _connectionString = "Server=(local); DataBase=AutoRepairShop; Integrated Security=true";
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Не удалось настроить соединение с сервером" + ex);
            }
        }
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}