using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RSPMTests
{
    [TestFixture]
    public class SourceReaderTests
    {
        [Test]
        public void LoadPackageSources_ConvertStringIntoUri_ParsedAllThreeUri()
        {
            RSPM.DefaultPackageManager packManager = new RSPM.DefaultPackageManager(null, new MockSourceReader());
            packManager.LoadPackageSources();
            var result = packManager.Sources;
            Assert.IsTrue(result.Count == 3);
        }
    }
}