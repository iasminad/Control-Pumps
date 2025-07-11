using DataModel;
using System;
using Communicator;

namespace Monitor
{
    class Program
    {
        static int idCounter = 1;
        static int numarPorniri = 0;
        static DateTime? timpPornire = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Aplicatia de monitorizare in retea a pornit...");
            Receiver receiver = new Receiver("127.0.0.1", 5000);
            receiver.DataReceived += ReceivedSomeData;
            receiver.StartListen();
        }

        private static void ReceivedSomeData(object sender, EventArgs e)
        {
            byte value = (byte)sender;

            if (!Enum.IsDefined(typeof(ProcessState), (int)value))
            {
                Console.WriteLine($"Eroare: Stare necunoscută pentru valoarea {value}");
                return;
            }

            ProcessState currentProcessState = (ProcessState)value;

            Console.WriteLine($"Stare curenta process: {currentProcessState}");

            bool estePornita = currentProcessState == ProcessState.Pornit;

            if (estePornita && timpPornire == null)
            {
                timpPornire = DateTime.Now;
                numarPorniri++;
            }
            else if (!estePornita && timpPornire != null)
            {
                timpPornire = null;
            }

            TimeSpan timpFunctionare = timpPornire.HasValue ? DateTime.Now - timpPornire.Value : TimeSpan.Zero;

            var postData = new ProcessStatusEvent
            {
                Id = idCounter++,
                EstePornita = estePornita,
                NumarPorniri = numarPorniri,
                TimpFunctionare = timpFunctionare,
                timpPornire = timpPornire
            };

            HttpHelper.PostDataToWebAPI(postData);
        }
    }
}
