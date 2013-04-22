using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using murray.common;

namespace murray.common_test
{
    [TestFixture]
    public class Test_Class_io
    {
        /// <summary>
        /// Tests for the method
        /// </summary>
        public class WriteTextFile
        {

            [Test]
            public void ThrowsExceptionWithInvalidPath()
            {
                //invalid path format
                Assert.Throws<NotSupportedException>(() => io.WriteTextFile("fdsAF:DSAFDsafd", "file contents"), "Unknown path format");

                //invalid local path
                Assert.Throws<DirectoryNotFoundException>(() => io.WriteTextFile("c:\\non_existant_folder\\foo.txt", "file contents"));

                //invalid unc path
                Assert.Throws<ArgumentException>(() => io.WriteTextFile("\\\\test_unc_path_style_server", "file contents"));
                Assert.Throws<IOException>(() => io.WriteTextFile("\\\\test_unc_path_style_server\\sharedfolder", "file contents"));
            }

            /// <summary>
            /// Make sure that even if we write a null string to the file, that when the file is read back, it returns an empty string
            /// </summary>
            [Test]
            public void WorksWithNullStringContents()
            {
                //Arrange
                var tempfile = Path.GetTempFileName();

                //Act
                io.WriteTextFile(tempfile, null);
                var contents = File.ReadAllText(tempfile);

                //Assert
                Assert.AreEqual(contents, string.Empty);
            }

            [Test]
            public void WorksWithNonNullContents()
            {
                //Arrange
                var contentsToTest = new List<string>{"", " ", "\t", Environment.NewLine, "foo", "foo\\bar"};

                foreach (var contentTest in contentsToTest)
                {
                    var tempfile = Path.GetTempFileName();

                    //Act
                    io.WriteTextFile(tempfile, contentTest);
                    var contents = File.ReadAllText(tempfile);

                    //Assert
                    Assert.AreEqual(contentTest, contents);
                }
            }
        }

        /// <summary>
        /// Tests for the method
        /// </summary>
        public class ReadASCIITextFile
        {
             
            [Test]
            public void ThrowsExceptionWithInvalidPath()
            {
                //invalid path format
                Assert.Throws<NotSupportedException>(() => io.ReadASCIITextFile("fdsAF:DSAFDsafd"));

                //invalid local path
                Assert.Throws<DirectoryNotFoundException>(() => io.ReadASCIITextFile("a:\\non_existant_folder\\foo.txt"));

                //invalid unc path
                Assert.Throws<ArgumentException>(() => io.ReadASCIITextFile("\\\\test_unc_path_style_server"));
                Assert.Throws<IOException>(() => io.ReadASCIITextFile("\\\\test_unc_path_style_server\\sharedfolder"));
            }

            [Test]
            public void WorksWithNullStringContents()
            {
                //Arrange
                var tempfile = Path.GetTempFileName();

                //Act
                var contents = io.ReadASCIITextFile(tempfile);

                //Assert
                Assert.AreEqual(contents, string.Empty);
            }

            [Test]
            public void WorksWithNonNullContents()
            {
                //Arrange
                var contentsToTest = new List<string> { "", " ", "\t", Environment.NewLine, "foo", "foo\\bar" };

                foreach (var contentTest in contentsToTest)
                {
                    var tempfile = Path.GetTempFileName();

                    //Act
                    io.WriteTextFile(tempfile, contentTest);
                    var contents = io.ReadASCIITextFile(tempfile);

                    //Assert
                    Assert.AreEqual(contentTest, contents);
                }
            }

            [Test]
            public void WorksWhenTriggeringGarbageCollector()
            {
                //Arrange
                var contentTest = "123456789";
                var tempfile = Path.GetTempFileName();

                //Act
                io.WriteTextFile(tempfile, contentTest);
                var contents = io.ReadASCIITextFile(tempfile, contentTest.Length - 1);

                //Assert
                Assert.AreEqual(contentTest, contents);
            }
        }


        /// <summary>
        /// Tests for the method
        /// </summary>
        public class ReadFullStream
        {

