using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using JsonFlatten_Dict_StringString;

namespace OGA.AppSettings.Writeable.JSONConfig
{
    public class cWriteable_JSONConfigProvider : FileConfigurationProvider
    {
        public bool WriteOnChange { get; set; }

        public cWriteable_JSONConfigProvider(cWriteable_JSONConfigSource source) : base(source)
        {
            WriteOnChange = true;
        }

        /// <summary>
        /// Loads the JSON data from a stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        public override void Load(Stream stream)
        {
            try
            {
                Data = OGA.AppSettings.Writeable.JSONConfig.JsonConfigurationFileParser.Parse(stream);
            }
            catch (JsonReaderException e)
            {
                string errorLine = string.Empty;
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    IEnumerable<string> fileContent;
                    using (var streamReader = new StreamReader(stream))
                    {
                        fileContent = ReadLines(streamReader);
                        errorLine = RetrieveErrorContext(e, fileContent);
                    }
                }

                throw new FormatException(Resources.FormatError_JSONParseError(e.LineNumber, errorLine), e);
            }
        }

        public bool Has_Section(string sectionname)
        {
            foreach(var s in this.Data)
            {
                if (s.Key.ToLower().StartsWith(sectionname.ToLower()))
                    return true;
            }

            return false;
        }

        public int Save()
        {
            // Get the file data...
            var file = Source.FileProvider?.GetFileInfo(Source.Path);
            if (file == null || !file.Exists)
            {
                // The file does not exist.
                // We are to save it for the first time.

                // See if a filepath was defined.
                if(Source.Path == "")
                {
                    // No path was defined.
                    var error = new StringBuilder($"The configuration file '{Source.Path}' was not defined and is not optional.");
                    if (!string.IsNullOrEmpty(file?.PhysicalPath))
                    {
                        error.Append($" The physical path is '{file.PhysicalPath}'.");
                    }
                    throw new FileNotFoundException(error.ToString());
                }
                // If here, the file path was set, but the file doesn't exist.
                // We will attempt to create the file for the first time, saving our data to it.
            }
            // If here, we are to save our config to the file.

            // Attempt to save config to the file...
            try
            {
                // Unflatten the dictionary to a JObject...
                var jo = this.Data.Unflatten();

                // Convert the Jobject to a string...
                string jj = jo.ToString();

                // Save the json string to the file...
                System.IO.File.WriteAllText(Source.Path, jj);

                return 1;
            }
            catch (Exception e)
            {
                return -2;
            }
            finally
            {
            }
        }

        private static string RetrieveErrorContext(JsonReaderException e, IEnumerable<string> fileContent)
        {
            string errorLine = null;
            if (e.LineNumber >= 2)
            {
                var errorContext = fileContent.Skip(e.LineNumber - 2).Take(2).ToList();
                // Handle situations when the line number reported is out of bounds
                if (errorContext.Count() >= 2)
                {
                    errorLine = errorContext[0].Trim() + Environment.NewLine + errorContext[1].Trim();
                }
            }
            if (string.IsNullOrEmpty(errorLine))
            {
                var possibleLineContent = fileContent.Skip(e.LineNumber - 1).FirstOrDefault();
                errorLine = possibleLineContent ?? string.Empty;
            }
            return errorLine;
        }

        private static IEnumerable<string> ReadLines(StreamReader streamReader)
        {
            string line;
            do
            {
                line = streamReader.ReadLine();
                yield return line;
            } while (line != null);
        }

        public override void Set(string key, string value)
        {
            bool ischanged = false;

            // See if the value is currently stored...
            string tempval = "";
            if(this.Data.TryGetValue(key, out tempval))
            {
                // The value already exists in config.
                // We will update if different.

                // See if different.
                if(tempval != value)
                {
                    // value is different.
                    base.Set(key, value);
                    ischanged = true;
                }
            }
            else
            {
                // Value is not yet in config.
                // Add a new value...
                base.Set(key, value);
                ischanged = true;
            }

            // See if we have changes to save...
            if (!ischanged)
                return;

            //// Since a configuration dictionary is a flattened JObject, when writing data back to the dictionary, we will be updating.
            ////  the key of an entire object.
            //// For this, we need to remove the flattened entries associated with this object, as they will collide with the object itself when things get saved.
            //// Remove all dictionary entries that start with the given key...
            //List<string> keystoremove = new List<string>();
            //foreach(var s in this.Data)
            //{
            //    if(s.Key.StartsWith(key))
            //        keystoremove.Add(s.Key);
            //}
            //foreach (var s in keystoremove)
            //{
            //    this.Data.Remove(s);
            //}
            //// At this point, the dictionary entries associated with the incoming section are removed.
            //// We can now safely add the updated value for the section.
            
            //// Add the section back to the dictionary...
            //base.Set(key, value);

            // See if we need to save updates to disk...
            if(WriteOnChange)
            {
                // We are to save the changes to disk.
                if (Save() != 1)
                {
                    // Failed to save config update to disk.
                    throw new System.IO.IOException("Failed to save config to file.");
                }
            }
        }

        //private static IDictionary<string, string> CreateAndSaveDefaultValues(
        //    EFConfigurationContext dbContext)
        //{
        //    // Quotes (c)2005 Universal Pictures: Serenity
        //    // https://www.uphe.com/movies/serenity
        //    var configValues =
        //        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        //        {
        //        { "quote1", "I aim to misbehave." },
        //        { "quote2", "I swallowed a bug." },
        //        { "quote3", "You can't stop the signal, Mal." }
        //        };

        //    dbContext.Values.AddRange(configValues
        //        .Select(kvp => new EFConfigurationValue
        //        {
        //            Id = kvp.Key,
        //            Value = kvp.Value
        //        })
        //        .ToArray());

        //    dbContext.SaveChanges();

        //    return configValues;
        //}
    }

}
