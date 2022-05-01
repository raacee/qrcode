using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Projet_Info
{
    public class MyImage
    {
        #region Fields
        private int _height;
        private int _width;
        private byte[] _imagebytes;
        private byte[] _headerbytes;
        private uint _headersize;
        private double _filesize;
        private double _imagesize;
        private int offsetSize;
        private int _padding;
        public string _path;
        private Pixel[,] _pixels;
        public int Height
        {
            get => _height;
            set => _height = value;
        }

        public int Width
        {
            get => _width;
            set => _width = value;
        }

        public byte[] Imagebytes
        {
            get => _imagebytes;
            set => _imagebytes = value;
        }

        public byte[] Headerbytes
        {
            get => _headerbytes;
            set => _headerbytes = value;
        }

        public uint Headersize
        {
            get => _headersize;
            set => _headersize = value;
        }

        public double Filesize
        {
            get => _filesize;
            set => _filesize = value;
        }

        public double Imagesize
        {
            get => _imagesize;
            set => _imagesize = value;
        }
        
        public int OffsetSize
        {
            get => offsetSize;
            set => offsetSize = value;
        }

        public int Padding
        {
            get => _padding;
            set => _padding = value;
        }

        public string Path
        {
            get => _path;
            set => _path = value;
        }

        public Pixel[,] Pixels
        {
            get => _pixels;
            set => _pixels = value;
        }

        
        #endregion

        public MyImage()
        {
            throw new Exception("WRONG CONSTRUCTOR");
        }
        public MyImage(string path)
        {
            this._path = path;
            byte[] allBytes = File.ReadAllBytes(this._path);
            if (allBytes == null || allBytes.Length == 0)
            {
                return;
            }
                
            for (int i = 0; i < 4; i++)
            {
                _headersize += (uint) Math.Floor(allBytes[i + 10] * Math.Pow(256,i));
            }
            
            this._headerbytes = new byte[_headersize];
            
            //getting header
            for (int i = 0; i < _headersize; i++)
            {
                this._headerbytes[i] = allBytes[i];
            }
            
            //getting image size, height, width, padding
            for (int i = 2; i < 6; i++)
            {
                _filesize += this._headerbytes[i] * Math.Pow(256, i - 2);
                offsetSize += (int) Math.Floor(this._headerbytes[i + 8] * Math.Pow(256, i - 2));
                _width += (int) Math.Floor(this._headerbytes[i + 16] * Math.Pow(256, i - 2));
                _height += (int) Math.Floor(this._headerbytes[i + 20] * Math.Pow(256, i - 2));
                _imagesize += this._headerbytes[i + 32] * Math.Pow(256, i - 2);
                
            }
            this._padding = _width % 4;
            
            this._pixels = new Pixel[_height, _width];
            _imagebytes = new byte[(int) _filesize - _headersize];
            
            for (uint i = _headersize; i < allBytes.Length; i++)
            {
                _imagebytes[i - _headersize] = allBytes[i];
            }
            
            //converting image to PIXEL array
            this._pixels = ToPixelArray(_imagebytes,_height,_width,_padding);
        }
        public static byte[] ToByteArray(Pixel[,] pixelmat)
        {
            List<byte> res = new List<byte>();
            int padding = pixelmat.GetLength(1) % 4;
            
            for (var index0 = 0; index0 < pixelmat.GetLength(0); index0++)
            for (var index1 = 0; index1 < pixelmat.GetLength(1); index1++)
            {
                var pixel = pixelmat[index0, index1];
                
                    res.Add(pixel.Blue);
                    res.Add(pixel.Green);
                    res.Add(pixel.Red);

                    if (index1 == pixelmat.GetLength(1) - 1)
                {
                    for (int i = 0; i < padding; i++)
                    {
                        res.Add(0);
                    }
                }
            }
            return res.ToArray();
        }
        public static Pixel[,] ToPixelArray(byte[] arr,int height, int width, int padding)
        {
            Pixel[,] res = new Pixel[height,width];
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width * 3 - padding; j += 3)
                {
                    res[i, j / 3] = new Pixel(
                        arr[i * width * 3 + j + i * padding + 2],
                        arr[i * width * 3 + j + i * padding + 1],
                        arr[i * width * 3 + j + i * padding]
                    );
                }
            }
            return res;
        }
        public byte[] AddBaseHeader(byte[] arr)
        {
            byte[] res = new byte[arr.Length + this._headersize];

            for (int i = 0; i < this._headersize; i++)
            {
                res[i] = _headerbytes[i];
            }
            
            for (uint i = this._headersize; i < res.Length; i++)
            {
                res[i] = arr[i-this._headersize];
            }
            return res;
        }
        public static string toLittleEndianString(int n)
        {
            byte[] resarr = new byte[4];
            resarr[0] = (byte)n;
            resarr[1] = (byte)(((uint)n >> 8) & 0xFF);
            resarr[2] = (byte)(((uint)n >> 16) & 0xFF);
            resarr[3] = (byte)(((uint)n >> 24) & 0xFF);
            string res = "";
            
            for (int i = 0; i < resarr.Length; i++)
            {
                res += Convert.ToByte(resarr[i]);
                if (i != 3) res += ' ';
            }
            return res;
        }
        public static byte[] HeaderBuilder(Pixel[,] pixels)
        {
            int padding = pixels.GetLength(1) % 4;
            string headerstring = $"66 77 " +
                            $"{toLittleEndianString(pixels.Length*3 + 54 + padding*pixels.GetLength(0))} " +
                            $"0 0 0 0 " +
                            $"54 0 0 0 " +
                            $"40 0 0 0 " +
                            $"{toLittleEndianString(pixels.GetLength(1))} " +
                            $"{toLittleEndianString(pixels.GetLength(0))} " +
                            $"1 0 " +
                            $"24 0 " +
                            $"0 0 0 0 " +
                            $"{toLittleEndianString(pixels.GetLength(0)*pixels.GetLength(1)+(pixels.GetLength(1)%4)*pixels.GetLength(0))} " +
                            $"0 0 0 0 " +
                            $"0 0 0 0 " +
                            $"0 0 0 0 " +
                            $"0 0 0 0";
            
            byte[] res = new byte[54];
            string[] headerstrarr = headerstring.Split(' ');

            for (var index = 0; index < headerstrarr.Length; index++)
            {
                var b = headerstrarr[index];
                res[index] = Convert.ToByte(headerstrarr[index]);
            }
            return res;
        }
        public static T[] ArrAppend<T>(T[] arr1, T[] arr2)
        {
            T[] res = new T[arr1.Length + arr2.Length];
            for (int i = 0; i < arr1.Length; i++)
            {
                res[i] = arr1[i];
            }

            for (int i = arr1.Length; i < res.Length; i++)
            {
                res[i] = arr2[i - arr1.Length];
            }

            return res;
        }
        //resize to do
        public MyImage Resize(int toHeight, int toWidth)
        {
            var heightcoeff = Convert.ToDouble(toHeight) / this._width;
            var widthcoeff = Convert.ToDouble(toWidth) / this._width;
            
            var pixelRes = new Pixel[toHeight,toWidth];

            for (int i = 0; i < toHeight; i++)
            {
                for (int j = 0; j < toWidth; j++)
                {
                    var a = Convert.ToInt32(Math.Floor(i / widthcoeff));
                    var b = Convert.ToInt32(Math.Floor(j / widthcoeff));
                    var c = this.Pixels[a, b];
                    pixelRes[i, j] = c;
                }
            }

            int newpadding = toWidth % 4;
            byte[] imagebytes = ToByteArray(pixelRes);
            
            byte[] header = HeaderBuilder(pixelRes);
            byte[] resarr = ArrAppend(header, imagebytes);
            File.WriteAllBytes("out_resized.bmp",resarr);
            return new MyImage("out_resized.bmp");
        }
        //
        public MyImage BlackAndWhite(string outputpath = "")
        {
            Pixel[,] pixelmat = new Pixel[this._height, this._width];

            for (int i = 0; i < this._height; i++)
            {
                for (int j = 0; j < this._width; j++)
                {
                    byte avg = Convert.ToByte(Math.Round((float)(this._pixels[i, j].Red + this._pixels[i, j].Green + this._pixels[i, j].Blue)/3));
                    pixelmat[i, j] = new Pixel(avg, avg, avg);
                }
            }
            
            byte[] bytearr = ToByteArray(pixelmat);

            byte[] allbytes = AddBaseHeader(bytearr);
            
            File.WriteAllBytes(outputpath+"out.bmp",allbytes);
            
            return new MyImage(outputpath+"out.bmp");
        }
        public MyImage Mirror(string outputpath = "")
        {
            Pixel[,] resarr = new Pixel[this._height, this._width];
            
            for (int i = 0; i < this._height; i++)
            {
                for (int j = 0; j < this._width; j++)
                {
                    var pixel = new Pixel(this._pixels[i, this._width-j-1].Red, this._pixels[i, this._width-j-1].Green, this._pixels[i, this._width-j-1].Blue);
                    resarr[i, j] = pixel;
                }
            }
            byte[] bytearr = ToByteArray(resarr);
            
            byte[] allbytes = AddBaseHeader(bytearr);
            
            File.WriteAllBytes(outputpath+"out.bmp",allbytes);
            
            return new MyImage(outputpath+"out.bmp");
        }
        public MyImage Rotate(double angle) 
        {
            #region Renaming math functions

            Func<double, double> ToRad = degrees => (double) degrees * Math.PI / 180;
            Func<double, double> cosin = degrees => Math.Cos(ToRad(degrees));
            Func<double, double> sin = degrees => Math.Sin(ToRad(degrees));
            
            Func<int[], int> distanceFromCenter = (int[] coordinates) =>
                (int) Math.Round(Math.Sqrt(Math.Pow(coordinates[0], 2) + Math.Pow(coordinates[1], 2)));
            #endregion
            
            int new_height = (int)(Math.Abs(cosin(angle)*this._height)+Math.Abs(sin(angle)*this._width));
            int new_width = (int)(Math.Abs(cosin(angle) * this._width) + Math.Abs(sin(angle) * this._height)-1);
            int center_original_height = (this._height+1) / 2 - 1;
            int center_original_width = (this._width+1) / 2 - 1; 
            int center_new_height = (new_height +1) / 2 - 1;
            int center_new_width = (new_width +1) / 2 - 1;
            Pixel[,] rotate = new Pixel[new_height,new_width];
            
            double new_x;
            double new_y;
            FillBlack(rotate);
            
            for (int i = 0; i< this._height; i++)
            {
                for (int j=0; j< this._width; j++)
                {
                    int x = this._width - 1 - j - center_original_width;
                    int y = this._height - 1 - i -center_original_height;

                    new_x = center_new_height - (int)(Math.Round(cosin(angle) * y - sin(angle) * x));
                    new_y = center_new_width - (int)(Math.Round(cosin(angle) * x + sin(angle) * y));
                    
                    if (0 <= new_x && new_x < new_height && 0 <= new_y && new_y < new_width)
                    {
                        rotate[(int)(new_x), (int)(new_y)] = this._pixels[i, j];
                    }
                }
            }

            byte[] bytearr = ToByteArray(rotate);
            var newheader = HeaderBuilder(rotate);
            var allbytes = ArrAppend(newheader,bytearr);

            File.WriteAllBytes("rotate_"+ this.Path, allbytes);
            return new MyImage("rotate_"+this.Path);
        }
        public MyImage Outlines()
        {
            int[,] convOutline = {{-1, -1, -1}, {-1, 8, -1}, {-1, -1, -1}};
            Pixel[,] newpixelarr = new Pixel[this._pixels.GetLength(0) + 2, this._pixels.GetLength(1) + 2];
            var pixelRes = new Pixel[this._pixels.GetLength(0), this._pixels.GetLength(1)];

            for (int i = 0; i < this._pixels.GetLength(0) + 2; i++)
            {
                for (int j = 0; j < this._pixels.GetLength(1) + 2; j++)
                {
                    try
                    {
                        if (i == 0 || j == 0 || i == this._pixels.GetLength(0) + 1 ||
                            j == this._pixels.GetLength(1) + 1)
                        {
                            newpixelarr[i, j] = new Pixel(0, 0, 0);
                        }

                        else
                        {
                            newpixelarr[i, j] = new Pixel(this._pixels[i - 1, j - 1].Red,
                                this._pixels[i - 1, j - 1].Green, this._pixels[i - 1, j - 1].Blue);

                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Index out of range at " + i + " " + j);
                    }
                }
            }
            
            for (int i = 1; i <= pixelRes.GetLength(0); i++)
            {
                for (int j = 1; j <= pixelRes.GetLength(1); j++)
                {
                    int newred;
                    int newgreen;
                    int newblue;

                    newred = newpixelarr[i - 1, j - 1].Red * convOutline[2, 0] 
                             + newpixelarr[i - 1, j].Red * convOutline[2, 1] 
                             + newpixelarr[i - 1, j + 1].Red * convOutline[2, 2]
                             + newpixelarr[i, j - 1].Red * convOutline[1, 0] 
                             + newpixelarr[i, j].Red * convOutline[1, 1] 
                             + newpixelarr[i, j + 1].Red * convOutline[1, 2]
                             + newpixelarr[i + 1, j - 1].Red * convOutline[0, 0] 
                             + newpixelarr[i + 1, j].Red * convOutline[0, 1] 
                             + newpixelarr[i + 1, j + 1].Red * convOutline[0, 2];
                    
                    newgreen = newpixelarr[i - 1, j - 1].Green * convOutline[2, 0]
                             + newpixelarr[i - 1, j].Green * convOutline[2, 1]
                             + newpixelarr[i - 1, j + 1].Green * convOutline[2, 2]
                             + newpixelarr[i, j - 1].Green * convOutline[1, 0]
                             + newpixelarr[i, j].Green * convOutline[1, 1] 
                             + newpixelarr[i, j + 1].Green * convOutline[1, 2]
                             + newpixelarr[i + 1, j - 1].Green * convOutline[0, 0]
                             + newpixelarr[i + 1, j].Green * convOutline[0, 1]
                             + newpixelarr[i + 1, j + 1].Green * convOutline[0, 2];
                    
                    newblue = newpixelarr[i - 1, j - 1].Blue * convOutline[2, 0] 
                              + newpixelarr[i - 1, j].Blue * convOutline[2, 1] 
                              + newpixelarr[i - 1, j + 1].Blue * convOutline[2, 2]
                              + newpixelarr[i, j - 1].Blue * convOutline[1, 0] 
                              + newpixelarr[i, j].Blue * convOutline[1, 1] 
                              + newpixelarr[i, j + 1].Blue * convOutline[1, 2]
                              + newpixelarr[i + 1, j - 1].Blue * convOutline[0, 0] 
                              + newpixelarr[i + 1, j].Blue * convOutline[0, 1]
                              + newpixelarr[i + 1, j + 1].Blue * convOutline[0, 2];

                    if (newred < 0) newred = (byte)0;
                    if (newgreen < 0) newgreen= (byte)0;
                    if (newblue < 0) newblue = (byte)0;

                    if (newred >= 256) newred = (byte)255;
                    if (newgreen >= 256) newgreen = (byte)255;
                    if (newblue >= 256) newblue = (byte)255;

                    pixelRes[i - 1, j - 1] = new Pixel(Convert.ToByte(newred), Convert.ToByte(newgreen),
                        Convert.ToByte(newblue));
                }
            }

            var bytearr = AddBaseHeader(ToByteArray(pixelRes));

            File.WriteAllBytes("out.bmp", bytearr);
            
            
            return new MyImage("out.bmp");

        }
        public MyImage Blur()
        {
            double[,] convblur = {
                {1, 2, 1}, 
                {2, 4, 2}, 
                {1, 2, 1}
            };
            Pixel[,] newpixelarr = new Pixel[this._pixels.GetLength(0) + 2, this._pixels.GetLength(1) + 2];
            var pixelRes = new Pixel[this._pixels.GetLength(0), this._pixels.GetLength(1)];

            for (int i = 0; i < this._pixels.GetLength(0) + 2; i++)
            {
                for (int j = 0; j < this._pixels.GetLength(1) + 2; j++)
                {
                    try
                    {
                        if (i == 0 || j == 0 || i == this._pixels.GetLength(0) + 1 ||
                            j == this._pixels.GetLength(1) + 1)
                        {
                            newpixelarr[i, j] = new Pixel(0, 0, 0);
                        }

                        else
                        {
                            newpixelarr[i, j] = new Pixel(this._pixels[i - 1, j - 1].Red,
                                this._pixels[i - 1, j - 1].Green, this._pixels[i - 1, j - 1].Blue);

                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Index out of range at " + i + " " + j);
                    }
                }
            }
            
            for (int i = 1; i <= pixelRes.GetLength(0); i++)
            {
                for (int j = 1; j <= pixelRes.GetLength(1); j++)
                {
                    double postred;
                    double postgreen;
                    double postblue;

                    postred = newpixelarr[i - 1, j - 1].Red * convblur[2, 0] 
                             + newpixelarr[i - 1, j].Red * convblur[2, 1] 
                             + newpixelarr[i - 1, j + 1].Red * convblur[2, 2]
                             + newpixelarr[i, j - 1].Red * convblur[1, 0] 
                             + newpixelarr[i, j].Red * convblur[1, 1] 
                             + newpixelarr[i, j + 1].Red * convblur[1, 2]
                             + newpixelarr[i + 1, j - 1].Red * convblur[0, 0] 
                             + newpixelarr[i + 1, j].Red * convblur[0, 1] 
                             + newpixelarr[i + 1, j + 1].Red * convblur[0, 2];

                    postred = Math.Round(postred/16);

                    postgreen = newpixelarr[i - 1, j - 1].Green * convblur[2, 0]
                                + newpixelarr[i - 1, j].Green * convblur[2, 1]
                                + newpixelarr[i - 1, j + 1].Green * convblur[2, 2]
                                + newpixelarr[i, j - 1].Green * convblur[1, 0]
                                + newpixelarr[i, j].Green * convblur[1, 1] 
                                + newpixelarr[i, j + 1].Green * convblur[1, 2]
                                + newpixelarr[i + 1, j - 1].Green * convblur[0, 0]
                                + newpixelarr[i + 1, j].Green * convblur[0, 1]
                                + newpixelarr[i + 1, j + 1].Green * convblur[0, 2];

                    postgreen = Math.Round(postgreen/16);
                    
                    postblue = newpixelarr[i - 1, j - 1].Blue * convblur[2, 0] 
                              + newpixelarr[i - 1, j].Blue * convblur[2, 1] 
                              + newpixelarr[i - 1, j + 1].Blue * convblur[2, 2]
                              + newpixelarr[i, j - 1].Blue * convblur[1, 0] 
                              + newpixelarr[i, j].Blue * convblur[1, 1] 
                              + newpixelarr[i, j + 1].Blue * convblur[1, 2]
                              + newpixelarr[i + 1, j - 1].Blue * convblur[0, 0] 
                              + newpixelarr[i + 1, j].Blue * convblur[0, 1]
                              + newpixelarr[i + 1, j + 1].Blue * convblur[0, 2];
                    
                    postblue = Math.Round(postblue/16);

                    if (postred < 0) postred = (byte)0;
                    if (postgreen < 0) postgreen= (byte)0;
                    if (postblue < 0) postblue = (byte)0;

                    if (postred >= 256) postred = (byte)255;
                    if (postgreen >= 256) postgreen = (byte)255;
                    if (postblue >= 256) postblue = (byte)255;

                    pixelRes[i - 1, j - 1] = new Pixel(
                        Convert.ToByte(postred), 
                        Convert.ToByte(postgreen),
                        Convert.ToByte(postblue)
                    );
                }
            }

            var bytearr = AddBaseHeader(ToByteArray(pixelRes));

            File.WriteAllBytes("out.bmp", bytearr);
            
            
            return new MyImage("out.bmp");

        }
        public MyImage Sharpness()
        {
            double[,] convSharpness = {
                {0, -1, 0}, 
                {-1, 5, -1}, 
                {0, -1, 0}
            };
            Pixel[,] newpixelarr = new Pixel[this._pixels.GetLength(0) + 2, this._pixels.GetLength(1) + 2];
            var pixelRes = new Pixel[this._pixels.GetLength(0), this._pixels.GetLength(1)];

            for (int i = 0; i < this._pixels.GetLength(0) + 2; i++)
            {
                for (int j = 0; j < this._pixels.GetLength(1) + 2; j++)
                {
                    try
                    {
                        if (i == 0 || j == 0 || i == this._pixels.GetLength(0) + 1 ||
                            j == this._pixels.GetLength(1) + 1)
                        {
                            newpixelarr[i, j] = new Pixel(0, 0, 0);
                        }

                        else
                        {
                            newpixelarr[i, j] = new Pixel(this._pixels[i - 1, j - 1].Red,
                                this._pixels[i - 1, j - 1].Green, this._pixels[i - 1, j - 1].Blue);

                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Index out of range at " + i + " " + j);
                    }
                }
            }
            
            for (int i = 1; i <= pixelRes.GetLength(0); i++)
            {
                for (int j = 1; j <= pixelRes.GetLength(1); j++)
                {
                    double postred;
                    double postgreen;
                    double postblue;

                    postred = newpixelarr[i - 1, j - 1].Red * convSharpness[2, 0] 
                             + newpixelarr[i - 1, j].Red * convSharpness[2, 1] 
                             + newpixelarr[i - 1, j + 1].Red * convSharpness[2, 2]
                             + newpixelarr[i, j - 1].Red * convSharpness[1, 0] 
                             + newpixelarr[i, j].Red * convSharpness[1, 1] 
                             + newpixelarr[i, j + 1].Red * convSharpness[1, 2]
                             + newpixelarr[i + 1, j - 1].Red * convSharpness[0, 0] 
                             + newpixelarr[i + 1, j].Red * convSharpness[0, 1] 
                             + newpixelarr[i + 1, j + 1].Red * convSharpness[0, 2];

                    postred = Math.Round(postred);

                    postgreen = newpixelarr[i - 1, j - 1].Green * convSharpness[2, 0]
                                + newpixelarr[i - 1, j].Green * convSharpness[2, 1]
                                + newpixelarr[i - 1, j + 1].Green * convSharpness[2, 2]
                                + newpixelarr[i, j - 1].Green * convSharpness[1, 0]
                                + newpixelarr[i, j].Green * convSharpness[1, 1] 
                                + newpixelarr[i, j + 1].Green * convSharpness[1, 2]
                                + newpixelarr[i + 1, j - 1].Green * convSharpness[0, 0]
                                + newpixelarr[i + 1, j].Green * convSharpness[0, 1]
                                + newpixelarr[i + 1, j + 1].Green * convSharpness[0, 2];

                    postgreen = Math.Round(postgreen);
                    
                    postblue = newpixelarr[i - 1, j - 1].Blue * convSharpness[2, 0] 
                              + newpixelarr[i - 1, j].Blue * convSharpness[2, 1] 
                              + newpixelarr[i - 1, j + 1].Blue * convSharpness[2, 2]
                              + newpixelarr[i, j - 1].Blue * convSharpness[1, 0] 
                              + newpixelarr[i, j].Blue * convSharpness[1, 1] 
                              + newpixelarr[i, j + 1].Blue * convSharpness[1, 2]
                              + newpixelarr[i + 1, j - 1].Blue * convSharpness[0, 0] 
                              + newpixelarr[i + 1, j].Blue * convSharpness[0, 1]
                              + newpixelarr[i + 1, j + 1].Blue * convSharpness[0, 2];
                    
                    postblue = Math.Round(postblue);

                    if (postred < 0) postred = (byte)0;
                    if (postgreen < 0) postgreen= (byte)0;
                    if (postblue < 0) postblue = (byte)0;

                    if (postred >= 256) postred = (byte)255;
                    if (postgreen >= 256) postgreen = (byte)255;
                    if (postblue >= 256) postblue = (byte)255;

                    pixelRes[i - 1, j - 1] = new Pixel(
                        Convert.ToByte(postred), 
                        Convert.ToByte(postgreen),
                        Convert.ToByte(postblue)
                    );
                }
            }

            var bytearr = AddBaseHeader(ToByteArray(pixelRes));

            File.WriteAllBytes("out.bmp", bytearr);
            
            
            return new MyImage("out.bmp");

        }        
        public MyImage HideInside(MyImage pathotherimage)
        {
            var oldpixels = this._pixels;
            var pixelsToHide = pathotherimage._pixels;
            var bytepixelsarr = new byte[oldpixels.Length];
            Pixel[,] resPixels = new Pixel[oldpixels.GetLength(0), oldpixels.GetLength(1)];

            #region local functions

            Func<byte,string> toBinaryString = b => Convert.ToString(b, 2).PadLeft(8, '0');
            
            Func<string,byte> binaryToByte = delegate(string s)
            {
                byte res = 0;
                for (var i = 0; i < s.Length; i++)
                {
                    //overflow wtf ???
                    res += Convert.ToByte(Convert.ToByte(s[i]) * Convert.ToByte(Math.Pow(2, 7 - i)));
                }
                return res;
            };
            #endregion

            for (int i = 0; i < pixelsToHide.GetLength(0); i++)
            {
                for (int j = 0; j < pixelsToHide.GetLength(1); j++)
                {
                    var bin1red = toBinaryString(oldpixels[i,j].Red);
                    var bin1green = toBinaryString(oldpixels[i,j].Green);
                    var bin1blue = toBinaryString(oldpixels[i,j].Blue);
                    
                    var bin2red = toBinaryString(pixelsToHide[i,j].Red);
                    var bin2green = toBinaryString(pixelsToHide[i,j].Green);
                    var bin2blue = toBinaryString(pixelsToHide[i,j].Blue);

                    var finalbinred = bin1red.Substring(0, 4) + bin2red.Substring(4);
                    var finalbingreen = bin1green.Substring(0, 4) + bin2green.Substring(4);
                    var finalbinblue = bin1blue.Substring(0, 4) + bin2blue.Substring(4);

                    byte newred = binaryToByte(finalbinred);
                    byte newgreen = binaryToByte(finalbingreen);
                    byte newblue = binaryToByte(finalbinblue);

                    resPixels[i, j] = new Pixel(newred, newgreen, newblue);
                }
            }
            
            bytepixelsarr = ToByteArray(resPixels);
            var bytefinalarr = AddBaseHeader(bytepixelsarr);
            File.WriteAllBytes("hidden.bmp", bytefinalarr);

            return new MyImage("hidden.bmp");
        }        
        public void ConsoleWriteHeader()
        {
            foreach (var n in this._headerbytes)
            {
                Console.Write(n+" ");
            }
        }
        public void ConsoleWritePixels()
        {
            foreach (var n in this.Pixels)
            {
                Console.Write(n.Blue+" "+n.Green+" "+n.Red+" ");
            }
        }
        public MyImage Histogramme_Rouge()
        {
            var pixelres = new Pixel[this._height, this._width];
            FillBlack(pixelres);
            
            for (int i = 0; i < _width; i++)
            {
                int redcounter = 0;
                
                for (int j = 0; j < _height; j++)
                {
                    redcounter += Pixels[j,i].Red;
                }

                redcounter = redcounter/_height;

                for (int j = 0; j < redcounter; j++)
                {
                    pixelres[j, i] = new Pixel(255, 0, 0);
                }
            }

            byte[] bytearr = ToByteArray(pixelres);
            var newheaderbytes = HeaderBuilder(pixelres);
            var allbytes = ArrAppend(newheaderbytes, bytearr);
            
            File.WriteAllBytes("redhist_"+this.Path, allbytes);
            return new MyImage(this._path);
        }
        public static MyImage Fractal()
        {
            Pixel[,] pixelres = new Pixel[1080, 1920];

            for (int x = 0; x < pixelres.GetLength(1); x++)
            {
                for (int y = 0; y < pixelres.GetLength(0); y++)
                {
                    double cx = (x - pixelres.GetLength(1) / 2) / (pixelres.GetLength(1) / 4.0);
                    double cy = (y - pixelres.GetLength(0) / 2) / (pixelres.GetLength(0) / 4.0);
                    double a = cx;
                    double b = cy;
                    int i = 0;
                    double aa;
                    double bb;

                    do
                    {
                        aa = a * a;
                        bb = b * b;
                        double twoab = 2.0 * a * b;
                        a = aa - bb + cx;
                        b = twoab + cy;
                        i++;
                    } while ((aa + bb < 16.0) && (i < 255));

                    pixelres[y, x] = new Pixel(Convert.ToByte(i), Convert.ToByte(i),
                        Convert.ToByte(i));
                }
            }

            var newheader = HeaderBuilder(pixelres);
            var pixelsarr = ToByteArray(pixelres);
            var allbytesres = ArrAppend(newheader, pixelsarr);
            File.WriteAllBytes("Mandelbrot_Set.bmp",allbytesres);
            return new MyImage("Mandelbrot_Set.bmp");
        }
        public void ConsoleWriteAll()
        {
            foreach (var n in this._headerbytes)
            {
                Console.Write(n+" ");
            }
            foreach (var n in this.Pixels)
            {
                Console.Write(n.Blue+" "+n.Green+" "+n.Red+" ");
            }
            
        }
        public static void FillBlack(Pixel[,] mat)
        {
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    mat[i, j] = new Pixel(0, 0, 0);
                }
            }
        }
    }
    
    public class Pixel
    {
        private byte _r;
        private byte _g;
        private byte _b;
        private byte[] colors = new byte[3];

        public byte Red 
        {
            get { return _r; }
            set { _r = value; } 
        }

        public byte Green
        {
            get { return _g; }
            set { _g = value; }
        }
        public byte Blue
        {
            get { return _b; }
            set { _b = value; }
        }

        public Pixel(bool white = false)
        {
            this._r = (byte)(0+Convert.ToByte(white)*255);
            this._b = (byte)(0+Convert.ToByte(white)*255);
            this._g = (byte)(0+Convert.ToByte(white)*255);
            this.colors = new byte[]
            {_b, _g, _r};
        }

        public Pixel(byte r, byte g, byte b)
        {
            this._r = r;
            this._g = g;
            this._b = b;
            this.colors = new byte[]{_b, _g, _r};
        }
        
        public override string ToString()
        {
            return this._b + " " + this._g + " " + this._r;
        }
    }

}
