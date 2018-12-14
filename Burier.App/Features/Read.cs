using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Burier.App.Features
{
    public class Read : ICommand
    {
        private readonly string _imagePath;
        private readonly string _outputPath;
        private readonly bool _ultraSecret;

        public Read(string imagePath,
            string outputPath,
            bool ultraSecret)
        {
            _imagePath = imagePath;
            _outputPath = outputPath;
            _ultraSecret = ultraSecret;
        }

        public void Execute()
        {
            byte[] outputBytes = null;
            using (var image = new Bitmap(Image.FromFile(_imagePath)))
            {
                outputBytes = Stenographer.ReadData(image);
            }

            if (!_ultraSecret)
            {
                if (string.IsNullOrEmpty(_outputPath))
                    throw new ApplicationException("Error: missing outputpath parameter");

                using (FileStream fileStream = new FileStream(_outputPath, FileMode.Create))
                {
                    fileStream.Write(outputBytes, 0, outputBytes.Length);
                }
                Console.WriteLine("Corpse unburied! Be careful!");
            }
            {
                var ultraSecretOutput = Encoding.UTF8.GetString(outputBytes);
                Console.WriteLine("---BEGIN---");
                Console.WriteLine(ultraSecretOutput);
                Console.WriteLine("----END----");
                Console.WriteLine();
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
