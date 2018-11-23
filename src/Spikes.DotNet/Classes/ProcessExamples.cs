using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using NUnit.Framework;

namespace Eca.Spikes.DotNet.Classes
{
    [TestFixture]
    public class ProcessExamples
    {
        [Test]
        public void Can_list_processes_using_WMI_query()
        {
            using (var searcher = new ManagementObjectSearcher($"Select * From Win32_Process"))
            using (var processes = searcher.Get())
            {
                var ids = processes.AsEnumerable().Select(x => x["ProcessId"]).Cast<uint>().ToList();
                Assert.That(ids, Is.Not.Empty);

                ids.ForEach(id => { Console.Out.WriteLine("id = {0}", id); });
            }
        }

        [Test]
        public void Can_join_parent_child_processes_using_WMI_query()
        {
            using (var searcher = new ManagementObjectSearcher($"Select * From Win32_Process"))
            using (var processes = searcher.Get())
            {
                var processList = processes.AsEnumerable().ToList();

                var query = from child in processList
                    join parent in processList on child["ParentProcessId"] equals parent["ProcessId"]
                        into leftJoin
                    from parent in leftJoin.DefaultIfEmpty()
                    select new {child, parent};

                var ids = query.Select(x => new
                {
                    ChildId = x.child["ProcessId"],
                    ParentId = x.parent?["ProcessId"]
                }).ToList();

                Assert.That(ids, Is.Not.Empty);

                ids.ForEach(id => { Console.Out.WriteLine("id = {0}", id); });
            }
        }

        [Test]
        public void Can_list_orphaned_process()
        {
            using (var searcher = new ManagementObjectSearcher($"Select * From Win32_Process"))
            using (var processes = searcher.Get())
            {
                var processList = processes.AsEnumerable().ToList();

                var parentchild = from child in processList
                    join parent in processList on child["ParentProcessId"] equals parent["ProcessId"]
                        into leftJoin
                    from parent in leftJoin.DefaultIfEmpty()
                    select new {child, parent};

                var orphans = from x in parentchild
                    let childCreation = ManagementDateTimeConverter.ToDateTime(x.child["CreationDate"].ToString())
                    where x.parent == null ||
                          childCreation < ManagementDateTimeConverter.ToDateTime(x.parent["CreationDate"].ToString())
                    select x.child;

                var results = orphans.Select(x => new { Id = x["ProcessId"], Name = x["Name"] }).ToList();

                Assert.That(results, Is.Not.Empty);

                results.ForEach(id => { Console.Out.WriteLine("process = {0}", id); });
            }
        }
    }

    public static class ManagementObjectExts
    {
        public static IEnumerable<ManagementBaseObject> AsEnumerable(this ManagementObjectCollection collection)
        {
            foreach (ManagementBaseObject o in collection)
            {
                yield return o;
            }
        }
    }
}