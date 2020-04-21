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
            string Intro1 = "<path> \t \t \t \t Absolute path of the input-file|output-file. eg: c:\\Images\\flower.jpg";
            string Intro2 = "<int> \t \t\t \t Dimension of the image width|height. [use range : 100-500]";
            string Intro3 = "<value> \t \t\t Value of the RGB color thresholding. [Optimal value: 100-240]";
            string Intro4 = "--input-image <path> \t \t Path containing the image file to be processed.";
            string Intro5 = "--output-image <path> \t \t Path containing the binary image file.";
            string Intro6 = "-width <int> \t\t \t Specification of the image width dimension.";
            string Intro7 = "-height <int> \t\t\t Specification of the image height dimension.";
            string Intro8 = "-red <value> \t\t \t Specification of the redThreshold value.";
            string Intro9 = "-green <value> \t\t\t Specification of the greenThreshold value.";
            string Intro10 = "-blue <value> \t\t \t Specification of the blueThreshold value.";

            Console.WriteLine("\nWelcome to Image Binarizer Application [Version 1.0.3]");
            Console.WriteLine("Copyright <c> 2020 daenet GmbH, Damir Dobric. All rights reserved.\n");
            Console.WriteLine("Do you want to binarize an image?");
            Console.WriteLine("\tY - Proceed");
            Console.WriteLine("\tN - Exit the application");
            Console.Write("Your option? ");
            
            args = new String[] { Console.ReadLine() };

            while (args[0].Equals("y") || args[0].Equals("Y"))
            {
                Console.WriteLine("Use the following command for help: -help ");
                Console.WriteLine(" ");
                string begin = Console.ReadLine();
                string[] args1 = begin.Split(' ');

                //Test if necessary input arguments were supplied.
                if (args1.Length == 1 && args1[0].Equals("-help"))
            {
                Console.WriteLine("\nruntime options:");
                Console.WriteLine($"\n{Intro1}\n{Intro2}\n{Intro3}\n{Intro4}\n{Intro5}\n{Intro6}\n{Intro7}\n{Intro8}\n{Intro9}\n{Intro10}\n");
                Console.WriteLine("\nPass the arguments as following:");
                Console.WriteLine("\nExample with automatic RGB:\n--input-image c:\\a.png --output-image c:\\Images\\out.txt -width 32 -height 32");
                Console.WriteLine("\nExample with explicit RGB:\n-input-image c:\\a.png --output-image c:\\Images\\out.txt -width 32 -height 32 -red 100 -green 100 -blue 100\n");
            }
            else
            {
                Console.WriteLine("\nError: All necessary arguments are not passed. Please pass the arguments first.");

                Console.WriteLine("\nPress any key to exit the application.");
                Console.ReadLine();
                return;
            }

            string inputImagePath = " ";
            string outputImagePath = " ";
            int imageWidth = 0;
            int imageHeight = 0;
            int redThreshold = -1;
            int greenThreshold = -1;
            int blueThreshold = -1;

            string readline = Console.ReadLine();
            string[] args3 = readline.Split(' ');

            if (args3.Length == 8)
            {
                if (args3[0].Equals("--input-image") && File.Exists(args3[1]))
                {
                    inputImagePath = args3[1];
                }
                else
                {
                    Console.WriteLine("\nError: Input file doesn't exist.");
                    Console.WriteLine("\nPress any key to exit the application.");
                    Console.ReadLine();
                    return;
                }

                int separatorIndex = args3[3].LastIndexOf(Path.DirectorySeparatorChar);

                if (args3[2].Equals("--output-image") && separatorIndex >= 0 && Directory.Exists(args3[3].Substring(0, separatorIndex)))
                {
                    outputImagePath = args3[3];
                }
                else
                {
                    Console.WriteLine("\nError: Output Directory doesn't exist.");
                    Console.WriteLine("\nPress any key to exit the application.");
                    Console.ReadLine();
                    return;
                }

                if (!args3[4].Equals("-width") || !int.TryParse(args3[5], out imageWidth))
                {
                    Console.WriteLine("\nError: Image Width should be integer.");
                    Console.WriteLine("\nPress any key to exit the application.");
                    Console.ReadLine();
                    return;
                }

                if (!args3[6].Equals("-height") || !int.TryParse(args3[7], out imageHeight))
                {
                    Console.WriteLine("\nError: Image Height should be integer.");
                    Console.WriteLine("\nPress any key to exit the application.");
                    Console.ReadLine();
                    return;
                }
            }

            else
            {
                if (args3.Length > 8)
                {
                    if (args3.Length < 14)
                    {
                        Console.WriteLine("\nError: All three Red, Green and Blue Thresholds should be passed.");
                        Console.WriteLine("\nPress any key to exit the application.");
                        Console.ReadLine();
                        return;
                    }
                    else
                    {
                        if (args3[0].Equals("--input-image") && File.Exists(args3[1]))
                        {
                            inputImagePath = args3[1];
                        }
                        else
                        {
                            Console.WriteLine("\nError: Input file doesn't exist.");
                            Console.WriteLine("\nPress any key to exit the application.");
                            Console.ReadLine();
                            return;
                        }

                        int separatorIndex = args3[3].LastIndexOf(Path.DirectorySeparatorChar);

                        if (args3[2].Equals("--output-image") && separatorIndex >= 0 && Directory.Exists(args3[3].Substring(0, separatorIndex)))
                        {
                            outputImagePath = args3[3];
                        }
                        else
                        {
                            Console.WriteLine("\nError: Output Directory doesn't exist.");
                            Console.WriteLine("\nPress any key to exit the application.");
                            Console.ReadLine();
                            return;
                        }

                        if (!args3[4].Equals("-width") || !int.TryParse(args3[5], out imageWidth))
                        {
                            Console.WriteLine("\nError: Image Width should be integer.");
                            Console.WriteLine("\nPress any key to exit the application.");
                            Console.ReadLine();
                            return;
                        }

                        if (!args3[6].Equals("-height") || !int.TryParse(args3[7], out imageHeight))
                        {
                            Console.WriteLine("\nError: Image Height should be integer.");
                            Console.WriteLine("\nPress any key to exit the application.");
                            Console.ReadLine();
                            return;
                        }
                        if (!args3[8].Equals("-red") || !(int.TryParse(args3[9], out redThreshold)) || redThreshold < 0 || redThreshold > 255)
                        {
                            Console.WriteLine("\nError: Red Threshold should be in between 0 and 255.");
                            Console.WriteLine("\nPress any key to exit the application.");
                            Console.ReadLine();
                            return;
                        }

                        if (!args3[10].Equals("-green") || !(int.TryParse(args3[11], out greenThreshold)) || greenThreshold < 0 || greenThreshold > 255)
                        {
                            Console.WriteLine("\nError: Green Threshold should be in between 0 and 255.");
                            Console.WriteLine("\nPress any key to exit the application.");
                            Console.ReadLine();
                            return;
                        }

                        if (!args3[12].Equals("-blue") || !(int.TryParse(args3[13], out blueThreshold)) || blueThreshold < 0 || blueThreshold > 255)
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


                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nImage Binarization completed.");
                Console.ResetColor();
                Console.WriteLine("Do you want to continue?");
                Console.WriteLine("\tY - Continue");
                Console.WriteLine("\tN - Exit the application");
                Console.Write("Your option? ");
                args = new String[] { Console.ReadLine() };
            }
            Console.WriteLine("\nPress any key to exit the application.");

            Console.ReadLine();

        }
    }
}