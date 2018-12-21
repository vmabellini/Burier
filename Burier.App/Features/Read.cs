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
        private readonly string _secretKey;
        private readonly bool _ultraSecretMode;

        public Read(string imagePath,
            string outputPath,
            string secretKey,
            bool ultraSecretMode)
        {
            _imagePath = imagePath;
            _outputPath = outputPath;
            _secretKey = secretKey;
            _ultraSecretMode = ultraSecretMode;
        }

        public void Execute()
        {
            string outputString = null;
            using (var image = new Bitmap(Image.FromFile(_imagePath)))
            {
                outputString = Stenographer.ReadData(image, _secretKey);
            }

            if (!_ultraSecretMode)
            {
                if (string.IsNullOrEmpty(_outputPath))
                    throw new ApplicationException("Error: missing outputpath parameter");

                File.WriteAllText(_outputPath, outputString);
                Console.WriteLine("Corpse unburied! Be careful!");
            }
            else
            {
                Console.WriteLine("---BEGIN---");
                Console.WriteLine(outputString);
                Console.WriteLine("----END----");
                Console.WriteLine();
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
