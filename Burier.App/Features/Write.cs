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
        private readonly string _secretKey;
        private readonly bool _ultraSecret;

        public Write(string imagePath,
            string outputPath,
            string secretDataPath,
            string secretKey,
            bool ultraSecret)
        {
            _imagePath = imagePath;
            _outputPath = outputPath;
            _secretDataPath = secretDataPath;
            _secretKey = secretKey;
            _ultraSecret = ultraSecret;
        }

        public void Execute()
        {
            if (string.IsNullOrEmpty(_outputPath))
                throw new ApplicationException("Error: missing outputpath parameter");

            string secretDataString = null;
            if (!_ultraSecret)
            {
                if (string.IsNullOrEmpty(_secretDataPath))
                    throw new ApplicationException("Error: missing secretDataPath parameter");

                var secretDataBytes = File.ReadAllBytes(_secretDataPath);
                secretDataString = Encoding.UTF8.GetString(secretDataBytes);
            }
            else
            {
                Console.WriteLine("Enter the text content to be hidden:");
                secretDataString = Console.ReadLine();
            }

            Bitmap outputBitmap = null;
            using (var fileStream = File.Open(_imagePath, FileMode.Open))
            {
                Stenographer stenographer = new Stenographer(fileStream, Console.Out);
                outputBitmap = stenographer.HideData(secretDataString, _secretKey);
            }

            outputBitmap.Save(_outputPath);

            Console.WriteLine($"Corpse buried at {_outputPath}! Take care!");
        }
    }
}
