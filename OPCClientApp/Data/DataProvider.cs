using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCClientApp.Data
{
    public sealed class DataProvider : IDisposable
    {
        public DataProvider()
        {
        }

        public DataProvider(bool connect)
        {
            if (connect)
                Connect();
        }

        private string sqlString = string.Empty;
        private string errDescription = string.Empty;
        private int errLine = 0;
        private DbProviderFactory providerFactory = null;
        private IDbConnection connection = null;
        private IDbCommand command = null;

        public bool Connect()
        {
            try
            {
                if (providerFactory == null)
                {
                    providerFactory = DbProviderFactories.GetFactory(ProviderName);
                }
                if (connection == null)
                {
                    connection = providerFactory.CreateConnection();
                    connection.ConnectionString = ConnectionString;
                }
                command = connection.CreateCommand();

                if (connection.State != ConnectionState.Open)
                    connection.Open();
                return connection.State == ConnectionState.Open;
            }
            catch (DbException dbexc)
            {
                errLine = 68;
                errDescription = dbexc.Message;
                System.Diagnostics.Trace.WriteLine(dbexc.Message);
                System.Diagnostics.Trace.WriteLine(dbexc.StackTrace);
            }
            catch (Exception exc)
            {
                errLine = 75;
                errDescription = exc.Message;
                System.Diagnostics.Trace.WriteLine(exc.Message);
                System.Diagnostics.Trace.WriteLine(exc.StackTrace);
            }
            return false;
        }

        public IDataReader Select()
        {
            return Select(CommandBehavior.Default);
        }

        public IDataReader Select(CommandBehavior behavior)
        {
            try
            {
                if (!IsConnected)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return null;
                }
                command.CommandText = sqlString;
                command.CommandTimeout = 3000;
                return command.ExecuteReader(behavior);
            }
            catch (DbException dbexc)
            {
                errLine = 258;
                errDescription = dbexc.Message;
                System.Diagnostics.Trace.WriteLine(dbexc.Message);
                System.Diagnostics.Trace.WriteLine(dbexc.StackTrace);
            }
            catch (Exception exc)
            {
                errLine = 263;
                errDescription = exc.Message;
                System.Diagnostics.Trace.WriteLine(exc.Message);
                System.Diagnostics.Trace.WriteLine(exc.StackTrace);
            }
            return null;
        }

        public DataTable GetTable()
        {
            try
            {
                if (!IsConnected)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return null;
                }
                command.CommandText = sqlString;
                command.CommandTimeout = 3000;
                DbDataAdapter dtAdapter = providerFactory.CreateDataAdapter();
                dtAdapter.SelectCommand = (DbCommand)command;
                DataTable dtTable = new DataTable();
                dtAdapter.Fill(dtTable);
                return dtTable;

            }
            catch (DbException dbexc)
            {
                errLine = 291;
                errDescription = dbexc.Message;
                System.Diagnostics.Trace.WriteLine(dbexc.Message);
                System.Diagnostics.Trace.WriteLine(dbexc.StackTrace);
            }
            catch (Exception exc)
            {
                errLine = 298;
                errDescription = exc.Message;
                System.Diagnostics.Trace.WriteLine(exc.Message);
                System.Diagnostics.Trace.WriteLine(exc.StackTrace);
            }
            return null;
        }

        public DataTable GetTable(int TimeOut)
        {
            try
            {
                if (!IsConnected)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return null;
                }

                command.CommandText = sqlString;
                command.CommandTimeout = TimeOut;
                DbDataAdapter dtAdapter = providerFactory.CreateDataAdapter();
                dtAdapter.SelectCommand = (DbCommand)command;
                DataTable dtTable = new DataTable();
                dtAdapter.Fill(dtTable);
                return dtTable;

            }
            catch (DbException dbexc)
            {
                errLine = 327;
                errDescription = dbexc.Message;
                System.Diagnostics.Trace.WriteLine(dbexc.Message);
                System.Diagnostics.Trace.WriteLine(dbexc.StackTrace);
            }
            catch (Exception exc)
            {
                errLine = 334;
                errDescription = exc.Message;
                System.Diagnostics.Trace.WriteLine(exc.Message);
                System.Diagnostics.Trace.WriteLine(exc.StackTrace);
            }
            return null;
        }

        public object Scalar(string strSql)
        {
            sqlString = strSql;
            return Scalar();
        }

        public object Scalar()
        {
            try
            {
                if (!IsConnected)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return null;
                }
                command.CommandText = sqlString;
                command.CommandTimeout = 3000;
                return command.ExecuteScalar();
            }
            catch (DbException dbexc)
            {
                errLine = 362;
                errDescription = dbexc.Message;
                System.Diagnostics.Trace.WriteLine(dbexc.Message);
                System.Diagnostics.Trace.WriteLine(dbexc.StackTrace);
            }
            catch (Exception exc)
            {
                errLine = 370;
                errDescription = exc.Message;
                System.Diagnostics.Trace.WriteLine(exc.Message);
                System.Diagnostics.Trace.WriteLine(exc.StackTrace);
            }
            return null;
        }

        public int Execute()
        {
            try
            {
                if (!IsConnected)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return -3;
                }
                command.CommandText = sqlString;
                command.CommandTimeout = 3000;
                int rowCount = command.ExecuteNonQuery();
                command.Parameters.Clear();
                return rowCount;
            }
            catch (DbException dbexc)
            {
                errLine = 395;
                errDescription = dbexc.Message;
                System.Diagnostics.Trace.WriteLine(dbexc.Message);
                System.Diagnostics.Trace.WriteLine(dbexc.StackTrace);
            }
            catch (Exception exc)
            {
                errLine = 402;
                errDescription = exc.Message;
                System.Diagnostics.Trace.WriteLine(exc.Message);
                System.Diagnostics.Trace.WriteLine(exc.StackTrace);
            }
            return -4;
        }

        public int Execute(IDbDataParameter[] parameters)
        {
            try
            {
                if (!IsConnected)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return -3;
                }
                command.CommandText = sqlString;
                command.CommandTimeout = 3000;
                command.Parameters.Clear();
                if (parameters != null && parameters.Length > 0)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        command.Parameters.Add(parameters[i]);
                    }
                }
                int rowCount = command.ExecuteNonQuery();
                command.Parameters.Clear();
                return rowCount;
            }
            catch (DbException dbexc)
            {
                errLine = 395;
                errDescription = dbexc.Message;
                System.Diagnostics.Trace.WriteLine(dbexc.Message);
                System.Diagnostics.Trace.WriteLine(dbexc.StackTrace);
            }
            catch (Exception exc)
            {
                errLine = 402;
                errDescription = exc.Message;
                System.Diagnostics.Trace.WriteLine(exc.Message);
                System.Diagnostics.Trace.WriteLine(exc.StackTrace);
            }
            return -4;
        }

        public int Execute(out IDbDataParameter parameter, string outParameter)
        {
            parameter = null;

            try
            {
                if (!IsConnected)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return -3;
                }
                command.CommandText = sqlString;
                command.CommandTimeout = 3000;
                int rowCount = command.ExecuteNonQuery();
                parameter = (IDbDataParameter)command.Parameters[outParameter];

                command.Parameters.Clear();
                return rowCount;
            }
            catch (DbException dbexc)
            {
                errLine = 431;
                errDescription = dbexc.Message;
                System.Diagnostics.Trace.WriteLine(dbexc.Message);
                System.Diagnostics.Trace.WriteLine(dbexc.StackTrace);
            }
            catch (Exception exc)
            {
                errLine = 438;
                errDescription = exc.Message;
                System.Diagnostics.Trace.WriteLine(exc.Message);
                System.Diagnostics.Trace.WriteLine(exc.StackTrace);
            }
            return -4;
        }

        public void Begin()
        {
            try
            {
                if (connection == null || command == null)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return;
                }
                command.Transaction = connection.BeginTransaction();
            }
            catch (DbException dbexc)
            {
                errLine = 459;
                errDescription = dbexc.Message;
                System.Diagnostics.Trace.WriteLine(dbexc.Message);
                System.Diagnostics.Trace.WriteLine(dbexc.StackTrace);
            }
            catch (Exception exc)
            {
                errLine = 466;
                errDescription = exc.Message;
                System.Diagnostics.Trace.WriteLine(exc.Message);
                System.Diagnostics.Trace.WriteLine(exc.StackTrace);
            }
        }

        public void Commit()
        {
            try
            {
                if (connection == null || command == null)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return;
                }
                if (command.Transaction == null)
                {
                    errDescription = "Once transaction baslatilmalidir!";
                    return;
                }
                command.Transaction.Commit();
            }
            catch (DbException dbexc)
            {
                errLine = 491;
                errDescription = dbexc.Message;
                System.Diagnostics.Trace.WriteLine(dbexc.Message);
                System.Diagnostics.Trace.WriteLine(dbexc.StackTrace);
            }
            catch (Exception exc)
            {
                errLine = 498;
                errDescription = exc.Message;
                System.Diagnostics.Trace.WriteLine(exc.Message);
                System.Diagnostics.Trace.WriteLine(exc.StackTrace);
            }
        }

        public void Rollback()
        {
            try
            {
                if (connection == null || command == null)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return;
                }
                if (command.Transaction == null)
                {
                    errDescription = "Once transaction baslatilmalidir!";
                    return;
                }
                command.Transaction.Rollback();
            }
            catch (DbException dbexc)
            {
                errLine = 523;
                errDescription = dbexc.Message;
                System.Diagnostics.Trace.WriteLine(dbexc.Message);
                System.Diagnostics.Trace.WriteLine(dbexc.StackTrace);
            }
            catch (Exception exc)
            {
                errLine = 530;
                errDescription = exc.Message;
                System.Diagnostics.Trace.WriteLine(exc.Message);
                System.Diagnostics.Trace.WriteLine(exc.StackTrace);
            }
        }

        public bool IsConnected
        {
            get
            {
                return (connection != null && connection.State == ConnectionState.Open);
            }
        }

        #region Parameters
        public int AddParameter(IDbDataParameter param)
        {
            try
            {
                if (connection == null || command == null)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return -1;
                }
                return command.Parameters.Add(param);
            }
            catch (DbException dbexc)
            {
                errDescription = dbexc.Message;
            }
            catch (Exception exc)
            {
                errDescription = exc.Message;
            }
            return -1;
        }
        public int AddParameter(string name, object val)
        {
            try
            {
                if (connection == null || command == null)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return -1;
                }
                IDbDataParameter param1 = command.CreateParameter();
                param1.ParameterName = name;
                param1.Value = val;
                return command.Parameters.Add(param1);
            }
            catch (DbException dbexc)
            {
                errDescription = dbexc.Message;
            }
            catch (Exception exc)
            {
                errDescription = exc.Message;
            }
            return -1;
        }
        public int AddParameter(string name, object val, ParameterDirection direction)
        {
            try
            {
                if (connection == null || command == null)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return -1;
                }
                IDbDataParameter param1 = command.CreateParameter();
                param1.ParameterName = name;
                param1.Value = val;
                param1.Direction = direction;
                return command.Parameters.Add(param1);
            }
            catch (DbException dbexc)
            {
                errDescription = dbexc.Message;
            }
            catch (Exception exc)
            {
                errDescription = exc.Message;
            }
            return -1;
        }
        public int AddParameter(string name, System.Data.DbType dbType, ParameterDirection direction)
        {
            try
            {
                if (connection == null || command == null)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return -1;
                }
                IDbDataParameter param1 = command.CreateParameter();
                param1.ParameterName = name;
                param1.DbType = dbType;
                param1.Direction = direction;
                return command.Parameters.Add(param1);
            }
            catch (DbException dbexc)
            {
                errDescription = dbexc.Message;
            }
            catch (Exception exc)
            {
                errDescription = exc.Message;
            }
            return -1;
        }
        public void ClearParameters()
        {
            try
            {
                if (connection == null || command == null)
                {
                    errDescription = "Once baglanti acilmalidir!";
                    return;
                }
                command.Parameters.Clear();
            }
            catch (DbException dbexc)
            {
                errDescription = dbexc.Message;
            }
            catch (Exception exc)
            {
                errDescription = exc.Message;
            }
            return;
        }
        #endregion

        public string ProviderName { get; set; } = "System.Data.SqlClient";
        public string ConnectionString { get; set; } = "packet size=4096;data source=192.168.9.71;persist security info=False;initial catalog=ors_2016;Connect Timeout=50;User=mikrobar;Password=Cw9QNMU8;Pooling=False;";

        public string SqlString
        {
            get { return sqlString; }
            set { sqlString = value; }
        }

        public int ErrorLine
        {
            get
            {
                return errLine;
            }
        }

        public string ErrorDesc
        {
            get
            {
                if (!string.IsNullOrEmpty(errDescription) && errDescription.Length > 100)
                    return errDescription.Substring(0, 100);
                return errDescription;
            }
        }

        #region IDisposable
        ~DataProvider()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (connection!=null )
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                    connection.Dispose();
                }
                if (command != null)
                {
                    command.Dispose();
                }
                providerFactory = null;
                connection = null;
                command = null;
                SqlConnection.ClearAllPools();
            }

            disposed = true;
        }
        #endregion

    }
}
