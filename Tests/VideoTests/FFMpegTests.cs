using System.Collections.Generic;
using System.IO;
using FFMpegWrapper;
using NUnit.Framework;
using Video.Utils;

namespace VideoTests
{
    [TestFixture(Description = "Тесты самой обертки над FFMPeg.")]
    public class FFMpegTests
    {
        private const string OutputFolder = "FFMpegWrapperFolder//";

        [SetUp]
        public void Init()
        {
            new VideoUtils().EnableDebugMode();
            if (!Directory.Exists(OutputFolder))
            {
                Directory.CreateDirectory(OutputFolder);
            }
        }

        [Test]
        public void SimpleCut_Test()
        {
            const string OutputFile = OutputFolder + "Simple20SecCut_Medium.avi";
            var ffmpeg = new FFMpeg();
            ffmpeg.Cut(100, 20, @"M:\POLAND VS QATAR SEMI-FINAL 24th Men's Handball World Championship Qatar 2015.mp4", OutputFile);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [Test]
        public void Concat3Videos_Test()
        {
            const string OutputFile = OutputFolder + "3Episodes60SecConcat_SuperFast.avi";
            const string FileToConcat1 = OutputFolder + "1EpisodeToConcat_SuperFast.avi";
            const string FileToConcat2 = OutputFolder + "2EpisodeToConcat_SuperFast.avi";
            const string FileToConcat3 = OutputFolder + "3EpisodeToConcat_SuperFast.avi";
            var ffmpeg = new FFMpeg(PresetParameters.SuperFast);
            ffmpeg.Cut(100, 20, @"M:\POLAND VS QATAR SEMI-FINAL 24th Men's Handball World Championship Qatar 2015.mp4", FileToConcat1);
            ffmpeg.Cut(300, 20, @"M:\POLAND VS QATAR SEMI-FINAL 24th Men's Handball World Championship Qatar 2015.mp4", FileToConcat2);
            ffmpeg.Cut(600, 20, @"M:\POLAND VS QATAR SEMI-FINAL 24th Men's Handball World Championship Qatar 2015.mp4", FileToConcat3);

            ffmpeg.Concat(OutputFile, FileToConcat1, FileToConcat2, FileToConcat3);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [Test]
        public void DrawImageOnOneVideo()
        {
            const string OutputFile = OutputFolder + "Image20SecCut_Medium.avi";
            var ffmpeg = new FFMpeg();

            var images = new List<DrawImageTimeRecord>();
            images.Add(new DrawImageTimeRecord(File.ReadAllBytes(SampleFiles.SamplePngImage), 100, 100, 1, 4));

            ffmpeg.DrawImage(SampleFiles.SampleVideo_5sec, images, OutputFile);

            Assert.IsTrue(File.Exists(OutputFile));
        }

        [TearDown]
        public void CleanUp()
        {
            Directory.Delete(OutputFolder, true);
        }
    }
}