using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TestModel;

namespace DALUtils.Tests
{
    [TestFixture]
    public class AddDeleteTests
    {
        [Test]
        public void AddGeneratesId_Test()
        {
            var newArtist = new artist();    
            var conStr = ConfigurationManager.ConnectionStrings["chinookEntities"].ConnectionString;
            conStr = conStr.Replace("%SportFolder%", Environment.GetEnvironmentVariable("SportFolder"));
            newArtist.Name = "NEW_TEST_ARTIST";
            var context = new chinookEntities(conStr);
            context.artists.Add(newArtist);
            context.SaveChanges();

            Assert.IsTrue(newArtist.ArtistId != 0);

            context.artists.Remove(newArtist);
            context.SaveChanges();
        }
    }
}
