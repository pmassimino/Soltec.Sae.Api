using MathNet.Numerics.Distributions;
using Microsoft.Extensions.Hosting.Internal;
using NPOI.SS.Formula.Functions;
using System.Data.OleDb;
using System.IO;

namespace Soltec.Sae.Api
{
    public class RetencionDGRService
    {         
        public RetencionDGRService(string connectionStringBase) 
        {
            this.ConnectionStringBase = connectionStringBase;
        }
        public string ConnectionStringBase { get; set; } = "";
        private string fields = "tip,ntra,num,pe,nu1,nu2,nu3,cod,cuit,nom,dir,loc,pos,pro,dgr,fec,fe1,re1,bi1,im1,iml,con ";
        public List<RetencionBase> List(string idCuenta,DateTime fecha, DateTime fechaHasta)
        {
            SujetoService sujetoService = new SujetoService(this.ConnectionStringBase);
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT " + fields +
                "FROM retdgr " +                
                "WHERE  (fec BETWEEN ctod('" + fecha.ToString("MM-dd-yyy") + "')"
                + " AND ctod('" + fechaHasta.ToString("MM-dd-yyy") + "')) " 
                + " and (cod = '" + idCuenta + "' or empty('" + idCuenta + "'))"                  
                + " order by fec,cod";
            OleDbDataReader reader = command.ExecuteReader();
            List<RetencionBase> result = new List<RetencionBase>();           
            while (reader.Read())
            {                
               result.Add(Parse(reader));                
            }            
            reader.Close();
            cnn.Close();
            return result;
        }

        public RetencionBase FindOne(string id)
        {
            string connectionString = this.ConnectionStringBase + "sae.dbc";
            OleDbConnection cnn = new OleDbConnection(connectionString);
            cnn.Open();
            OleDbCommand command = cnn.CreateCommand();
            command.CommandText = "SELECT " + fields +
                "FROM retdgr " +
                "WHERE nu3='" + id + "'";

            OleDbDataReader reader = command.ExecuteReader();
            RetencionBase result = null;
            while (reader.Read())
            {
                result = Parse(reader);
            }
            reader.Close();
            cnn.Close();
            return result;
        }
        private RetencionBase Parse(OleDbDataReader reader)
        {
            RetencionBase item = new RetencionBase();
            item.Sec = "";
            item.Orden = "";
            item.Id = reader["nu3"].ToString().Trim();
            string pidTipo = reader["tip"].ToString().Trim();
            item.Tipo = "INGBRUTOS";                                         
            item.FechaPase = (DateTime)reader["fec"];
            item.FechaComprobante = (DateTime)reader["fec"];
            item.FechaVencimiento = (DateTime)reader["fec"];
            item.Pe = reader["pe"].ToString().Trim() == "" ? 0 : Convert.ToInt16(reader["pe"]);
            item.Numero = reader["num"].ToString().Trim() +" " + reader["nu1"].ToString().Trim() + " " + reader["nu2"].ToString().Trim() + " " +  reader["nu3"].ToString().Trim();
            item.NumeroComprobante = reader["con"].ToString().Trim();
            //Sujeto
            item.IdCuenta = reader["cod"].ToString().Trim();            
          
            Sujeto tmpSujeto = new Sujeto();
            tmpSujeto.Id = reader["cod"].ToString().Trim();
            tmpSujeto.Nombre = reader["nom"].ToString().Trim();            
            tmpSujeto.NumeroDocumento = reader["cuit"].ToString().Trim();
            tmpSujeto.Domicilio = reader["dir"].ToString().Trim();
            tmpSujeto.CodigoPostal = reader["pos"].ToString().Trim();
            tmpSujeto.Provincia = reader["pro"].ToString().Trim();
            tmpSujeto.Localidad = reader["loc"].ToString().Trim();
            tmpSujeto.NumeroIngBruto = reader["dgr"].ToString().Trim();

            item.Cuenta = tmpSujeto;
            //Importes
            item.BaseImponible = (decimal)reader["bi1"];
            item.Alicuota = (decimal)reader["re1"];
            item.Importe = (decimal)reader["im1"];
            item.Obs = reader["iml"].ToString().Trim();

            return item;
        }
        

    }
}
