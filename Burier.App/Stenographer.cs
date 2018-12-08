using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Burier.App
{
    public class Stenographer
    {
        private readonly string _filePath;

        public Stenographer(string filePath)
        {
            _filePath = filePath;
        }

        public int BitCapacity()
        {
            if (!Readable())
                return 0;

            using (var image = new Bitmap(Image.FromFile(_filePath)))
            {
                return (image.Width * image.Height) / 8;
            }
        }

        public bool Readable()
        {
            try
            {
                using (var image = new Bitmap(Image.FromFile(_filePath)))
                {
                    return true;
                }
            }
            catch
            {
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


            var bitsToWrite = new BitArray(content);
            int bitIndex = 0;

            var output = new Bitmap(Image.FromFile(_filePath));

            int R = 0, G = 0, B = 0;

            //Loop all pixels
            for (int y = 0; y < output.Height; y++)
            {
                for (int x = 0; x < output.Width; x++)
                {
                    Color pixel = output.GetPixel(x, y);

                    //Remove the least significant byte value
                    R = pixel.R - pixel.R % 2;
                    G = pixel.G - pixel.G % 2;
                    B = pixel.B - pixel.B % 2;

                    //Clear all
                    output.SetPixel(x, y, Color.FromArgb(R, G, B));
                }
            }

            //Loop again but write the data on the clean pixels
            for (int y = 0; y < output.Height; y++)
            {
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

                    R = pixel.R + Convert.ToByte(next3bits[0]);
                    G = pixel.G + Convert.ToByte(next3bits[1]);
                    B = pixel.B + Convert.ToByte(next3bits[2]);

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

                    //Check if there's a sequence of 8 false bits - end
                    if (bitList.Count % 8 == 0 && bitList.Count > 8)
                    {
                        var subset = bitList.GetRange(bitList.Count - 8, 8);
                        if (subset.All(item => item == false))
                            ended = true;
                    }
                }
            }

            //Remove the last 8 false bits
            bitList = bitList.GetRange(0, bitList.Count - 8);

            var bitArray = new BitArray(bitList.ToArray());

            if (bitArray.Count % 8 != 0)
                throw new ApplicationException($"Invalid number of bits: {bitArray.Count}.");

            var bytes = new byte[bitArray.Count / 8];
            bitArray.CopyTo(bytes, 0);

            return bytes;
        }
    }
}
