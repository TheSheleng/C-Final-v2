using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Menu
{
    public class InoutStr
    {
        public string Title { get; private set; }
        public InoutStr(string title)
        {
            Title = title;
        }
        public string GetChoice()
        {
            Console.Write($"\n\t{Title}");

            string? str;
            do str = Console.ReadLine();
            while (str == null || str == "");
            Console.Clear();
            return str;
        }
    }
    public class Msg
    {
        public string Message { get; private set; }
        public Msg(string msg)
        {
            Message = msg;
        }
        public void Print()
        {
            Console.WriteLine($"\n\t{Message.Replace("\n", "\n\t")}");
            Console.WriteLine($"\tНажмите на любую клавишу, чтобы продолжить;");
            Console.ReadKey();
            Console.Clear();
        }
    }
    public class YesOrNo
    {
        public string Title { get; private set; }
        public YesOrNo(string title)
        {
            Title = title;
        }
        void PrintMenu(bool chise)
        {
            Console.WriteLine($"\n\t{Title.Replace("\n", "\n\t")}");
            if (chise) Console.WriteLine($"\t > Yes;\t   No;");
            else Console.WriteLine($"\t   Yes;\t > No;");
        }
        public bool GetChoice()
        {
            bool сhoice = false;
            while (true)
            {
                Console.Clear();
                PrintMenu(сhoice);

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.LeftArrow: сhoice = true; break;
                    case ConsoleKey.RightArrow: сhoice = false; break;
                    case ConsoleKey.Enter: Console.Clear(); return сhoice;
                }
            }
        }
    }
    public class Dropdown
    {
        public string Title { get; private set; }
        public string[] Puncts { get; private set; }
        public Dropdown(string title, string fPunct, params string[] Puncts)
        {
            this.Title = title;

            this.Puncts = new string[Puncts.Length + 1];
            this.Puncts[0] = fPunct;
            for (int i = 0; i < Puncts.Length; i++)
            {
                this.Puncts[i + 1] = Puncts[i];
            }
        }
        void PrintMenu(int chise)
        {
            Console.WriteLine($"\n\t{Title.Replace("\n", "\n\t")}");
            for (int i = 0; i < Puncts.Length; i++)
            {
                Console.WriteLine($"\t {(chise == i ? ">" : "")} {Puncts[i]};");
            }
        }
        public int GetChoice()
        {
            int сhoice = 0;
            while (true)
            {
                Console.Clear();
                PrintMenu(сhoice);

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow: сhoice = (сhoice == 0 ? Puncts.Length - 1 : сhoice - 1); break;
                    case ConsoleKey.DownArrow: сhoice = (сhoice == Puncts.Length - 1 ? 0 : сhoice + 1); break;
                    case ConsoleKey.Enter: Console.Clear(); return сhoice;
                }
            }
        }
    }
}
