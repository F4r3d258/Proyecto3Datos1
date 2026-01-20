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
        Button btnHistorial;
        Label lblResultado;
        DataGridView dgvHistorial;
        string clienteId = Guid.NewGuid().ToString(); // Crea un ID único para el cliente

        public Form1()
        {
            CrearInterfaz();
        }

        private void CrearInterfaz()
        {
            this.Text = "Cliente - Evaluador";
            this.Width = 600;
            this.Height = 500;

            txtExpresion = new TextBox();
            txtExpresion.SetBounds(20, 20, 540, 30);

            btnCalcular = new Button();
            btnCalcular.Text = "Calcular";
            btnCalcular.SetBounds(20, 60, 120, 30);
            btnCalcular.Click += BtnCalcular_Click;

            btnHistorial = new Button();
            btnHistorial.Text = "Ver historial";
            btnHistorial.SetBounds(160, 60, 120, 30);
            btnHistorial.Click += BtnHistorial_Click;

            lblResultado = new Label();
            lblResultado.Text = "Resultado:";
            lblResultado.SetBounds(20, 100, 540, 30);

            dgvHistorial = new DataGridView();
            dgvHistorial.SetBounds(20, 140, 540, 300);

            dgvHistorial.AllowUserToAddRows = false;
            dgvHistorial.ReadOnly = true;
            dgvHistorial.RowHeadersVisible = false;
            dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistorial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistorial.AutoGenerateColumns = false;

            dgvHistorial.Columns.Add("Fecha", "Fecha");
            dgvHistorial.Columns.Add("Expresion", "Expresión");
            dgvHistorial.Columns.Add("Resultado", "Resultado");

            this.Controls.Add(txtExpresion);
            this.Controls.Add(btnCalcular);
            this.Controls.Add(btnHistorial);
            this.Controls.Add(lblResultado);
            this.Controls.Add(dgvHistorial);
        }

        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            try
            {
                TcpClient cliente = new TcpClient("127.0.0.1", 5000); // Conectar al servidor 
                NetworkStream stream = cliente.GetStream();

                string expresion = txtExpresion.Text;
                string mensaje = clienteId + "|" + expresion;
                byte[] datos = Encoding.UTF8.GetBytes(mensaje);
                stream.Write(datos, 0, datos.Length);

                byte[] buffer = new byte[1024];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string respuesta = Encoding.UTF8.GetString(buffer, 0, bytes);

                lblResultado.Text = "Resultado: " + respuesta;

                cliente.Close();
            }
            catch
            {
                MessageBox.Show("Error al conectar con el servidor");
            }
        }

        private void BtnHistorial_Click(object sender, EventArgs e)
        {
            try
            {
                TcpClient cliente = new TcpClient("127.0.0.1", 5000);
                NetworkStream stream = cliente.GetStream();

                string mensaje = "HISTORIAL|" + clienteId;
                byte[] datos = Encoding.UTF8.GetBytes(mensaje);
                stream.Write(datos, 0, datos.Length);

                byte[] buffer = new byte[8192];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string respuesta = Encoding.UTF8.GetString(buffer, 0, bytes);

                cliente.Close();

                dgvHistorial.Rows.Clear();

                if (respuesta == "VACIO")
                {
                    MessageBox.Show("No hay historial");
                    return;
                }

                string[] lineas = respuesta.Split('\n');

                for (int i = 1; i < lineas.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lineas[i])) continue;

                    string[] columnas = lineas[i].Split(',');

                    if (columnas.Length == 3)
                        dgvHistorial.Rows.Add(columnas[0], columnas[1], columnas[2]);
                }
            }
            catch
            {
                MessageBox.Show("Error al obtener historial");
            }
        }
    }
}
