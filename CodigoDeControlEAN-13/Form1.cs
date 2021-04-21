using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodigoDeControlEAN_13
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }
        #region Ean13
        private void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                txtCodigoBarras.Text = txtCodigo.Text;
                txtCodigiControl.Text = Convert.ToString(ObtenerDigitoVerificadorEAN13(txtCodigo.Text));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ObtenerDigitoVerificadorEAN13(string Codigo)
        {
            if (Codigo.Length == 12)
            {
                int suma = 0;
                //Recorrer los caracteres del código de barras
                for (int i = 0; i < Codigo.Length; i++)
                {
                    //Verificar que el caracter sea un número
                    if (Char.IsDigit(Codigo[i]))
                    {
                        //Convertir el caracter a entero
                        int valor = int.Parse(Codigo[i].ToString());
                        //Identificar el valor del corrector (La posición es par o impar)
                        //suma += ((i + 1) % 2 == 0) ? (valor * 3) : valor; // esta forma igual funciona 
                        suma += ((13 - i) % 2 == 0) ? (valor * 3) : valor;  
                    }
                    else
                    {
                        throw new Exception("Caracter inválido");
                    }
                }
                
                int sobrante = suma % 10; // obtenemos el residuo (el sobrante nunca es mayor a 9)
                int digitoVerificador = 0; 
                if (sobrante > 0) 
                    digitoVerificador = 10 - sobrante; // a 10 le restamos el residuo para obtener el digito de control
                return digitoVerificador;
            }
            else
            {
                throw new Exception("La longitud del código EAN13 debe ser de 12 caracteres");
            }
        }

        #endregion

        #region Calendario
        private static readonly Dictionary<int, int> mesesDictionary = new Dictionary<int, int>() {
        {1, 31},
        {2, 28},
        {3, 31},
        {4, 30},
        {5, 31},
        {6, 30},
        {7, 31},
        {8, 31},
        {9, 30},
        {10, 31},
        {11, 30},
        {12, 31}
        };

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                int d, m, a;
                d = int.Parse(txtDia.Text);
                m = int.Parse(txtMes.Text);
                a = int.Parse(txtAño.Text);
                int diastrascurridos = DiasTranscurridos(d,m,a);
                txtResultado.Text = diastrascurridos.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
               
        private const int BaseYear = 2000;
        private const int DiasBisiesto = 366;
        private const int DiasNoBisiesto = 365;
        public int DiasTranscurridos(int d, int m, int a)
        {
            //Identificar si el año es bisiesto;
            var bisiesto = EsBisiesto(a);
            //Identificar el día máximo por mes;
            var maxDia = mesesDictionary[m];
            //Si es febrero, y a la vez el año es bisiesto, sumar uno
            if (m == 2 && bisiesto)
                maxDia++;
            //Validaciones
            if (a < BaseYear)
                throw new Exception("Ingrese una fecha mayor o igual a 01/Enero/2000");
            if (m < 1 || m > 12)
                throw new Exception("Ingrese una fecha válida (mes no válido)");
            if (d < 1 || d > maxDia)
                throw new Exception("Ingrese una fecha válida (día no válido)");
            int diasTranscurridos = DiasAñosCompletos(a) + DiasUltimoAño(d, m, a);
            return diasTranscurridos - 1;
        }
        private bool EsBisiesto(int a)
        {
            if (a % 4 == 0 && (a % 100 != 0 || a % 400 == 0))
                return true;
            return false;
        }
        private int DiasAñosCompletos(int a)
        {
            int dias = 0;
            for (int i = BaseYear; i < a; i++)
            {
                dias += EsBisiesto(i) ? DiasBisiesto : DiasNoBisiesto;
            }
            return dias;
        }
        private int DiasUltimoAño(int d, int m, int a)
        {
            int dias = 0;
            //Desde el mes de enero, ir sumando los días
            for (int i = 1; i < m; i++)
            {
                var aux = mesesDictionary[i];
                dias += (i == 2 && EsBisiesto(a)) ? aux + 1 : aux;
            }
            dias += d;
            return dias;
        }

        #endregion
    }
}
