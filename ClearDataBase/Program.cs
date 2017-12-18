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
                //Console.WriteLine(CultureInfo.CurrentUICulture.Name);
                ColorConsole.WriteLine(strings.main_msg.Red());
                ColorConsole.Write(options.DataBase.Yellow(), $" {strings.main_msg_continue} {strings.yes}: ".Red());
                var answer = Console.ReadLine();
                if (answer.ToUpper() == strings.yes)
                {
                    var parser = new Parser(options, new LogScreenWriter());
                    parser.Execute();
                }
            }

            Console.WriteLine(strings.hit_close);
            Console.ReadKey();
        }

    }
}
