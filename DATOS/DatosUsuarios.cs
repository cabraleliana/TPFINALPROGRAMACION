﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidades;
using System.Collections;

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

        public int TraerIdUsuario(string usuario)
        {
            DataSet ds = new DataSet();
            string consulta = $"select id_usuario from usuarios where nombre_usuario='{usuario}'";
            ds = accesoDatos.getData(consulta);
            int id = Convert.ToInt32(ds.Tables[0].Rows[0]["id_usuario"]);
            return id;

        }


        public Boolean verificarContraseña(string usuario , string contraseña)
        {
            string consulta = $"Select * from usuarios where nombre_usuario = '{usuario}' and contraseña COLLATE Latin1_General_CS_AS = '{contraseña}'";
            return accesoDatos.existe(consulta);


        }


        public (string nombre, int id) traerNombre_Rol(string usuario, string contraseña) // retornamos el nombre y el id del rol
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
            string consulta = $"select id_usuario as idMedico, nombre , apellido ,legajo , p.nombre_provincia as provincia, lo.nombre_localidad as localidad , es.nombre_especialidad as especialidad from usuarios inner join provincias as p on p.id_provincia = id_provincia_us inner join localidades as lo on lo.id_localidad =id_localidad_us inner join Especialidades as es on es.id_especialidad = id_especialidad_us where id_rol_us = 2 and activo=1";
            ds = accesoDatos.getData(consulta);
            return ds;

        }

        public DataSet traerDiasHorarios(int idMedico) //llenar el select de la tabla medicos para ver sus horarios
        {
            DataSet ds = new DataSet();
            string consulta = $"SELECT CONCAT(LTRIM(RTRIM(dias)), ' ', LTRIM(RTRIM(horario_inicio)), '-', LTRIM(RTRIM(horario_fin))) AS DiasHorarios FROM HorariosMedicos INNER JOIN usuarios AS us ON us.id_usuario = id_usuario_HM WHERE us.id_usuario = '{idMedico}'";
            ds = accesoDatos.getData(consulta);

            return ds;


        }


        public DataSet obtenerDatosMedicos(string idMedico) // para llenar el editar con sus datos
        {
            DataSet ds = new DataSet();
            string consulta = $"select nombre , apellido , sexo ,  nacionalidad , direccion , correo_electronico , telefono , legajo , dni , id_especialidad_us as idEspecialidad,es.nombre_especialidad as especialidad,id_provincia_us as idProvincia ,pr.nombre_provincia as provincia , id_localidad_us as idLocalidad, lo.nombre_localidad as localidad from usuarios inner join especialidades as es on es.id_especialidad = id_especialidad_us inner join localidades as lo on lo.id_provincia = id_provincia_us and lo.id_localidad = id_localidad_us inner join provincias as pr on pr.id_provincia = id_provincia_us where id_usuario = {idMedico} and activo=1";
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
        private int ArmarParametrosMedicoAgregar(SqlCommand comando, Usuarios usuario)
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
            comando.Parameters.Add("@activo", SqlDbType.Bit).Value = 1;

            int idMedico = _ = Convert.ToInt32(comando.ExecuteScalar());
            return idMedico;

        }

        public int AgregarMedico(Usuarios usuario)
        {
            using (SqlConnection conexion = accesoDatos.obtenerConexion())
            {
                //conexion.Open();
                using (SqlCommand comando = new SqlCommand("SP_AgregarMedico", conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                   int idMedico = ArmarParametrosMedicoAgregar(comando, usuario);

                    return idMedico;
                }
            }
        }

        public DataTable ObtenerRoles()
        {
            string consulta = "SELECT id_rol, nombre_rol FROM Roles";
            DataSet ds = accesoDatos.getData(consulta);
            return ds.Tables[0]; // Devolver la primera tabla
        }


 

        public DataSet medicosFiltrados(string legajo, string apellido, string especialidad)
        {
            DataSet ds = new DataSet();
            string consulta = $"select id_usuario as idMedico, nombre , apellido ,legajo , p.nombre_provincia as provincia, lo.nombre_localidad as localidad , es.nombre_especialidad as especialidad from usuarios inner join provincias as p on p.id_provincia = id_provincia_us inner join localidades as lo on lo.id_localidad =id_localidad_us inner join Especialidades as es on es.id_especialidad = id_especialidad_us where id_rol_us = 2 and 1=1 and activo=1";
            if (legajo!="")
            {
                consulta += $" AND legajo = '{legajo}'";
            }
            if (apellido!="")
            {
                consulta += $" AND apellido = '{apellido}' ";
            }
            if (especialidad != "")
            {
                consulta += $" AND id_especialidad_us = {especialidad} ";
            }
            ds = accesoDatos.getData(consulta);
            return ds;

        }


        public int medicoEditado(string id , string idEspecialidad , string idProvincia, string idLocalidad , string nombre , string apellido , string sexo , string nacionalidad , string direccion  , string correo , string telefono , string legajo , string dni)
        {
            string consulta = $"update usuarios set id_especialidad_us = {idEspecialidad} ,  id_provincia_us = {idProvincia} , id_localidad_us = {idLocalidad} , id_rol_us = 2 ,  nombre = '{nombre}' , apellido = '{apellido}' , sexo = '{sexo}' ,  nacionalidad = '{nacionalidad}' , direccion = '{direccion}' ,  correo_electronico  = '{correo}' ,  telefono = '{telefono}' ,  legajo = '{legajo}' ,  dni = '{dni}' where id_usuario = {id}";
            
            int filasAfectadas =  accesoDatos.execute(consulta);
            return filasAfectadas;

        }


        public DataSet selectMedicosPorEspecialidad(string idEspecialidad) //me trae los medicos segun la especialidad elegida
        {
            string consulta = $"select Usuarios.nombre as medico , Usuarios.id_usuario as idMedico from usuarios inner join Especialidades as e on e.id_especialidad = Usuarios.id_especialidad_us where Usuarios.id_especialidad_us = {idEspecialidad} and activo=1";
            DataSet ds = accesoDatos.getData(consulta);

            return ds;

        }

        //Falopeada de Fran pero que si les copa dejenla (es para saber la especialidad del medico porque no me acordaba
        //cual era cual)

        public string ObtenerEspecialidad(int idUsuario)
        {
            string consulta = @"
                SELECT nombre_especialidad 
                FROM Especialidades 
                WHERE id_especialidad = (
                SELECT id_especialidad_us 
                FROM Usuarios 
                WHERE id_usuario = @idUsuario
                )";

            using (SqlConnection conexion = accesoDatos.obtenerConexion())
            {
                if (conexion == null)
                {
                    return "Error: No se pudo obtener una conexión válida.";
                }

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@idUsuario", idUsuario);

                    try
                    {
                        // Verifica si la conexión está abierta antes de abrirla.
                        if (conexion.State != System.Data.ConnectionState.Open)
                        {
                            conexion.Open();
                        }

                        object resultado = comando.ExecuteScalar();
                        return resultado != null ? resultado.ToString() : "Sin especialidad";
                    }
                    catch (Exception ex)
                    {
                        return $"Error al obtener especialidad: {ex.Message}";
                    }
                    finally
                    {
                        // Si por algún motivo la conexión quedó abierta, ciérrala.
                        if (conexion.State == System.Data.ConnectionState.Open)
                        {
                            conexion.Close();
                        }
                    }
                }
            }
        }




    }
}
