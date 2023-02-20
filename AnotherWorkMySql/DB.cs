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

        public bool InsertRow<T>(T obj) where T : class
        {
            var typeInfo = obj.GetType();
            string table = GetSQLTableName(typeInfo);

            var props = typeInfo.GetProperties();
            List<string> columns = new List<string>();
            List<MySqlParameter> parameters = new();
            SearchDTOColumns(obj, props, columns, parameters);

            string sql = $"insert into {table} ({string.Join(',', columns)}) values ({string.Join(',', columns.Select(s => $"@{s}"))})";

            return ExecuteSql(sql, parameters.ToArray());
        }

        public bool Update<T>(T obj) where T : class
        {
            var typeInfo = obj.GetType();
            string table = GetSQLTableName(typeInfo);

            var props = typeInfo.GetProperties();
            List<string> columns = new List<string>();
            List<MySqlParameter> parameters = new();
            SearchDTOColumns(obj, props, columns, parameters);

            string columnID;
            object valueID = GetIDValue(typeInfo, obj, out columnID);
            parameters.Add(new MySqlParameter("id", valueID));
            
            string sql = $"update {table} set {string.Join(',', columns.Select(s => $"{s} = @{s}"))} where {columnID} = @id";
            
            return ExecuteSql(sql, parameters.ToArray());
        }

        private static void SearchDTOColumns<T>(T obj, System.Reflection.PropertyInfo[] props, List<string> columns, List<MySqlParameter> parameters) where T : class
        {
            foreach (var prop in props)
            {
                var primaryAttrib = prop.
                    GetCustomAttributes(false).
                    FirstOrDefault(s => s is DBPrimaryColumnAttribute);
                if (primaryAttrib != null && ((DBPrimaryColumnAttribute)primaryAttrib).CreateByUser)
                {
                    FillListColumn(obj, columns, parameters, prop, primaryAttrib);
                }
                else
                {
                    var defaultColumn = prop.
                        GetCustomAttributes(false).
                        FirstOrDefault(s => s is DBColumnAttribute && !(s is DBPrimaryColumnAttribute));
                    if (defaultColumn != null)
                        FillListColumn(obj, columns, parameters, prop, defaultColumn);
                }
            }
        }

        private static void FillListColumn<T>(T obj, List<string> columns, List<MySqlParameter> parameters, System.Reflection.PropertyInfo prop, object? primaryAttrib) where T : class
        {
            string title = ((DBColumnAttribute)primaryAttrib).Title;
            columns.Add(title);
            parameters.Add(new MySqlParameter(title, prop.GetValue(obj)));
        }

        public bool RemoveRow<T>(T obj) where T : class
        {
            var typeInfo = obj.GetType();
            string table = GetSQLTableName(typeInfo);

            string columnID;
            object value = GetIDValue(typeInfo, obj, out columnID);

            string sql = $"delete from {table} where {columnID} = @idValue";
            MySqlParameter[] parameters = new MySqlParameter[]
                { 
                    new MySqlParameter("idValue", value)
                };
            return ExecuteSql(sql, parameters);
        }

        private object GetIDValue(Type typeInfo, object obj, out string ColumnIdName)
        {
            var props = typeInfo.GetProperties();
            var attrib = props.
                Select(s => (s, s.GetCustomAttributes(false)
                    .FirstOrDefault(s => s is DBPrimaryColumnAttribute))).Where(s => s.Item2 != null).First();
            ColumnIdName = ((DBPrimaryColumnAttribute)attrib.Item2).Title;
            var value = attrib.s.GetValue(obj);
            return value;
        }

        private bool ExecuteSql(string sql, MySqlParameter[] parameters = null)
        {
            int rowsAffected = 0;
            if (OpenConnection())
            {
                rowsAffected = MySqlHelper.ExecuteNonQuery(connection, sql, parameters);
                CloseConnection();
            }
            return rowsAffected != 0;
        }

        private static string GetSQLTableName(Type typeInfo)
        {
            var at = (DBTableAttribute)typeInfo.GetCustomAttributes(false).First(s => s is DBTableAttribute);
            return at.Title;
        }
    }
}
