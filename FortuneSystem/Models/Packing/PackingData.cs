using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.Pedidos;
using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Packing
{
    public class PackingData
    {
        PedidosData objPedido = new PedidosData();
        CatUsuarioData objCatUser = new CatUsuarioData();
        //Muestra la lista de tallas de Packing por estilo
        public IEnumerable<PackingM> ListaTallasPacking(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Packing";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                int i = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        Talla = leer["TALLA"].ToString()        

                    };
                    listTallas.Add(tallas);
                    i++;
                }
                if (i == 0)
                {
                    listTallas = ObtenerTallas(id);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }            

            return listTallas;
        }

        //Muestra la lista de tallas TOTAL de Packing por estilo
        public List<PackingM> ObtenerTallas(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;                
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select DISTINCT(S.ORDEN), S.TALLA from ITEM_SIZE I  " +
                   "INNER JOIN CAT_ITEM_SIZE S ON S.ID=I.TALLA_ITEM " +
                   "WHERE I.ID_SUMMARY= '" + id + "' ";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        Talla = leer["TALLA"].ToString()

                    };
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }             
            return listTallas;
        }

        //Muestra la lista de tallas de Packing Assort por estilo
        public List<PackingM> ObtenerTallasAssort(int? id, string namePack)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            List<PackingTypeSize> listadoEstilos = ObtenerListadoEstilosPackAssort(id, namePack);
            string valores = "";
            for (int v = 0; v < listadoEstilos.Count; v++)
            {
                if (v > 0)
                {
                    valores += "," + listadoEstilos[v].IdSummary;
                }
                else
                {
                    valores += listadoEstilos[v].IdSummary;
                }

            }
            string query = valores;
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select DISTINCT(S.ORDEN), S.TALLA from ITEM_SIZE I  " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=I.TALLA_ITEM " +
                    "WHERE I.ID_SUMMARY in(" + query + ")";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        Talla = leer["TALLA"].ToString()

                    };
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            return listTallas;
        }

        //Muestra la lista de ratios packing Assort
        public IEnumerable<PackingTypeSize> ListaRatiosPackAssort(int? id, string namePack, int numBlock)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            List<PackingTypeSize> listadoEstilos = ObtenerListadoEstilosPackAssort(id, namePack);
            string valores = "";
            for (int v = 0; v < listadoEstilos.Count; v++)
            {
                if (v > 0)
                {
                    valores += "," + listadoEstilos[v].IdSummary;
                }
                else
                {
                    valores += listadoEstilos[v].IdSummary;
                }

            }
            string query = valores;
            int numTotalCartones = ObtenerCantCartonesAssort(id,numBlock);
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select s.talla, pt.ratio FROM PACKING_TYPE_SIZE pt " +
                       "INNER JOIN CAT_ITEM_SIZE S ON S.ID=pt.id_talla " +
                       "WHERE pt.PACKING_NAME='"+namePack+"' and pt.ID_SUMMARY in(" + query + ") and type_packing=3 ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                int cant = 0;
                while (leer.Read())
                {
                    PackingTypeSize packing = new PackingTypeSize() {
                        Talla = leer["TALLA"].ToString()
                    };

                    packing.Ratio += Convert.ToInt32(leer["RATIO"]);

                    PackingTypeSize result = listTallas.Find(x => x.Talla == packing.Talla);
                    if (result == null)
                    {
                        listTallas.Add(packing);
                    }
                    else
                    {
                        if (result.Talla == packing.Talla)
                        {
                            cant = result.Ratio + packing.Ratio;
 
                            result.Ratio = cant;
                        }
                    }

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            for (int v = 0; v < listTallas.Count; v++)
            {
               
                listTallas[v].TotalPiezas = listTallas[v].Ratio * numTotalCartones;
            }
            return listTallas;
        }

        //Muestra la lista de estilos registrados en packing assort
        public List<PackingTypeSize> ObtenerListadoEstilosPackAssort(int? id, string packName)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listEstilos = new List<PackingTypeSize>();
            int pedidoId = Convert.ToInt32(id);
            string query = ObtenerEstilosPorIdPedido(pedidoId);
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT DISTINCT id_summary FROM PACKING_TYPE_SIZE " +
                    "WHERE PACKING_NAME='"+ packName + "' and ID_SUMMARY in(" + query + ") and type_packing=3";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingTypeSize estilos = new PackingTypeSize()
                    {
                        IdSummary = Convert.ToInt32(leer["ID_SUMMARY"])

                    };
                    listEstilos.Add(estilos);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            return listEstilos;
        }

        public List<PackingM> ObtenerCajasPacking(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;

                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT P.ID_PACKING, P.CANT_BOX, P.TOTAL_PIECES FROM PACKING P, CAT_ITEM_SIZE S " +
                    "WHERE P.ID_SUMMARY= '" + id + "' AND S.ID=P.ID_TALLA  ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {

                        IdPacking = Convert.ToInt32(leer["ID_PACKING"]),
                        CantBox = Convert.ToInt32(leer["CANT_BOX"]),
                        TotalPiezas = Convert.ToInt32(leer["TOTAL_PIECES"])

                    };

                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();

            }           

            return listTallas;
        }


        //Muestra la lista de tallas TOTAL de Packing por estilo
        public List<PackingSize> ListaTallasCalidadPack(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingSize> listTallas = new List<PackingSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select S.TALLA " +
                    "from PACKING_SIZE PZ, CAT_ITEM_SIZE S " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "' AND S.ID=PZ.ID_TALLA ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingSize tallas = new PackingSize()
                    {
                        Talla = leer["TALLA"].ToString()
                    };
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }           

            return listTallas;
        }

        //Muestra la lista de tallas de Packing Size por estilo
        public List<PackingSize> ObtenerListaPackingSizePorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingSize> listTallas = new List<PackingSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;             
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select PZ.ID_PACKING_SIZE, PZ.ID_TALLA, S.TALLA, PZ.QUALITY " +
                    "from PACKING_SIZE PZ, CAT_ITEM_SIZE S " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "' AND S.ID=PZ.ID_TALLA ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {

                    PackingSize tallas = new PackingSize()
                    {
                        IdPackingSize = Convert.ToInt32(leer["ID_PACKING_SIZE"]),
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString(),
                        Calidad = Convert.ToInt32(leer["QUALITY"])
                    };
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
            }           

            return listTallas;
        }

        //Muestra la lista de tallas de Packing Type Size por estilo
        public List<PackingTypeSize> ObtenerListaPackingTypeSizePorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;              
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select PZ.ID_PACKING_TYPE_SIZE, PZ.ID_TALLA, S.TALLA, PZ.PIECES, PZ.RATIO, PZ.TYPE_PACKING " +
                    "from PACKING_TYPE_SIZE PZ, CAT_ITEM_SIZE S " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "' AND PZ.TYPE_PACKING IN(1,2) AND S.ID=PZ.ID_TALLA ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingTypeSize tallas = new PackingTypeSize()
                    {
                        IdPackingTypeSize = Convert.ToInt32(leer["ID_PACKING_TYPE_SIZE"]),
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString(),
                        Pieces = Convert.ToInt32(leer["PIECES"]),
                        Ratio = Convert.ToInt32(leer["RATIO"]),
                        IdTipoEmpaque = Convert.ToInt32(leer["TYPE_PACKING"])
                    };
                    ObtenerNombreTipoEmpaque(tallas);
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }     

            return listTallas;
        }

        //Buscar un estilo en Packing Typesize
        public List<PackingTypeSize> BuscarPackingTypeSizePorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select * from PACKING_TYPE_SIZE PZ " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "' ";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingTypeSize tallas = new PackingTypeSize()
                    {
                        IdPackingTypeSize = Convert.ToInt32(leer["ID_PACKING_TYPE_SIZE"]),
                     
                    };
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas de Packing Type Size por estilo
        public List<PackingTypeSize> ObtenerListaPackingTypeHTPorEstilo(int? id, int numeroPO, int tEmpaque)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select PZ.ID_PACKING_TYPE_SIZE, PZ.ID_TALLA, S.TALLA, PZ.QTY, PZ.CARTONES, PZ.PARTIAL_NO, PZ.TYPE_PACKING, PZ.RATIO,PZ.TOTAL_UNITS, PZ.TOTAL_CARTONS  " +
                    "from PACKING_TYPE_SIZE PZ " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=pz.ID_TALLA   " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "' and pz.type_packing= '" + tEmpaque + "' and pz.number_po= '" + numeroPO + "'  ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingTypeSize tallas = new PackingTypeSize()
                    {
                        IdPackingTypeSize = Convert.ToInt32(leer["ID_PACKING_TYPE_SIZE"]),
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString(),
                        Cantidad = Convert.ToInt32(leer["QTY"]),
                        Cartones = Convert.ToInt32(leer["CARTONES"]),
                        PartialNumber = Convert.ToInt32(leer["PARTIAL_NO"]),
                        IdTipoEmpaque = Convert.ToInt32(leer["TYPE_PACKING"]),
                        Ratio = Convert.ToInt32(leer["ratio"]),
                        TotalUnits = Convert.ToInt32(leer["TOTAL_UNITS"]),
                        TotalCartones = Convert.ToInt32(leer["TOTAL_CARTONS"])
                    };
                    ObtenerNombreTipoEmpaque(tallas);
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas de Packing Type Size por estilo
        public List<PackingTypeSize> ObtenerListaPackingTypeBulkHTBox(int? id, int numeroPO, int tEmpaque)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select PZ.ID_PACKING_TYPE_SIZE, PZ.ID_TALLA, S.TALLA, PZ.QTY, PZ.CARTONES, PZ.PARTIAL_NO, PZ.TYPE_PACKING,P.CANT_BOX, PZ.RATIO, " +
                    "P.TOTAL_PIECES, PZ.TOTAL_CARTONS,PZ.TOTAL_UNITS " +
                    "from PACKING_TYPE_SIZE PZ " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=pz.ID_TALLA  " +
                    "INNER JOIN PACKING P ON P.ID_PACKING_TYPE_SIZE=PZ.ID_PACKING_TYPE_SIZE " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "' and pz.type_packing= '" + tEmpaque + "' and pz.number_po= '" + numeroPO + "'  ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingTypeSize tallas = new PackingTypeSize()
                    {
                        IdPackingTypeSize = Convert.ToInt32(leer["ID_PACKING_TYPE_SIZE"]),
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString(),
                        Cantidad = Convert.ToInt32(leer["QTY"]),
                        Cartones = Convert.ToInt32(leer["CARTONES"]),
                        PartialNumber = Convert.ToInt32(leer["PARTIAL_NO"]),
                        IdTipoEmpaque = Convert.ToInt32(leer["TYPE_PACKING"]),
                        Ratio = Convert.ToInt32(leer["RATIO"]),
                        TotalCartones = Convert.ToInt32(leer["TOTAL_CARTONS"]),
                        TotalUnits = Convert.ToInt32(leer["TOTAL_UNITS"])
                        
                    };
                    PackingM packingM = new PackingM();
                    packingM.CantBox = Convert.ToInt32(leer["CANT_BOX"]);
                    packingM.TotalPiezas = Convert.ToInt32(leer["TOTAL_PIECES"]);
                    ObtenerNombreTipoEmpaque(tallas);
                    tallas.PackingM = packingM;
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas de Packing Type Size ppk por estilo
        public List<PackingTypeSize> ObtenerListaPackingPPKPorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                /*/ comando.CommandText = "select  distinct (PZ.ID_TALLA), S.TALLA, PZ.RATIO " +
                     "from PACKING_TYPE_SIZE PZ, CAT_ITEM_SIZE S " +
                     "WHERE PZ.ID_SUMMARY= '" + id + "'and PZ.type_packing = 2 AND S.ID=PZ.ID_TALLA ORDER BY S.TALLA";*/
                comando.CommandText = "select DISTINCT(number_po), id_talla, s.talla, ratio from packing_type_size  " +
                                      "INNER JOIN CAT_ITEM_SIZE S ON S.ID=ID_TALLA " +
                                      " WHERE ID_SUMMARY= '" + id + "'and type_packing = 2";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingTypeSize tallas = new PackingTypeSize()
                    {
                        NumberPO = Convert.ToInt32(leer["number_po"]),
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString(),                     
                        Ratio = Convert.ToInt32(leer["RATIO"])
                    };
                   
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas de Batch por estilo
        public IEnumerable<PackingTypeSize> ObtenerListaPackingPPK(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select DISTINCT(number_po) from packing_type_size where ID_SUMMARY= '" + id + "'and type_packing = 2";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingTypeSize tallas = new PackingTypeSize()
                    {

                        NumberPO = Convert.ToInt32(leer["number_po"])
                    };

                    tallas.ListaEmpaque = ListaTallasNumeroPO(tallas.NumberPO, id);
                    foreach (var item in tallas.ListaEmpaque)
                    {
                        tallas.NumberPO = item.NumberPO;
                        tallas.Talla = item.Talla;
                        tallas.IdTalla = item.IdTalla;
                        tallas.Ratio = item.Ratio;
                        tallas.TotalRatio = item.TotalRatio;

                    }
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
            }

            return listTallas;
        }

        //Muestra de Packing Type Size por estilo
        public List<PackingTypeSize> ObtenerCajasPackingPorEstilo(int? id, int idBatch)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;              
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = " select P.ID_PACKING,P.CANT_BOX, P.TOTAL_PIECES, PZ.ID_PACKING_TYPE_SIZE, PZ.ID_TALLA, S.TALLA, PZ.PIECES, PZ.RATIO, PZ.TYPE_PACKING, P.ID_BATCH, P.PARTIAL " +
                    "from PACKING_TYPE_SIZE PZ, CAT_ITEM_SIZE S, PACKING P " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "'AND P.ID_BATCH='" + idBatch + " ' AND S.ID=PZ.ID_TALLA AND PZ.ID_PACKING_TYPE_SIZE=P.ID_PACKING_TYPE_SIZE ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {

                    PackingM pack = new PackingM();
                    PackingTypeSize tallas = new PackingTypeSize();
                    tallas.IdPackingTypeSize = Convert.ToInt32(leer["ID_PACKING_TYPE_SIZE"]);
                    tallas.IdTalla = Convert.ToInt32(leer["ID_TALLA"]);
                    tallas.Talla = leer["TALLA"].ToString();
                    tallas.Pieces = Convert.ToInt32(leer["PIECES"]);
                    tallas.Ratio = Convert.ToInt32(leer["RATIO"]);
                    tallas.IdTipoEmpaque = Convert.ToInt32(leer["TYPE_PACKING"]);
                    pack.IdPacking = Convert.ToInt32(leer["ID_PACKING"]);
                    pack.CantBox = Convert.ToInt32(leer["CANT_BOX"]);
                    pack.TotalPiezas = Convert.ToInt32(leer["TOTAL_PIECES"]);
                    if (!Convert.IsDBNull(leer["PARTIAL"]))
                    {
                        pack.Partial = Convert.ToInt32(leer["PARTIAL"]);
                    }
                    tallas.PackingM = pack;
                    ObtenerNombreTipoEmpaque(tallas);
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }          

            return listTallas;
        }

        //Muestra de Packing Type Size por estilo
        public List<PackingTypeSize> ObtenerCajasPackingPPKPorEstilo(int? id, int idBatch)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = " select P.ID_PACKING,P.CANT_BOX, P.TOTAL_PIECES, PZ.ID_PACKING_TYPE_SIZE, PZ.ID_TALLA, S.TALLA, PZ.PIECES, PZ.RATIO, PZ.TYPE_PACKING " +
                    "from PACKING_TYPE_SIZE PZ, CAT_ITEM_SIZE S, PACKING P " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "'  AND ID_BATCH='" + idBatch + " ' AND S.ID=PZ.ID_TALLA AND PZ.ID_PACKING_TYPE_SIZE=P.ID_PACKING_TYPE_SIZE ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {

                    PackingM pack = new PackingM();
                    PackingTypeSize tallas = new PackingTypeSize();
                    tallas.IdPackingTypeSize = Convert.ToInt32(leer["ID_PACKING_TYPE_SIZE"]);
                    tallas.IdTalla = Convert.ToInt32(leer["ID_TALLA"]);
                    tallas.Talla = leer["TALLA"].ToString();
                    tallas.Pieces = Convert.ToInt32(leer["PIECES"]);
                    tallas.Ratio = Convert.ToInt32(leer["RATIO"]);
                    tallas.IdTipoEmpaque = Convert.ToInt32(leer["TYPE_PACKING"]);
                    pack.IdPacking = Convert.ToInt32(leer["ID_PACKING"]);
                    pack.CantBox = Convert.ToInt32(leer["CANT_BOX"]);
                    pack.TotalPiezas = Convert.ToInt32(leer["TOTAL_PIECES"]);

                    tallas.PackingM = pack;
                    ObtenerNombreTipoEmpaque(tallas);
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }         

            return listTallas;
        }

        //Muestra la lista de tallas de Packing Type Size por estilo
        public List<PackingTypeSize> ObtenerListaPackingTypeSizePiezasyRatioPorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select PZ.ID_PACKING_TYPE_SIZE,PZ.PIECES, PZ.RATIO " +
                    "from PACKING_TYPE_SIZE PZ " +
                    "WHERE PZ.ID_SUMMARY= '" + id + "'";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {

                    PackingTypeSize tallas = new PackingTypeSize()
                    {
                        IdPackingTypeSize = Convert.ToInt32(leer["ID_PACKING_TYPE_SIZE"]),
                        Pieces = Convert.ToInt32(leer["PIECES"]),
                        Ratio = Convert.ToInt32(leer["RATIO"])
                    };
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }      

            return listTallas;
        }
        public void ObtenerNombreTipoEmpaque(PackingTypeSize packingTypeSize)
        {
            switch (packingTypeSize.IdTipoEmpaque)
            {
                case 1:
                    packingTypeSize.NombreTipoPak = "BULK";
                    break;
                case 2:
                    packingTypeSize.NombreTipoPak = "PPK";
                    break;
                case 3:
                    packingTypeSize.NombreTipoPak = "ASSORTMENT";
                    break;
                default:
                    packingTypeSize.NombreTipoPak = "-";
                    break;
            }
        }

        //Muestra la lista de tallas de Batch por id
        public IEnumerable<PackingM> ListaTallasBatchId(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Batch_Packing";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        Talla = leer["TALLA"].ToString(),
                        IdBatch = Convert.ToInt32(leer["ID_BATCH"])


                    };

                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }          

            return listTallas;
        }
       

        //Muestra la lista de tallas TOTAL de Packing por estilo
        public IEnumerable<PackingM> ListaTallasTotalPacking(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Total_Packing";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {

                        // Printed = Convert.ToInt32(leer["TOTAL"]),
                        
                    };

                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }     

            return listTallas;
        }

        //Muestra la lista de tallas TOTAL de Packing por estilo
        public IEnumerable<int> ListaTotalTallasPackingBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT S.ORDEN,T.ID_TALLA, S.TALLA FROM PACKING T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' GROUP by S.ORDEN, T.ID_TALLA, S.TALLA ORDER BY cast(S.ORDEN AS int) ASC ";
                leer = comando.ExecuteReader();
                int total = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
                    total = SumaTotalBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(total);

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
            }      

            return listTallas;
        }

        //Muestra la lista de tallas Partial de Packing Bulk por estilo
        public IEnumerable<int> ListaTallasPartialPackingBulkEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT S.ORDEN,T.ID_TALLA, S.TALLA FROM PACKING T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' GROUP by S.ORDEN, T.ID_TALLA, S.TALLA ORDER BY cast(S.ORDEN AS int) ASC ";
                leer = comando.ExecuteReader();
                int total = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
                    total = PartialPacking(id, tallas.IdTalla);
                    listTallas.Add(total);

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
            }

            return listTallas;
        }

        //Muestra la lista de tallas TOTAL Extra de PACKING por estilo
        public IEnumerable<int> ListaTotalETallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT S.ORDEN,T.ID_TALLA, S.TALLA FROM PACKING T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "'  GROUP by S.ORDEN, T.ID_TALLA, S.TALLA ORDER BY cast(S.ORDEN AS int) ASC ";
                leer = comando.ExecuteReader();
                int totalExtra = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
                    totalExtra = SumaTotalExtraBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(totalExtra);
                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            } 
            return listTallas;
        }

        //Muestra la lista de tallas TOTAL Defect de PACKING por estilo
        public IEnumerable<int> ListaTotalDefTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT S.ORDEN,T.ID_TALLA, S.TALLA FROM PACKING T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' GROUP by S.ORDEN, T.ID_TALLA, S.TALLA ORDER BY cast(S.ORDEN AS int) ASC ";
                leer = comando.ExecuteReader();
                int totalDefect = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
                    totalDefect = SumaTotalDefectBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(totalDefect);
                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }      
                return listTallas;
        }

        //Muestra la lista de tallas TOTAL Cajas de PACKING por estilo
        public IEnumerable<int> ListaTotalCajasTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT S.ORDEN,T.ID_TALLA, S.TALLA FROM PACKING T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "'  GROUP by S.ORDEN, T.ID_TALLA, S.TALLA ORDER BY cast(S.ORDEN AS int) ASC ";
                leer = comando.ExecuteReader();
                int totalCajas = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    }; 
                     totalCajas = SumaTotalCajasdBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(totalCajas);

                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }    
            
            return listTallas;
        }

        //Muestra la lista de tallas TOTAL Cajas de PACKING por estilo bulk HT
        public IEnumerable<int> ListaTotalCajasTallasBatchBulkHTEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT  S.ORDEN,T.ID_TALLA, S.TALLA FROM PACKING T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "'  GROUP by S.ORDEN, T.ID_TALLA, S.TALLA ORDER BY cast(S.ORDEN AS int) ASC ";
                leer = comando.ExecuteReader();
                int totalCajas = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
                    totalCajas = SumaTotalCajasdBacheBulkHTTalla(id, tallas.IdTalla);
                    listTallas.Add(totalCajas);

                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas de Packing bulk por estilo
        public IEnumerable<int> ListaTallasPackingBulkEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT  S.ORDEN,T.ID_TALLA, S.TALLA FROM packing_type_size T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' and t.type_packing = 1 GROUP by S.ORDEN, T.ID_TALLA, S.TALLA ORDER BY cast(S.ORDEN AS int) ASC ";
                leer = comando.ExecuteReader();
                int totalPiezas = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
                    totalPiezas = SumaTotalPiezasTalla(id, tallas.IdTalla);
                    listTallas.Add(totalPiezas);

                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas de Packing PPK HT por estilo
        public IEnumerable<int> ListaTallasPackingPPKEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT S.ORDEN,T.ID_TALLA, S.TALLA FROM packing_type_size T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' and t.type_packing = 2 GROUP by S.ORDEN, T.ID_TALLA, S.TALLA ORDER BY cast(S.ORDEN AS int) ASC ";
                leer = comando.ExecuteReader();
                int totalPiezas = 0;
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
                   // totalPiezas = SumaTotalPiezasPPKTalla(id, tallas.IdTalla);
                    listTallas.Add(totalPiezas);

                }
                leer.Close();
            }
            finally
            {

                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de suma de  tallas por Batch
        public int SumaTotalBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT TOTAL_PIECES  FROM PACKING WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();                
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["TOTAL_PIECES"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return suma;
        }

        //Muestra la lista de partial 
        public int PartialPacking(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT PARTIAL  FROM PACKING WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["PARTIAL"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return suma;
        }

        //Muestra la lista de suma de Cajas tallas por Batch
        public int SumaTotalCajasdBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT CANT_BOX  FROM PACKING WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["CANT_BOX"]);

                }
                leerF.Close();
            }
            finally
            {

                conex.CerrarConexion();
                conex.Dispose();
            }   
            
            return suma;
        }

        //Muestra la lista de suma de Cajas tallas por Batch Bulk HT
        public int SumaTotalCajasdBacheBulkHTTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT P.CANT_BOX, PZ.QTY  FROM PACKING P, PACKING_TYPE_SIZE PZ WHERE P.ID_SUMMARY='" + idEstilo + "' " +
                                  "AND P.ID_TALLA='" + idTalla + "' AND PZ.ID_PACKING_TYPE_SIZE=P.ID_PACKING_TYPE_SIZE ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    int cajas= Convert.ToInt32(leerF["CANT_BOX"]);
                    if (cajas != 0)
                    {
                        suma += Convert.ToInt32(leerF["QTY"]);
                    }                  

                }
                leerF.Close();
            }
            finally
            {

                conex.CerrarConexion();
                conex.Dispose();
            }

            return suma;
        }


        //Muestra la lista de suma de Piezas por talla
        public int SumaTotalPiezasTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT QTY  FROM packing_type_size WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["QTY"]);

                }
                leerF.Close();
            }
            finally
            {

                conex.CerrarConexion();
                conex.Dispose();
            }

            return suma;
        }

        //Muestra la lista de suma de Piezas PPK HT por talla
        public int SumaTotalPiezasPPKTalla(int? idEstilo, int idTalla, int? nPO, int tipoEmp)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT p.total_pieces  FROM packing p " +
                    "INNER JOIN packing_type_size PZ ON P.ID_PACKING_TYPE_SIZE= PZ.id_packing_type_size " +
                    "WHERE p.ID_SUMMARY='" + idEstilo + "' AND p.ID_TALLA='" + idTalla + "' AND  PZ.number_po='" + nPO + "' and  PZ.type_packing='" + tipoEmp + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["total_pieces"]);

                }
                leerF.Close();
            }
            finally
            {

                conex.CerrarConexion();
                conex.Dispose();
            }

            return suma;
        }


        //Obtener el numero de cajas registras bulk ht
        public int ObtenerNoCajasBulkHT(int? idEstilo, int idTalla, int numeroPO, int tipoE)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "select p.cant_box from  PACKING P, PACKING_TYPE_SIZE PZ " +
                                  "WHERE P.ID_SUMMARY='" + idEstilo + "' AND P.ID_TALLA='" + idTalla + "' AND " +
                                  "pz.number_po='" + numeroPO + "' AND pz.type_packing='" + tipoE + "' and PZ.ID_PACKING_TYPE_SIZE=P.ID_PACKING_TYPE_SIZE";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma = Convert.ToInt32(leerF["cant_box"]);

                }
                leerF.Close();
            }
            finally
            {

                conex.CerrarConexion();
                conex.Dispose();
            }

            return suma;
        }

        //Muestra la lista de suma de Extra tallas por Batch
        public int SumaTotalExtraBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT EXTRA  FROM PACKING WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["EXTRA"]);

                }
                leerF.Close();
            }
           finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }         

            return suma;
        }

        //Muestra la lista de suma de DEFECT tallas por Batch
        public int SumaTotalDefectBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT DEFECT FROM PACKING WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["DEFECT"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
            }         

            return suma;
        }

        //Muestra la lista de tallas de Batch por estilo
        public IEnumerable<PackingM> ListaBatch(int? id, int tipoEmpaque)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct ID_BATCH FROM PACKING WHERE ID_SUMMARY='" + id + "'";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {

                        IdBatch = Convert.ToInt32(leer["ID_BATCH"])
                    };

                    tallas.Batch = ListaTallasBatch(tallas.IdBatch, id, tipoEmpaque);
                    foreach (var item in tallas.Batch)
                    {
                        tallas.TipoTurno = item.TipoTurno;
                        tallas.NombreUsr = item.NombreUsr;
                        tallas.IdPacking = item.IdPacking;
                        tallas.NombreUsrModif = item.NombreUsrModif;
                        tallas.TipoEmpaque = item.PackingTypeSize.IdTipoEmpaque;

                    }
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
            }        

            return listTallas;
        }

        //Muestra la lista de tallas de Batch por estilo
        public IEnumerable<PackingM> ListaBatchHT(int? id)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct ID_BATCH FROM PACKING WHERE ID_SUMMARY='" + id + "'";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {

                        IdBatch = Convert.ToInt32(leer["ID_BATCH"])
                    };

                    tallas.Batch = ListaTallasBatchHT(tallas.IdBatch, id);
                    foreach (var item in tallas.Batch)
                    {
                        tallas.TipoTurno = item.TipoTurno;
                        tallas.NombreUsr = item.NombreUsr;
                        tallas.IdPacking = item.IdPacking;
                        tallas.NombreUsrModif = item.NombreUsrModif;
                        tallas.TipoEmpaque = item.PackingTypeSize.IdTipoEmpaque;
                        tallas.NumberPO = item.PackingTypeSize.NumberPO;


                    }
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
            }

            return listTallas;
        }

        //Muestra la lista de informacion de packing por estilo
        public IEnumerable<PackingTypeSize> ListaInfoPacking(int? idEstilo, int tipoEmpaque)
        {
            Conexion conn = new Conexion();
            List<PackingTypeSize> listPacking = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT ID_PACKING_TYPE_SIZE, ID_TALLA,PIECES,RATIO,TYPE_PACKING,ID_SUMMARY FROM PACKING_TYPE_SIZE WHERE TYPE_PACKING='" + tipoEmpaque + "' AND ID_SUMMARY='" + idEstilo + "'";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingTypeSize infoPack = new PackingTypeSize() {
                        IdPackingTypeSize = Convert.ToInt32(leer["ID_PACKING_TYPE_SIZE"]),
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Pieces = Convert.ToInt32(leer["PIECES"]),
                        Ratio = Convert.ToInt32(leer["RATIO"]),
                        IdTipoEmpaque = Convert.ToInt32(leer["TYPE_PACKING"]),
                        IdSummary = Convert.ToInt32(leer["ID_SUMMARY"])
                    };
                   
                    listPacking.Add(infoPack);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
            }

            return listPacking;
        }

        //Muestra la lista de bath registrados de packing assort
        public IEnumerable<PackingAssortment> ListaBatchAssort(int idBlock, int idPedido)
        {
            Conexion conn = new Conexion();
            List<PackingAssortment> listado = new List<PackingAssortment>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct ID_BATCH FROM PACKING_ASSORT WHERE id_block='" + idBlock + "' AND id_pedido='"+ idPedido + "' ";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PackingAssortment packAssort = new PackingAssortment()
                    {

                        IdBatch = Convert.ToInt32(leer["ID_BATCH"])
                    };

                    packAssort.BatchAssort = ListaPackingAssortBatch(packAssort.IdBatch, idPedido, idBlock);
                    foreach (var item in packAssort.BatchAssort)
                    {
                        packAssort.TipoTurno = item.TipoTurno;
                        packAssort.NombreUsr = item.NombreUsr;
                        packAssort.IdPackingAssort = item.IdPackingAssort;
                        packAssort.NombreUsrModif = item.NombreUsrModif;
                        packAssort.CantCartons = item.CantCartons;
                        packAssort.TotalPiezas = item.TotalPiezas;
                        packAssort.IdBatch = item.IdBatch;
                        

                    }
                    listado.Add(packAssort);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
            }

            return listado;
        }



        //Muestra la lista de tallas de UN Batch por estilo y id Batch seleccionado
        public IEnumerable<PackingM> ListaCantidadesTallaPorIdBatchEstilo(int? idEstilo, int idBatch)
        {
            Conexion conn = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            List<PackingTypeSize> listaEmpaque = new List<PackingTypeSize>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT ID_PACKING, ID_TALLA, S.TALLA, CANT_BOX,TOTAL_PIECES FROM PACKING " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=PACKING.ID_TALLA " +
                    "WHERE ID_SUMMARY='" + idEstilo + "' AND ID_BATCH='" + idBatch + " ' ORDER by cast(S.ORDEN AS int) ASC ";
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        Talla = leer["TALLA"].ToString(),
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        CantBox = Convert.ToInt32(leer["CANT_BOX"]),                        
                        TotalPiezas = Convert.ToInt32(leer["TOTAL_PIECES"])
                    
                };
                    

                    listaEmpaque = ObtenerListaPackingTypeSizePiezasyRatioPorEstilo(idEstilo);
                    tallas.ListEmpaque = listaEmpaque;
                    listTallas.Add(tallas);
                }
                leer.Close();
            }
           finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }   
         
            return listTallas;
        }

        //Obtener el tipo de empaque de una talla
        public int ObtenerTipoEmpaque(int? idEstilo)
        {
            Conexion conn = new Conexion();
            int tipoEmpaque = 0;
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct pt.type_packing FROM packing_type_size pt WHERE pt.ID_SUMMARY='" + idEstilo + "'";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {


                    if (!Convert.IsDBNull(leer["TYPE_PACKING"]))
                    {
                        tipoEmpaque = Convert.ToInt32(leer["TYPE_PACKING"]);
                    }

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }           
            return tipoEmpaque;
        }

        //Muestra la lista de tallas por Batch
        public List<PackingM> ListaTallasBatch(int? batch, int? id, int tipoEmpaque)
        {
            Conexion conex = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "SELECT PT.TYPE_PACKING, P.ID_PACKING, P.ID_SUMMARY, P.ID_BATCH, CONCAT(U.Nombres,' ',U.Apellidos)AS NOMBRE,P.ID_USUARIO_MODIF, P.TURNO, " +
                    "P.ID_TALLA, S.TALLA, P.CANT_BOX, P.TOTAL_PIECES FROM PACKING P, CAT_ITEM_SIZE S, USUARIOS U, PACKING_TYPE_SIZE PT " +
                    "WHERE P.ID_BATCH='" + batch + "' AND P.ID_SUMMARY='" + id + "' AND S.ID=P.ID_TALLA AND U.Id=P.ID_USUARIO " +
                    "AND P.ID_SUMMARY=PT.ID_SUMMARY AND PT.TYPE_PACKING='"+ tipoEmpaque + "' " +
                    "GROUP BY PT.TYPE_PACKING ,P.ID_PACKING,P.ID_SUMMARY, P.ID_BATCH,P.TURNO, P.ID_TALLA, S.TALLA, P.CANT_BOX, P.TOTAL_PIECES, " +
                    "U.Nombres, U.Apellidos,P.ID_USUARIO_MODIF, S.ORDEN ORDER by cast(S.ORDEN AS int) ASC ";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        Talla = leerF["TALLA"].ToString(),
                        IdBatch = Convert.ToInt32(leerF["ID_BATCH"]),
                        IdPacking = Convert.ToInt32(leerF["ID_PACKING"]),
                        IdSummary = Convert.ToInt32(leerF["ID_SUMMARY"]),
                        TipoTurno = Convert.ToInt32(leerF["TURNO"]),
                        NombreUsr = leerF["NOMBRE"].ToString(),
                        CantBox = Convert.ToInt32(leerF["CANT_BOX"]),
                        TotalPiezas = Convert.ToInt32(leerF["TOTAL_PIECES"])
                   


                    };

                    if (!Convert.IsDBNull(leerF["ID_USUARIO_MODIF"]))
                    {
                        tallas.UsuarioModif = Convert.ToInt32(leerF["ID_USUARIO_MODIF"]);
                    }

                    if (tallas.UsuarioModif != 0)
                    {
                        tallas.NombreUsrModif = objCatUser.Obtener_Nombre_Usuario_PorID(tallas.UsuarioModif);
                    }
                    else
                    {
                        tallas.NombreUsrModif = "-";
                    }

                    PackingTypeSize tipoEmp = new PackingTypeSize();
                    tipoEmp.IdTipoEmpaque = ObtenerTipoEmpaque(id);
                    tallas.PackingTypeSize = tipoEmp;
                    listTallas.Add(tallas);
                }
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }        

            return listTallas;
        }

        //Muestra la lista de packing assort por Batch
        public List<PackingAssortment> ListaPackingAssortBatch(int idBatch, int idPedido, int idBlock)
        {
            Conexion conex = new Conexion();
            List<PackingAssortment> listado = new List<PackingAssortment>();
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "SELECT p.ID_PACKING_ASSORT, P.ID_PEDIDO, P.ID_BATCH, CONCAT(U.Nombres,' ',U.Apellidos)AS NOMBRE,P.ID_USUARIO_MODIF, P.TURNO,    " +
                    "P.CANT_CARTONS, P.TOTAL_PIECES FROM PACKING_ASSORT P,USUARIOS U " +
                    "WHERE P.ID_BATCH='" + idBatch + "' AND P.ID_PEDIDO='" + idPedido + "'AND P.id_block='"+idBlock+"' AND U.Id=P.ID_USUARIO ";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    PackingAssortment registros = new PackingAssortment()
                    {
                        IdBatch = Convert.ToInt32(leerF["ID_BATCH"]),
                        IdPackingAssort = Convert.ToInt32(leerF["ID_PACKING_ASSORT"]),
                        IdPedido = Convert.ToInt32(leerF["ID_PEDIDO"]),
                        TipoTurno = Convert.ToInt32(leerF["TURNO"]),
                        NombreUsr = leerF["NOMBRE"].ToString(),
                        CantCartons = Convert.ToInt32(leerF["CANT_CARTONS"]),
                        TotalPiezas = Convert.ToInt32(leerF["TOTAL_PIECES"])


                    };

                    if (!Convert.IsDBNull(leerF["ID_USUARIO_MODIF"]))
                    {
                        registros.UsuarioModif = Convert.ToInt32(leerF["ID_USUARIO_MODIF"]);
                    }

                    if (registros.UsuarioModif != 0)
                    {
                        registros.NombreUsrModif = objCatUser.Obtener_Nombre_Usuario_PorID(registros.UsuarioModif);
                    }
                    else
                    {
                        registros.NombreUsrModif = "-";
                    }


                    listado.Add(registros);
                }
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return listado;
        }

        //Muestra la lista de tallas por Batch
        public List<PackingM> ListaTallasBatchHT(int? batch, int? id)
        {
            Conexion conex = new Conexion();
            List<PackingM> listTallas = new List<PackingM>();
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "SELECT p.ID_PACKING, P.ID_SUMMARY, P.ID_BATCH, CONCAT(U.Nombres,' ',U.Apellidos)AS NOMBRE,P.ID_USUARIO_MODIF, P.TURNO, PZ.NUMBER_PO, PZ.TYPE_PACKING,   " +
                    " P.ID_TALLA, S.TALLA, P.CANT_BOX, P.TOTAL_PIECES, PZ.QTY FROM PACKING P, CAT_ITEM_SIZE S, USUARIOS U, PACKING_TYPE_SIZE PZ " +
                    "WHERE P.ID_BATCH='" + batch + "' AND P.ID_SUMMARY='" + id + "'AND S.ID=P.ID_TALLA AND U.Id=P.ID_USUARIO AND PZ.ID_PACKING_TYPE_SIZE=P.ID_PACKING_TYPE_SIZE " +
                    "ORDER by cast(S.ORDEN AS int) ASC ";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    PackingM tallas = new PackingM()
                    {
                        Talla = leerF["TALLA"].ToString(),
                        IdBatch = Convert.ToInt32(leerF["ID_BATCH"]),
                        IdPacking = Convert.ToInt32(leerF["ID_PACKING"]),
                        IdSummary = Convert.ToInt32(leerF["ID_SUMMARY"]),
                        TipoTurno = Convert.ToInt32(leerF["TURNO"]),
                        NombreUsr = leerF["NOMBRE"].ToString(),
                        CantBox = Convert.ToInt32(leerF["CANT_BOX"]),
                        TotalPiezas = Convert.ToInt32(leerF["TOTAL_PIECES"]),
                        CantidadP = Convert.ToInt32(leerF["QTY"])

                };



                    if (!Convert.IsDBNull(leerF["ID_USUARIO_MODIF"]))
                    {
                        tallas.UsuarioModif = Convert.ToInt32(leerF["ID_USUARIO_MODIF"]);
                    }

                    if (tallas.UsuarioModif != 0)
                    {
                        tallas.NombreUsrModif = objCatUser.Obtener_Nombre_Usuario_PorID(tallas.UsuarioModif);
                    }
                    else
                    {
                        tallas.NombreUsrModif = "-";
                    }

                    PackingTypeSize tipoEmp = new PackingTypeSize();
                    tipoEmp.NumberPO = Convert.ToInt32(leerF["NUMBER_PO"]);
                    tipoEmp.IdTipoEmpaque = Convert.ToInt32(leerF["TYPE_PACKING"]);
                    tallas.PackingTypeSize = tipoEmp;
                    listTallas.Add(tallas);
                }
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas por Numero PO
        public List<PackingTypeSize> ListaTallasNumeroPO(int? numeroPO, int? idEstilo)
        {
            Conexion conex = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "select id_packing_type_size, id_talla, s.talla, ratio from packing_type_size " +
                                "INNER JOIN CAT_ITEM_SIZE S ON S.ID = ID_TALLA  " +
                                "WHERE ID_SUMMARY='" + idEstilo + "' AND number_po='" + numeroPO + "' and type_packing=2 " +
                                "ORDER by cast(S.ORDEN AS int) ASC ";
                leerF = c.ExecuteReader();
              
                while (leerF.Read())
                {
                    PackingTypeSize tallas = new PackingTypeSize()
                    {
                        IdPackingTypeSize = Convert.ToInt32(leerF["id_packing_type_size"]),
                        IdTalla = Convert.ToInt32(leerF["ID_TALLA"]),
                        Talla = leerF["TALLA"].ToString(),
                        Ratio = Convert.ToInt32(leerF["RATIO"]),
                        NumberPO = Convert.ToInt32(numeroPO) 


                    };
                    
                 
                    tallas.TotalRatio= SumaTotalPiezasPPKTalla(idEstilo, tallas.IdTalla, numeroPO, 2);

                  
                    listTallas.Add(tallas);
                }
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return listTallas;
        }
        //Muestra la lista de piezas por tallas de estilo
        public List<PackingTypeSize> ListaTotalPiezasTallasAssortPorEstilo(int? idEstilo)
        {
            Conexion conex = new Conexion();
            List<PackingTypeSize> listTotalPiezas = new List<PackingTypeSize>();
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "select pt.id_talla, s.orden, s.talla, pt.total_pieces, pt.id_summary from packing_type_size pt " +
                                "INNER JOIN CAT_ITEM_SIZE S ON S.ID=pt.ID_TALLA  " +
                                "WHERE pt.ID_SUMMARY='" + idEstilo + "' AND PT.TYPE_PACKING=3 GROUP by S.ORDEN, pT.ID_TALLA, S.TALLA, pt.total_pieces, pt.id_summary " +
                                "ORDER BY cast(S.ORDEN AS int) ASC  ";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    PackingTypeSize tallas = new PackingTypeSize()
                    {
                        IdSummary = Convert.ToInt32(leerF["id_summary"]),
                        IdTalla = Convert.ToInt32(leerF["ID_TALLA"]),
                        Talla = leerF["TALLA"].ToString(),
                        TotalPieces = Convert.ToInt32(leerF["total_pieces"])


                    };

                    listTotalPiezas.Add(tallas);
                }
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return listTotalPiezas;
        }

        //Agregar las tallas de un batch
        public void AgregarTallasPacking(PackingM packing)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "AgregarPacking";
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@idSummary", packing.IdSummary);
                com.Parameters.AddWithValue("@idBatch", packing.IdBatch);
                com.Parameters.AddWithValue("@idTalla", packing.IdTalla);
                com.Parameters.AddWithValue("@cantB", packing.CantBox);
                com.Parameters.AddWithValue("@partial", packing.Partial);
                com.Parameters.AddWithValue("@total", packing.TotalPiezas);
                com.Parameters.AddWithValue("@turno", packing.IdTurno);
                com.Parameters.AddWithValue("@idUsr", packing.Usuario);
                com.Parameters.AddWithValue("@noPPK", packing.CantidadPPKS);
                com.Parameters.AddWithValue("@idPack", packing.IdPackingTypeSize);

                com.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }        

        }

        //Agregar pallet a packing assort
        public void AgregarPackingAssort(PackingM packing)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "AgregarPackingAssort";
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@idBatch", packing.PackingAssort.IdBatch);
                com.Parameters.AddWithValue("@idBlock", packing.PackingAssort.IdBlock);
                com.Parameters.AddWithValue("@turno", packing.PackingAssort.IdTurno);
                com.Parameters.AddWithValue("@cart", packing.PackingAssort.CantCartons);
                com.Parameters.AddWithValue("@totalp", packing.PackingAssort.TotalPiezas);
                com.Parameters.AddWithValue("@packN", packing.PackingAssort.PackingName);
                com.Parameters.AddWithValue("@idUsr", packing.PackingAssort.Usuario);
                com.Parameters.AddWithValue("@idUsrM", packing.PackingAssort.UsuarioModif);
                com.Parameters.AddWithValue("@idPedido", packing.PackingAssort.IdPedido);

                com.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

        }

        //Agregar las tallas de Packing
        public void AgregarTallasP(PackingM packing)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "AgregarPackingSize";
                com.CommandType = CommandType.StoredProcedure;


                com.Parameters.AddWithValue("@idTalla", packing.PackingSize.IdTalla);
                com.Parameters.AddWithValue("@calidad", packing.PackingSize.Calidad);
                com.Parameters.AddWithValue("@idSummary", packing.PackingSize.IdSummary);
                com.ExecuteNonQuery();
            }
            finally
            {

                conex.CerrarConexion();
                conex.Dispose();
            }

            

        }

        //Agregar las tallas de Packing Type Size
        public void AgregarTallasTypePack(PackingM packing)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "AgregarTypePackingSize";
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@idTalla", packing.PackingTypeSize.IdTalla);
                com.Parameters.AddWithValue("@piece", packing.PackingTypeSize.Pieces);
                com.Parameters.AddWithValue("@ratio", packing.PackingTypeSize.Ratio);
                com.Parameters.AddWithValue("@typeP", packing.PackingTypeSize.IdTipoEmpaque);
                com.Parameters.AddWithValue("@numPO", packing.PackingTypeSize.NumberPO);
                com.Parameters.AddWithValue("@packF", packing.PackingTypeSize.IdFormaEmpaque);
                com.Parameters.AddWithValue("@qty", packing.PackingTypeSize.Cantidad);
                com.Parameters.AddWithValue("@cart", packing.PackingTypeSize.Cartones);
                com.Parameters.AddWithValue("@partial", packing.PackingTypeSize.PartialNumber);
                com.Parameters.AddWithValue("@totPiezas", packing.PackingTypeSize.TotalPieces);
                com.Parameters.AddWithValue("@totUnit", packing.PackingTypeSize.TotalUnits);
                com.Parameters.AddWithValue("@tCartons", packing.PackingTypeSize.TotalCartones);
                com.Parameters.AddWithValue("@packName", packing.PackingTypeSize.PackingName);
                com.Parameters.AddWithValue("@assortName", packing.PackingTypeSize.AssortName);
                com.Parameters.AddWithValue("@idBlock", packing.PackingTypeSize.IdBlockPack);
                com.Parameters.AddWithValue("@idSummary", packing.PackingTypeSize.IdSummary);
                com.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }        
            

        }

        //Permite actualizar la información de un batch
        public void ActualizarTallasPacking(PackingM packing)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "ActualizarBatchPacking";
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@id", packing.IdPacking);
                com.Parameters.AddWithValue("@idSummary", packing.IdSummary);
                com.Parameters.AddWithValue("@idBatch", packing.IdBatch);
                com.Parameters.AddWithValue("@idTalla", packing.IdTalla);
                com.Parameters.AddWithValue("@cantB", packing.CantBox);
                com.Parameters.AddWithValue("@partial", packing.Partial);
                com.Parameters.AddWithValue("@total", packing.TotalPiezas);
                com.Parameters.AddWithValue("@turno", packing.IdTurno);
                com.Parameters.AddWithValue("@idUsr", packing.Usuario);
                com.Parameters.AddWithValue("@idPack", packing.IdPackingTypeSize);
                com.Parameters.AddWithValue("@idUsrMod", packing.UsuarioModif);

                com.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }     

        }

        //Permite obtener el id del batch de los registro 
        public int ObtenerIdBatch(int id)
        {
            int idBatch = 0;
            int idTotal = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT distinct ID_BATCH FROM PACKING WHERE ID_SUMMARY='" + id + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idBatch += Convert.ToInt32(leerF["ID_BATCH"]);
                    idTotal++;
                }
                leerF.Close();
            }
            finally 
            {
                conex.CerrarConexion();
                conex.Dispose();
            }           
           
            return idTotal;
        }


        //Permite obtener el id del batch de los registro assortment
        public int ObtenerIdBatchAssort(int id, int idBlock)
        {
            int idBatch = 0;
            int idTotal = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT distinct ID_BATCH FROM PACKING_ASSORT WHERE ID_PEDIDO='" + id + "' and ID_BLOCK='" + idBlock + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idBatch += Convert.ToInt32(leerF["ID_BATCH"]);
                    idTotal++;
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return idTotal;
        }

        //Permite obtener el id del packing size de los registro 
        public int ObtenerIdPackingSize(int id)
        {
            int idPack = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "select id_packing_type_size from packing where id_Packing='" + id + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idPack += Convert.ToInt32(leerF["id_packing_type_size"]);

                }
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
                      
            return idPack;
        }

        //Permite obtener el idPacking del batch de los registro por idestilo
        public int ObtenerIdPackingPorBatchEstilo(int idBatch, int idSummary, int idTalla)
        {

            int idPacking = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT ID_PACKING FROM PACKING WHERE ID_BATCH='" + idBatch + "' AND ID_SUMMARY='" + idSummary + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idPacking += Convert.ToInt32(leerF["ID_PACKING"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }  
           
            return idPacking;
        }

        public int Obtener_Utlimo_Packing_Type()
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "SELECT ID_PACKING_TYPE_SIZE FROM PACKING_TYPE_SIZE WHERE ID_PACKING_TYPE_SIZE = (SELECT MAX(ID_PACKING_TYPE_SIZE) FROM PACKING_TYPE_SIZE)";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return Convert.ToInt32(reader["ID_PEDIDO"]);
                }
                reader.Close();
            }
            finally {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return 0;
        }

        //Permite obtener el idUsuario del batch registrado
        public int ObtenerIdUsuarioPorBatchEstilo(int idBatch, int idSummary, int idTalla)
        {

            int idUsuario = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT ID_USUARIO FROM PACKING WHERE ID_BATCH='" + idBatch + "' AND ID_SUMMARY='" + idSummary + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idUsuario += Convert.ToInt32(leerF["ID_USUARIO"]);


                }
                leerF.Close();
            }
            catch (Exception)
            {

                conex.CerrarConexion();
                conex.Dispose();
            }

            return idUsuario;
        }

        //Permite obtener el id packing type size registrado
        public int ObtenerIdPackingtypeSize(int idTipoE, int idSummary, int idTalla, int idNumberPO)
        {

            int idPacking = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "select id_packing_type_size from packing_type_size where type_packing='" + idTipoE + "' AND ID_SUMMARY='" + idSummary + "' AND ID_TALLA='" + idTalla + "'and number_po ='" + idNumberPO + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idPacking += Convert.ToInt32(leerF["id_packing_type_size"]);


                }
                leerF.Close();
            }
            catch (Exception)
            {

                conex.CerrarConexion();
                conex.Dispose();
            }

            return idPacking;
        }



        

        //Permite obtener el id BLOCK packing type size registrado
        public int ObtenerNumBlock(List<ItemDescripcion> listaEstilos)
        {
            int idBlock = 0;
            int idTotal = 0;
            Conexion conex = new Conexion();            
            string valores = "";
            for (int v = 0; v < listaEstilos.Count; v++)
            {
                if (v > 0)
                {
                    valores += "," + listaEstilos[v].IdSummary;
                }
                else
                {
                    valores += listaEstilos[v].IdSummary;
                }

            }
            string query = valores;
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "select DISTINCT ID_BLOCK_PACK FROM PACKING_TYPE_SIZE WHERE ID_SUMMARY in("+query+") ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    if (!Convert.IsDBNull(leerF["ID_BLOCK_PACK"]))
                    {
                        idBlock = Convert.ToInt32(leerF["ID_BLOCK_PACK"]);
                    }
                    
                    idTotal++;
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return idBlock;
        }
        //Registrar las cantidades de los packing registrados del Assortment
        public void ActualizarCantidadesPackAssort(int pcs, int cart, int totalUnits, string packName)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "UPDATE PACKING_TYPE_SIZE SET PIECES='" + pcs + "', TOTAL_CARTONS='" + cart + "', TOTAL_UNITS='" + totalUnits + "'  WHERE PACKING_NAME='" + packName + "' and type_packing=3";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                conex.CerrarConexion();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

        }

        //Cambia la información de un tipo de empaque Bulk 
        public void CambiarTipoPackBulk(int idTipoEmpaque, int numPiezas)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "UPDATE PACKING_TYPE_SIZE SET RATIO='" + numPiezas + "', TYPE_PACKING=2 WHERE ID_PACKING_TYPE_SIZE='" + idTipoEmpaque + "'";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                conex.CerrarConexion();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

        }

        //Cambia la información de un tipo de empaque PPK 
        public void CambiarTipoPackPPK(int idTipoEmpaque, int numRatio)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "UPDATE PACKING_TYPE_SIZE SET PIECES='" + numRatio + "', TYPE_PACKING=1 WHERE ID_PACKING_TYPE_SIZE='" + idTipoEmpaque + "'";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                conex.CerrarConexion();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

        }

        //Permite obtener el packing name registrados
        public int ObtenerPackingName()
        {
            int idBlock = 0;
            int idTotal = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "select DISTINCT ID_BLOCK_PACK FROM PACKING_TYPE_SIZE ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    if (!Convert.IsDBNull(leerF["ID_BLOCK_PACK"]))
                    {
                        idBlock = Convert.ToInt32(leerF["ID_BLOCK_PACK"]);
                    }

                    idTotal++;
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return idBlock;
        }

        //Permite obtener el packing name registrados
        public int ObtenerIdBlock(int id, string packingName)
        {
            
            int idBlock = 0;            
            Conexion conex = new Conexion();
            string query = ObtenerEstilosPorIdPedido(id);
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "select DISTINCT ID_BLOCK_PACK FROM PACKING_TYPE_SIZE " +
                    "WHERE PACKING_NAME='"+packingName+"' and ID_SUMMARY in(" + query + ") and type_packing=3";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    if (!Convert.IsDBNull(leerF["ID_BLOCK_PACK"]))
                    {
                        idBlock = Convert.ToInt32(leerF["ID_BLOCK_PACK"]);
                    }

               
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return idBlock;
        }

        //Permite obtener el total de cartones de un packing assort
        public int ObtenerTotalCartonesAssort(int id, string packingName)
        {
            int totalCartones = 0;
            Conexion conex = new Conexion();
            string query = ObtenerEstilosPorIdPedido(id);
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "select DISTINCT TOTAL_CARTONS FROM PACKING_TYPE_SIZE " +
                    "WHERE PACKING_NAME='" + packingName + "' and ID_SUMMARY in(" + query + ") and type_packing=3";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    if (!Convert.IsDBNull(leerF["TOTAL_CARTONS"]))
                    {
                        totalCartones = Convert.ToInt32(leerF["TOTAL_CARTONS"]);
                    }


                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return totalCartones;
        }


        //Permite obtener el numero total de cartones registrados de un packing assort
        public int ObtenerCantCartonesAssort(int? id, int numBlock)
        {
            int totalCartones = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT CANT_CARTONS FROM PACKING_ASSORT " +
                    "WHERE id_block='" + numBlock + "' and id_pedido='"+id+"'";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    if (!Convert.IsDBNull(leerF["CANT_CARTONS"]))
                    {
                        totalCartones += Convert.ToInt32(leerF["CANT_CARTONS"]);
                    }


                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return totalCartones;
        }

        //Permite obtener el total de piezas de un packing assort
        public int ObtenerTotalPiezasAssort(int id, string packingName)
        {          
            int totalPiezas = 0;
            Conexion conex = new Conexion();
            string query = ObtenerEstilosPorIdPedido(id);
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "select DISTINCT TOTAL_UNITS FROM PACKING_TYPE_SIZE " +
                    "WHERE PACKING_NAME='" + packingName + "' and ID_SUMMARY in(" + query + ") and type_packing=3";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    if (!Convert.IsDBNull(leerF["TOTAL_UNITS"]))
                    {
                        totalPiezas = Convert.ToInt32(leerF["TOTAL_UNITS"]);
                    }


                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return totalPiezas;
        }

        //Muestra la lista de packingName 
        public List<PackingTypeSize> ListaPackingName(List<ItemDescripcion> listaEstilos)
        {
            Conexion conex = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            string valores = "";    
            for (int v = 0; v < listaEstilos.Count; v++)
            {                
                    if (v > 0)
                    {
                        valores += "," + listaEstilos[v].IdSummary;
                    }
                    else
                    {
                        valores += listaEstilos[v].IdSummary;
                    }
                
           }
            string query =valores;
          try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "select  DISTINCT (packing_name) FROM PACKING_TYPE_SIZE " +
                    "WHERE ID_SUMMARY in("+query+") and type_packing=3 ";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    PackingTypeSize tallas = new PackingTypeSize();
                   
                       if (!Convert.IsDBNull(leerF["packing_name"]))
                    {
                        tallas.PackingRegistrado = leerF["packing_name"].ToString();
                    }
                    listTallas.Add(tallas);
                }
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return listTallas;
        }

        //Muestra la lista de packingName 
        public int ListadoPackingTypeAssort(int id)
        {
            Conexion conex = new Conexion();           
            int numeroRegistros = 0;
            string query = ObtenerEstilosPorIdPedido(id);
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "select count(id_packing_type_size) as REGISTROS FROM PACKING_TYPE_SIZE " +
                    "WHERE ID_SUMMARY in(" + query + ") AND TYPE_PACKING=3";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    numeroRegistros = Convert.ToInt32(leerF["REGISTROS"]);
                }
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return numeroRegistros;
        }

        //Muestra la lista de packing registrados 
        public List<PackingTypeSize> ListadoPackingPorIdEstilo(int id, string packingName)
        {
           
            Conexion conex = new Conexion();
            List<PackingTypeSize> listTallas = new List<PackingTypeSize>();
            List<PackingTypeSize> list = new List<PackingTypeSize>();
            string query = ObtenerEstilosPorIdPedido(id);
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "select pt.id_packing_type_size, pt.id_talla, s.talla, pt.ratio, pt.id_summary, RTRIM(ITD.ITEM_STYLE) as ITEM_STYLE, " +
                    "RTRIM(ITD.DESCRIPTION) as DESCRIPTION, RTRIM(C.CODIGO_COLOR) AS COLOR, PT.PIECES, " +
                    "PT.TOTAL_CARTONS, PT.TOTAL_UNITS, PT.ID_BLOCK_PACK FROM PACKING_TYPE_SIZE pt " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=pt.id_talla " +
                    "INNER JOIN PO_SUMMARY PS ON PS.ID_PO_SUMMARY = PT.ID_SUMMARY " +
                    "INNER JOIN item_description ITD ON PS.ITEM_ID = ITD.ITEM_ID " +
                    "INNER JOIN cat_colores C ON PS.ID_COLOR = C.ID_COLOR " +
                    "WHERE  pt.ID_SUMMARY in(" + query + ") and pt.pACKING_NAME='" + packingName+ "' and pt.type_packing=3 ORDER by cast(S.ORDEN AS int) ASC ";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    PackingTypeSize tallas = new PackingTypeSize() {
                        IdPackingTypeSize = Convert.ToInt32(leerF["id_packing_type_size"]),
                        IdTalla = Convert.ToInt32(leerF["id_talla"]),
                        Talla = leerF["TALLA"].ToString(),
                        Ratio = Convert.ToInt32(leerF["ratio"]),
                        IdSummary = Convert.ToInt32(leerF["id_summary"]),
                        Pieces = Convert.ToInt32(leerF["PIECES"]),
                        TotalCartones = Convert.ToInt32(leerF["TOTAL_CARTONS"]),
                        TotalUnits = Convert.ToInt32(leerF["TOTAL_UNITS"]),
                        IdBlockPack = Convert.ToInt32(leerF["ID_BLOCK_PACK"])
                        
                    };

                    ItemDescripcion estilo = new ItemDescripcion() {
                        ItemEstilo = leerF["ITEM_STYLE"].ToString(),
                        Descripcion = leerF["DESCRIPTION"].ToString()
                    };

                    CatColores colores = new CatColores() {
                        CodigoColor = leerF["COLOR"].ToString()
                    };

                    estilo.CatColores = colores;
                    tallas.ItemDescripcion = estilo;
                    int idEstilo = tallas.IdSummary;

                    PackingTypeSize result = listTallas.Find(x => x.IdSummary == idEstilo);
                    if (result == null)
                    {
                        listTallas.Add(tallas);
                        result = listTallas.Find(x => x.IdSummary == idEstilo);
                        result.Ratios =  "";
                        result.TallasGrl = "";
                        result.Ratios = tallas.Ratio.ToString();
                        result.TallasGrl = tallas.Talla;
                        
                    }
                    else
                    {
                        if (result.IdSummary == tallas.IdSummary)
                        {
                            if (result.Ratios == null)
                            {
                                result.Ratios += result.Ratio + "-" + tallas.Ratio;
                                result.TallasGrl += result.Talla + "-" + tallas.Talla;
                            }
                            else
                            {
                                result.Ratios += "-" + tallas.Ratio;
                                result.TallasGrl += "-" + tallas.Talla;
                            }
                        }
                        
                    }
                }
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return listTallas;
        }

        //Muestra el total de cartones faltantes de un packing registrado assort
        public int ObtenerTotalCartonesFaltantesAssort(int id, int idBlock, int numTotalCart)
        {
            Conexion conex = new Conexion();
            List<PackingAssortment> listTallas = new List<PackingAssortment>();
            int totalCartonesFaltantes = 0;
            int totalCartones = 0;
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "select cant_cartons from packing_assort where "+
                    "id_block='" + idBlock + "' and id_pedido='" + id + "' ";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    PackingAssortment packAssort = new PackingAssortment();
                    packAssort.TotalCartonesFalt += Convert.ToInt32(leerF["cant_cartons"]);
                    totalCartones += packAssort.TotalCartonesFalt;
                }
                totalCartonesFaltantes = numTotalCart - totalCartones;     
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return totalCartonesFaltantes;
        }

        //Muestra el total de piezas faltantes de un packing registrado assort
        public int ObtenerTotalPiezasFaltantesAssort(int id, int idBlock, int numTotalPiezas)
        {
            Conexion conex = new Conexion();
            List<PackingAssortment> listTallas = new List<PackingAssortment>();
            int totalPiezasFaltantes = 0;
            int totalPiezas = 0;
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "select total_pieces from packing_assort where " +
                    "id_block='" + idBlock + "' and id_pedido='" + id + "' ";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    PackingAssortment packAssort = new PackingAssortment();
                    packAssort.TotalPiezasFalt += Convert.ToInt32(leerF["total_pieces"]);
                    totalPiezas += packAssort.TotalPiezasFalt;
                }
                totalPiezasFaltantes = numTotalPiezas - totalPiezas;
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return totalPiezasFaltantes;
        }

        public string ObtenerEstilosPorIdPedido(int id)
        {
            List<ItemDescripcion> listaEstilos = objPedido.ListaEstilosPorIdPedido(id).ToList();
            string valores = "";
            for (int v = 0; v < listaEstilos.Count; v++)
            {
                if (v > 0)
                {
                    valores += "," + listaEstilos[v].IdSummary;
                }
                else
                {
                    valores += listaEstilos[v].IdSummary;
                }

            }
            string query = valores;

            return query;
        }


    }
}

