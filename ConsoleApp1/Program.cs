using System;
using System.Collections;
using TekVISANet;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args) 
        {
            string response = "";
            Console.WriteLine("Hello World!");
            VISA TVA = new VISA();
            ArrayList resources = new ArrayList();
            TVA.FindResources("?*", out resources);

            foreach (string s in resources)
            {
                Console.WriteLine(s);
            }

            TVA.Open(resources[0].ToString());
            TVA.Write("*IDN?");
            bool status = TVA.Read(out response);
            if (status)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("IDN (device information):");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(response);
            }

            TVA.Write("AUTOSet:DISABLE");
            bool res = TVA.Write("CURVE?");
            Console.WriteLine(res);

            //Setting the oscilloscope for save data
            TVA.Write("SEL:REFA ON");
            TVA.Write("DATA:SOU REFA");

            TVA.Write("DATA:ENCDG SRPbinary");
            TVA.Write("DATA:WIDTH 1");
            TVA.Write("DATA:START 1");
            TVA.Write("DATA:STOP 2500");
          
            TVA.Write("ACQ:STOPAFTER SEQUENCE;MODE SAMPLE;STATE RUN;");
            TVA.Write("ACQ:STATE STOP");
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

        }
    }
}
