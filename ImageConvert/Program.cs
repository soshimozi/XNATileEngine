using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace ImageConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: ImageConvert <inputimage> | <image directory> <output directory> <cellwidth> <cellheight>");
                return;
            }

            string inputFilename = args[0];
            string outputDirectory = args[1];
            int cellWidth = int.Parse(args[2]);
            int cellHeight = int.Parse(args[3]);


            FileAttributes fa = File.GetAttributes(inputFilename);

            if ((fa & FileAttributes.Directory) == FileAttributes.Directory)
            {
                // this is a directory
                processDirectory(inputFilename, outputDirectory, cellWidth, cellHeight);
            }
            else
            {
                processFile(inputFilename, outputDirectory, cellWidth, cellHeight);
            }
        
        }

        private static void processFile(string inputFilename, string outputPath, int cellWidth, int cellHeight)
        {
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            // now load image
            Image inputImage = Image.FromFile(inputFilename);

            int imageWidth = inputImage.Width;
            int imageHeight = inputImage.Height;

            int cellsAcross = imageWidth / cellWidth;
            int cellsDown = imageHeight / cellHeight;

            List<Image> imageList = new List<Image>();


            //int imageIndex = 0;
            for (int y = 0; y < cellsDown; y++)
            {
                for (int x = 0; x < cellsAcross; x++)
                {
                    Bitmap cellImage = new Bitmap(cellWidth, cellHeight);
                    using (Graphics imageGraphics = Graphics.FromImage(cellImage))
                    {
                        imageGraphics.DrawImage(inputImage, new Rectangle(0, 0, cellWidth, cellHeight), x * cellWidth, y * cellHeight, cellWidth, cellHeight, GraphicsUnit.Pixel);
                    }

                    imageList.Add(cellImage);
                    //string filename = string.Format("{0}_{1}.png", Path.GetFileNameWithoutExtension(inputFilename), imageIndex);

                    //filename = Path.Combine(outputPath, filename);
                    //cellImage.Save(filename);

                    //imageIndex++;
                }
            }

            // now we have our list of images, calcuate the size of final image
            int finalImageWidth = cellWidth * imageList.Count;
            int finalImageHeight = cellHeight;

            Image finalImage = new Bitmap(finalImageWidth, finalImageHeight);

            // now copy each of these images into final image
            using (Graphics finalImageGraphics = Graphics.FromImage(finalImage))
            {
                for (int i = 0; i < imageList.Count; i++)
                {
                    finalImageGraphics.DrawImage(imageList[i], i * cellWidth, 0);
                }
            }

            string filename = string.Format("{0}_strip.png", Path.GetFileNameWithoutExtension(inputFilename));
            filename = Path.Combine(outputPath, filename);
            finalImage.Save(filename);
        }

        private static void processDirectory(string inputFilename, string outputPath, int cellWidth, int cellHeight)
        {
            string[] pngFilenames = Directory.GetFiles(inputFilename, "*.png");
            string[] bmpFilenames = Directory.GetFiles(inputFilename, "*.bmp");
            string[] tgaFilenames = Directory.GetFiles(inputFilename, "*.tga");

            List<string> allFilenames = new List<string>();

            allFilenames.InsertRange(0, tgaFilenames);
            allFilenames.InsertRange(0, bmpFilenames);
            allFilenames.InsertRange(0, pngFilenames);

            // now process all files
            foreach (string filename in allFilenames)
            {
                processFile(filename, outputPath, cellWidth, cellHeight);
            }
        }
    }
}
