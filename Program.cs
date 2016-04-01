using ArcGisTileTest.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace ArcGisTileTest
{
    class Program
    {

        static ParallelOptions _parallelOptions = new ParallelOptions();

        /// <summary>
        ///
        /// </summary>
        /// <param name="args">
        /// <para>Options file path</para>
        /// </param>
        /// <returns></returns>
        [STAThread]
        static int Main(string[] args)
        {
            DateTime startTime = DateTime.Now;

            // If no opptions file was specified, display help.
            if (args.Length < 1)
            {
                Console.Error.WriteLine(Resources.HelpFormat, Path.GetFileName(Application.ExecutablePath));
                return (int)ReturnCode.OptionsFileNotProvided;
            }

            // Write the start time to the console.
            Trace.TraceInformation("Start Time\t{0}", startTime);


            // Read the options file and convert to an Options object.
            var serializer = new JavaScriptSerializer();
            Options options = null;
            try
            {
                options = serializer.Deserialize<Options>(File.ReadAllText(args[0]));
            }
            catch (Exception ex)
            {
                Trace.TraceError("There was a problem reading the options file.\n{0}", ex);
                return (int)ReturnCode.InvalidOptionsFile;
            }

            _parallelOptions.MaxDegreeOfParallelism = options.MaxDegreeOfParallelism ?? -1;

            // Create the specified output directory if it does not already exist.
            if (!Directory.Exists(options.OutputDirectory))
            {
                Directory.CreateDirectory(options.OutputDirectory);
            }


            // Query the map service to get information about it.
            var uriBuilder = new UriBuilder(options.Url);
            uriBuilder.Query = "f=json";
            var req = HttpWebRequest.Create(uriBuilder.Uri);

            string json;
            using (var response = req.GetResponse())
            {
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);
                json = reader.ReadToEnd();
            }

            // Convert the JSON into a MapService object.
            var layerInfo = serializer.Deserialize<MapService>(json);
            var tileInfo = layerInfo.tileInfo;

            // Exit if the map service information does not contain tiling information.
            if (tileInfo == null)
            {
                Trace.TraceError("Map Service does not contain tiling information");
                return (int)ReturnCode.MapServiceIsNotHaveTileInfo;
            }


            ////// Run tests for each level of detail in parallel.
            ////Parallel.ForEach(tileInfo.lods, parallelOptions, lod => TestLod(options, layerInfo, lod));

            foreach (var lod in tileInfo.lods)
            {
                TestLod(options, layerInfo, lod);
            }

            // Get the time that the tests were completed, and write the elapsed time to the console.
            DateTime endTime = DateTime.Now;

            Trace.TraceInformation("End Time:\t{0}", endTime);
            Trace.TraceInformation("Elapsed Time:\t{0}", endTime - startTime);

            Trace.TraceInformation("Merging text table files...");
            Regex tableFileRe = new Regex(@"\d+_\d+\.csv$");
            var csvFiles = Directory.GetFiles(options.OutputDirectory, "*.csv", SearchOption.TopDirectoryOnly).Where(fn => tableFileRe.IsMatch(fn));

            // Merge the files together.
            if (csvFiles.Count() > 0)
            {
                using (var streamWriter = new StreamWriter(Path.Combine(options.OutputDirectory, "results.csv")))
                {
                    for (int i = 0, length = csvFiles.Count(); i < length; i++)
                    {
                        using (var streamReader = new StreamReader(csvFiles.ElementAt(i)))
                        {
                            int rowCount = 0;
                            string rowText;
                            while (!streamReader.EndOfStream)
                            {
                                rowText = streamReader.ReadLine();
                                if (i == 0 || rowCount > 0)
                                {
                                    streamWriter.WriteLine(rowText);
                                }
                                rowCount++;
                            }
                        }
                    }

                }
            }

            // Delete the pre-merged files.
            for (int i = 0, length = csvFiles.Count(); i < length; i++)
            {
                File.Delete(csvFiles.ElementAt(i));
            }

            return (int)ReturnCode.NoError;
        }

        /// <summary>
        /// Tests the tiles in a level of detail and writes the results to a text table file.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="layerInfo"></param>
        /// <param name="lod"></param>
        private static void TestLod(Options options, MapService layerInfo, LevelOfDetail lod)
        {
            if ((!options.StartLevel.HasValue || lod.level >= options.StartLevel) && (!options.EndLevel.HasValue || lod.level <= options.EndLevel))
            {
                Debug.WriteLine("Begin testing LOD#{0}...", lod.level);
                var level = lod.level;
                var firstTile = layerInfo.GetFirstTile(level);
                var lastTile = layerInfo.GetLastTile(level);
                ParallelLoopResult plResult = Parallel.For(firstTile.Row, lastTile.Row, _parallelOptions, r => TestRow(options, layerInfo, lod, level, firstTile, lastTile, r));
                while (!plResult.IsCompleted)
                {
                    // DO nothing.
                }
                Debug.WriteLine("End testing LOD#{0}...", lod.level);
            }
        }

        private static void TestRow(Options options, MapService layerInfo, LevelOfDetail lod, int level, Tile firstTile, Tile lastTile, int row)
        {
            // Exit the method if the row is out of the range of user-specified rows.
            if (
                (options.StartLevel.HasValue && options.StartLevel == level && options.StartRow.HasValue && row < options.StartRow)
                ||
                (options.EndLevel.HasValue && options.EndLevel == level && options.EndRow.HasValue && row > options.EndRow)
                )
            {
                return;
            }
            WebRequest webRequest;
            using (var sw = new StreamWriter(Path.Combine(options.OutputDirectory, string.Format("{0:00}_{1:000000}.csv", level, row)), false, System.Text.Encoding.ASCII))
            {
                sw.WriteLine("\"LOD\",\"Row\",\"Col\",\"xmin\",\"ymin\",\"xmax\",\"ymax\",\"ContentLength\",\"Error\"");
                for (int c = firstTile.Column; c < lastTile.Column; c++)
                {
                    var tileUrl = options.Url.GetTileUrl(level, row, c);

                    webRequest = HttpWebRequest.Create(tileUrl);
                    WebResponse response = null;
                    var envelope = layerInfo.GetTileExtent(level, row, c);
                    Exception theException = null;
                    int? contentLength = default(int?);
                    try
                    {
                        webRequest.Method = "HEAD";
                        response = webRequest.GetResponse();
                        string clStr = response.Headers[HttpResponseHeader.ContentLength];
                        int tempCl;
                        if (!string.IsNullOrWhiteSpace(clStr) && int.TryParse(clStr, out tempCl))
                        {
                            contentLength = tempCl;
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("An exception occured at LOD {0}, row {1}, column {2}{3}{4}", lod.level, row, c, Environment.NewLine, ex);
                        theException = ex;
                    }
                    finally
                    {
                        if (response != null)
                        {
                            response.Close();
                        }
                    }
                    if (theException != null ||
                        (options.WriteErrorsOnly.HasValue && !options.WriteErrorsOnly.Value) ||
                        (options.MinimumValidContentLength.HasValue && (!contentLength.HasValue || contentLength.Value < options.MinimumValidContentLength.Value))
                        )
                    {
                        sw.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                            level, row, c, envelope.xmin, envelope.ymin, envelope.xmax, envelope.ymax,
                            contentLength.HasValue ? contentLength.Value.ToString() : string.Empty,
                            theException != null ? string.Format("\"{0}\"", theException.Message) : string.Empty);
                        sw.Flush();
                    }
                }
            }
        }
    }
}
