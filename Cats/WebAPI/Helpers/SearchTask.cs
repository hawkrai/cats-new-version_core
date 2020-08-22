using System;
using System.IO;
using System.Threading.Tasks;
using Quartz;

namespace WebAPI.Helpers
{
    public class SearchTask : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var indecesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Lucene_indeces");
            if (Directory.Exists(indecesDirectory))
                Directory.Delete(indecesDirectory, true);

            return Task.CompletedTask;
        }
    }
}