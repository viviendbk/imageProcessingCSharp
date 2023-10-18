using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace projet_info
{
    public class MyImage
    {
        string ImageType;
        int FileSize;
        int OffsetSize;
        int Width;
        int Height;
        int NbBitsPerColor;
        int HeaderInfo;
        public Pixel[,] ImageRgb;


        #region Propriétés

        public string TypeImage
        {
            get { return this.ImageType; }
            set { this.ImageType = value; }
        }

        public int SizeFile
        {
            get { return this.FileSize; }
            set { this.FileSize = value; }
        }

        public int SizeOffset
        {
            get { return this.OffsetSize; }
            set { this.OffsetSize = value; }
        }

        public int Hauteur
        {
            get { return this.Height; }
            set { this.Height = value; }
        }

        public int Largeur
        {
            get { return this.Width; }
            set { this.Width = value; }
        }

        #endregion

        public MyImage(string myfile)
        {
            byte[] MyFile = File.ReadAllBytes(myfile);
            char B = Encoding.ASCII.GetChars(MyFile)[0];
            char M = Encoding.ASCII.GetChars(MyFile)[1];
            this.ImageType = Convert.ToString(B + "" + M);
            this.FileSize = Convertir_Endian_To_Int(MyFile, 2);
            this.OffsetSize = Convertir_Endian_To_Int(MyFile, 10);
            this.HeaderInfo = Convertir_Endian_To_Int(MyFile, 14);
            this.Width = Convertir_Endian_To_Int(MyFile, 18);
            this.Height = Convertir_Endian_To_Int(MyFile, 22);
            this.NbBitsPerColor = MyFile[28] + MyFile[29] * 256;
            this.ImageRgb = new Pixel[this.Height, this.Width];
            int compt = 54;

            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    byte r = MyFile[compt];
                    byte g = MyFile[compt + 1];
                    byte b = MyFile[compt + 2];
                    this.ImageRgb[i, j] = new Pixel(r, g, b);
                    compt += 3;
                }
            }
        }
        public MyImage(Pixel[,] Image)
        {
            this.NbBitsPerColor = 24;
            this.Width = Image.GetLength(1);
            this.Height = Image.GetLength(0);
            this.ImageType = "BM";
            this.FileSize = 3 * Image.GetLength(1) * Image.GetLength(0) + 54;
            this.OffsetSize = 54;
            this.ImageRgb = Image;
        }

        #region TD 2
        public void From_Image_To_File(string file)
        {
            List<byte> FileInstance = new List<byte>();
            FileInstance.Add(66); //B
            FileInstance.Add(77); //M
            for (int i = 0; i < 4; i++)
            {
                FileInstance.Add(Convertir_Int_To_Endian(FileSize)[i]);
            }
            for (int i = 0; i < 4; i++)
            {
                FileInstance.Add(0);
            }
            for (int i = 0; i < 4; i++)
            {
                FileInstance.Add(Convertir_Int_To_Endian(OffsetSize)[i]);
            }
            FileInstance.Add(40);
            FileInstance.Add(0);
            FileInstance.Add(0);
            FileInstance.Add(0);
            for (int i = 0; i < 4; i++)
            {
                FileInstance.Add(Convertir_Int_To_Endian(Width)[i]);
            }
            for (int i = 0; i < 4; i++)
            {
                FileInstance.Add(Convertir_Int_To_Endian(Height)[i]);
            }
            FileInstance.Add(1);
            FileInstance.Add(0);
            for (int i = 0; i < 2; i++)
            {
                FileInstance.Add(Convertir_Int_To_Endian(NbBitsPerColor)[i]);
            }
            for (int i = 0; i < 4; i++)
            {
                FileInstance.Add(0);
            }
            for (int i = 0; i < 4; i++)
            {
                FileInstance.Add(Convertir_Int_To_Endian(FileSize - OffsetSize)[i]);
            }
            while (FileInstance.Count() < 54)
            {
                FileInstance.Add(0);
            }
            for (int i = this.Height - 1; i >= 0; i--)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    FileInstance.Add(ImageRgb[i, j].Blue);
                    FileInstance.Add(ImageRgb[i, j].Green);
                    FileInstance.Add(ImageRgb[i, j].Red);
                }
                for (int j = 0; j < this.Width % 4; j++)
                {
                    FileInstance.Add(0);
                }
            }
            byte[] tab = new byte[FileInstance.Count];
            for (int i = 0; i < tab.Length; i++)
            {
                tab[i] = FileInstance[i];
            }
            File.WriteAllBytes(file, tab);
        }
        public int Convertir_Endian_To_Int(byte[] tab, int index)
        {
            int result = 0;
            for (int i = index; i < index + 4; i++)
            {
                result += tab[i] * Convert.ToInt32(Math.Pow(256, i - index));
            }
            return result;
        }

        public byte[] Convertir_Int_To_Endian(int val)
        {
            byte[] cpt = new byte[4];
            for (int i = 3; i >= 0; i--)
            {
                cpt[i] = 0;
                while (val - Convert.ToInt32(Math.Pow(256, i)) >= 0)
                {
                    val -= Convert.ToInt32(Math.Pow(256, i));
                    cpt[i]++;
                }
            }
            return cpt;
        }
        #endregion

        #region TD 3
        public void Retrecir_Image(int coef)
        {
            Pixel[,] NewImage = new Pixel[this.Height / coef, this.Width / coef];

            for (int i = 0; i < NewImage.GetLength(0); i++)
            {
                for (int j = 0; j < NewImage.GetLength(1); j++)
                {
                    NewImage[i, j] = ImageRgb[i * coef, j * coef];
                }
            }
            this.ImageRgb = new Pixel[NewImage.GetLength(0), NewImage.GetLength(1)];
            this.ImageRgb = NewImage;
            this.Height = NewImage.GetLength(0);
            this.Width = NewImage.GetLength(1);
            this.FileSize = 54 + this.Width * 3 * this.Height;
        }

        public void Agrandir_Image(int coef)
        {
            Pixel[,] NewImage = new Pixel[this.Height * coef, this.Width * coef];

            for (int i = 0; i < NewImage.GetLength(0); i++)
            {
                for (int j = 0; j < NewImage.GetLength(1); j++)
                {
                    NewImage[i, j] = ImageRgb[i / coef, j / coef];
                }
            }
            this.ImageRgb = new Pixel[NewImage.GetLength(0), NewImage.GetLength(1)];
            this.ImageRgb = NewImage;
            this.Height = NewImage.GetLength(0);
            this.Width = NewImage.GetLength(1);
            this.FileSize = 54 + Width * 3 * Height;
        }
        public void From_Color_To_Grey()
        {
            byte rgb;
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    rgb = (byte)(Convert.ToInt32(ImageRgb[i, j].Red + ImageRgb[i, j].Green + ImageRgb[i, j].Blue) / 3);
                    ImageRgb[i, j] = new Pixel(rgb, rgb, rgb);
                }
            }
        }
        /// <summary>
        /// met un effet miroir
        /// </summary>
        /// <param name="Image"></param>
        /// <returns></returns>
        public void Mirror_Effect()
        {
            Pixel[,] ImageMiroir = new Pixel[ImageRgb.GetLength(0), ImageRgb.GetLength(1)];
            for (int i = 0; i < ImageRgb.GetLength(0); i++)
            {
                for (int j = 0; j < ImageRgb.GetLength(1); j++)
                {
                    ImageMiroir[i, ImageRgb.GetLength(1) - j - 1] = ImageRgb[i, j];
                }
            }
            this.ImageRgb = ImageMiroir;
        }
        /// <summary>
        /// Tourne l'image en fonction de l'angle qui est entré en paramètre
        /// </summary>
        /// <param name="angle"></param>
        public void Rotation(double angle)
        {
            double rad = ((double)angle * Math.PI) / 180.0;
            int newHeight = (int)(Math.Cos(rad) * this.Height + Math.Sin(rad) * this.Width) * (Convert.ToInt32((Math.Cos(rad) * this.Height + Math.Sin(rad) * this.Width) < 0) * - 1 + Convert.ToInt32((Math.Cos(rad) * this.Height + Math.Sin(rad) * this.Width) > 0));
            int newWidth = (int)(Math.Cos(rad) * this.Width + Math.Sin(rad) * this.Height) * (Convert.ToInt32((Math.Cos(rad) * this.Width + Math.Sin(rad) * this.Height) < 0) * - 1 + Convert.ToInt32((Math.Cos(rad) * this.Width + Math.Sin(rad) * this.Height) > 0));
            int abscisseCentre = newWidth / 2;
            int ordonneeCentre = newHeight / 2;
            Pixel[,] newImage = new Pixel[newHeight, newWidth];

            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth; j++)
                {
                    int abscisse = Convert.ToInt32(Math.Cos(rad) * (j - abscisseCentre) + Math.Sin(rad) * (i - ordonneeCentre) + this.Width / 2);
                    int ordonnee = Convert.ToInt32(-Math.Sin(rad) * (j - abscisseCentre) + (Math.Cos(rad) * (i - ordonneeCentre)) + this.Height / 2);
                    
                    newImage[i, j] = new Pixel(0, 0, 0);
                    if (abscisse < Width && abscisse >= 0 && ordonnee < Height && ordonnee >= 0)
                    {
                        newImage[i, j] = this.ImageRgb[ordonnee, abscisse];
                    }
                }
            }

            this.ImageRgb = new Pixel[newHeight, newWidth];
            this.Height = newHeight;
            this.Width = newWidth;
            this.FileSize = 54 + (3 * newHeight * newWidth);
            this.ImageRgb = newImage;
        }



        #endregion

        #region TD 4

        /// <summary>
        /// Applique un filtre "Détection des contours"
        /// </summary>
        public void Outline_Detection()
        {
            From_Color_To_Grey();
            int[,] MatConvolution = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
            Convolution(1, MatConvolution);
        }
        /// <summary>
        /// Applique un filtre "Flou"
        /// </summary>
        public void Blurring()
        {
            int[,] MatConvolution = { { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 }, { 1, 1, 1, 1, 1 } };
            Convolution(25, MatConvolution);
        }
        /// <summary>
        /// Applique un filre "Repoussage"
        /// </summary>
        public void Repelling()
        {
            int[,] MatConvolution = { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };
            Convolution(1, MatConvolution);
        }
        /// <summary>
        /// Applique un filtre "Renforcement des contours"
        /// </summary>
        public void Edge_Reinforcement()
        {
            From_Color_To_Grey();
            int[,] MatConvolution = { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } };
            Convolution(1, MatConvolution);
        }
        /// <summary>
        /// Fonction qui permet d'utiliser les filtres
        /// </summary>
        /// <param name="coef"></param>
        /// <param name="MatConvolution"></param>
        public void Convolution(int coef, int[,] MatConvolution)
        {
            Pixel[,] NewImage = new Pixel[Height, Width];
            int[] tabPixel = { 0, 0, 0 };
            int index = Convert.ToInt32(coef != 1) * 2 + Convert.ToInt32(coef == 1); ;

            for (int i = index; i < this.Height - 2; i++)
            {
                for (int j = index; j < this.Width - 2; j++)
                {
                    for (int k = -index; k < index + 1; k++)
                    {
                        for (int s = -index; s < index + 1; s++)
                        {
                            tabPixel[0] += ImageRgb[i + k, j + s].Red * MatConvolution[k + index, s + index];
                            tabPixel[1] += ImageRgb[i + k, j + s].Green * MatConvolution[k + index, s + index];
                            tabPixel[2] += ImageRgb[i + k, j + s].Blue * MatConvolution[k + index, s + index];
                        }
                    }
                    NewImage[i, j] = new Pixel((byte)(NewValPixel(tabPixel[0], coef)), (byte)(NewValPixel(tabPixel[1], coef)), (byte)(NewValPixel(tabPixel[2], coef)));
                    for (int u = 0; u < 3; u++) { tabPixel[u] = 0; }
                }
            }
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    if (NewImage[i, j] == null)
                    {
                        NewImage[i, j] = new Pixel(0, 0, 0);
                    }
                }
            }
            this.ImageRgb = NewImage;
        }

        public int NewValPixel(int val, int coef)
        {
            return Convert.ToInt32(val / coef > 255) * 255 + Convert.ToInt32(val / coef < 0) * 0 + 
                    Convert.ToInt32(val / coef > 0 && val / coef < 255) * val / coef;
        }
        #endregion

        #region TD 5
        /// <summary>
        /// Créée une fractale de Mandelbrot
        /// </summary>
        public void Fractale_Mandelbrot()
        {
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    double Reel_C = -2.1 + ((0.6 + 2.1) / this.Width) * j;
                    double Im_C = -1.2 + ((1.2 + 1.2) / this.Height) * i;
                    double Reel_Z = 0;
                    double Im_Z = 0;
                    int iteration = 0;
                    do
                    {
                        double temp = Reel_Z; 
                        Reel_Z = (Reel_Z * Reel_Z) - (Im_Z * Im_Z) + Reel_C; 
                        Im_Z = (2 * Im_Z * temp) + Im_C;
                        iteration++;
                    } while (iteration < 50 && Reel_Z * Reel_Z + Im_Z * Im_Z < 4);

                    ImageRgb[i, j] = new Pixel(0, 0, 0);
                    ImageRgb[i, j].Red = (byte)(Convert.ToInt32(iteration < 50) * (iteration * 255 / 50));
                }
            }
        }
        /// <summary>
        /// Crée une fractale de julia
        /// </summary>
        public void Fractale_Julia(double zoom)
        {
            zoom = Convert.ToInt32(zoom < 4) * 4 + Convert.ToInt32(zoom >= 4) * zoom;
            Pixel[,] julia = new Pixel[(int)(zoom * 2), (int)(zoom * 2.4)];
            for (int i = 0; i < julia.GetLength(0); i++)
            {
                for (int j = 0; j < julia.GetLength(1); j++)
                {
                    double Reel_C = 0.35;
                    double Im_C = 0.05;
                    double Reel_Z = i / (zoom) - 1;
                    double Im_Z = j / (zoom) - 1.2;
                    int iteration = 0;
                    do
                    {
                        double temp = Reel_Z;
                        Reel_Z = (Reel_Z * Reel_Z) - (Im_Z * Im_Z) + Reel_C; 
                        Im_Z = (2 * Im_Z * temp) + Im_C;
                        iteration++;
                    } while (iteration < 100 && (Reel_Z * Reel_Z + Im_Z * Im_Z) < 4);

                    julia[i, j] = new Pixel(0, 0, 0);
                    julia[i, j].Red = (byte)(Convert.ToInt32(iteration < 100) * (iteration * 255 / 100));
                }
            }
            this.ImageRgb = new Pixel[julia.GetLength(0), julia.GetLength(1)];
            this.ImageRgb = julia;
            this.Height = julia.GetLength(0);
            this.Width = julia.GetLength(1);
            this.FileSize = 54 + Width * 3 * Height;
        }
        /// <summary>
        /// Crée un histogramme des couleurs
        /// </summary>
        public void Histogramme()
        {
            int[] tabCountRed = new int[256];
            int[] tabCountBlue = new int[256];
            int[] tabCountGreen = new int[256];
            int newOccurrencePixel = 0;

            // comptage de l'occurence de chaque pixel dans l'image
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    tabCountRed[ImageRgb[i, j].Red]++;
                    tabCountBlue[ImageRgb[i, j].Blue]++;
                    tabCountGreen[ImageRgb[i, j].Green]++;
                }
            }

            // détermination de l'échelle des ordonnées qui sera 256 / maxOccurrencePixel
            for (int i = 0; i < 256; i++)
            {
                newOccurrencePixel = Convert.ToInt32(tabCountRed[i] > newOccurrencePixel) * tabCountRed[i] + Convert.ToInt32(tabCountRed[i] < newOccurrencePixel) * newOccurrencePixel;
                newOccurrencePixel = Convert.ToInt32(tabCountBlue[i] > newOccurrencePixel) * tabCountBlue[i] + Convert.ToInt32(tabCountBlue[i] < newOccurrencePixel) * newOccurrencePixel;
                newOccurrencePixel = Convert.ToInt32(tabCountGreen[i] > newOccurrencePixel) * tabCountGreen[i] + Convert.ToInt32(tabCountGreen[i] < newOccurrencePixel) * newOccurrencePixel;
            }

            Pixel[,] Histogramme = new Pixel[256, 256];

            // remplissage de la matice rgb
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    Histogramme[i, j] = new Pixel(0, 0, 0);
                    for (int k = 0; k < i * Convert.ToInt32(tabCountRed[j] * 256 / newOccurrencePixel == i) ; k++)
                    {
                        Histogramme[k, j].Red = (byte)j;
                    }           
                    for (int k = 0; k < i * Convert.ToInt32(tabCountBlue[j] * 256 / newOccurrencePixel == i); k++)
                    {
                        Histogramme[k, j].Blue = (byte)j;
                    }             
                    for (int k = 0; k < i * Convert.ToInt32(tabCountGreen[j] * 256 / newOccurrencePixel == i); k++)
                    {
                        Histogramme[k, j].Green = (byte)j;
                    }              
                }
            }
            this.ImageRgb = new Pixel[Histogramme.GetLength(0), Histogramme.GetLength(1)];
            this.ImageRgb = Histogramme;
            this.Height = Histogramme.GetLength(0);
            this.Width = Histogramme.GetLength(1);
            this.FileSize = 54 + Width * 3 * Height;
            Agrandir_Image(4);
        }
        /// <summary>
        /// Encrypte une image
        /// </summary>
        public void Encryptage()
        {
            int cpt = 0;
            for(int i = 0; i < this.Height; i++)
            {
                for(int j = 0; j < this.Width; j++)
                {
                    ImageRgb[i, j].Red += (byte)(cpt % 256);
                    ImageRgb[i, j].Green += (byte)(cpt % 256);
                    ImageRgb[i, j].Blue += (byte)(cpt % 256);
                    cpt++;
                }
            }
        }
        /// <summary>
        /// Décrypter une image
        /// </summary>
        public void Decryptage()
        {
            int cpt = 0;
            for (int i = 0; i < this.Height; i++)
            {
                for (int j = 0; j < this.Width; j++)
                {
                    ImageRgb[i, j].Red -= (byte)(cpt % 256);
                    ImageRgb[i, j].Green -= (byte)(cpt % 256);
                    ImageRgb[i, j].Blue -= (byte)(cpt % 256);
                    cpt++;
                }
            }
        }
        #endregion
    }
}


