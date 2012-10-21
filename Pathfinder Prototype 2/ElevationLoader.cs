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
    class ElevationLoader
    {
        private String format;
        private int width;
        private int height;
        private String[] hexMap;
        private int imageStartIndex;

        private bool p6Valid = true;

        private Bitmap image;

        private float[,] heightMap;


        private float verticalHeight = 0.1f; // 0.1m


        private float maxHeight = -1;

        private float minHeight = 0;

        public ElevationLoader(String imagePath)
        {

            byte[] bytes = getBytesFromFile(imagePath);

            String hexData = HexStr(bytes);

            hexMap = createHexArray(hexData);

            getHeaderType();

            getDimensions();



            generateHeightMap();
            generateBitmap();
        }


        public float[,] getHeightMap()
        {
            return heightMap;
        }

        private void getHeaderType()
        {
            if (hexMap[0] == "50")
            {
                if (hexMap[1] == "36")
                {
                    p6Valid = true;
                }
            }
        }


        private void getDimensions()
        {
            //get width
            String[] widthData = new String[100];
            String[] heightData = new String[100];

            int startIndex = 3;
            int widthIndex = 0;
            for (int i = startIndex; i < 100; i++)
            {
                if (hexMap[i] != "20")
                {
                    widthData[widthIndex] = hexMap[i];
                    widthIndex++;
                }
                else
                {
                    widthIndex++;
                    break;
                }

            }

            String widthValue = "";

            for (int i = 0; i < widthIndex + 1; i++)
            {
                if (widthData[i] != null)
                {
                    widthValue += System.Convert.ToChar(System.Convert.ToUInt32(widthData[i].Substring(0, 2), 16)).ToString();
                }
                else
                {
                    break;
                }
            }



            int heightIndex = 0;
            for (int i = widthIndex + startIndex; i < 100; i++)
            {
                if (hexMap[i] != "20")
                {
                    heightData[heightIndex] = hexMap[i];
                }
                else
                {
                    break;
                }

                heightIndex++;
            }

            String heightValue = "";

            for (int i = 0; i < heightIndex + 1; i++)
            {
                if (heightData[i] != null)
                {
                    heightValue += System.Convert.ToChar(System.Convert.ToUInt32(heightData[i].Substring(0, 2), 16)).ToString();
                }
                else
                {
                    break;
                }
            }


            width = Int32.Parse(widthValue);
            height = Int32.Parse(heightValue);

            imageStartIndex = heightIndex + widthIndex + startIndex - 2;

            String val = hexMap[imageStartIndex];
        }






        private String[] createHexArray(String hexData)
        {
            hexData = hexData.Substring(2);
            String[] hexMap = new String[hexData.Length / 2];

            int hexIndex = 0;

            for (int i = 0; i < hexData.Length / 2; i++)
            {
                hexMap[i] = hexData.Substring(hexIndex, 2);
                hexIndex += 2;
            }

            return hexMap;
        }


        public static byte[] getBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)

            FileStream fs = File.OpenRead(fullFilePath);
            try
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                return bytes;
            }
            finally
            {
                fs.Close();
            }

        }



        public static string HexStr(byte[] p)
        {

            char[] c = new char[p.Length * 2 + 2];

            byte b;

            // c[0] = '0'; c[1] = 'x';

            for (int y = 0, x = 2; y < p.Length; ++y, ++x)
            {

                b = ((byte)(p[y] >> 4));

                c[x] = (char)(b > 9 ? b + 0x37 : b + 0x30);

                b = ((byte)(p[y] & 0xF));

                c[++x] = (char)(b > 9 ? b + 0x37 : b + 0x30);

            }

            return new string(c);

        }



        private void generateHeightMap()
        {
            heightMap = new float[width, height];

            int hexMapIndex = imageStartIndex + 7;
            int widthCounter = 0;
            int heightCounter = 0;

            int x = 0;
            int y = 0;

            do
            {
                do
                {
                    int red = getColorValue(hexMap[hexMapIndex]);
                    int green = getColorValue(hexMap[hexMapIndex + 1]);
                    int blue = getColorValue(hexMap[hexMapIndex + 2]);

                    heightMap[x, y] = getHeight(red, green);

                    x++;
                    widthCounter += 3;
                    hexMapIndex += 3;
                } while (x < width);
                heightCounter++;
                widthCounter = 0;
                y++;
                x = 0;

            } while (y < height);

        }



        private float getHeight(int red, int green)
        {
            float height = 0;


            height = ((red * 256 + green));

            return height;
        }


        private void setMaxHeight()
        {

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (heightMap[x, y] > maxHeight)
                    {
                        maxHeight = heightMap[x, y];
                    }
                }
            }
        }


        private void setMinHeight()
        {
            minHeight = heightMap[0, 0];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (heightMap[x, y] < minHeight)
                    {
                        minHeight = heightMap[x, y];
                    }
                }
            }
        }


        private void generateBitmap()
        {
            setMaxHeight();
            setMinHeight();
            Bitmap bitmap = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float heightTemp = heightMap[x, y];
                    int value = getHeightColor(heightTemp);

                    System.Drawing.Color tempColor = System.Drawing.Color.FromArgb(255, value, value, value);

                    bitmap.SetPixel(x, y, tempColor);
                }
            }



            image = bitmap;
        }



        public int getHeightColor(float height)
        {
            float valueR = height - minHeight;

            float valueT = (valueR / (maxHeight - minHeight));
            float tempNumber = valueT * 255f;

            // int tempNumber = Int32.Parse(temp);
            return (int)tempNumber;
        }

        public int getColorValue(String hex)
        {
            // String temp = System.Convert.ToChar(System.Convert.ToUInt32(hex.Substring(0, 2), 16)).ToString();

            int tempNumber = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);

            // int tempNumber = Int32.Parse(temp);
            return tempNumber;
        }



        public Bitmap getBitmap()
        {
            return image;
        }


        public ImageSource getImageSource()
        {


            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
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