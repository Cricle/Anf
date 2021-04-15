using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf.Easy.Test
{
    [TestClass]
    public class PathHelperTest
    {
        [TestMethod]
        public void GivenString_EnsureName_MustCanCreateFile()
        {
            var name = "dagiefgwi!@#%^";
            var act=PathHelper.EnsureName(name);
            var path = Path.Combine(Environment.CurrentDirectory,act);
            File.Create(path).Dispose();
            File.Delete(path);
        }
        [TestMethod]
        [DataRow('/')]
        [DataRow('\\')]
        public void GivenInvaldChar(char c)
        {
            var isInvalidChar = PathHelper.IsInvalidChar(c);
            Assert.IsTrue(isInvalidChar);
        }
        [TestMethod]
        public void GivenExistsFolder_EnsureCreate_NothingTodo()
        {
            var dirName = "pathhelper_testfolder";
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
            PathHelper.EnsureCreated(dirName);
            var exist = Directory.Exists(dirName);
            Assert.IsTrue(exist);
        }
        [TestMethod]
        public void GivenNotExistsFolder_EnsureCreate_NothingTodo()
        {
            var dirName = "pathhelper_testfolder_exists";
            if (Directory.Exists(dirName))
            {
                Directory.Delete(dirName);
            }
            PathHelper.EnsureCreated(dirName);
            var exist = Directory.Exists(dirName);
            Assert.IsTrue(exist);
        }
    }
}