            [Test]
            public void ThrowsExceptionWithNullStream()
            {
                //invalid path format
                Assert.Throws<ArgumentException>(() => io.ReadFullStream(null));
            }

            [Test]
            public void WorksWithEmptyMemoryStream()
            {
                //Arrange
                var stream = new MemoryStream();

                //Act
                var bytes = io.ReadFullStream(stream);

                //Assert
                Assert.AreEqual(bytes.Length, 0);
            }

            [Test]
            public void WorksWithValidMemoryStream()
            {
                //Arrange
                var buffer = new byte[10];
                var stream = new MemoryStream(buffer);

                //Act
                var bytes = io.ReadFullStream(stream);

                //Assert
                Assert.AreEqual(bytes.Length, 10);
            }

            [Test]
            public void WorksWithNonMemoryStream()
            {
                //Arrange
                var testString = "abcdef";
                var tempfile = Path.GetTempFileName();
                io.WriteTextFile(tempfile, testString);
                var stream = File.OpenRead(tempfile);

                //Act
                var bytes = io.ReadFullStream(stream);

                //Assert
                Assert.AreEqual(bytes.Length, testString.Length);
            }
        }


        /// <summary>
        /// Tests for the method
        /// </summary>
        public class GetCurrentDirectory
        {
            [Test]
            public void PathContainsBackslash()
            {
                //Arrange
                //Act
                var dir = io.GetCurrentDirectory();

                Console.WriteLine("Current dir: " + dir);

                //Assert
                Assert.IsTrue(dir.Contains("\\"));
            }

            [Test]
            public void DoesNotContainFileProtocol()
            {
                //Arrange
                //Act
                var dir = io.GetCurrentDirectory();

                Console.WriteLine("Current dir: " + dir);

                //Assert
                Assert.IsFalse(dir.Contains("file:\\"));
            }
        }

        /// <summary>
        /// Tests for the method
        /// </summary>
        public class GetCurrentDirectoryFromAssembly
        {
            [Test]
            public void ReturnsNullWhenNullAssemblyPassed()
            {
                Assert.IsNull(io.GetCurrentDirectoryFromAssembly(null));
            }
            [Test]
            public void DoesNotThrowExceptions()
            {
                Assert.DoesNotThrow(() => io.GetCurrentDirectoryFromAssembly(null));
                Assert.DoesNotThrow(() => io.GetCurrentDirectoryFromAssembly(Assembly.GetExecutingAssembly()));
            }

            [Test]
            public void ReturnsStringWithBackslash()
            {
                //arrange and act
                var path = io.GetCurrentDirectoryFromAssembly(Assembly.GetExecutingAssembly());

                //assert
                Assert.IsTrue(path.Contains("\\"));
            }
        }


        public class EnsurePathToFile
        {
            [Test]
            public void ThrowsExceptionWithInvalidPath()
            {
                //null 
                Assert.Throws<ArgumentNullException>(() => io.EnsurePathToFile(null));

                //only a filename 
                Assert.Throws<ArgumentException>(() => io.EnsurePathToFile("foo.txt"));

                //invalid path format
                Assert.Throws<ArgumentException>(() => io.EnsurePathToFile("fdsAF:DSAFDsafd"));

                //invalid unc path
                Assert.Throws<ArgumentNullException>(() => io.EnsurePathToFile("\\\\test_unc_path_style_server"));
                Assert.Throws<ArgumentNullException>(() => io.EnsurePathToFile("\\\\test_unc_path_style_server\\sharedfolder"));
            }

            [Test]
            public void CreatesDirectoryWhenDirectoryDoesNotExist()
            {
                //Arrange
                var path = Path.Combine(Path.GetTempPath(), "tempdirA" + DateTime.Now.Ticks);
                var pathAndFilename = Path.Combine(path, "foo.txt");
                
                //Act 
                Assert.IsFalse(Directory.Exists(path));
                io.EnsurePathToFile(pathAndFilename);

                //Assert
                Assert.IsTrue(Directory.Exists(path));
            }

