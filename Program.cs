using System;
using System.Timers;
using DemoMultiThread.testDir;

namespace DemoMultiThread
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Test.print();
            for (int index = -1; ++index < 5;)
            {
                var worker = new DemoThread("DemoThread");
                worker.Register();
                worker.Start();
            }

            CustomBaseThread.PubJob("DemoThread", -1, new PlainJob("id1", DateTime.Now.Millisecond + 100));
            CustomBaseThread.PubJob("DemoThread", -1, new PlainJob("id2", DateTime.Now.Millisecond + 100));
            
            CustomBaseThread.StopAllWorker();
        }
    }
}