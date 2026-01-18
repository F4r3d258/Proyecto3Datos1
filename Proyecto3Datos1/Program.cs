using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ArbolExpresionProyecto
{
    public class NodoExpresion
    {
        public string Valor;
        public NodoExpresion Izquierdo;
        public NodoExpresion Derecho;

        public NodoExpresion(string valor)
        {
            Valor = valor;
            Izquierdo = null;
            Derecho = null;
        }

        public bool EsOperador()
        {
            return Valor == "+" || Valor == "-" || Valor == "*" ||
                   Valor == "/" || Valor == "%" || Valor == "**" ||
                   Valor == "&" || Valor == "|" || Valor == "^" ||
                   Valor == "~";
        }
    }

    public class ArbolExpresion
    {
        public NodoExpresion Raiz;

        public void ConstruirDesdePostfija(string[] tokens)
        {
            Stack<NodoExpresion> pila = new Stack<NodoExpresion>();

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];

                if (EsOperando(token))
                {
                    pila.Push(new NodoExpresion(token));
                }
                else
                {
                    NodoExpresion operador = new NodoExpresion(token);

                    if (token == "~")
                    {
                        operador.Derecho = pila.Pop();
                    }
                    else
                    {
                        operador.Derecho = pila.Pop();
                        operador.Izquierdo = pila.Pop();
                    }

                    pila.Push(operador);
                }
            }

            Raiz = pila.Pop();
        }

        private bool EsOperando(string token)
        {
            int numero;
            return int.TryParse(token, out numero);
        }

        public int Evaluar()
        {
            return EvaluarNodo(Raiz);
        }

        private int EvaluarNodo(NodoExpresion nodo)
        {
            if (nodo == null)
                throw new Exception("Nodo nulo");

            if (!nodo.EsOperador())
                return int.Parse(nodo.Valor);

            if (nodo.Valor == "~")
            {
                int valor = EvaluarNodo(nodo.Derecho);
                return valor == 0 ? 1 : 0;
            }

            int izq = EvaluarNodo(nodo.Izquierdo);
            int der = EvaluarNodo(nodo.Derecho);

            if (nodo.Valor == "+") return izq + der;
            if (nodo.Valor == "-") return izq - der;
            if (nodo.Valor == "*") return izq * der;
            if (nodo.Valor == "/") return izq / der;
            if (nodo.Valor == "%") return izq % der;
            if (nodo.Valor == "**") return (int)Math.Pow(izq, der);
            if (nodo.Valor == "&") return (izq != 0 && der != 0) ? 1 : 0;
            if (nodo.Valor == "|") return (izq != 0 || der != 0) ? 1 : 0;
            if (nodo.Valor == "^") return (izq != der) ? 1 : 0;

            throw new Exception("Operador no soportado");
        }
    }

    class Program
    {
        static readonly object fileLock = new object();
        static string archivoCSV = Path.Combine(
            AppContext.BaseDirectory, "registro_operaciones.csv");

        static void Main()
        {
            TcpListener servidor = new TcpListener(IPAddress.Loopback, 5000);
            servidor.Start();

            Console.WriteLine("Servidor iniciado en puerto 5000...");

            while (true)
            {
                TcpClient cliente = servidor.AcceptTcpClient();
                NetworkStream stream = cliente.GetStream();

                byte[] buffer = new byte[4096];
                int bytesLeidos = stream.Read(buffer, 0, buffer.Length);
                string mensajeCompleto = Encoding.UTF8.GetString(buffer, 0, bytesLeidos).Trim();
                string[] partes = mensajeCompleto.Split('|');

                if (partes[0] == "HISTORIAL")
                {
                    string clienteId = partes[1];
                    string historial = LeerHistorial(clienteId);

                    byte[] datosHistorial = Encoding.UTF8.GetBytes(historial);
                    stream.Write(datosHistorial, 0, datosHistorial.Length);
                    cliente.Close();
                    continue;
                }

                try
                {
                    string clienteId = partes[0];
                    string expresion = partes[1];

                    List<string> postfija = ConvertidorPostfijo.InfijaAPostfija(expresion);

                    ArbolExpresion arbol = new ArbolExpresion();
                    arbol.ConstruirDesdePostfija(postfija.ToArray());

                    int resultado = arbol.Evaluar();
                    string respuesta = resultado.ToString();

                    RegistrarOperacion(clienteId, expresion, respuesta);

                    byte[] datos = Encoding.UTF8.GetBytes(respuesta);
                    stream.Write(datos, 0, datos.Length);
                }
                catch
                {
                    byte[] error = Encoding.UTF8.GetBytes("ERROR");
                    stream.Write(error, 0, error.Length);
                }

                cliente.Close();
            }
        }

        static string LeerHistorial(string clienteId)
        {
            if (!File.Exists(archivoCSV))
            return "VACIO";

            var lineas = File.ReadAllLines(archivoCSV);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Fecha,Expresion,Resultado");

            foreach (var linea in lineas)
            {
                var partes = linea.Split(',');
                if (partes.Length == 4 && partes[0] == clienteId)
                {
                    sb.AppendLine($"{partes[1]},{partes[2]},{partes[3]}");
                }
            }

            return sb.ToString().Split('\n').Length <= 1 ? "VACIO" : sb.ToString();
        }

        static string LeerHistorial()
        {
            if (!File.Exists(archivoCSV))
                return "VACIO";

            string contenido = File.ReadAllText(archivoCSV);

            if (contenido.Split('\n').Length <= 1)
                return "VACIO";

            return contenido;
        }

        static void RegistrarOperacion(string clienteId, string expresion, string resultado)
        {
            string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string linea = $"{clienteId},{fecha},{expresion},{resultado}";

            lock (fileLock)
            {
                if (!File.Exists(archivoCSV))
                {
                    File.WriteAllText(archivoCSV, "Fecha,Expresion,Resultado\n");
                }

                File.AppendAllText(archivoCSV, linea + "\n");
            }
        }
    }
}
