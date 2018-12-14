using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Burier.App.Features
{
    public class Write : ICommand
    {
        private readonly string _imagePath;
        private readonly string _outputPath;
        private readonly string _secretDataPath;
        private readonly bool _ultraSecret;

        public Write(string imagePath,
            string outputPath,
            string secretDataPath,
            bool ultraSecret)
        {
            _imagePath = imagePath;
            _outputPath = outputPath;
            _secretDataPath = secretDataPath;
            _ultraSecret = ultraSecret;
        }

        public void Execute()
        {
            if (string.IsNullOrEmpty(_outputPath))
                throw new ApplicationException("Error: missing outputpath parameter");

            byte[] secretDataBytes = null;
            if (!_ultraSecret)
            {
                if (string.IsNullOrEmpty(_secretDataPath))
                    throw new ApplicationException("Error: missing secretDataPath parameter");

                secretDataBytes = File.ReadAllBytes(_secretDataPath);
            }
            else
            {
                Console.WriteLine("Enter the text content to be hidden:");
                var ultraSecretContent = Console.ReadLine();

                secretDataBytes = Encoding.UTF8.GetBytes(ultraSecretContent);
            }

            Bitmap outputBitmap = null;
            using (var fileStream = File.Open(_imagePath, FileMode.Open))
            {
                Stenographer stenographer = new Stenographer(fileStream, Console.Out);
                outputBitmap = stenographer.HideData(secretDataBytes);
            }

            outputBitmap.Save(_outputPath);

            Console.WriteLine($"Corpse buried at {_outputPath}! Take care!");
        }
    }
}
