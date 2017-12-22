using ColoredConsole;
using System;
using System.IO;

namespace LogUtils
{
    public interface ILogWriter
    {
        void Message(string mens);
        void MessageLn(string mens);

        void Error(string mens);
        void ErrorLn(string mens);

        int GetExecOrder();
    }

    public class LogFileWriter : ILogWriter
    {
        private string fileName;
        private int execOrder = 0;

        public LogFileWriter(bool clearFile = true)
        {
            fileName = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)  + "\\ClearDataBase.log";
            
            if (clearFile && File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        public void Error(string mens)
        {
            Message(mens);
        }

        public void ErrorLn(string mens)
        {
            MessageLn(mens);
        }

        public int GetExecOrder()
        {
            return execOrder++;
        }

        public void Message(string mens)
        {
            using (StreamWriter sw = new StreamWriter(fileName, File.Exists(fileName)))
            {
                sw.Write(mens);
                sw.Close();
            }
        }

        public void MessageLn(string mens)
        {
            using (StreamWriter sw = new StreamWriter(fileName, File.Exists(fileName)))
            {
                sw.WriteLine(mens);
                sw.Close();
            }
        }
    }

    public class LogScreenWriter : ILogWriter
    {
        private readonly ILogWriter file = new LogFileWriter();

        public void Error(string mens)
        {
            ColorConsole.Write(mens.Red());
            file.Error(mens);
        }

        public void ErrorLn(string mens)
        {
            ColorConsole.WriteLine(mens.Red());
            file.ErrorLn(mens);
        }

        public int GetExecOrder()
        {
            return file.GetExecOrder();
        }

        public void Message(string mens)
        {
            Console.Write(mens);
            file.Message(mens);
        }

        public void MessageLn(string mens)
        {
            Console.WriteLine(mens);
            file.MessageLn(mens);
        }
    }
}
