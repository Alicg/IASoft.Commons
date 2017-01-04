using System;
using System.Collections.Generic;
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
            newArtist.Name = "NEW_TEST_ARTIST";
            var context = new chinookEntities();
            context.artists.Add(newArtist);
            context.SaveChanges();

            Assert.IsTrue(newArtist.ArtistId != 0);

            context.artists.Remove(newArtist);
            context.SaveChanges();
        }
    }
}