            [Test]
            public void DoesNotThrowExceptionWhenCallingMultipleTimesAfterCreatingDirectory()
            {
                //Arrange
                var path = Path.Combine(Path.GetTempPath(), "tempdirB" + DateTime.Now.Ticks);
                var pathAndFilename = Path.Combine(path, "foo.txt");

                //Act 
                Assert.IsFalse(Directory.Exists(path));
                io.EnsurePathToFile(pathAndFilename);
                io.EnsurePathToFile(pathAndFilename);
                io.EnsurePathToFile(pathAndFilename);

                //Assert
                Assert.IsTrue(Directory.Exists(path));
            }


            [Test]
            public void CreatesDirectoryWhenDirectoryDoesNotExistAndUsesCache()
            {
                //Arrange
                var path = Path.Combine(Path.GetTempPath(), "tempdirC" + DateTime.Now.Ticks);
                var pathAndFilename = Path.Combine(path, "foo.txt");
                var cacheMs = 1000;
                var sw = new Stopwatch();

                //Act 
                Assert.IsFalse(Directory.Exists(path));
                sw.Start();
                io.EnsurePathToFile(pathAndFilename, cacheMs);
                var ticks1 = sw.ElapsedTicks;
                io.EnsurePathToFile(pathAndFilename, cacheMs);
                var ticks2 = sw.ElapsedTicks - ticks1;
                io.EnsurePathToFile(pathAndFilename, cacheMs);
                var ticks3 = sw.ElapsedTicks - ticks1 - ticks2;
                io.EnsurePathToFile(pathAndFilename, cacheMs);
                var ticks4 = sw.ElapsedTicks - ticks1 - ticks2 - ticks3;
                io.EnsurePathToFile(pathAndFilename, cacheMs);
                var ticks5 = sw.ElapsedTicks - ticks1 - ticks2 - ticks3 - ticks4;

                Console.WriteLine("First check ticks: " + ticks1);
                Console.WriteLine("Cache check 1 ticks: " + ticks2);
                Console.WriteLine("Cache check 2 ticks: " + ticks3);
                Console.WriteLine("Cache check 3 ticks: " + ticks4);
                Console.WriteLine("Cache check 4 ticks: " + ticks5);

                //Assert
                Assert.IsTrue(Directory.Exists(path));
                Assert.IsTrue(ticks1 > ticks2);
                Assert.IsTrue(ticks2 < (ticks1 / 2), "using the cache should not take much time at all");

                //Now let's sleep a little and make sure that our cache gets cleared
                Console.WriteLine("Waiting {0} ms to clear cache", cacheMs);
                Thread.Sleep(cacheMs);
                sw.Restart();

                io.EnsurePathToFile(pathAndFilename, cacheMs);
                ticks1 = sw.ElapsedTicks;
                io.EnsurePathToFile(pathAndFilename, cacheMs);
                ticks2 = sw.ElapsedTicks - ticks1;
                io.EnsurePathToFile(pathAndFilename, cacheMs);
                ticks3 = sw.ElapsedTicks - ticks1 - ticks2;
                io.EnsurePathToFile(pathAndFilename, cacheMs);
                ticks4 = sw.ElapsedTicks - ticks1 - ticks2 - ticks3;
                io.EnsurePathToFile(pathAndFilename, cacheMs);
                ticks5 = sw.ElapsedTicks - ticks1 - ticks2 - ticks3 - ticks4;

                Console.WriteLine("First check ticks: " + ticks1);
                Console.WriteLine("Cache check 1 ticks: " + ticks2);
                Console.WriteLine("Cache check 2 ticks: " + ticks3);
                Console.WriteLine("Cache check 3 ticks: " + ticks4);
                Console.WriteLine("Cache check 4 ticks: " + ticks5);

                //Assert
                Assert.IsTrue(Directory.Exists(path));
                Assert.IsTrue(ticks1 > ticks2);
                Assert.IsTrue(ticks2 < (ticks1 / 2), "using the cache should not take much time at all");

            }

           
        }
    }
}
