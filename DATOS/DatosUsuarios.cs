﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;

namespace DATOS
{
    public class DatosUsuarios
    {
        Conexion accesoDatos = new Conexion();
        public Boolean verificarUsuario(string usuario)
        {
            string consulta = $"Select * from usuarios where nombre_usuario = '{usuario}'";
            return accesoDatos.existe(consulta);

        }

        public Boolean verificarContraseña(string usuario , string contraseña)
        {
            string consulta = $"Select * from usuarios where nombre_usuario = '{usuario}' and contraseña COLLATE Latin1_General_CS_AS = '{contraseña}'";
            return accesoDatos.existe(consulta);


        }


        public (string nombre, int id) traerNombre_Rol(string usuario, string contraseña)
        {
            DataSet ds = new DataSet();
            string consulta = $"Select nombre,id_rol_us from usuarios where nombre_usuario = '{usuario}' and contraseña COLLATE Latin1_General_CS_AS = '{contraseña}'";
            ds = accesoDatos.getData(consulta);
            if (ds.Tables[0].Rows.Count > 0)
            {
                string nombre = ds.Tables[0].Rows[0]["nombre"].ToString();
                int id = Convert.ToInt32(ds.Tables[0].Rows[0]["id_rol_us"]);
                return (nombre, id);
            }
            else
            {
                return (string.Empty, 0);
            }


        }




        public DataSet obtenerTablaMedicos()
        {
            DataSet ds = new DataSet();
            string consulta = $"select id_usuario as idMedico, nombre , apellido ,legajo , p.nombre_provincia as provincia, lo.nombre_localidad as localidad , es.nombre_especialidad as especialidad from usuarios inner join provincias as p on p.id_provincia = id_provincia_us inner join localidades as lo on lo.id_localidad =id_localidad_us inner join Especialidades as es on es.id_especialidad = id_especialidad_us where id_rol_us = 2";
            ds = accesoDatos.getData(consulta);
            return ds;

        }




        public DataSet traerDiasHorarios(int idMedico)
        {
            DataSet ds = new DataSet();
            string consulta = $"SELECT CONCAT(LTRIM(RTRIM(dias)), ' ', LTRIM(RTRIM(horario_inicio)), '-', LTRIM(RTRIM(horario_fin))) AS DiasHorarios FROM HorariosMedicos INNER JOIN usuarios AS us ON us.id_usuario = id_usuario_HM WHERE us.id_usuario = '{idMedico}'";
            ds = accesoDatos.getData(consulta);

            return ds;


        }


        public DataSet obtenerDatosMedicos(string idMedico)
        {
            DataSet ds = new DataSet();
            string consulta = $"select nombre , apellido , sexo ,  nacionalidad , direccion , correo_electronico , telefono , legajo , dni , id_especialidad_us as idEspecialidad,es.nombre_especialidad as especialidad,id_provincia_us as idProvincia ,pr.nombre_provincia as provincia , id_localidad_us as idLocalidad, lo.nombre_localidad as localidad from usuarios inner join especialidades as es on es.id_especialidad = id_especialidad_us inner join localidades as lo on lo.id_provincia = id_provincia_us and lo.id_localidad = id_localidad_us inner join provincias as pr on pr.id_provincia = id_provincia_us where id_usuario = {idMedico}";
            ds = accesoDatos.getData(consulta);

            return ds;


        }
        private void ArmaraParametrosMedicoEliminar(ref SqlCommand comando, Usuarios us)
        {
            SqlParameter sqlparametros = new SqlParameter();
            sqlparametros = comando.Parameters.Add("@idusuario", SqlDbType.Int);
            sqlparametros.Value = us.getIdUsuario();
        }
        public int EliminarMed(Usuarios us)
        {
            SqlCommand comando = new SqlCommand();
            ArmaraParametrosMedicoEliminar(ref comando, us);
            return accesoDatos.ExecProceAlmace(comando, "SP_EliminarMedico");

        }

        /// Esto es mas o menos como quiero hacerlo.
        private void ArmarParametrosMedicoAgregar(SqlCommand comando, Usuarios usuario)
        {
            comando.Parameters.Add("@id_provincia_us", SqlDbType.Int).Value = usuario.id_provincia;
            comando.Parameters.Add("@id_localidad_us", SqlDbType.Int).Value = usuario.id_localidad;
            comando.Parameters.Add("@id_especialidad_us", SqlDbType.Int).Value = usuario.id_especialidad;
            comando.Parameters.Add("@id_rol_us", SqlDbType.Int).Value = usuario.id_rol;
            comando.Parameters.Add("@dni", SqlDbType.Char, 8).Value = usuario.dni;
            comando.Parameters.Add("@nombre", SqlDbType.VarChar, 30).Value = usuario.nombre;
            comando.Parameters.Add("@apellido", SqlDbType.VarChar, 30).Value = usuario.apellido;
            comando.Parameters.Add("@sexo", SqlDbType.VarChar, 30).Value = usuario.sexo;
            comando.Parameters.Add("@nacionalidad", SqlDbType.VarChar, 30).Value = usuario.nacionalidad;
            comando.Parameters.Add("@direccion", SqlDbType.VarChar, 50).Value = usuario.direccion;
            comando.Parameters.Add("@legajo", SqlDbType.Char, 4).Value = usuario.legajo;
            comando.Parameters.Add("@correo_electronico", SqlDbType.VarChar, 50).Value = usuario.correo_electronico;
            comando.Parameters.Add("@telefono", SqlDbType.VarChar, 50).Value = usuario.telefono;
            comando.Parameters.Add("@nombre_usuario", SqlDbType.VarChar, 50).Value = usuario.nombre_usuario;
            comando.Parameters.Add("@contraseña", SqlDbType.VarChar, 50).Value= usuario.contraseña;

        }

        public int AgregarMedico(Usuarios usuario)
        {
            using (SqlConnection conexion = accesoDatos.obtenerConexion())
            {
                //conexion.Open();
                using (SqlCommand comando = new SqlCommand("SP_AgregarMedico", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    ArmarParametrosMedicoAgregar(comando, usuario);

                    return comando.ExecuteNonQuery();
                }
            }
        }

        public DataTable ObtenerRoles()
        {
            string consulta = "SELECT id_rol, nombre_rol FROM Roles";
            DataSet ds = accesoDatos.getData(consulta);
            return ds.Tables[0]; // Devolver la primera tabla
        }


    }
}
