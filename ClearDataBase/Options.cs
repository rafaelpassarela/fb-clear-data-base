using CommandLine;
using CommandLine.Text;

namespace ClearDataBase
{
    class Options
    {
        [Option('d', "database", Required = true, HelpText = "Caminho para o Banco de Dados. Ex.: C:\\BANCO.FDB")]
        public string DataBase { get; set; }

        [Option('s', "server", DefaultValue ="LOCALHOST", HelpText ="Servidor do banco de dados (nome ou IP)")]
        public string ServerName { get; set; }

        [Option('u', "user", DefaultValue = "sysdba", HelpText = "Nome do usuário.")]
        public string UserName { get; set; }

        [Option('p', "password", DefaultValue = "masterkey", HelpText = "Senha do usuário.")]
        public string Password { get; set; }

        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));            
        }

    }
}
