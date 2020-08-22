using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace WebAPI.Helpers
{
    public class TaskScheduler
    {
        public static async Task Start()
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();

            var task = JobBuilder.Create<SearchTask>().Build();

            var trigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule
                (s =>
                    s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                )
                .Build();

            await scheduler.ScheduleJob(task, trigger);
        }
    }
}