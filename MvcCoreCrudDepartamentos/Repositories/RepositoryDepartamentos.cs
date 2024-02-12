using Microsoft.AspNetCore.Http.HttpResults;
using MvcCoreCrudDepartamentos.Models;
using System.Data;
using System.Data.SqlClient;

#region PROCEDIMIENTOS ALMACENADOS
//create procedure SP_INSERTDEPARTAMENTO
//(@NOMBRE NVARCHAR(50), @LOCALIDAD NVARCHAR(50))
//AS
//	declare @nextId int
//	select @nextId = max(DEPT_NO) + 1 from DEPT
//	insert into DEPT values (@nextId, @NOMBRE, @LOCALIDAD)
//GO
#endregion

namespace MvcCoreCrudDepartamentos.Repositories
{
    public class RepositoryDepartamentos
    {
        SqlConnection cn;
        SqlCommand com;
        SqlDataReader reader;

        public RepositoryDepartamentos()
        {
            string connectionString = @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=SA;Password=MCSD2023";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
        }

        //VAMOS A REALIZAR LAS CONSULTAS SOBRE LA BASE DE DATOS
        //DE FORMA ASINCRONA
        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            string sql = "select * from DEPT";
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            this.reader = await this.com.ExecuteReaderAsync();
            List<Departamento> departamentos = new List<Departamento>();
            while (await this.reader.ReadAsync())
            {
                Departamento dept = new Departamento();
                dept.IdDepartamento = int.Parse(this.reader["DEPT_NO"].ToString());
                dept.Nombre = this.reader["DNOMBRE"].ToString();
                dept.Localidad = this.reader["LOC"].ToString();
                departamentos.Add(dept);
            }
            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            return departamentos;
        }

        public async Task<Departamento> FindDepartamentoAsync(int id)
        {
            string sql = "select * from DEPT where DEPT_NO=@id";
            this.com.Parameters.AddWithValue("@id", id);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            this.reader = await this.com.ExecuteReaderAsync();
            //COMO ESTAMOS BUSCANDO, POR NORMA, SIEMPRE 
            //DEBEMOS AVERIGUAR SI EXISTEN DATOS O NO EN EL REPOSITORY
            //SIEMPRE QUE NO EXISTAN DATOS EN LA BUSQUEDA, EL REPO
            //DEBE DEVOLVER NULL
            Departamento departamento = null;
            if (await this.reader.ReadAsync())
            {
                //TENEMOS DATOS
                departamento = new Departamento();
                departamento.IdDepartamento =
                    int.Parse(this.reader["DEPT_NO"].ToString());
                departamento.Nombre = this.reader["DNOMBRE"].ToString();
                departamento.Localidad = this.reader["LOC"].ToString();
            }
            else
            {
                //NO TENEMOS DATOS
            }
            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
            return departamento;
        }

        public async Task InsertDepartamentoAsync
            (string nombre, string localidad)
        {
            this.com.Parameters.AddWithValue("@NOMBRE", nombre);
            this.com.Parameters.AddWithValue("@LOCALIDAD", localidad);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "SP_INSERTDEPARTAMENTO";
            await this.cn.OpenAsync();
            int af = await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
        }
        
        public async Task UpdateDepartamentoAsync
            (int id, string nombre, string localidad)
        {
            string sql = "update DEPT set DNOMBRE=@NOMBRE, LOC=@LOCALIDAD "
                + " where DEPT_NO=@id";
            this.com.Parameters.AddWithValue("@NOMBRE", nombre);
            this.com.Parameters.AddWithValue("@LOCALIDAD", localidad);
            this.com.Parameters.AddWithValue("@id", id);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            int af = await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
        }

        public async Task DeleteDepartamentoAsync(int id)
        {
            string sql = "delete from DEPT where DEPT_NO=@id";
            this.com.Parameters.AddWithValue("@id", id);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            int af = await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
        }
    }
}
