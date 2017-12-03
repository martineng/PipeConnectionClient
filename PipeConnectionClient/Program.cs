using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Principal;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.IO.Pipes;

namespace PipeConnectionClient
{
    class Program
    {
        private static int numClients = 2;

        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "spawnclient")
                {
                    NamedPipeClientStream myPipeClient = new NamedPipeClientStream(".", "mypipe", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);

                    Console.WriteLine("Connecting to the server...");

                    myPipeClient.Connect();

                    StreamString theStreamString = new StreamString(myPipeClient);

                    // A signature string was hard coded in PipeConnection Server Side,
                    // Before we do the action, we need to verify and validate the server
                    if (theStreamString.ReadString() == "Hello, Friend")
                    {
                        // Sending the name of the file which will be returned by the server later
                        theStreamString.WriteString("openthis.txt");

                        Console.Write(theStreamString.ReadString());
                    } // END IF
                    else
                    {
                        Console.WriteLine("Server not found.");
                    } // END ELSE

                    myPipeClient.Close();

                    Thread.Sleep(4000);
                } // END IF
            }
            else
            {
                Console.WriteLine("Name pipe client stream with impersonatin example");
                startClient();
            } // END ELSE
            
        } // END main()

        // This method create pipe process for client
        private static void startClient()
        {
            int count;
            string currentProcessName = Environment.CommandLine;
            Process[] pList = new Process[numClients];

            Console.WriteLine("Spawning client processes...");

            if (currentProcessName.Contains(Environment.CurrentDirectory))
            {
                currentProcessName = currentProcessName.Replace(Environment.CurrentDirectory, String.Empty);
            } // END IF

            currentProcessName = currentProcessName.Replace("\\", String.Empty);
            currentProcessName = currentProcessName.Replace("\"", String.Empty);

            for (count = 0; count < numClients; count++)
            {
                pList[count] = Process.Start(currentProcessName, "spawnclient");
            } // END FOR

            while (count > 0)
            {
                for (int pCount = 0; pCount < numClients; pCount++)
                {
                    if (pList[pCount] != null)
                    {
                        if (pList[pCount].HasExited)
                        {
                            Console.WriteLine("Client process[{0} has exited.]", pList[pCount].Id);
                            pList[pCount] = null;
                            count--;
                        } // END IF
                        else
                        {
                            Thread.Sleep(250);
                        } // END ELSE
                    } // END IF
                } // END FOR
            } // END WHILE

            Console.WriteLine("Client processes finished, exiting.");
            Console.WriteLine("Enter any key to exit the program");
            Console.ReadLine();
        }
    }
}
