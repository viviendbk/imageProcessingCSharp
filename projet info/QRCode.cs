using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projet_info
{
    // quand on fait un qrcode : vérifier que tous les charactères sont valide en alphanumérique
    public class QRCode
    {
        string mot;
        int version;
        byte[] nbCharacteresInBit;
        byte[] alphanumeriqueBitMode = { 0, 0, 1, 0 };
        List<byte> donneesMot;
        List<byte> listdonneesTotal;
        byte[] listBitInByte;
        byte[] correction;
        Pixel[,] matQRCode;
        byte[] masquageInPixel = { 0, 0, 0, 255, 0, 0, 0, 0, 0, 255, 255, 255, 0, 255, 255 };

        public QRCode(string mot)
        {
            this.mot = mot;
            this.version = Convert.ToInt32(mot.Length <= 25) + Convert.ToInt32(mot.Length > 25) * 2;
            Pixel[,] QR_Code = new Pixel[Convert.ToInt32(mot.Length <= 25) * 25 + Convert.ToInt32(mot.Length > 25) * 47, Convert.ToInt32(mot.Length <= 25) * 25 + Convert.ToInt32(mot.Length > 25) * 47];
            this.nbCharacteresInBit = IntToBit(mot.Length, 9);
            this.donneesMot = MotToDonnes(mot);
            this.listdonneesTotal = AssemblageDonneesEtAjustement();
            this.listBitInByte = BitToInt(listdonneesTotal);
            this.correction = ReedSolomonAlgorithm.Encode(listBitInByte, Convert.ToInt32(this.version == 1) * 7 + Convert.ToInt32(this.version == 2) * 10, ErrorCorrectionCodeType.QRCode);
            
            for (int i = 0; i < this.correction.Length; i++) // ajout de la correction
            {
                byte[] correctionInBit = IntToBit(correction[i], 8);
                for (int j = 0; j < 8; j++)
                {
                    ListdonneesTotal.Add(correctionInBit[j]);
                }
            }
            // si le qrcode est de version 2, il faut ajouter un octet pour pouvoir remplir toutes les cases
            for (int i = 0; i < Convert.ToInt32(this.version == 2) * 8; i++) ListdonneesTotal.Add(0);
            for(int i = 0; i < donneesMot.Count / 11; i++)
            {
                for(int j = 0; j < 11; j++)
                {
                    Console.Write(donneesMot[j + i * 11]);
                }
                Console.Write("   ");
            }
            this.matQRCode = CreateQRCode();
        }

        #region Propriétés

        public List<byte> ListdonneesTotal
        {
            get { return this.listdonneesTotal; }
            set { this.listdonneesTotal = value; }
        }
        #endregion

        /// <summary>
        /// converti un int en chaine de nbBit bits
        /// </summary>
        /// <param name="val"></param>
        /// <param name="nbBite"></param>
        /// <returns></returns>
        public static byte[] IntToBit(int val, int nbBit)
        {
            byte[] tabByte = new byte[nbBit];
            for(int i = 0; i < nbBit; i++)
            {
                tabByte[i] = (byte)Convert.ToInt32(val - Math.Pow(2, nbBit - 1 - i) >= 0);
                val -= Convert.ToInt32(val - Math.Pow(2, nbBit - 1 - i) >= 0) * Convert.ToInt32(Math.Pow(2, nbBit - 1 - i));
            }
            return tabByte;
        }

        /// <summary>
        /// Converti une chaine de bit en int
        /// </summary>
        /// <param name="ListBit"></param>
        /// <returns></returns>
        public byte[] BitToInt(List<byte> ListBit)
        {
            byte[] tabval = new byte[ListBit.Count / 8];

            for (int i = 0; i < tabval.Length; i++)
            {
                for(int j = 0; j < 8; j++) tabval[i] += (byte)(ListBit[i * 8 + j] * Math.Pow(2, 7 - j));
            }

            return tabval;
        }

        /// <summary>
        /// Converti un char en int avec la méthode alphanumérique
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int ConvertCharToInt(char c)
        {
            return Convert.ToInt32(c == ' ') * 36 + Convert.ToInt32(c == '$') * 37 + Convert.ToInt32(c == '%') * 38 + Convert.ToInt32(c == '*') * 39 + Convert.ToInt32(c == '+') * 40 + Convert.ToInt32(c == '-') * 41 +
                Convert.ToInt32(c == '.') * 42 + Convert.ToInt32(c == '/') * 43 + Convert.ToInt32(c == ':') * 44 +
                Convert.ToInt32(!" $%*+-./:".Contains(Convert.ToString(c))) * (Convert.ToInt32(48 <= Convert.ToInt32(c) && Convert.ToInt32(c) < 58) * (Convert.ToInt32(c) - 48) + 
                Convert.ToInt32(65 <= Convert.ToInt32(c) && Convert.ToInt32(c) < 91) * (Convert.ToInt32(c) - 55)); ;
        }

        /// <summary>
        /// Renvoie une chaine de bit qui contient la valeur du mot en mettant les lettes par pair en base 45
        /// </summary>
        /// <param name="mot"></param>
        /// <returns></returns>
        public static List<byte> MotToDonnes(string mot)
        {
            int cpt = 0;
            int[] tabValeurs = new int[Convert.ToInt32(mot.Length % 2 == 0) * mot.Length / 2 + Convert.ToInt32(mot.Length % 2 != 0) * (mot.Length / 2 + 1)];
            int longueur =  tabValeurs.Length;
            List<byte[]> ListTabBit = new List<byte[]>();
            List<byte> toReturn = new List<byte>();

            for (int i = 0; i < tabValeurs.Length; i++)
            {
                tabValeurs[i] += Convert.ToInt32(Math.Pow(45, Convert.ToInt32(i != tabValeurs.Length - 1 || mot.Length % 2 == 0)) * ConvertCharToInt(Convert.ToChar(Convert.ToString(mot[cpt]).ToUpper())));
                
                if (cpt + 1 < mot.Length) tabValeurs[i] += ConvertCharToInt(Convert.ToChar(Convert.ToString(mot[cpt + 1]).ToUpper()));

                cpt = cpt + 2;
            }
            for(int i = 0; i < tabValeurs.Length - 1; i++) ListTabBit.Add(IntToBit(tabValeurs[i], 11));

            ListTabBit.Add(IntToBit(tabValeurs[tabValeurs.Length - 1], Convert.ToInt32(mot.Length % 2 == 0) * 11 + Convert.ToInt32(mot.Length % 2 != 0) * 6));

            for (int i = 0; i < ListTabBit.Count; i++)
            {
                for(int j = 0; j < ListTabBit[i].Length; j++) toReturn.Add(ListTabBit[i][j]);
            }
            return toReturn;
        }

        public int LongueurRequise()
        {
            return Convert.ToInt32(this.version == 1) * 152 + Convert.ToInt32(this.version == 2) * 272;
        }

        /// <summary>
        /// assemble le masquage, le nombre de charactèe sur 9 bit, la chaine de bit qui contient la chaine de charactère du qr code, la terminaison et les blocs supplémentaires
        /// </summary>
        /// <returns></returns>
        public List<byte> AssemblageDonneesEtAjustement()
        {
            List<byte> toReturn = new List<byte>();
            byte[] bloc1 = { 1, 1, 1, 0, 1, 1, 0, 0 };
            byte[] bloc2 = { 0, 0, 0, 1, 0, 0, 0, 1 };

            for (int i = 0; i < alphanumeriqueBitMode.Length; i++) toReturn.Add(alphanumeriqueBitMode[i]);

            for (int i = 0; i < nbCharacteresInBit.Length; i++) toReturn.Add(nbCharacteresInBit[i]);

            for (int i = 0; i < donneesMot.Count; i++) toReturn.Add(donneesMot[i]);

            // on regarde s'il faut rajouter la terminaison "0 0 0 0"
            for (int i = 0; i < Convert.ToInt32(LongueurRequise() - toReturn.Count > 4) * 4 + Convert.ToInt32(LongueurRequise() - toReturn.Count <= 4) * (LongueurRequise() - toReturn.Count); i++) toReturn.Add(0);

            // on veut que ça fasse un multiple de 8 
            for (int i = 0; i < Convert.ToInt32(toReturn.Count % 8 != 0) * toReturn.Count % 8; i++) toReturn.Add(0);
            int nbBlockToAdd = (LongueurRequise() - toReturn.Count) / 8;
            // ajout des blocs pour atteindre la bonne valeur          
            for (int i = 0; i < nbBlockToAdd / 2; i++)
            {
                for (int j = 0; j < 8; j++) toReturn.Add(bloc1[j]);
                for (int j = 0; j < 8; j++) toReturn.Add(bloc2[j]);
            }
            for (int i = 0; i < Convert.ToInt32(nbBlockToAdd % 2 != 0) * 8; i++) toReturn.Add(bloc1[i]);

            return toReturn;
        }

        /// <summary>
        /// rempli le QRCode
        /// </summary>
        /// <returns></returns>
        public Pixel[,] CreateQRCode()
        {
            Pixel[,] MatriceQRCode = new Pixel[Convert.ToInt32(this.version == 1) * 21 + Convert.ToInt32(this.version == 2) * 25, Convert.ToInt32(this.version == 1) * 21 + Convert.ToInt32(this.version == 2) * 25];
            int cpt = 0;
            byte cpt2 = 0;
            byte decalage = 0;

            for (int i = 0; i < 8; i++) // finder pattern
            {
                for(int j = 0; j < 8; j++)
                {
                    MatriceQRCode[i, j] = new Pixel(ValPixel(1, i, j, MatriceQRCode.GetLength(0)), ValPixel(1, i, j, MatriceQRCode.GetLength(0)), ValPixel(1, i, j, MatriceQRCode.GetLength(0)));
                }
            }
            for (int i = MatriceQRCode.GetLength(0) - 8; i < MatriceQRCode.GetLength(0); i++) // finder pattern
            {
                for (int j = 0; j < 8; j++)
                {
                    MatriceQRCode[i, j] = new Pixel(ValPixel(2, i, j, MatriceQRCode.GetLength(0)), ValPixel(2, i, j, MatriceQRCode.GetLength(0)), ValPixel(2, i, j, MatriceQRCode.GetLength(0)));
                }
            }
            for (int i = 0; i < 8; i++) // finder pattern
            {
                for (int j = MatriceQRCode.GetLength(0) - 8; j < MatriceQRCode.GetLength(0); j++)
                {
                    MatriceQRCode[i, j] = new Pixel(ValPixel(3, i, j, MatriceQRCode.GetLength(0)), ValPixel(3, i, j, MatriceQRCode.GetLength(0)), ValPixel(3, i, j, MatriceQRCode.GetLength(0)));
                }
            }
            for(int i = 16; i < Convert.ToInt32(this.version == 2) * 21; i++ ) // motif d'alignement
            {
                for(int j = 16; j < 21; j++)
                {
                    MatriceQRCode[i, j] = new Pixel(ValPixel(4, i, j, MatriceQRCode.GetLength(0)), ValPixel(4, i, j, MatriceQRCode.GetLength(0)), ValPixel(4, i, j, MatriceQRCode.GetLength(0)));
                }
            }
            for(int i = 8; i < MatriceQRCode.GetLength(0) - 8; i++ ) // motifs de syncronisation
            {
                MatriceQRCode[6, i] = new Pixel((byte)cpt, (byte)cpt, (byte)cpt);
                MatriceQRCode[i, 6] = new Pixel((byte)cpt, (byte)cpt, (byte)cpt);
                cpt = (byte)(Convert.ToInt32(cpt == 0) * 255);
            }

            cpt = 0;

            for(int i = 0; i < 8; i++) // masquage
            {
                MatriceQRCode[8, i] = new Pixel(masquageInPixel[Convert.ToInt32(i < 6) * i + Convert.ToInt32(i > 6) * (i - 1)], masquageInPixel[Convert.ToInt32(i < 6) * i + Convert.ToInt32(i > 6) * (i - 1)], 
                                                masquageInPixel[Convert.ToInt32(i < 6) * i + Convert.ToInt32(i > 6) * (i - 1)]);
                MatriceQRCode[8, i + Convert.ToInt32(this.version == 1) * 13 + Convert.ToInt32(this.version == 2) * 17] = new Pixel(this.masquageInPixel[i + 7], this.masquageInPixel[i + 7], this.masquageInPixel[i + 7]);
                MatriceQRCode[MatriceQRCode.GetLength(0) - 1 - i, 8] = new Pixel(masquageInPixel[i], masquageInPixel[i], masquageInPixel[i]);
                MatriceQRCode[MatriceQRCode.GetLength(0) - (Convert.ToInt32(this.version == 1) * 13 + Convert.ToInt32(this.version == 2) * 17) - i, 8] = new Pixel(this.masquageInPixel[Convert.ToInt32(i < 2) * (i + 7) + Convert.ToInt32(i > 2) * (i + 6)], this.masquageInPixel[Convert.ToInt32(i < 2) * (i + 7) + Convert.ToInt32(i > 2) * (i + 6)], 
                                                                                  this.masquageInPixel[Convert.ToInt32(i < 2) * (i + 7) + Convert.ToInt32(i > 2) * (i + 6)]);
            }

            MatriceQRCode[0, 8] = new Pixel(this.masquageInPixel[14], this.masquageInPixel[14], this.masquageInPixel[14]);
            MatriceQRCode[4 * this.version + 9, 8] = new Pixel(0, 0, 0);   
            
            for(int i = 0; i < MatriceQRCode.GetLength(0) / 4; i++) // remplissage avec la chaine de bit obtenue
            {
                for (int j = 0; j < MatriceQRCode.GetLength(0); j++) // montée
                {
                    for (int k = 0; k < 2; k++) 
                    {
                        if (((MatriceQRCode.GetLength(0) - 1 - j) + (MatriceQRCode.GetLength(0) - 1 - cpt2 % 2 - 4 * i - decalage)) % 2 != 0 && MatriceQRCode[MatriceQRCode.GetLength(0) - 1 - j, MatriceQRCode.GetLength(0) - 1 - cpt2 % 2 - 4 * i - decalage] == null)
                        {
                            MatriceQRCode[MatriceQRCode.GetLength(0) - 1 - j, MatriceQRCode.GetLength(0) - 1 - cpt2 % 2 - 4 * i - decalage] = new Pixel((byte)(Convert.ToInt32(ListdonneesTotal[cpt] == 0) * 255), (byte)(Convert.ToInt32(ListdonneesTotal[cpt] == 0) * 255),
                                                                                                                                    (byte)(Convert.ToInt32(ListdonneesTotal[cpt] == 0) * 255));
                            cpt++;
                            cpt2++;
                        }
                        else if (((MatriceQRCode.GetLength(0) - 1 - j) + (MatriceQRCode.GetLength(0) - 1 - cpt2 % 2 - 4 * i - decalage)) % 2 == 0 && MatriceQRCode[MatriceQRCode.GetLength(0) - 1 - j, MatriceQRCode.GetLength(0) - 1 - cpt2 % 2 - 4 * i - decalage] == null)
                        {
                            MatriceQRCode[MatriceQRCode.GetLength(0) - 1 - j, MatriceQRCode.GetLength(0) - 1 - cpt2 % 2 - 4 * i - decalage] = new Pixel((byte)(Convert.ToInt32(ListdonneesTotal[cpt] != 0) * 255), (byte)(Convert.ToInt32(ListdonneesTotal[cpt] != 0) * 255),
                                                                                                                                    (byte)(Convert.ToInt32(ListdonneesTotal[cpt] != 0) * 255));
                            cpt++;
                            cpt2++;
                        }
                        else
                        {
                            cpt2++;
                        }
                    }
                }

                cpt2 = 0;

                for (int j = 0; j < MatriceQRCode.GetLength(1); j++) // descente
                {
                    for (int k = 0; k < 2; k++)
                    {
                        if (i == MatriceQRCode.GetLength(0) / 4 - 2)
                        {
                            decalage = 1;
                        }
                        if ((j + (MatriceQRCode.GetLength(0) - 1 - 2 - cpt2 % 2 - i * 4 - decalage)) % 2 != 0 && MatriceQRCode[j, MatriceQRCode.GetLength(0) - 1 - 2 - cpt2 % 2 - i * 4 - decalage] == null)
                        {
                            MatriceQRCode[j, MatriceQRCode.GetLength(0) - 1 - 2 - cpt2 % 2 - i * 4 - decalage] = new Pixel((byte)(Convert.ToInt32(ListdonneesTotal[cpt] == 0) * 255), (byte)(Convert.ToInt32(ListdonneesTotal[cpt] == 0) * 255),
                                                                                                                                    (byte)(Convert.ToInt32(ListdonneesTotal[cpt] == 0) * 255));
                            cpt++;
                            cpt2++;
                        }
                        else if ((j + (MatriceQRCode.GetLength(0) - 1 - 2 - cpt2 % 2 - i * 4 - decalage)) % 2 == 0 && MatriceQRCode[j, MatriceQRCode.GetLength(0) - 1 - 2 - cpt2 % 2 - i * 4 - decalage] == null)
                        {
                            MatriceQRCode[j, MatriceQRCode.GetLength(0) - 1 - 2 - cpt2 % 2 - i * 4 - decalage] = new Pixel((byte)(Convert.ToInt32(ListdonneesTotal[cpt] != 0) * 255), (byte)(Convert.ToInt32(ListdonneesTotal[cpt] != 0) * 255),
                                                                                                                                    (byte)(Convert.ToInt32(ListdonneesTotal[cpt] != 0) * 255));
                            cpt++;
                            cpt2++;
                        }
                        else
                        {
                            cpt2++;
                        }
                    }
                }        
            }

            return MatriceQRCode;
        }

        public byte ValPixel(int emplacement, int i, int j, int hauteur)
        {
            return (byte)(Convert.ToInt32(emplacement == 1) * (Convert.ToInt32(i == 0 || j == 0 || i == 6 || j == 6 || i >= 2 && i <= 4 && j >= 2 && j <= 4) * 0 +
                  Convert.ToInt32(((i == 1 || i == 5) && j >= 1 && j <= 5) || ((i >= 2 && i <= 5) && (j == 1 || j == 5)) || (i == 7 || j == 7)) * 255) +
                  
                  Convert.ToInt32(emplacement == 2) * (Convert.ToInt32((i == hauteur - 7 || i == hauteur - 1) && j < 7 || (i <= hauteur - 1 && i >= hauteur - 7 && j == 6 || (i >= hauteur - 7 && j == 0))) * 0 + 
                  Convert.ToInt32(((i == hauteur - 6 || i == hauteur - 2) && j >= 1 && j <= 5) || ((i >= hauteur - 6 && i <= hauteur - 3) && (j == 1 || j == 5)) || (i == hauteur - 8 || j == 7)) * 255) +
                  
                  Convert.ToInt32(emplacement == 3) * (Convert.ToInt32(i >= 0 && i < 7 && (j == hauteur - 7 || j == hauteur- 1) || ((i == 0 || i == 6) && j >= hauteur - 7 || j >= hauteur - 5 && j <= hauteur - 3 && i >= 2 && i <= 4)) * 0 +
                  Convert.ToInt32(((j == hauteur - 6 || j == hauteur - 2) && i >= 1 && i <= 5) || ((j >= hauteur - 6 && j <= hauteur - 3) && (i == 1 || i == 5)) || (i == 7 && j >= hauteur - 8) || (j == hauteur - 8 && (i >= 0 || i < 8))) * 255) +
                  
                  Convert.ToInt32(emplacement == 4) * (Convert.ToInt32((i == 16 || i == 20) && j >= 16 || (j == 16 || j == 20) && i >= 16 || i == 18 && j == 18) * 0 +
                  Convert.ToInt32((i == 17 || i == 19) && (j >= 17 && j < 20) || (j == 17 || j == 19) && (i >= 17 && i < 20)) * 255));
        }
    }
}
