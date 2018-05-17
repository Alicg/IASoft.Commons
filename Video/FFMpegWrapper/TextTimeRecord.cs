using System.Collections.Generic;
using System.Linq;
using Utils.Extensions;

namespace FFMpegWrapper
{
    public class TextTimeRecord
    {
        public TextTimeRecord(string text, double startSecond, double endSecond)
        {
            const int CharsInLine = 40;
            var ffMpegPreparedText = text.Replace("\\\\", "\\\\\\\\").Replace("'", "'\\\\\\''").Replace("%", "\\\\\\%")
                .Replace(":", "\\\\\\:");
            this.Lines = GetTranspositionedText(ffMpegPreparedText, CharsInLine).ToList();
            this.StartSecond = startSecond;
            this.EndSecond = endSecond;
        }
        public TextTimeRecord(List<string> lines, double startSecond, double endSecond)
        {
            this.Lines = lines;
            this.StartSecond = startSecond;
            this.EndSecond = endSecond;
        }

        public List<string> Lines { get; }

        public double StartSecond { get; set; }

        public double EndSecond { get; set; }

        private static string[] GetTranspositionedText(string text, int charsInLine)
        {
            if (text.Length <= charsInLine)
                return new[] { text };
            var lastSpacePosition = 0;
            while (true)
            {
                var spacePosition = text.IndexOf(' ', lastSpacePosition + 1, charsInLine - lastSpacePosition);
                if (spacePosition == -1)
                    break;
                lastSpacePosition = spacePosition;
            }
            var oneLineText = text.Substring(0, lastSpacePosition);
            var remainingLines = GetTranspositionedText(text.Substring(lastSpacePosition + 1, text.Length - lastSpacePosition - 1), charsInLine);
            return new[] { oneLineText }.Union(remainingLines).ToArray();
        }
    }
}