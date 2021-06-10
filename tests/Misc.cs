using kriss.Classes;
using NUnit.Framework;
using System.Linq;

namespace tests
{
    public class Misc
    {
        [SetUp]
        public void Setup()
        {
            DataLayer.Init();
        }

        [Test]
        public void ThereAreNoLongDialougesWithReplies()
        {
            var res = DataLayer.Chapters.SelectMany(c => c.Nodes.Where(n => n.Type == "Dialogue"));

            var longOnes = res.SelectMany(x => x.Dialogues.Where(y => y.Break == true));

            var withReplies = longOnes.Where(l => l.Replies != null);

            Assert.IsTrue(!withReplies.Any());
        }
    }
}