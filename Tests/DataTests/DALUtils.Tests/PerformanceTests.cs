using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using TestModel;

namespace DALUtils.Tests
{
    [TestFixture]
    public class PerformanceTests
    {
        [Test]
        public void AddManyEntitiesToBigTable()
        {
            const int SizeOfTestCollection = 1000;
            var context = new chinookEntities();
            var maxId = context.tracks.Max(v => v.TrackId);
            var sw = new Stopwatch();
            var totalAddTime = 0.0;
            for (int i = 0; i < SizeOfTestCollection; i++)
            {
                var track = new track {Name = "Test" + i, MediaTypeId = 1, Milliseconds = 1, UnitPrice = 1};
                sw.Restart();
                context.tracks.Add(track);
                sw.Stop();
                totalAddTime += sw.ElapsedMilliseconds;
            }
            sw.Restart();
            context.SaveChanges();
            sw.Stop();
            var saveChangesAddTime = sw.ElapsedMilliseconds;

            var totalRemoveTime = 0.0;
            var removeCounter = 0;
            foreach (var track in context.tracks.Local.ToArray())
            {
                if (track.TrackId > maxId)
                {
                    sw.Restart();
                    context.tracks.Remove(track);
                    sw.Stop();
                    totalRemoveTime += sw.ElapsedMilliseconds;
                    removeCounter++;
                }
            }

            sw.Restart();
            context.SaveChanges();
            sw.Stop();
            var saveChangesRemoveTime = sw.ElapsedMilliseconds;

            Console.WriteLine("Avarage add time:" + (totalAddTime / SizeOfTestCollection));
            Console.WriteLine("Add save changes:" + saveChangesAddTime);
            Console.WriteLine("Avarage remove time:" + (totalRemoveTime / SizeOfTestCollection));
            Console.WriteLine("Remove save changes:" + saveChangesRemoveTime);
            
            Assert.AreEqual(removeCounter, SizeOfTestCollection);
        }
    }
}