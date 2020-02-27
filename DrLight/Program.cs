using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace DoctorLight
{
    internal class Program
    {
        /// <summary>
        /// Internal cache that consists of reference directions & tangents
        /// </summary>
        internal static Dictionary<Vector3d, Vector3d> SegmentCache;
        
        /// <summary>
        /// Internal lookup table consisting of the previous direction and it's replacement if any
        /// </summary>
        internal static Dictionary<Vector3d, Vector3d> ReplacementLookup;
        
        public static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("------------------ Doctor Light ------------------");
            Console.WriteLine("|                                                |");
            Console.WriteLine("|                                                |");
            Console.WriteLine("|                  version 1.0                   |");
            Console.WriteLine("|                                                |");
            Console.WriteLine("|                                                |");
            Console.WriteLine("|                                                |");
            Console.WriteLine("---------------- By Dustin Slane -----------------");
            Console.WriteLine("");
            Console.WriteLine("");
            
            string pwd = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location);
            
            // Create a reference directory if it doesn't exist
            if (!Directory.Exists(pwd + @"\reference"))
            {
                Directory.CreateDirectory(pwd + @"\reference");
            }

            // Get the .light file(s) that was dropped on this program
            if (args.Length == 0 || !args[0].EndsWith(".light"))
            {
                Console.WriteLine("Please drag your .light file on this exe");
                Console.ReadKey();
                return;
            }
            
            ReplacementLookup = new Dictionary<Vector3d, Vector3d>();
            
            // Create the cache json from all the reference light files
            if (!File.Exists(pwd + @"\cache.json"))
            {
                List<string> paths = Directory.GetFiles(pwd + @"\reference").Where(a => a.EndsWith(".light")).ToList();
                if (paths.Count == 0)
                {
                    Console.WriteLine("Please fill your reference folder up with reference .light files.");
                    Console.ReadKey();
                    return;
                }
                
                SegmentCache = ToPairs(GetSegments(paths));

                // Write to file
                var cache = SegmentCache.Select(a => new Pair {Distance = a.Key, Tangent = a.Value}).ToList();
                
                string serialized = JsonConvert.SerializeObject(cache);
                
                File.WriteAllText(pwd + @"\cache.json", serialized);
            }
            else
            {
                // Json file found, load the cache.
                SegmentCache = JsonConvert.DeserializeObject<List<Pair>>(File.ReadAllText(pwd + @"\cache.json"))
                    .ToDictionary(a => a.Distance, a => a.Tangent);
            }

            // Go through each submitted .light file
            foreach (string path in args)
            {
                Console.WriteLine("");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("");
                Console.WriteLine($"File {path}");
                int matched = 0;
                int guessed = 0;
                
                // Get all the Direction / Tangent segments
                var segments = ToPairs(SegmentsFromFile(path));
                var corrected = new Dictionary<Vector3d, Vector3d>(segments);

                foreach (var kvp in segments)
                {
                    // Try and match the direction directly where possible so that the tangent is 100% correct
                    if (SegmentCache.TryGetValue(kvp.Key, out var match))
                    {
                        // Only replace if it is actually different of course
                        if (match != kvp.Value)
                        {
                            corrected[kvp.Key] = match;
                            matched++;
                        }
                    }
                    else
                    {
                        // No match. Find the closest direction to our direction
                        var closest = SegmentCache
                            .OrderBy(a => Vector3d.Distance(kvp.Key, a.Key)).First();
                        
                        // If the tangent is different or the direction is different we need to replace it
                        if (closest.Value != kvp.Value || closest.Key != kvp.Key)
                        {
                            corrected.Remove(kvp.Key);
                            corrected[closest.Key] = closest.Value;
                            
                            // Place the old key & the new key in the lookup table
                            // This links together the old direction-tangent & the new direction-tangent
                            ReplacementLookup[kvp.Key] = closest.Key;
                            guessed++;
                        }
                    }
                }
                
                WriteToFile(path, segments, corrected);
                
                
                Console.WriteLine("");
                Console.WriteLine($"Doctor light found {segments.Count} segments");
                Console.WriteLine($"Doctor light corrected {matched+guessed} (matched {matched}, guessed {guessed})");
                Console.WriteLine("");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("");
            }
            Console.WriteLine("------------------     Done     ------------------");
            Console.ReadKey();
        }

        /// <summary>
        /// Write the changes to file
        /// </summary>
        /// <param name="path">File Path</param>
        /// <param name="oldValues">Old Direction-Tangent dictionary</param>
        /// <param name="processed">New Direction-Tangent dictionary</param>
        public static void WriteToFile(string path, Dictionary<Vector3d, Vector3d> oldValues, Dictionary<Vector3d, Vector3d> processed)
        {
            string fileText = File.ReadAllText(path);

            foreach (var old in oldValues)
            {
                RenderSearchReplace(old, processed, out string replace, out string search, 8);

                // If the pair is found, replace in text and continue
                if (fileText.IndexOf(search, StringComparison.Ordinal) != -1)
                {
                    fileText = fileText.Replace(search,replace);
                    continue;
                    
                }
                
                // Try with less precision
                RenderSearchReplace(old, processed, out string replace7, out string search7, 7);
                
                // If the pair is found, replace in text and continue
                if (fileText.IndexOf(search7, StringComparison.Ordinal) != -1)
                {
                    fileText = fileText.Replace(search7,replace7);
                    continue;
                    
                }
                
                // Try with less precision
                RenderSearchReplace(old, processed, out string replace6, out string search6, 6);
                
                // If the pair is found, replace in text and continue
                if (fileText.IndexOf(search6, StringComparison.Ordinal) != -1)
                {
                    fileText = fileText.Replace(search6,replace6);
                    continue;
                    
                }

                // Can't find it boss
                Console.WriteLine("Could not find old direction-tangent combination in file. Could not replace.");
                Console.WriteLine($"Direction {old.Key} Tangent {old.Value}");
            }

            File.WriteAllText(path, fileText);
        }

        public static void RenderSearchReplace(KeyValuePair<Vector3d, Vector3d> old, Dictionary<Vector3d, Vector3d> processed, 
            out string replace, out string search, int precision)
        {
            search = Render(old.Key, old.Value, precision);
                
            // If the direction wasn't changed, replace the value directly
            if (processed.TryGetValue(old.Key, out var newEntry))
            {
                replace = Render(old.Key, newEntry, precision);
            }
            else
            {
                // The direction was also changed, find the correct direction in the lookup table for this wrong entry
                replace = Render(ReplacementLookup[old.Key], processed[ReplacementLookup[old.Key]], precision );
            }
        }

        /// <summary>
        /// Rendering the text lines to be exactly the same as the source file. Renders a direction-tangent pair.
        /// </summary>
        /// <param name="direction">Direction to place in the light file</param>
        /// <param name="tangent">Tangent to place in the light file</param>
        /// <param name="precision">Precision is 8 unless specified. Supported are 7 & 6</param>
        /// <returns></returns>
        public static string Render(Vector3d direction, Vector3d tangent, int precision = 8)
        {
            // All values in the files should have 8 decimal spaces. This is why I needed double precision vectors.
            // Sometimes they don't. So here's the option to be less precise
            switch (precision)
            {
                case 7:
                    return $"\t\tDirection {direction.x:F7} {direction.y:F7} {direction.z:F7}\r\n" +
                           $"\t\tTangent {tangent.x:F7} {tangent.y:F7} {tangent.z:F7}\r\n";
                
                case 6:
                    return $"\t\tDirection {direction.x:F6} {direction.y:F6} {direction.z:F6}\r\n" +
                           $"\t\tTangent {tangent.x:F6} {tangent.y:F6} {tangent.z:F6}\r\n";
                
                default:
                    return $"\t\tDirection {direction.x:F8} {direction.y:F8} {direction.z:F8}\r\n" +
                           $"\t\tTangent {tangent.x:F8} {tangent.y:F8} {tangent.z:F8}\r\n";
            }
            
        }

        /// <summary>
        /// Get segments Direction and Tangents from all files
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static List<string> GetSegments(List<string> paths)
        {
            List<string> contents = new List<string>();
            foreach (string path in paths)
            {
                contents.AddRange(SegmentsFromFile(path));
            }

            return contents;
        }

        /// <summary>
        /// Get all segments from a file (i.e. all the key-values within an attribute)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> SegmentsFromFile(string path)
        {
            List<string> contents = new List<string>();
            try
            {
                string text = File.ReadAllText(path);
                text = text.Replace("\t", "").Replace("\r\n", ":");
                contents.AddRange(text.EverythingBetween("Attribute:{:", ":}:"));

            }
            catch (Exception e)
            {
                Console.WriteLine($"{path} wasnt loadable");
            }
            return contents;
        }

        /// <summary>
        /// Create Direction-Tangent pairs from segments
        /// </summary>
        /// <param name="segments">Segments loaded from a file</param>
        /// <returns></returns>
        public static Dictionary<Vector3d, Vector3d> ToPairs(List<string> segments)
        {
            var dict = new Dictionary<Vector3d, Vector3d>();
            foreach (var segment in segments)
            {
                try
                {
                    // Extract x,y,z from segments for direction and tangent
                    string[] direction = segment.EverythingBetween(":Direction", ":")[0].Trim().Split(' ');
                    string[] tangent =  segment.EverythingBetween(":Tangent", ":")[0].Trim().Split(' ');

                    // Convert text x,y,z to a double-precision vector3
                    Vector3d dir = new Vector3d(Convert.ToDouble(direction[0]), Convert.ToDouble(direction[1]),Convert.ToDouble(direction[2]));
                    Vector3d tan = new Vector3d(Convert.ToDouble(tangent[0]), Convert.ToDouble(tangent[1]),Convert.ToDouble(tangent[2]));

                    dict[dir] = tan;
                    
                    
                }
                catch (Exception e)
                {
                    //
                }
            }

            return dict;
        }
    }

    public class Pair
    {
        public Vector3d Distance { get; set; }
        public Vector3d Tangent { get; set; }
    }

}