using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using System.Threading.Tasks;
using Structing.Quartz.Annotations;
using Anf.Hitokoto;

namespace Anf.Hitokoto.JobConfigers
{
    [ConfigJob(typeof(FlushWordJob))]
    internal class FlushWordJobConfiger : IJobConfiger
    {
        public const string JobIdentity = "Anf.Services.JobConfigers.FlushWordJob.Key";
        public const string TriggerIdentity = "Anf.Services.JobConfigers.FlushWordJob.Trigger";

        public Task ComplatedAsync(IComplatedScheduleJobContext context)
        {
            return Task.CompletedTask;
        }

        public Task<ConfigResults> ConfigKeyAsync(IJobKeyScheduleJobContext context)
        {
            context.JobBuilder.RequestRecovery()
                .WithIdentity(JobIdentity);
            return Task.FromResult(ConfigResults.Continue);
        }

        public Task<ConfigResults> ConfigTriggerAsync(IJobTriggerScheduleJobContext context)
        {
            var options = context.ReadyContext.GetRequiredService<IOptions<WordFetchOptions>>();
            context.TriggerBuilder.WithIdentity(TriggerIdentity).WithSimpleSchedule(x =>
            {
                x.WithInterval(options.Value.IntervalTime)
                    .RepeatForever();
            });
            return Task.FromResult(ConfigResults.Continue);
        }
    }
}
