using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models
{
    public class Conexion
    {
        private static readonly object objectLock = new object();
        private bool _disposed;
        private SqlConnection conn = new SqlConnection("Server=tcp:fortunesp.database.windows.net,1433;Initial Catalog=FortuneTest;Persist Security Info=False;User ID=AdminFB;Password=Admin@2018;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=10000;");
        //A!3$$traFoR7un3
        //private SqlConnection conn = new SqlConnection("Server=localhost/FORTUNE;Database=FortuneTest;User Id=administrator; Password=A!3$$traFoR7un3; Connection Timeout=10000;");
        //private SqlConnection conn = new SqlConnection("Server=localhost;Database=FortuneTest;User Id=administrator; Password=A!3$$traFoR7un3; Connection Timeout=10000;");

        public SqlConnection AbrirConexion()
        {
            if (conn.State == ConnectionState.Closed)
            {               
                    conn.Open();             
            }
            return conn;
        }
        public SqlConnection CerrarConexion()
        {
            if(conn.State == ConnectionState.Closed)
            {
                conn.Close();
                SqlConnection.ClearPool(conn);
            }
            return conn;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (objectLock)
            {
                if (_disposed == false)
                {
                    if (disposing == true)
                    {
                        if (conn != null)
                        {
                            conn.Dispose();
                            conn = null;
                        }              
                        _disposed = true;
                    }
                }
            }
        }


    }
}