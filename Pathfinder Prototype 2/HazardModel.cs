﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;

namespace Pathfinder_Prototype_2
{
    class HazardModel
    {
        private int sectorSize;

        private float[,] hazardModel;
        private float[,] slopeModel;

        private Bitmap hazardBitmap;

        private int slopeWidth;
        private int slopeHeight;
        private int width;
        private int height;


        public HazardModel(float[,] slope, int sectorSizeT)
        {
            slopeModel = slope;

            sectorSize = sectorSizeT;

            slopeWidth = slope.GetLength(0);
            slopeHeight = slope.GetLength(1);

            generateHazardModel();
            generateHazardImage();
        }



        public float[,] getHazardModel()
        {
            return hazardModel;
        }

        private void generateHazardModel()
        {
            width = slopeWidth / sectorSize;
            height = slopeHeight / sectorSize;
            hazardModel = new float[width, height];

            int slopeX = 0;
            int slopeY = 0;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    hazardModel[x, y] = slopeModel[slopeX, slopeY];

                    for (int a = 0; a < sectorSize; a++)
                    {
                        for (int b = 0; b < sectorSize; b++)
                        {
                            if (hazardModel[x, y] < slopeModel[slopeX + a, slopeY + b])
                            {
                                hazardModel[x, y] = slopeModel[slopeX + a, slopeY + b];
                            }
                        }
                    }

                    slopeY += sectorSize;
                }
                slopeY = 0;
                slopeX += sectorSize;
            }
        }


        private void generateHazardImage()
        {
            Bitmap bitmap = new Bitmap(slopeWidth, slopeHeight);

            int slopeX = 0;
            int slopeY = 0;


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    System.Drawing.Color tempColor = getHazardColorValue(hazardModel[x, y]);

                    bitmap.SetPixel(slopeX, slopeY, tempColor);
                    for (int a = 0; a < sectorSize; a++)
                    {
                        for (int b = 0; b < sectorSize; b++)
                        {
                            bitmap.SetPixel(slopeX + a, slopeY + b, tempColor);
                        }
                    }

                    slopeY += sectorSize;
                }
                slopeY = 0;
                slopeX += sectorSize;
            }

            hazardBitmap = bitmap;
        }


        private System.Drawing.Color getHazardColorValue(float gradient)
        {
            System.Drawing.Color color = System.Drawing.Color.White;

            gradient = Math.Abs(gradient);

            float green = 255;
            float red = 255;
            float blue = 0;

            if (gradient <= 1f)
            {

                red = (1f - gradient) * 255;

            }
            else if (gradient <= 3f)
            {
                float percent = (gradient) / (3f);

                green = (1f - percent) * 255;
            }
            else
            {
                green = 0;
            }

            color = System.Drawing.Color.FromArgb(255, (int)red, (int)green, (int)blue);

            return color;
        }




        public ImageSource getHazardModelImage()
        {
            MemoryStream ms = new MemoryStream();
            hazardBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();

            ImageSource img = bi;

            return img;
        }

    }
}