using System;
using System.Collections.Concurrent;
using System.Threading;

namespace DemoMultiThread.testDir
{
    public class DemoThread : CustomBaseThread
    {
        private ConcurrentDictionary<string, long> CONCURRENT_CACHE = new ConcurrentDictionary<string, long>();
        private bool _running = true;

        public DemoThread(string name) : base(name)
        {
        }

        protected override void RunThread()
        {
            while (_running)
            {
                long currentTime = DateTime.Now.Millisecond;
                lock (jobQueue)
                {
                    while (!jobQueue.IsEmpty)
                    {
                        var isOk = jobQueue.TryTake(out var job);
                        if (isOk)
                        {
                            ResolveJob(job);
                        }
                    }
                }
                lock (CONCURRENT_CACHE)
                {
                    foreach (var entry in CONCURRENT_CACHE)
                    {
                        if (currentTime < entry.Value)
                        {
                            Console.WriteLine($"Processed id: {entry.Key}, removing {entry.Key} from cached");
                            CONCURRENT_CACHE.TryRemove(entry.Key, out var isOk);
                            Console.WriteLine($"JOB_QUEUE size: {CONCURRENT_CACHE.Count}");
                        }
                        else
                        {
                            Console.WriteLine($"Skipping entry with key {entry.Key}");
                        }
                    }
                }

                try
                {
                    Console.WriteLine("Do sleep");
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Concurrent exception, ", ex);
                }
            }
        }

        protected override void Stop()
        {
            lock (this)
            {
                this._running = false;
            }
        }

        public void ResolveJob(PlainJob job)
        {
            lock (CONCURRENT_CACHE)
            {
                // if this job is still not processed then update it
                Console.WriteLine($"Job with id: {job.Id}, startTime: {job.StartTime}");
                CONCURRENT_CACHE.AddOrUpdate(job.Id, job.StartTime, (key, value) => value);
            }
        }
    }
}