using ColoredConsole;
using Domain;
using System;

namespace ClearDataBase
{
    class Program
    {
        static void Main(string[] args)
        {
            Options options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, options);

            if (options.DataBase == null)
            {
                Console.Write(options.GetUsage());
            }
            else
            {
                ColorConsole.WriteLine("Esse processo irá remover todas as Triggers, Procedures e Chaves do banco ".Red());
                ColorConsole.Write(options.DataBase.Yellow(), " Para continuar, escreva SIM: ".Red());
                var answer = Console.ReadLine();
                if (answer.ToUpper() == "SIM")
                {
                    var parser = new Parser(options, new LogScreenWriter());
                    parser.Execute();
                }
            }
            
            Console.WriteLine("Aperte qualquer tecla para fechar.");
            Console.ReadKey();
        }

    }
}
