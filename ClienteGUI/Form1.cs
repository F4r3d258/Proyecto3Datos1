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
    }
}
