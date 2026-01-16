using System;
using System.Collections.Generic;

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
        static void Main(string[] args)
        {
            // (5 * 7) + (12 / 6)
            string[] postfija1 = { "5", "7", "*", "12", "6", "/", "+" };

            ArbolExpresion arbol1 = new ArbolExpresion();
            arbol1.ConstruirDesdePostfija(postfija1);

            Console.WriteLine("Resultado 1: " + arbol1.Evaluar());

            // ~(5 & 0)
            string[] postfija2 = { "5", "0", "&", "~" };

            ArbolExpresion arbol2 = new ArbolExpresion();
            arbol2.ConstruirDesdePostfija(postfija2);

            Console.WriteLine("Resultado 2: " + arbol2.Evaluar());

            Console.ReadKey();
        }
    }
}
