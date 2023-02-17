using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AnotherWorkMySql
{
    internal class DB
    {
        private DB() { }
        static DB instance;
        public static DB GetInstance()
        {
            if (instance == null)
                instance = new();
            return instance;
        }
        public MySqlConnection Connection { get => connection; }

        MySqlConnection connection;
        public void ConfigureConnection()
        {
            MySqlConnectionStringBuilder sb =
                new MySqlConnectionStringBuilder();
            sb.Server = "192.168.200.13";
            sb.UserID = "student";
            sb.Password = "student";
            sb.Database = "Rectoran1125";
            sb.CharacterSet = "utf8";
            connection = new MySqlConnection(
                sb.ToString());
        }

        public bool OpenConnection()
        {
            if (connection == null)
                ConfigureConnection();
            try
            {
                connection.Open();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }

        public void CloseConnection()
        {
            try
            {
                connection.Close();
            }
            catch
            { }
        }

        public void TestConnection()
        {
            if (OpenConnection())
            {
                connection.Close();
                System.Windows.MessageBox.Show("Успешно");
            }
        }


        public bool RemoveRow<T>(T obj) where T : class
        {
            int id = 0;
            var typeInfo = obj.GetType();
            var at = (DBTableAttribute)typeInfo.GetCustomAttributes(false).First(s=>s is DBTableAttribute);
            string table = at.Title;

            // поиск свойства, указывающего на ID можно опустить, если использовать общий базовый класс с свойством ID для всех DTO-объектов
            // или использовать специальный аттрибут для первичного ключа
            var props = typeInfo.GetProperties();
            foreach (var prop in props)
            {
                var attrib = (DBColumnAttribute)prop.GetCustomAttributes(false).First(s => s is DBColumnAttribute);
                if (attrib.Title == "id")
                {
                    id = (int)prop.GetValue(obj);
                    break;
                }
            }
            string sql = $"delete from {table} where id = {id}";
            int rowsAffected = 0;
            if (OpenConnection())
            {
                rowsAffected = MySqlHelper.ExecuteNonQuery(connection, sql);
                CloseConnection();
            }
            return rowsAffected != 0;
        }
    }
}
