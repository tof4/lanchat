using System.IO;
using System.Net;
using Lanchat.ClientCore;
using Lanchat.Tests.Mock;
using NUnit.Framework;

namespace Lanchat.Tests.ClientCore
{
    [NonParallelizable]
    public class ConfigManagerTests
    {
        [SetUp]
        public void Setup()
        {
            FileOperations.Prepare();
        }

        [TearDown]
        public void TearDown()
        {
            FileOperations.CleanUp();
        }

        [Test]
        public void CreatingNewConfig()
        {
            var config = Storage.LoadConfig();
            Assert.IsTrue(config.Fresh);
        }

        [Test]
        public void ConfigLoading()
        {
            Storage.SaveConfig(new Config());
            var config = Storage.LoadConfig();
            Assert.IsFalse(config.Fresh);
        }

        [Test]
        public void ConfigSaving()
        {
            var config = Storage.LoadConfig();
            config.Nickname = "test";
            var loadedConfig = Storage.LoadConfig();
            Assert.AreEqual(config.Nickname, loadedConfig.Nickname);
        }

        [Test]
        public void LoadingInvalidJson()
        {
            File.WriteAllText(Paths.ConfigFile, "not a json");
            var config = Storage.LoadConfig();
            Assert.IsTrue(config.Fresh);
        }

        [Test]
        public void CreatingDirectory()
        {
            Directory.Delete(Paths.RootDirectory, true);
            Storage.CreateDirectories();
            Assert.IsTrue(Directory.Exists(Paths.RootDirectory));
        }
    }
}