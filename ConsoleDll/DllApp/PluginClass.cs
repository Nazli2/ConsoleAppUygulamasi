using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DllApp
{
    public class PluginClass
    {
        public class UserTable
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
        public void Run(SqlConnection cnn, String cmd)
        {
            using (cnn)
            {
                try
                {
                    cnn.Open();
                    Console.WriteLine("Bağlantı başarılı!");

                    if (cmd.StartsWith("select", StringComparison.OrdinalIgnoreCase))
                    {
                        var users = cnn.Query<UserTable>(cmd).ToList();

                        foreach (var u in users)
                        {
                            Console.WriteLine("Id : {0}", u.Id);
                            Console.WriteLine("Adı : {0}", u.FirstName);
                            Console.WriteLine("Soyadı : {0}", u.LastName);
                            Console.WriteLine("");
                        }
                    }
                    else
                    {
                        var rowsAffected = cnn.Execute(cmd, new DynamicParameters());
                        Console.WriteLine("{0} adet kayıt etkilendi", rowsAffected);
                    }

                }
                catch (Exception)
                {

                }
            }
        }
    }
}
