using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace DemoMultiThread.testDir
{
    public abstract class CustomBaseThread
    {
        private Thread _thread;

        private static Dictionary<string, List<CustomBaseThread>> workers =
            new Dictionary<string, List<CustomBaseThread>>();

        protected ConcurrentBag<PlainJob> jobQueue = new ConcurrentBag<PlainJob>();

        protected CustomBaseThread(string name)
        {
            _thread = new Thread(new ThreadStart(this.RunThread));
            _thread.Name = name;
        }

        public void Start() => _thread.Start();
        public void Join() => _thread.Join();
        public bool IsAlive => _thread.IsAlive;
        protected abstract void RunThread();

        protected abstract void Stop();

        public void Register()
        {
            if (_thread.Name != null)
            {
                var id = _thread.Name;
                if (!workers.ContainsKey(id))
                {
                    var listWorker = new List<CustomBaseThread>();
                    workers.Add(id, listWorker);
                    listWorker.Add(this);
                }
                else
                {
                    var isOk = workers.TryGetValue(id, out var listWorker);
                    if (isOk)
                    {
                        listWorker.Add(this);
                    }
                }

                var isOk2 = workers.TryGetValue(id, out var listWorker2);
                Console.WriteLine(
                    $"Register new worker id:{_thread.Name}, size: {listWorker2?.Count ?? -1}");
            }
        }

        public static void PubJob(string threadName, int chooseWorkerIndex, PlainJob job)
        {
            if (threadName != null)
            {
                var isOk = workers.TryGetValue(threadName, out var listWorker);

                if (isOk)
                {
                    if (chooseWorkerIndex > -1)
                    {
                        listWorker[listWorker.Count % chooseWorkerIndex].jobQueue.Add(job);
                    }
                    else
                    {
                        listWorker[new Random().Next(listWorker.Count)].jobQueue.Add(job);
                    }
                }
            }
        }

        public static void StopAllWorker()
        {
            try
            {
                foreach (var entry in workers)
                {
                    var index = -1;
                    foreach (var worker in entry.Value)
                    {
                        ++index;
                        Console.WriteLine($"Stop worker {index}, group {entry.Key}");
                        worker.Stop();
                    }
                    Console.WriteLine($"Stop all worker in group {entry.Key}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when stop workers: ", ex);
            }
        }
    }
}