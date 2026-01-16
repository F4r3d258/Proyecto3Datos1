using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClienteGUI
{
    public partial class Form1 : Form
    {
        TextBox txtExpresion;
        Button btnCalcular;
        Label lblResultado;
        DataGridView dgvHistorial;
        Button btnHistorial;

        public Form1()
        {
            InitializeComponent();
            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            this.Text = "Cliente - Evaluador";
            this.Width = 400;
            this.Height = 200;

            txtExpresion = new TextBox();
            txtExpresion.Left = 20;
            txtExpresion.Top = 20;
            txtExpresion.Width = 340;

            btnCalcular = new Button();
            btnCalcular.Text = "Calcular";
            btnCalcular.Left = 20;
            btnCalcular.Top = 60;
            btnCalcular.Click += BtnCalcular_Click;

            lblResultado = new Label();
            lblResultado.Left = 20;
            lblResultado.Top = 100;
            lblResultado.Width = 340;
            lblResultado.Text = "Resultado:";

            btnHistorial = new Button();
            btnHistorial.Text = "Ver historial";
            btnHistorial.Left = 120;
            btnHistorial.Top = 60;
            btnHistorial.Click += BtnHistorial_Click;

            dgvHistorial = new DataGridView();
            dgvHistorial.Left = 20;
            dgvHistorial.Top = 140;
            dgvHistorial.Width = 340;
            dgvHistorial.Height = 200;
            dgvHistorial.ColumnCount = 3;
            dgvHistorial.Columns[0].Name = "Fecha";
            dgvHistorial.Columns[1].Name = "Expresión";
            dgvHistorial.Columns[2].Name = "Resultado";

            this.Height = 400;

            this.Controls.Add(btnHistorial);
            this.Controls.Add(dgvHistorial);
            this.Controls.Add(txtExpresion);
            this.Controls.Add(btnCalcular);
            this.Controls.Add(lblResultado);
        }

        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            string expresion = txtExpresion.Text;

            if (!ExpresionValida(expresion))
            {
                MessageBox.Show("La expresión contiene caracteres no válidos");
                return;
            }

            try
            {
                TcpClient cliente = new TcpClient("127.0.0.1", 5000);
                NetworkStream stream = cliente.GetStream();

                byte[] datos = Encoding.UTF8.GetBytes(expresion);
                stream.Write(datos, 0, datos.Length);

                byte[] buffer = new byte[1024];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string respuesta = Encoding.UTF8.GetString(buffer, 0, bytes);

                lblResultado.Text = respuesta == "ERROR"
                    ? "Resultado: Error en expresión"
                    : "Resultado: " + respuesta;

                cliente.Close();
            }
            catch
            {
                MessageBox.Show("No se pudo conectar con el servidor");
            }
        }
        private bool ExpresionValida(string expr)
        {
            string permitidos = "0123456789+-*/%()~&|^ ";

            foreach (char c in expr)
            {
                if (!permitidos.Contains(c))
                    return false;
            }

            return true;
        }

        private void BtnHistorial_Click(object sender, EventArgs e)
        {
            try
            {
                TcpClient cliente = new TcpClient("127.0.0.1", 5000);
                NetworkStream stream = cliente.GetStream();

                byte[] datos = Encoding.UTF8.GetBytes("HISTORIAL");
                stream.Write(datos, 0, datos.Length);

                byte[] buffer = new byte[8192];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string respuesta = Encoding.UTF8.GetString(buffer, 0, bytes);

                cliente.Close();

                if (respuesta == "VACIO")
                {
                    MessageBox.Show("No hay historial disponible");
                    return;
                }

                dgvHistorial.Rows.Clear();

                string[] lineas = respuesta.Split('\n');

                for (int i = 1; i < lineas.Length; i++) // saltar encabezado
                {
                    if (string.IsNullOrWhiteSpace(lineas[i])) continue;
                    string[] datosFila = lineas[i].Split(',');
                    dgvHistorial.Rows.Add(datosFila);
                }
            }
            catch
            {
                MessageBox.Show("Error al obtener historial");
            }
        }

    }
}
