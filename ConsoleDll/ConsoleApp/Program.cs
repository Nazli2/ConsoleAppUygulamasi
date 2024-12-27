using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace ConsoleApp
{
    internal class Program
    {
        private static Assembly _currentAssembly = null; 
        private static FileSystemWatcher _dllWatcher; 
        private static string _dllPath = @"DllApp.dll";

        static void Main(string[] args)
        {
            string connectionString = "Server=DESKTOP-B2CLBK8\\SQLEXPRESS;Database=ConsoleDllDb;Integrated Security=True;";
            InitializeDllWatcher(); 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                while (true)
                {
                    Console.WriteLine("1. Kayıt Listele");
                    Console.WriteLine("2. Kayıt Ekle");
                    Console.WriteLine("3. Kayıt Sil");
                    Console.WriteLine("4. Çıkış");
                    Console.Write("Seçim (1/2/3/4): ");
                    string secim = Console.ReadLine();
                    string sqlCommand = null;

                    switch (secim)
                    {
                        case "1":
                            sqlCommand = "Select * From UserTable";
                            break;
                        case "2":
                            Console.Write("Ad Girin: ");
                            string adi = Console.ReadLine();
                            Console.Write("Soyad Girin: ");
                            string soyadi = Console.ReadLine();
                            sqlCommand = $"Insert Into UserTable(FirstName, LastName) Values('{adi}', '{soyadi}')";
                            break;
                        case "3":
                            Console.Write("Silinecek Id Girin: ");
                            int tabloId = Convert.ToInt32(Console.ReadLine());
                            sqlCommand = $"Delete From UserTable Where Id = {tabloId}";
                            break;
                        case "4":
                            Console.WriteLine("Programdan çıkılıyor...");
                            return;
                        default:
                            Console.WriteLine("Geçersiz seçim!");
                            continue;
                    }

                    if (sqlCommand != null)
                    {
                        ExecuteDllMethod(connection, sqlCommand); 
                    }
                }
            }
        }

        /// <summary>
        /// DLL'deki metodu çalıştırır
        /// </summary>
        private static void ExecuteDllMethod(SqlConnection connection, string sqlCommand)
        {
            try
            {
                if (_currentAssembly == null)
                {
                    Console.WriteLine("DLL yükleniyor...");
                    LoadDll(); 
                }

                Type myClassType = _currentAssembly.GetType("DllApp.PluginClass");
                object myClassInstance = Activator.CreateInstance(myClassType);
                MethodInfo myMethodWithParams = myClassType.GetMethod("Run");

                object[] parameters = new object[] { connection, sqlCommand };
                myMethodWithParams.Invoke(myClassInstance, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }

        /// <summary>
        /// DLL'i yükler
        /// </summary>
        private static void LoadDll()
        {
            try
            {
                if (File.Exists(_dllPath))
                {
                    _currentAssembly = Assembly.LoadFrom(_dllPath);
                    Console.WriteLine("DLL başarıyla yüklendi.");
                }
                else
                {
                    Console.WriteLine("DLL dosyası bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DLL yüklenirken hata oluştu: {ex.Message}");
            }
        }

        /// <summary>
        /// DLL değişikliklerini izlemek için watcher başlatır
        /// </summary>
        private static void InitializeDllWatcher()
        {
            _dllWatcher = new FileSystemWatcher
            {
                Path = AppDomain.CurrentDomain.BaseDirectory,
                Filter = _dllPath,
                NotifyFilter = NotifyFilters.LastWrite 
            };

            _dllWatcher.Changed += (sender, e) =>
            {
                Console.WriteLine("DLL değiştirildi. Yeni sürüm yüklenecek...");
                _currentAssembly = null; 
                LoadDll(); 
            };

            _dllWatcher.EnableRaisingEvents = true; 
        }
    }
}
