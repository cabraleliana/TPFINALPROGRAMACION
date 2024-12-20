﻿using DATOS;
using Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Negocio
{
    public class NegocioPacientes
    {
        DatosPacientes dts =  new DatosPacientes();

        public DataSet ObtenerTablaPacientes()
        {
            return dts.ObtenerTablaPacientes();


        }

        public bool AltaPaciente(int idProvincia, int idLocalidad, string dni, string nombre, string apellido, string sexo, string nacionalidad, string direccion, string correoElectronico, string telefono, DateTime fechaNacimiento)
        {
            DatosPacientes ds = new DatosPacientes();
            Pacientes paciente = new Pacientes
            {
                id_provincia = idProvincia,
                id_localidad = idLocalidad,
                dni = dni,
                nombre = nombre,
                apellido = apellido,
                sexo = sexo,
                nacionalidad = nacionalidad,
                direccion = direccion,
                correo_electronico = correoElectronico,
                telefono = telefono,
                fecha_nacimiento = fechaNacimiento
            };

            // Aquí puedes agregar cualquier otra validación necesaria
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
            {
                Console.WriteLine("El nombre y el apellido son obligatorios.");
                return false;
            }

            if (dni.Length != 8)
            {
                Console.WriteLine("El DNI debe tener 8 caracteres.");
                return false;
            }

            return ds.AgregarPaciente(paciente) == 1;
        }


        public DataTable ObtenerProvincias()
        {
            return dts.ObtenerProvincias();
        }

        public DataTable ObtenerLocalidades(int idProvincia)
        {
            return dts.ObtenerLocalidades(idProvincia);
        }
        public bool EliminarPaciente(int id)
        {
            DatosPacientes dp = new DatosPacientes();
            Pacientes pa = new Pacientes();
            pa.setId_Paciente(id);
            int op = dp.EliminarPac(pa);
            if (op == 1)
                return true;
            else
                return false;
        }
        public DataSet ObtenerPacientesPorDni(string dni)
        {
            return dts.ObtenerPacientesPorDni(dni);
        }
        public DataSet Filtros(string dni, int IdMedico, string desde, string hasta)
        {
            return dts.Filtros(dni, IdMedico, desde, hasta);
        }
        public DataSet ObtenerPacientesxMedicos(int ID)
                {
                    return dts.obtenerPacientesxMedicos(ID);
                }

        public DataSet obtenerDatosPacientes(string idPaciente)
        {
            return dts.ObtenerDatosPacientes(idPaciente);


        }
        public Boolean pacienteEditado(string id, string idProvincia, string idLocalidad, string nombre, string apellido, string sexo, string nacionalidad, string direccion, string correo, string telefono, DateTime fecha ,string dni)
        {
            int filasAfectadas = dts.pacienteEditado( id, idProvincia, idLocalidad, nombre, apellido, sexo, nacionalidad, direccion, correo, telefono, fecha ,dni);
            if (filasAfectadas > 0)
            {
                return true;

            }
            return false;

        }

     
        
    }
}
