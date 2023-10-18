using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Media;
using System.Diagnostics;

namespace projet_info
{
    public class Program
    {
        static void Main(string[] args)
        {


            /*QRCode test = new QRCode("HELLO WORLD");
            MyImage QRtest = new MyImage(test.MatQRCode);
            QRtest.From_Image_To_File("sortie51");*/

            MyImage test1 = new MyImage("coco.bmp");
            test1.Rotation(62);
            test1.From_Image_To_File("clara");
            Process.Start("clara");
            /*
            for(int i = 0; i < test.MatQRCode.GetLength(0); i++)
            {
                for(int j = 0; j < test.MatQRCode.GetLength(1); j++)
                {
                    if (test.MatQRCode[i ,j].Red == 0)
                    {
                        Console.Write("1 ");
                    }
                    else if (test.MatQRCode [i ,j].Red == 255)
                    {
                        Console.Write("0 ");
                    }
                    else if(test.MatQRCode[i, j].Red == 50)
                    {
                        Console.Write("x ");
                    }
                    else
                    {
                        Console.Write(test.MatQRCode[i, j].Red);
                    }
                }
                Console.WriteLine();
            }
            */
            /*
            MyImage test1 = new MyImage("coco.bmp");
            test1.Mirror_Effect();
            test1.From_Image_To_File("sortie11");
            */
            // Process.Start("sortie11");

            Console.WriteLine("b");
            Console.ReadLine();
            Console.ReadKey();
           
        }
    /*    static bool IsCharAvailable(char c)
        {
            return "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:".Contains(Convert.ToString(c).ToUpper());
        }*/
    }
}
