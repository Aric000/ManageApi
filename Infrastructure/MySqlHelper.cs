using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace Infrastructure
{
    public class MySqlHelper
    {
        private static readonly string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"] ?? "";

        /// <summary> 
        /// 获取一个有效的数据库连接对象 
        /// </summary> 
        /// <returns></returns> 
        private static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        /// <summary> 
        /// 给定连接的数据库用假设参数执行一个sql命令（不返回数据集） 
        /// </summary>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>执行命令所影响的行数</returns> 
        public static int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text,
            params MySqlParameter[]? commandParameters)
        {
            var cmd = new MySqlCommand();
            using var conn = GetConnection();
            PrepareCommand(cmd, conn, null, cmdText, cmdType, commandParameters);
            var val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }


        /// <summary> 
        /// 用现有的数据库连接执行一个sql命令（不返回数据集） 
        /// </summary> 
        /// <param name="connection">一个现有的数据库连接</param> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>执行命令所影响的行数</returns> 
        public static int ExecuteNonQuery(MySqlConnection connection, string cmdText,
            CommandType cmdType = CommandType.Text, params MySqlParameter[]? commandParameters)
        {
            var cmd = new MySqlCommand();
            PrepareCommand(cmd, connection, null, cmdText, cmdType, commandParameters);
            var val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary> 
        ///使用现有的SQL事务执行一个sql命令（不返回数据集） 
        /// </summary> 
        /// <remarks> 
        ///举例: 
        /// int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="trans">一个现有的事务</param> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>执行命令所影响的行数</returns> 
        public static int ExecuteNonQuery(MySqlTransaction? trans, string cmdText,
            CommandType cmdType = CommandType.Text, params MySqlParameter[]? commandParameters)
        {
            var cmd = new MySqlCommand();
            PrepareCommand(cmd, trans?.Connection, trans, cmdText, cmdType, commandParameters);
            var val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary> 
        /// 用执行的数据库连接执行一个返回数据集的sql命令 
        /// </summary> 
        /// <remarks> 
        /// 举例: 
        /// MySqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>包含结果的读取器</returns> 
        public static MySqlDataReader ExecuteReader(string cmdText, CommandType cmdType = CommandType.Text,
            params MySqlParameter[]? commandParameters)
        {
            var cmd = new MySqlCommand();
            var conn = GetConnection();
            try
            {
                PrepareCommand(cmd, conn, null, cmdText, cmdType, commandParameters);
                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return reader;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary> 
        /// 返回DataSet 
        /// </summary> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns></returns> 
        public static DataSet GetDataSet(string cmdText, CommandType cmdType = CommandType.Text,
            params MySqlParameter[]? commandParameters)
        {
            var cmd = new MySqlCommand();
            var conn = GetConnection();
            try
            {
                PrepareCommand(cmd, conn, null, cmdText, cmdType, commandParameters);
                var adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;
                var ds = new DataSet();

                adapter.Fill(ds);
                cmd.Parameters.Clear();
                conn.Close();
                return ds;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 用指定的数据库连接字符串执行一个命令并返回一个数据表 
        /// </summary>
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        public static DataTable GetDataTable(string cmdText, CommandType cmdType = CommandType.Text,
            params MySqlParameter[]? commandParameters)
        {
            var cmd = new MySqlCommand();
            var conn = GetConnection();
            try
            {
                PrepareCommand(cmd, conn, null, cmdText, cmdType, commandParameters);
                var adapter = new MySqlDataAdapter();
                adapter.SelectCommand = cmd;
                var ds = new DataTable();

                adapter.Fill(ds);
                cmd.Parameters.Clear();
                conn.Close();
                return ds;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary> 
        /// 用指定的数据库连接字符串执行一个命令并返回一个数据集的第一列 
        /// </summary> 
        /// <remarks> 
        ///例如: 
        /// Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>用 Convert.To{Type}把类型转换为想要的 </returns> 
        public static object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.Text,
            params MySqlParameter[]? commandParameters)
        {
            var cmd = new MySqlCommand();
            using var connection = GetConnection();
            PrepareCommand(cmd, connection, null, cmdText, cmdType, commandParameters);
            var val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }


        /// <summary> 
        /// 用指定的数据库连接字符串执行一个命令并返回一个数据集的第一列 
        /// </summary> 
        /// <remarks> 
        ///例如: 
        /// Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>用 Convert.To{Type}把类型转换为想要的 </returns> 
        public static string ExecuteScalarValue(string cmdText, CommandType cmdType = CommandType.Text,
            params MySqlParameter[]? commandParameters)
        {
            var cmd = new MySqlCommand();
            using var connection = GetConnection();
            PrepareCommand(cmd, connection, null, cmdText, cmdType, commandParameters);
            var val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();

            var str = "";
            if (val != null && val.ToString() != "")
            {
                str = val.ToString();
            }

            return str;
        }

        /// <summary>
        /// 返回插入值ID
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static object ExecuteNonExist(string cmdText, CommandType cmdType = CommandType.Text,
            params MySqlParameter[]? commandParameters)
        {
            var cmd = new MySqlCommand();
            using var connection = GetConnection();
            PrepareCommand(cmd, connection, null, cmdText, cmdType, commandParameters);
            object val = cmd.ExecuteNonQuery();

            return cmd.LastInsertedId;
        }

        /// <summary> 
        /// 用指定的数据库连接执行一个命令并返回一个数据集的第一列 
        /// </summary> 
        /// <remarks> 
        /// 例如: 
        /// Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new MySqlParameter("@prodid", 24)); 
        /// </remarks> 
        /// <param name="connection">一个存在的数据库连接</param> 
        /// <param name="cmdType">命令类型(存储过程, 文本, 等等)</param> 
        /// <param name="cmdText">存储过程名称或者sql命令语句</param> 
        /// <param name="commandParameters">执行命令所用参数的集合</param> 
        /// <returns>用 Convert.To{Type}把类型转换为想要的 </returns> 
        public static object ExecuteScalar(MySqlConnection connection, string cmdText,
            CommandType cmdType = CommandType.Text, params MySqlParameter[]? commandParameters)
        {
            var cmd = new MySqlCommand();
            PrepareCommand(cmd, connection, null, cmdText, cmdType, commandParameters);
            var val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }


        /// <summary> 
        /// 准备执行一个命令 
        /// </summary> 
        /// <param name="cmd">sql命令</param> 
        /// <param name="conn">OleDb连接</param> 
        /// <param name="trans">OleDb事务</param> 
        /// <param name="cmdType">命令类型例如 存储过程或者文本</param> 
        /// <param name="cmdText">命令文本,例如:Select * from Products</param> 
        /// <param name="cmdParams">执行命令的参数</param> 
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction? trans,
            string cmdText, CommandType cmdType = CommandType.Text, params MySqlParameter[]? cmdParams)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }

            cmd.CommandType = cmdType;
            if (cmdParams != null)
            {
                foreach (var param in cmdParams)
                {
                    cmd.Parameters.Add(param);
                }
            }
        }
    }
}