namespace DemoMultiThread.testDir
{
    public class PlainJob
    {
        private string _id;
        private long _startTime;

        public PlainJob()
        {
        }

        public PlainJob(string id, long startTime)
        {
            _id = id;
            _startTime = startTime;
        }

        public string Id
        {
            get => _id;
            set => _id = value;
        }

        public long StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }
    }
}