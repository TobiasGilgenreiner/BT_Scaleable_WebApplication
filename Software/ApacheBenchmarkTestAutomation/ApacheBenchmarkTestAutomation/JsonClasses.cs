using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacheBenchmarkTestAutomation
{
    public class Server
    {
        public string Tag { get; set; }
        public int ID { get; set; }
        public string Url { get; set; }
    }

    public class Platform
    {
        public string PlatformName { get; set; }
        public List<Server> Servers { get; set; }
    }

    public class Tag
    {
        public string Name { get; set; }
        public List<string> Endpoints { get; set; }
    }

    public class JsonRoot
    {
        public List<Platform> Platforms { get; set; }
        public List<Tag> Tags { get; set; }
    }

    public class TestConfiguration
    {
        public List<int> Requests { get; set; }
        public List<int> Concurrent { get; set; }
    }
}
