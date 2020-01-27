using System;
using System.IO;

namespace ImageBinarizerApp
{
    /// <summary>
    /// Program Class of Console App
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry point for Program
        /// </summary>
        /// <param name="args">Argument of main method</param>
        static void Main(string[] args)
        {
            Console.WriteLine("\nWelcome to Image Binarizer Application [Version 1.0.2]");
            Console.WriteLine("Copyright <c> 2019 daenet GmbH, Damir Dobric. All rights reserved.");            
            Console.WriteLine("\nUse following command for help:");
            Console.WriteLine("dotnet ImageBinarizerApp -help");
            //args = new String[] { Console.ReadLine() };
            
            //Test if necessary input arguments were supplied.
            if (args.Length < 8)
            {
                if(args.Length == 1 && args[0].Equals("-help"))
                {
                    Console.WriteLine("\nHelp:");
                    Console.WriteLine("\nPass the arguments as following:");
                    Console.WriteLine("\nExample with automatic RGB:\ndotnet ImageBinarizerApp --input-image c:\\a.png --output-image d:\\out.txt -width 32 -height 32");
                    Console.WriteLine("\nExample with explicit RGB:\ndotnet ImageBinarizerApp --input-image c:\\a.png --output-image d:\\out.txt -width 32 -height 32 -red 100 -green 100 -blue 100");
                }
                else
                {
                    Console.WriteLine("\nError: All necessary arguments are not passed. Please pass the arguments first.");
                }
                Console.WriteLine("\nPress any key to exit the application.");
                Console.ReadLine();
                return;
            }
            else
            {
                String inputImagePath = "";
                String outputImagePath = "";
                int imageWidth = 0;
                int imageHeight = 0;
                int redThreshold = -1;
                int greenThreshold = -1;
                int blueThreshold = -1;

                if(args[0].Equals("--input-image") && File.Exists(args[1]))
                {
                    inputImagePath = args[1];
                }
                else
                {
                    Console.WriteLine("\nError: Input file doesn't exist.");
                    Console.WriteLine("\nPress any key to exit the application.");
                    Console.ReadLine();
                    return;
                }

                int separatorIndex = args[3].LastIndexOf(Path.DirectorySeparatorChar);
                if (args[2].Equals("--output-image") && separatorIndex >= 0 && Directory.Exists(args[3].Substring(0, separatorIndex)))
                {
                    outputImagePath = args[3];
                }
                else
                {
                    Console.WriteLine("\nError: Output Directory doesn't exist.");
                    Console.WriteLine("\nPress any key to exit the application.");
                    Console.ReadLine();
                    return;
                }

                if (!args[4].Equals("-width") || !int.TryParse(args[5], out imageWidth))
                {
                    Console.WriteLine("\nError: Image Width should be integer.");
                    Console.WriteLine("\nPress any key to exit the application.");
                    Console.ReadLine();
                    return;
                }

                if (!args[6].Equals("-height") || !int.TryParse(args[7], out imageHeight))
                {
                    Console.WriteLine("\nError: Image Height should be integer.");
                    Console.WriteLine("\nPress any key to exit the application.");
                    Console.ReadLine();
                    return;
                }

                if(args.Length > 8)
                {
                    if(args.Length < 14)
                    {
                        Console.WriteLine("\nError: All three Red, Green and Blue Thresholds should be passed.");
                        Console.WriteLine("\nPress any key to exit the application.");
                        Console.ReadLine();
                        return;
                    }
                    else
                    {
                        if (!args[8].Equals("-red") || !(int.TryParse(args[9], out redThreshold)) || redThreshold < 0 || redThreshold > 255)
                        {
                            Console.WriteLine("\nError: Red Threshold should be in between 0 and 255.");
                            Console.WriteLine("\nPress any key to exit the application.");
                            Console.ReadLine();
                            return;
                        }

                        if (!args[10].Equals("-green") || !(int.TryParse(args[11], out greenThreshold)) || greenThreshold < 0 || greenThreshold > 255)
                        {
                            Console.WriteLine("\nError: Green Threshold should be in between 0 and 255.");
                            Console.WriteLine("\nPress any key to exit the application.");
                            Console.ReadLine();
                            return;
                        }

                        if (!args[12].Equals("-blue") || !(int.TryParse(args[13], out blueThreshold)) || blueThreshold < 0 || blueThreshold > 255)
                        {
                            Console.WriteLine("\nError: Blue Threshold should be in between 0 and 255.");
                            Console.WriteLine("\nPress any key to exit the application.");
                            Console.ReadLine();
                            return;
                        }
                    }                    
                }
                else
                {
                    redThreshold = -1;
                    greenThreshold = -1;
                    blueThreshold = -1;
                }

                Console.WriteLine("\nImage Binarization in progress...");

                try
                {
                    ImageBinarizerApplication obj = new ImageBinarizerApplication();
                    obj.Binarizer(inputImagePath, outputImagePath, imageWidth, imageHeight, redThreshold, greenThreshold, blueThreshold);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\nError: {e.Message}");
                    Console.WriteLine("\nPress any key to exit the application.");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("\nImage Binarization completed.");
                Console.WriteLine("\nPress any key to exit the application.");

                Console.ReadLine();
            }            
        }
    }
}
