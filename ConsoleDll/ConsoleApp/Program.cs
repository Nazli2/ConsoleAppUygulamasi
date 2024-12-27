using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp
{
    internal class Program
    {

        static void Main(string[] args)
        {
            string connectionString = "Server=DESKTOP-B2CLBK8\\SQLEXPRESS;Database=ConsoleDllDb;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                string dllPath = @"DllApp.dll";
                string sqlCommand;

                Console.WriteLine("1.Kayıt Listele");
                Console.WriteLine("2.Kayıt Ekle");
                Console.WriteLine("3.Kayıt Sil");
                Console.WriteLine("4.Çıkış");
                Console.Write("Seçim (1/2/3/4) :");
                string secim = Console.ReadLine();
                string adi, soyadi = null;
                int tabloId = 0;

                switch (Convert.ToInt32(secim))
                {
                    case 1:
                        sqlCommand = "Select * From UserTable";
                        break;
                    case 2:
                        Console.Write("Ad Girin :");
                        adi = Console.ReadLine();
                        Console.Write("Soyad Girin :");
                        soyadi = Console.ReadLine();

                        sqlCommand = "Insert Into UserTable(FirstName, LastName) Values('" + adi + "','" + soyadi + "')";
                        break;
                    case 3:
                        Console.Write("Silinecek Id Girin :");
                        tabloId = Convert.ToInt32(Console.ReadLine());
                        sqlCommand = "Delete From UserTable Where Id = " + tabloId.ToString();
                        break;

                    default:
                        sqlCommand = null;
                        break;
                }

                if (sqlCommand != null)
                {
                    Assembly assembly = Assembly.LoadFrom(dllPath);
                    Type myClassType = assembly.GetType("DllApp.PluginClass");
                    object myClassInstance = Activator.CreateInstance(myClassType);

                    MethodInfo myMethodWithParams = myClassType.GetMethod("Run");
                    object[] parameters = new object[] { connection, sqlCommand };
                    myMethodWithParams.Invoke(myClassInstance, parameters);
                }
            }
            Console.WriteLine("Program sonu");
            Console.ReadLine();
        }
    }
}
