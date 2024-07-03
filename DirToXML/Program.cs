using System.IO;
using System.Xml.Linq;

namespace DirToXML
{
    public class Program
    {
        static void Main(string[] args)
        {
            //input from the user
            string? dirUserLocation = null;
            //converted from user input to safer directory
            string? fullDirLocation = null;
            //used in the foreach, instead of a for loop
            bool grabNextLocation = false;
            //user in the foreach, instead of a for loop
            bool grabNextOutputName = false;
            string? dirUserOutputLocation = null;
            string? fullOutputLocation = null;
            //check the args passed in
            foreach (string arg in args)
            {
                //if the last run set the grabNext bool, set next text to the dirLocation
                if (grabNextLocation)
                {
                    dirUserLocation = arg;
                    grabNextLocation = false;
                }
                else if (grabNextOutputName)
                {
                    dirUserOutputLocation = arg;
                    grabNextOutputName = false;
                }
                if (arg == "-l" || arg == "--location")
                {
                    grabNextLocation = true;
                } else if (arg == "-o" || arg == "--output")
                {
                    grabNextOutputName = true;
                }
            }
            
            //prompt for user input 
            if (string.IsNullOrEmpty(dirUserLocation))
            {
                Console.WriteLine("Please set the location with the -l (or --location) flag");
                Console.WriteLine("If you want current Directory just pass . into -l");
                Console.WriteLine("You can also set the output file just by passing the location and name into -o (or --output)");
                //prompt user to try again upon running code again
                return;
            }

            fullDirLocation = GetFullDirFromUserInput(dirUserLocation);

            fullOutputLocation = GetFullDirFromUserInput(dirUserOutputLocation);

            ConvertFullDirectoryToXmlList(fullDirLocation, fullOutputLocation);
            
        }

        public static string? GetDirectoryPath(string? path, bool retJustName = false)
        {
            if (!string.IsNullOrEmpty(path))
            {
                int lastIndex = path.LastIndexOf(Path.DirectorySeparatorChar);

                // Check if path has a filename
                if (lastIndex != -1 && lastIndex + 1 < path.Length)
                {
                    if (retJustName)
                    {
                        return path.Substring(lastIndex + 1);
                    }
                    // Extract directory path (excluding filename)
                    return path.Substring(0, lastIndex + 1);
                }
            }
            // Path is likely a directory without a filename (or null), return as-is
            return path;
        }

        public static string? GetFullDirFromUserInput(string? input)
        {
            string? retVal = null;
            string cwd = Directory.GetCurrentDirectory();
            string? dirInput = GetDirectoryPath(input);
            bool fileNameNotSent = dirInput != input;
            if (!string.IsNullOrEmpty(dirInput))
            {
                //replace all backslashes with double backslashes
                dirInput.Replace("\\", "\\\\");

                //if exact directory
                if (Directory.Exists(dirInput))
                {
                    retVal = dirInput;
                }

                if (retVal == null && (dirInput == "." || dirInput == "\".\""))
                {
                    retVal = cwd;
                } else if (string.IsNullOrEmpty(retVal))
                {
                    //relative path code section
                    string strTmpRelPath = cwd + dirInput;

                    //check if relativePath
                    if (Directory.Exists(strTmpRelPath))
                    {
                        retVal = strTmpRelPath;
                    }
                }
            }

            //put filename back on retVal
            if (fileNameNotSent)
            {
                retVal += GetDirectoryPath(input, true);
            }

            return retVal;
        }

        public static void ConvertFullDirectoryToXmlList(string? path, string? outputFile)
        {
            if (string.IsNullOrEmpty(outputFile))
            {
                string cwd = Directory.GetCurrentDirectory();
                outputFile = cwd + "output.xml";
            }

            if (!string.IsNullOrEmpty(path))
            {
                string[] filePaths = Directory.GetFiles(path);
                
                XElement rootElement = new XElement("Files");

                foreach (string filePath in filePaths)
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    string fileName = fileInfo.Name;
                    XElement fileElement = new XElement("File", fileName);
                    rootElement.Add(fileElement);
                }

                try
                {
                    rootElement.Save(outputFile);
                    Console.WriteLine("Files exported to XML file: " + outputFile);
                } catch (Exception ex) 
                {
                    Console.WriteLine("Error saving XML file: " + ex.ToString());
                    //aint nobody got time for hidding errors.
                    throw;
                }
            }
        }
    }
}
