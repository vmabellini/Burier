using Mono.Options;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Burier.App
{
    class Program
    {
        static OptionSet _optionSet = null;

        static void Main(string[] args)
        {
            string imagePath = null;
            string outputPath = null;
            string secretDataPath = null;
            Mode mode = Mode.Read;
            Stenographer stenographer = null;
            _optionSet = new OptionSet()
            {
                { "read", "Read the buried data from a file", x => mode = Mode.Read },
                { "write", "Bury secret data to a file", x => mode = Mode.Write },
                { "info", "Provides information about the image file", x => mode = Mode.Info },
                { "imagepath=", "File path of the image to read", x => imagePath = x },
                { "datapath=", "Path of the secret data to bury", x => secretDataPath = x },
                { "outputpath=", "File path of the output to write", x => outputPath = x },
                { "help", "Show this help", x => _optionSet.WriteOptionDescriptions(Console.Error) }
            };
            _optionSet.Parse(args);

            stenographer = new Stenographer(imagePath, Console.Out);
            if (!stenographer.Readable())
            {
                Console.WriteLine("Error: image format not readable");
                return;
            }

            byte[] outputBytes = null;
            byte[] secretDataBytes = null;
            switch (mode)
            {
                case Mode.Read:
                    if (string.IsNullOrEmpty(outputPath))
                        throw new ApplicationException("Error: missing outputpath parameter");

                    using (var image = new Bitmap(Image.FromFile(imagePath)))
                    {
                        outputBytes = Stenographer.ReadData(image);
                    }
                    using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
                    {
                        fileStream.Write(outputBytes, 0, outputBytes.Length);
                    }
                    Console.WriteLine("Corpse unburied! Be careful!");

                    break;
                case Mode.Write:
                    if (string.IsNullOrEmpty(outputPath))
                        throw new ApplicationException("Error: missing outputpath parameter");
                    if (string.IsNullOrEmpty(secretDataPath))
                        throw new ApplicationException("Error: missing secretDataPath parameter");

                    secretDataBytes = File.ReadAllBytes(secretDataPath);
                    var outputBitmap = stenographer.HideData(secretDataBytes);

                    outputBitmap.Save(outputPath);
                   
                    Console.WriteLine($"Corpse buried at {outputPath}! Take care!");

                    break;
                case Mode.Info:
                    //TODO
                    break;
            }
        }

        public enum Mode
        {
            Read, Write, Info
        }
    }
}
