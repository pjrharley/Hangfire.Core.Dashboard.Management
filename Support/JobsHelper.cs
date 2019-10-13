using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Hangfire.Core.Dashboard.Management.Metadata;

namespace Hangfire.Core.Dashboard.Management.Support
{
    public static class JobsHelper
    {
        public static List<JobMetadata> Metadata { get; private set; }
        internal static List<QueueAttribute> Pages { get; set; }

        internal static void GetAllJobs(Assembly assembly)
        {
            if(Metadata == null) 
                Metadata = new List<JobMetadata>();
            if(Pages == null)
                Pages = new List<QueueAttribute>();

            foreach (Type ti in  assembly.GetTypes().Where(x => !x.IsInterface && typeof(IJob).IsAssignableFrom(x) && x.Name != (typeof(IJob).Name)))
            {
                var q="default";
                var title = "Default";
                //var menuName = "Default";
                if (ti.GetCustomAttributes(true).OfType<Hangfire.QueueAttribute>().Any())
                {
                    var attr = ti.GetCustomAttribute<QueueAttribute>();
                    q =  attr.Queue;
                    //title = attr.Title;
                    //menuName = attr.MenuName;
                    if(!Pages.Any(x => x.Queue == q)) Pages.Add(attr);
                }
                

                foreach (var methodInfo in ti.GetMethods().Where(m => m.DeclaringType == ti))
                {
                    var meta = new JobMetadata { Type = ti, Queue = q, PageTitle = title};

                    meta.MethodInfo = methodInfo;

                    if (methodInfo.GetCustomAttributes(true).OfType<DescriptionAttribute>().Any())
                    {
                        meta.Description = methodInfo.GetCustomAttribute<DescriptionAttribute>().Description;
                    }

                    if (methodInfo.GetCustomAttributes(true).OfType<DisplayNameAttribute>().Any())
                    {
                        meta.DisplayName = methodInfo.GetCustomAttribute<DisplayNameAttribute>().DisplayName;
                    }

                    Metadata.Add(meta);
                }
            }
        }
    }
}
