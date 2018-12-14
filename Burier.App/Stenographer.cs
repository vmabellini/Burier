using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Burier.App
{
    public class Stenographer
    {
        public const int USELESS_BITS = 48;
        public const int EOF_BITS = 16;

        private readonly Stream _contentStream;
        private readonly TextWriter _output;

        public Stenographer(Stream contentStream,
            TextWriter output)
        {
            _contentStream = contentStream;
            _output = output;
        }

        public int BitCapacity()
        {
            if (!Readable())
                return 0;

            using (var image = new Bitmap(Image.FromStream(_contentStream)))
            {
                var bitSize = (image.Width * image.Height) / 8;
                bitSize -= USELESS_BITS; //Remove the useless bits
                return bitSize;
            }
        }

        public bool Readable()
        {
            try
            {
                using (var image = new Bitmap(Image.FromStream(_contentStream)))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine(ex.Message);
                return false;
            }
        }

        public Bitmap HideData(byte[] content)
        {
            if (!Readable())
                throw new ApplicationException("Unable to read from this image.");

            var capacity = BitCapacity();
            if (content.Length > capacity)
                throw new ApplicationException("File is too big to hide on this image.");

            int size = content.Length;
            byte[] sizeToBytes = BitConverter.GetBytes(size);
            var package = new List<byte>();
            package.AddRange(sizeToBytes);
            package.AddRange(content);

            var bitsToWrite = new BitArray(package.ToArray());
            int bitIndex = 0;

            _output.WriteLine($"Hiding {content.Length * 8} bits...");

            var output = new Bitmap(Image.FromStream(_contentStream));

            int R = 0, G = 0, B = 0;
            
            _output.WriteLine($"Writing data...");

            //Loop again but write the data on the clean pixels
            for (int y = 0; y < output.Height; y++)
            {
                if (bitIndex >= bitsToWrite.Count)
                    break;
                for (int x = 0; x < output.Width; x++)
                {
                    if (bitIndex >= bitsToWrite.Count)
                        break;

                    Color pixel = output.GetPixel(x, y);

                    //Set the next 3 bits to write
                    var next3bits = new BitArray(new bool[] { false, false, false });
                    for (var index = 0; index < next3bits.Count; index++)
                    {
                        if (bitIndex >= bitsToWrite.Count)
                            continue;

                        next3bits[index] = bitsToWrite.Get(bitIndex);
                        bitIndex++;
                    }

                    //Remove the least significant bit value
                    R = pixel.R - pixel.R % 2;
                    G = pixel.G - pixel.G % 2;
                    B = pixel.B - pixel.B % 2;

                    //Apply the actual data
                    R = R + Convert.ToByte(next3bits[0]);
                    G = G + Convert.ToByte(next3bits[1]);
                    B = B + Convert.ToByte(next3bits[2]);

                    output.SetPixel(x, y, Color.FromArgb(R, G, B));
                }
            }

            return output;
        }

        public static byte[] ReadData(Bitmap bitmap)
        {
            var bitList = new List<bool>();
            var ended = false;
            
            for (int y = 0; y < bitmap.Height; y++)
            {
                if (ended) break;

                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (ended) break;

                    var pixel = bitmap.GetPixel(x, y);

                    var bit1 = Convert.ToBoolean(pixel.R % 2);
                    var bit2 = Convert.ToBoolean(pixel.G % 2);
                    var bit3 = Convert.ToBoolean(pixel.B % 2);

                    bitList.Add(bit1);
                    bitList.Add(bit2);
                    bitList.Add(bit3);
                }
            }

            var sizeByteArray = new BitArray(bitList.GetRange(0, 32).ToArray());
            var outputSizeBytes = new byte[4];
            sizeByteArray.CopyTo(outputSizeBytes, 0);
            var outputSize = BitConverter.ToInt32(outputSizeBytes);
            
            Console.WriteLine($"Output size: {outputSize * 8} bits");
            
            bitList = bitList.GetRange(32, outputSize * 8);

            var outputArray = new BitArray(bitList.ToArray());

            if (outputArray.Count % 8 != 0)
                throw new ApplicationException($"Invalid number of bits: {outputArray.Count}.");

            var bytes = new byte[outputArray.Count / 8];
            outputArray.CopyTo(bytes, 0);

            return bytes;
        }
    }
}
