using System;
using System.Text;
using NUnit.Framework;
using murray.common.extensions;

namespace murray.common_test
{
    [TestFixture]
    public class Test_Class_StringBuilderExtensions
    {
        /// <summary>
        /// Tests for the method
        /// </summary>
        public class AppendLineFormat
        {

            [Test]
            public void WorksWithNullFormatAndNoParams()
            {
                var sb = new StringBuilder();
                sb.AppendLineFormat(null);
                Assert.AreEqual(Environment.NewLine, sb.ToString());
            }

            [Test]
            public void ThrowsExceptionWithNullFormatParamAndExtraParams()
            {
                var sb = new StringBuilder();
                sb.AppendLineFormat(null);
                Assert.AreEqual(Environment.NewLine, sb.ToString());

                Assert.Throws<ArgumentNullException>(() => sb.AppendLineFormat(null, 1, 2, 3));
            }

            [Test]
            public void ThrowsExceptionWithExtraParams()
            {
                var sb = new StringBuilder();
                sb.AppendLineFormat(null);
                Assert.AreEqual(Environment.NewLine, sb.ToString());

                Assert.Throws<FormatException>(() => sb.AppendLineFormat("{0}{1}{3}", 1)); //missing 2 params into format
            }

            [Test]
            public void WorksWithExtraParams()
            {
                var sb = new StringBuilder();
                sb.AppendLineFormat(null);
                Assert.AreEqual(Environment.NewLine, sb.ToString());

                sb.AppendLineFormat("{0}", 1, 2, 3); //2 extra params into format

                Assert.AreEqual(Environment.NewLine + "1" + Environment.NewLine, sb.ToString());
            }
        }

    }
}
