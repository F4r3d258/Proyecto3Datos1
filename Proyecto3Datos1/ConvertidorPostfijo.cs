using System;
using System.Collections.Generic;
using System.Text;

namespace ArbolExpresionProyecto
{
    public class ConvertidorPostfijo
    {
        public static List<string> InfijaAPostfija(string expresion)
        {
            List<string> salida = new List<string>();
            Stack<string> operadores = new Stack<string>();

            for (int i = 0; i < expresion.Length; i++)
            {
                char c = expresion[i];

                if (c == ' ')
                    continue;

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
