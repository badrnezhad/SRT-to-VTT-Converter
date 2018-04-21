using PC.VideoUtilites.Utilities.HelperClass.Enums;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace PC.VideoUtilities.Utilities.UtilitiesClass
{
    public class SubtitleUtility
    {
        public static void ConvertSrtToVtt(string srcFilePath, string destFilePath, uint _offsetMs = 0, SubtitleOffsetDirectionEnum _nOffsetDirection = SubtitleOffsetDirectionEnum.Forward)
        {
            using (var strReader = new StreamReader(srcFilePath))
            using (var strWriter = new StreamWriter(destFilePath.Replace(".srt", ".vtt")))
            {
                var rgxDialogNumber = new Regex(@"^\d+$");
                var rgxTimeFrame = new Regex(@"(\d\d:\d\d:\d\d,\d\d\d) --> (\d\d:\d\d:\d\d,\d\d\d)");

                strWriter.WriteLine("WEBVTT");
                strWriter.WriteLine("");

                string sLine;
                while ((sLine = strReader.ReadLine()) != null)
                {
                    if (rgxDialogNumber.IsMatch(sLine))
                        continue;

                    Match match = rgxTimeFrame.Match(sLine);
                    if (match.Success)
                    {
                        if (_offsetMs > 0)
                        {
                            var tsStartTime = TimeSpan.Parse(match.Groups[1].Value.Replace(',', '.'));
                            var tsEndTime = TimeSpan.Parse(match.Groups[2].Value.Replace(',', '.'));

                            long startTimeMs = ((int)_nOffsetDirection) * _offsetMs + (uint)tsStartTime.TotalMilliseconds;
                            long endTimeMs = ((int)_nOffsetDirection) * _offsetMs + (uint)tsEndTime.TotalMilliseconds;
                            tsStartTime = TimeSpan.FromMilliseconds(startTimeMs < 0 ? 0 : startTimeMs);
                            tsEndTime = TimeSpan.FromMilliseconds(endTimeMs < 0 ? 0 : endTimeMs);

                            sLine = tsStartTime.ToString(@"hh\:mm\:ss\.fff") +
                                    " --> " +
                                    tsEndTime.ToString(@"hh\:mm\:ss\.fff");
                        }
                        else
                        {
                            sLine = sLine.Replace(',', '.'); // Simply replace the comma in the time with a period
                        }
                    }

                    strWriter.WriteLine(sLine); // Write out the line
                }
            }
        }
    }
}
