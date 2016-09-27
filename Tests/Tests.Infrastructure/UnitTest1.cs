using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SXCore.Infrastructure.Services.FileStorage;
using SXCore.Infrastructure.Values;
using SXCore.Infrastructure.Services;
using System.IO;

namespace Tests.Infrastructure
{
    [TestClass]
    public class UnitTest1
    {
        AppSettingsProvider _settings;
        FTPServerConfig _ftpConfig;

        [TestInitialize]
        public void Init()
        {
            _settings = new AppSettingsProvider();
            _ftpConfig = new FTPServerConfig(_settings.GetSettings("ftpconfig"));
        }

        [TestMethod]
        public void test_ftp()
        {
            var filename = "index.html";
            var ftp = new FTPFileStorageService(_ftpConfig, "new.ivan.litskevich.ru/www/");

            var listing = ftp.ListDirectory("", true);

            if (!ftp.Exist(filename))
                throw new Exception("File not exist!");

            var size = ftp.SizeOfFile(filename);

            var index = ftp.ReadFile(filename);
            File.WriteAllBytes("index.html", index);

            var destname = "test/dir1/dir11/dir111/" + filename;
            if (ftp.Exist(destname))
                ftp.DeleteFile(destname);

            ftp.CopyFile(filename, destname, false);

            ftp.AppendFile(destname, index);

            filename = Guid.NewGuid().ToString();
            Assert.AreEqual(false, ftp.Exist(filename));
        }
    }
}
