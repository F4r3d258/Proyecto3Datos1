using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ClienteGUI
{
    public partial class Form1 : Form
    {
        TextBox txtExpresion;
        Button btnEnviar;
        Label lblEstado;

        public Form1()
        {
            InitializeComponent();
            ConstruirInterfaz();
        }

        private void ConstruirInterfaz()
        {
            Text = "Cliente Calculadora";
            Size = new Size(400, 200);
            StartPosition = FormStartPosition.CenterScreen;

            Label lblTitulo = new Label();
            lblTitulo.Text = "Ingrese la expresión:";
            lblTitulo.Location = new Point(20, 20);
            lblTitulo.AutoSize = true;

            txtExpresion = new TextBox();
            txtExpresion.Location = new Point(20, 45);
            txtExpresion.Width = 340;

            btnEnviar = new Button();
            btnEnviar.Text = "Enviar";
            btnEnviar.Location = new Point(20, 80);
            btnEnviar.Click += BtnEnviar_Click;

            lblEstado = new Label();
            lblEstado.Text = "Estado:";
            lblEstado.Location = new Point(20, 120);
            lblEstado.AutoSize = true;

            Controls.Add(lblTitulo);
            Controls.Add(txtExpresion);
            Controls.Add(btnEnviar);
            Controls.Add(lblEstado);
        }

        private void BtnEnviar_Click(object sender, EventArgs e)
        {
            string expresion = txtExpresion.Text.Trim();

            if (expresion.Length == 0)
            {
                lblEstado.Text = "Estado: expresión vacía";
                return;
            }

            if (!ExpresionValida(expresion))
            {
                lblEstado.Text = "Estado: símbolos inválidos";
                return;
            }

            lblEstado.Text = "Estado: expresión válida";
        }

        private bool ExpresionValida(string expresion)
        {
            string patron = @"^[0-9\s+\-*/%()&|^~]+$";
            return Regex.IsMatch(expresion, patron);
        }
    }
}
