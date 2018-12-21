using Mono.Options;
using System;
using System.IO;

namespace Burier.App.Features
{
    public class Controller
    {
        OptionSet _optionSet = null;

        string _imagePath = null;
        string _outputPath = null;
        string _secretDataPath = null;
        string _secretKey = null;
        Mode _mode = Mode.Read;
        bool _ultraSecret = false;
        bool _help = false;
        Stenographer stenographer = null;

        public Controller(string[] args)
        {
            _optionSet = new OptionSet()
            {
                { "read", "Read the buried data from a file", x => _mode = Mode.Read },
                { "write", "Bury secret data to a file", x => _mode = Mode.Write },
                { "info", "Provides information about the image file", x => _mode = Mode.Info },
                { "secretkey=", "Secret key to encrypt/decrypt content", x => _secretKey = x },
                { "ultra-secret", "Ultra-secret mode! No IO! Hide and restore UTF-8 texts!", x => _ultraSecret = true },
                { "imagepath=", "File path of the image to read", x => _imagePath = x },
                { "datapath=", "Path of the secret data to bury", x => _secretDataPath = x },
                { "outputpath=", "File path of the output to write", x => _outputPath = x },
                { "help", "Show this help", x => _help = true }
            };
            _optionSet.Parse(args);
        }

        public void Execute()
        {
            if (_help)
            {
                _optionSet.WriteOptionDescriptions(Console.Out);
                return;
            }

            using (var fileStream = File.Open(_imagePath, FileMode.Open))
            {
                stenographer = new Stenographer(fileStream, Console.Out);
                if (!stenographer.Readable())
                {
                    Console.WriteLine("Error: image format not readable");
                    return;
                }
            }

            ICommand command = null;
            switch (_mode)
            {
                case Mode.Read:
                    command = new Read(_imagePath, _outputPath, _secretKey, _ultraSecret);
                    break;
                case Mode.Write:
                    command = new Write(_imagePath, _outputPath, _secretDataPath, _secretKey, _ultraSecret);
                    break;
                case Mode.Info:
                    break;
            }
            command.Execute();

        }

        public enum Mode
        {
            Read, Write, Info
        }
    }
}
