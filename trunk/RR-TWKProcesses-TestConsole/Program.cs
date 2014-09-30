using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DLX_RR_Processes;


/*
    #########################################################################
    ##  THIS IS A SIMPLE TEST CONSOLE FOR TWK actionGeneral CLASS OBJECTS  ##
    #########################################################################
*/
namespace TaskWorker_TestConsole
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Hello. presss a key to start");
      string kpressed;
      kpressed = Console.ReadKey(true).KeyChar.ToString();

      //Bring a reference from your project and use instead of TaskWorker_TestAssembly.testlib1

      //processVendorCreateFolder();
      processArchiver();

      //Pass your <action/> defined as what your TWK action class is expecting.  My example class does care about the actionXML.  
      
      Console.WriteLine("Done!!!!  Please a key to close.");
      kpressed = Console.ReadKey(true).KeyChar.ToString();
    }


    static void processArchiver()
    {

        try
        {

            string loc = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
            string aStr = System.IO.File.ReadAllText(loc + @"\RR_ActionXML\process_Archiver.xml");
            process_Archiver ehn = new process_Archiver();

            ehn.ActionXML = System.Xml.Linq.XElement.Parse(aStr);
            ehn.Completed += new TaskWorker_BLL.actionGeneral.CompletedEventHandler(t1_Completed);
            ehn.EventLog += new TaskWorker_BLL.actionGeneral.EventLogEventHandler(t1_EventLog);
            try
            { ehn.Start(); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
       


    }

    static void processVendorCreateFolder()
    {
        
        string loc = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
        process_Vendor_CreateFolder t1 = new process_Vendor_CreateFolder();
        string aStr = System.IO.File.ReadAllText(loc + @"\RR_ActionXML\process_Vendor_CreateFolder.xml");
         t1.ActionXML = System.Xml.Linq.XElement.Parse(aStr);
         t1.Completed += new TaskWorker_BLL.actionGeneral.CompletedEventHandler(t1_Completed);
         t1.EventLog += new TaskWorker_BLL.actionGeneral.EventLogEventHandler(t1_EventLog);
         try
         { t1.Start(); }
         catch (Exception ex)
         {
             Console.WriteLine(ex.Message);
         }
    }

    private static void t1_EventLog(ref TaskWorker_BLL.actionGeneral sender, string Msg, TaskWorker_BLL.twk_LogLevels Level, System.Reflection.MethodBase exMethod)
    {
        Console.WriteLine(Msg);
    }


    static void t1_Completed(object Sender, string UpdateStatus, string Comment)
    {
      Console.WriteLine("Process has complete.  Press a enter to close");
      Console.ReadLine();
      //throw new NotImplementedException();
    }




  }
}
