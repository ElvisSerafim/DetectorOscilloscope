using System;
using System.Collections;
using TekVISANet;
using MySqlConnector;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args) 
        {
            string response = "";
            Console.WriteLine("Hello World!");
            VISA TVA = new VISA();
            ArrayList resources = new ArrayList();
            TVA.FindResources("?*", out resources); //Searching for oscilloscope connected on PC

            //Showing the osciloscope ID
            foreach (string s in resources)
            {
                Console.WriteLine(s);
            }

            TVA.Open(resources[0].ToString()); //Connecting with the oscilloscope
            TVA.Write("*IDN?");
            bool status = TVA.Read(out response);
            if (status)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("IDN (device information):"); //ID oscilloscope
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(response);
            }

            TVA.Write("AUTOSet:DISABLE");
            bool res = TVA.Write("CURVE?");
            Console.WriteLine(res);

            //Setting the oscilloscope for save data
            TVA.Write("SEL:REFA ON");
            TVA.Write("DATA:SOU REFA");

            //Setting the wave length that will be stored
            TVA.Write("DATA:ENCDG SRPbinary");
            TVA.Write("DATA:WIDTH 1");
            TVA.Write("DATA:START 1");
            TVA.Write("DATA:STOP 2500");
          
            //Starting the acquisition data on Oscilloscope
            TVA.Write("ACQ:STOPAFTER SEQUENCE;MODE SAMPLE;STATE RUN;");
            TVA.Write("ACQ:STATE STOP");

            //Saving the data on REFA memory
            TVA.Write("SAVE:WAVE CH2, REFA");

            
            //Setting the oscilloscope for get negative width
            string nWidth;
            TVA.Write("MEASUREMENT:IMMED:TYPE NWIDTH");
            TVA.Write("MEASUREMENT:IMMED:SOURCE CH2");
            TVA.Write("MEASUREMENT:IMMED:VALUE?");
            TVA.Read(out nWidth);
            Console.WriteLine(nWidth);

            //Setting the oscilloscope for get falling time
            string fallTime;
            TVA.Write("MEASUREMENT:IMMED:TYPE FALL");
            TVA.Write("MEASUREMENT:IMMED:SOURCE CH2");
            TVA.Write("MEASUREMENT:IMMED:VALUE?");
            TVA.Read(out fallTime);
            Console.WriteLine(fallTime);


            //Setting the oscilloscope for get rise time
            string riseTime;
            TVA.Write("MEASUREMENT:IMMED:TYPE RISE");
            TVA.Write("MEASUREMENT:IMMED:SOURCE CH2");
            TVA.Write("MEASUREMENT:IMMED:VALUE?");
            TVA.Read(out riseTime);
            Console.WriteLine(riseTime);

            //Setting the oscilloscope for get Vpp(peak-to-peak value)
            string pkp;
            TVA.Write("MEASUREMENT:IMMED:TYPE PK2PK");
            TVA.Write("MEASUREMENT:IMMED:SOURCE CH2");
            TVA.Write("MEASUREMENT:IMMED:VALUE?");
            TVA.Read(out pkp);
            Console.WriteLine(pkp);

            //Setting the Storage information
            var builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                Database = "detectorlabensol",
                UserID = "root",
                Password = "projetoMCA20",
                SslMode = MySqlSslMode.Required,
            };

            //Establishing the connection with the Database
            using (var conn = new MySqlConnection(builder.ConnectionString))
            {
                Console.WriteLine("Opening connection");
                await conn.OpenAsync();

                using (var command = conn.CreateCommand())
                {

                    command.CommandText = @"INSERT INTO detection (negativeWitdh, fallTime, riseTime, peakToPeak) VALUES (@name1, @name2, @name3, @name4);"; //Inserting the data on table
                    command.Parameters.AddWithValue("@name1", nWidth);
                    command.Parameters.AddWithValue("@name2", fallTime);
                    command.Parameters.AddWithValue("@name3", riseTime);
                    command.Parameters.AddWithValue("@name4", pkp);

                    int rowCount = await command.ExecuteNonQueryAsync();
                    Console.WriteLine(String.Format("Number of rows inserted={0}", rowCount));
                }

                // connection will be closed by the 'using' block
                Console.WriteLine("Closing connection");
            }

        }
    }
}
