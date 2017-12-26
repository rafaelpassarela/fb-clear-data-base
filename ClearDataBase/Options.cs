using CommandLine;
using CommandLine.Text;
using System.Text;

namespace ClearDataBase
{
    class Options
    {
        [Option('d', "database", Required = true, HelpText = "DataBase full patch. Ex.: C:\\DATABASENAME.FDB")]
        public string DataBase { get; set; }

        [Option('s', "server", DefaultValue ="LOCALHOST", HelpText ="DataBase server name/IP. Ex.: LOCALHOST")]
        public string ServerName { get; set; }

        [Option('u', "user", DefaultValue = "sysdba", HelpText = "User name.")]
        public string UserName { get; set; }

        [Option('p', "password", DefaultValue = "masterkey", HelpText = "User password.")]
        public string Password { get; set; }

        [Option('e', "encoding", DefaultValue = "utf", HelpText = "SQL encoding type (for sql files). [UTF, ISO, UNICODE, 1252]")]
        public string EncodingName { get; set; }

        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));            
        }

        public Encoding GetEncoding()
        {
            switch (EncodingName)
            {
                case "UTF":
                    return Encoding.UTF8;
                case "ISO":
                    return Encoding.ASCII;
                case "UNICODE":
                    return Encoding.Unicode;
                case "1252":
                    return Encoding.GetEncoding(1252);
            default:
                    return null;
            }
        }

    }
}
