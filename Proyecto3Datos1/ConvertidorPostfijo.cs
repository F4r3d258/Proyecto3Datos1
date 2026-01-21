using System;
using System.Collections.Generic;
using System.Text;

namespace ArbolExpresionProyecto
{
    public class ConvertidorPostfijo
    {
        //metodo que convierte notaciones infijas a postfijas
        public static List<string> InfijaAPostfija(string expresion)
        {
            List<string> salida = new List<string>();
            Stack<string> operadores = new Stack<string>();

            for (int i = 0; i < expresion.Length; i++)
            {
                char c = expresion[i];

                if (c == ' ')
                    continue;

                //se pueden admitir numeros de más de un dígito
                if (char.IsDigit(c))
                {
                    StringBuilder numero = new StringBuilder();
                    while (i < expresion.Length && char.IsDigit(expresion[i]))
                    {
                        numero.Append(expresion[i]);
                        i++;
                    }
                    i--;
                    salida.Add(numero.ToString());
                }
                else if (c == '(')
                {
                    operadores.Push("(");
                }
                else if (c == ')')
                {
                    while (operadores.Peek() != "(")
                        salida.Add(operadores.Pop());

                    operadores.Pop();
                }
                else
                {
                    string op = c.ToString();

                    //se determina el doble asterisco como operador de potencia
                    if (c == '*' && i + 1 < expresion.Length && expresion[i + 1] == '*')
                    {
                        op = "**";
                        i++;
                    }

                    while (operadores.Count > 0 &&
                           Prioridad(operadores.Peek()) >= Prioridad(op))
                    {
                        salida.Add(operadores.Pop());
                    }

                    operadores.Push(op);
                }
            }

            while (operadores.Count > 0)
                salida.Add(operadores.Pop());

            return salida;
        }

        //se determina la prioridad de las operaciones
        private static int Prioridad(string op)
        {
            if (op == "~") return 4;
            if (op == "**") return 3;
            if (op == "*" || op == "/" || op == "%") return 2;
            if (op == "+" || op == "-") return 1;
            if (op == "&" || op == "|" || op == "^") return 0;
            return -1;
        }
    }
}
