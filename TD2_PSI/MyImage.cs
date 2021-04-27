using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace TD2_PSI
{

    #region Constructeur
    /// <summary>
    /// The MyImage class provides informations from a BMP image 
    /// </summary>
    public class MyImage
    {
        byte[] dataImage;
        string typeImage;
        int size;
        int offset;
        int width;
        int height;
        int bitsPerColor;
        Pixel[,] imageMatrix;

        /// <summary>
        /// Constructor of the class 
        /// </summary>
        /// <param name="myfile"> Name of the file to use </param>
        public MyImage(string myfile)
        {
            this.dataImage = File.ReadAllBytes(myfile);
            typeImage = Encoding.ASCII.GetString(ExtractData(0, 1));

            // Taille du fichier
            this.size = Convertir_Endian_To_Int(ExtractData(2, 5));

            // Offset
            this.offset = Convertir_Endian_To_Int(ExtractData(10, 13));

            // Width
            this.width = Convertir_Endian_To_Int(ExtractData(18, 21));

            // Height
            this.height = Convertir_Endian_To_Int(ExtractData(22, 25));

            // Bits per color
            this.bitsPerColor = Convertir_Endian_To_Int(ExtractData(28, 29));

            if (bitsPerColor != 24)
            {
                throw new Exception("24-bits file only");
            }

            // Matrix of the image

            this.imageMatrix = new Pixel[this.height, this.width];
            int k = this.offset;
            for (int i = 0; i < this.imageMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < this.imageMatrix.GetLength(1); j++)
                {
                    Pixel pix = new Pixel(dataImage[k], dataImage[k + 1], dataImage[k + 2]);
                    k += 3;
                    this.imageMatrix[i, j] = pix;
                }
            }
        }

        public MyImage(int _width, int _height)         //création d'image (pour fractale)
        {
            imageMatrix = new Pixel[_height, _width];
            this.width = _width;
            this.height= _height;
            this.size = _width * _height * 3 +54; //taille fichier
            offset = 54;
            bitsPerColor = 8;
            typeImage = "BM";
            dataImage = new byte[offset]; 
            dataImage[0] = 66; dataImage[1] = 77; //BM
            Convertir_Int_To_Endian(dataImage, this.size, 2, 5); //tailler fichier
            for (int i = 6; i <= 9; i++) { dataImage[i] = 0; }
            Convertir_Int_To_Endian(dataImage, this.offset, 10, 13); //header
            Convertir_Int_To_Endian(DataImage, 40, 14, 17); //header info
            Convertir_Int_To_Endian(DataImage, this.width, 18, 21); //width
            Convertir_Int_To_Endian(DataImage, this.height, 22, 25); //height
            dataImage[26] = 1; dataImage[27] = 0;
            Convertir_Int_To_Endian(DataImage, bitsPerColor, 28, 29); //bits per color
            for (int i = 30; i <= 33; i++) { dataImage[i] = 0; } //compression (image non compressée)
            Convertir_Int_To_Endian(dataImage, (this.size-54), 34, 37); //tailler image
            for (int i = 38; i <= 53; i++) { dataImage[i] = 0; }

        }

        #endregion

        #region Propriétés
        /// <summary>
        /// Get the type of the picture
        /// </summary>
        public string TypeImage
        {
            get { return this.typeImage; }
        }

        /// <summary>
        /// Get the picture's data array
        /// </summary>
        public byte[] DataImage
        {
            get { return this.dataImage; }
        }

        /// <summary>
        /// Get the picture's pixel matrix
        /// </summary>
        public Pixel[,] ImageMatrix
        {
            get { return this.imageMatrix; }
        }

        /// <summary>
        /// Get the size of the picture
        /// </summary>
        public int Size
        {
            get { return this.size; }
        }

        /// <summary>
        /// Get the Width of the picture
        /// </summary>
        public int Width
        {
            get { return this.width; }
        }

        /// <summary>
        /// Get the Height of the picture
        /// </summary>
        public int Height
        {
            get { return this.height; }
        }

        /// <summary>
        /// Get the length of the header (offset)
        /// </summary>
        public int Offset
        {
            get { return this.offset; }
        }

        /// <summary>
        /// Get the count of bits per color 
        /// </summary>
        public int BitsPerColor
        {
            get { return this.bitsPerColor; }
        }
        public Pixel[,] ImageMatrixe
        {
            get { return this.imageMatrix; }
        }

        #endregion

        #region Outils

        /// <summary>
        /// Returns a string describing the picture
        /// </summary>
        /// <returns> A string describing the picture </returns>
        public string toString()
        {
            string imageInfo = "";

            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    imageInfo = String.Concat(imageInfo, imageMatrix[i, j].ToString(), " ");
                }
                imageInfo += "\n";
            }

            return "Taille de l'image : " + this.size + " o" + "\n" +
                "Offset : " + this.offset + " o" + "\n" +
                "Largeur : " + this.width + " px" + "\n" +
                "Hauteur : " + this.height + " px" + "\n" +
                "Bits par couleur : " + this.bitsPerColor + "\n" +
                "Matrice de l'image : \n" + imageInfo;
        }

        /// <summary>
        /// Get a subarray from the picture's data array
        /// </summary>
        /// <param name="firstIndex"> Start index </param>
        /// <param name="secondIndex"> End index </param>
        /// <returns> A byte array from the picture's data array </returns>
        public byte[] ExtractData(int firstIndex, int secondIndex)
        {
            byte[] extractedData = new byte[secondIndex - firstIndex + 1];
            for (int i = firstIndex; i <= secondIndex; i++)
            {
                extractedData[i - firstIndex] = this.dataImage[i];
            }
            return extractedData;
        }

        /// <summary>
        /// Creates an image from the image's data array
        /// </summary>
        /// <param name="file"> Name of the image </param>
        public void From_Image_To_File(string file)
        {
            file += ".bmp";
            try
            {
                byte[] tabToConvert = new byte[this.offset + this.width * this.height * 3];

                // Header de base
                for (int i = 0; i < offset; i++)
                {
                    tabToConvert[i] = ExtractData(0, 54)[i];
                }

                // Modification Header
                Convertir_Int_To_Endian(tabToConvert, this.width, 18, 22);
                Convertir_Int_To_Endian(tabToConvert, this.height, 22, 26);
                Convertir_Int_To_Endian(tabToConvert, this.width * this.height * 3, 34, 38);

                // Matrice image
                int compteur = this.offset;
                for (int i = 0; i < this.height; i++)
                {
                    for (int j = 0; j < this.width; j++)
                    {
                        tabToConvert[compteur] = this.imageMatrix[i, j].R;
                        tabToConvert[compteur + 1] = this.imageMatrix[i, j].G;
                        tabToConvert[compteur + 2] = this.imageMatrix[i, j].B;
                        compteur = compteur + 3;
                    }
                }
                File.WriteAllBytes(file, tabToConvert);
            }
            catch (IOException e)
            {
                Console.WriteLine("Le fichier n'a pas pu être créé" + e);
            }
            Console.WriteLine("Le fichier " + file + " a bien été créé");
        }


        /// <summary>
        /// Converts a byte array in the little endian system to an integer
        /// </summary>
        /// <param name="tab"> Byte array in the little endian system </param>
        /// <returns></returns>
        public int Convertir_Endian_To_Int(byte[] tab)
        {
            int intValue = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                intValue += tab[i] * Convert.ToInt32(Math.Pow(256, i));
            }
            return intValue;
        }

        /// <summary>
        /// Converts an integer to a byte array in the little endian system
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public void Convertir_Int_To_Endian(byte[] genTab, int val, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                genTab[i] = (byte)(val % 256);
                val /= 256;
            }
        }
        #endregion

        #region Modification couleur
        public void GrayScaleImage()
        {
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    this.imageMatrix[i, j] = new Pixel(imageMatrix[i, j].MoyennePix(), imageMatrix[i, j].MoyennePix(), imageMatrix[i, j].MoyennePix());

                }
            }
        }

        public void Black_And_White()
        {
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    Pixel p;
                    if (imageMatrix[i, j].MoyennePix() < 127)
                    {
                        p = new Pixel(0, 0, 0);
                    }
                    else
                    {
                        p = new Pixel(255, 255, 255);
                    }
                    this.imageMatrix[i, j] = p;
                }
            }
        }
        #endregion

        #region Miroirs

        public void VerticalMirror_RL()
        {

            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    this.imageMatrix[i, j] = imageMatrix[i, width - 1 - j];
                }
            }

        }

        public void VerticalMirror_LR()
        {

            for (int i = 0; i < this.height; i++)
            {
                for (int j = width - 1; j > 0; j--)
                {
                    this.imageMatrix[i, j] = imageMatrix[i, width - 1 - j];
                }
            }
        }

        public void HorizontalMirror_UD()
        {
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    this.imageMatrix[i, j] = imageMatrix[height - 1 - i, j];
                }
            }
        }

        public void HorizontalMirror_DU()
        {
            for (int i = height - 1; i > 0; i--)
            {
                for (int j = 0; j < this.width; j++)
                {
                    this.imageMatrix[i, j] = imageMatrix[height - 1 - i, j];
                }
            }
        }

        #endregion

        #region Redimensionnement
        /// <summary>
        /// Agrandissement de l'image
        /// </summary>
        /// <param name="coeff">Coefficient d'agrandissement</param>
        public void Enlarge(int coeff)
        {
            if (coeff > 1)
            {
                // Nouvelle matrice agrandie par le coefficient
                // Pas de retranchement pour un multiple de 4 car on part du principe que l'image de base est de multiple 4
                Pixel[,] enlargeMatrix = new Pixel[height * coeff, width * coeff];

                // On multiple les coordonnées par le coefficient
                // Chaque pixel sera représenté par une matrice de la taille du coefficient d'agrandissement 
                for (int i = 0; i < this.height; i++)
                {
                    for (int j = 0; j < this.width; j++)
                    {
                        for (int k = 0; k < coeff; k++)
                        {
                            for (int l = 0; l < coeff; l++)
                            {
                                enlargeMatrix[i * coeff + k, j * coeff + l] = this.imageMatrix[i, j];
                            }
                        }
                    }

                    this.width *= coeff;
                    this.height *= coeff;
                    this.imageMatrix = enlargeMatrix;
                }
            }
            else Console.WriteLine("Le coefficient d'agrandissement doit être supérieur à 1");
            
        }


        /// <summary>
        /// Permet de faire un rétrecissement de l'image
        /// </summary>
        /// <param name="coeff">Coefficient de réduction</param>
        public void ReduceSelect(int coeff)
        {
            if (coeff > 1)
            {
                // On modifie la taille de la matrice de pixel de sorte à ce qu'on ait toujours un multiple de 4
                this.width /= coeff;
                this.width -= this.width % 4;
                this.height /= coeff;
                this.height -= this.height % 4;

                Pixel[,] reduceMatrix = new Pixel[this.height, this.width];

                // On remplit la matrice
                for (int i = 0; i < this.height; i++)
                {
                    for (int j = 0; j < this.width; j++)
                    {

                        reduceMatrix[i, j] = imageMatrix[i * coeff, j * coeff];
                    }
                }

                this.imageMatrix = reduceMatrix;
            }

        }
        #endregion

        /// <summary>
        /// Permet d'effectuer une rotation de l'image
        /// </summary>
        /// <param name="degAngle">Angle de rotation en degrés</param>
        public void Rotate(int degAngle)
        {

            // Angle en radian
            double radAngle = (degAngle * 2 * Math.PI) / 360;

            // La taille du nouveau tableau est égal à la norme de la longueur et la largeur
            // Il ne peut pas y avoir de coordonnées d'indice plus grand que la norme
            // On n'oublie pas de ramener la taille du tableau à un multiple de 4
            int newSize = (int)(Math.Sqrt(this.width * this.width + this.height * this.height));
            newSize += 4 - (newSize % 4);
            //int radius = (int)(Math.Sqrt(2)/2 * Math.Sqrt(this.width * this.width + this.height * this.height));

            // Matrice de l'image après rotation
            Pixel[,] RotImage = new Pixel[newSize, newSize];

            // Coordonnées de l'origine
            int MidX = height / 2;
            int MidY = width / 2;


            for (int i = 0; i < newSize; i++)
            {
                for (int j = 0; j < newSize; j++)
                {

                    // On applique une matrice de rotation (transformation en coordonnées polaires)
                    int indexH = (int)((i - newSize / 2) * Math.Cos(radAngle) + (j - newSize / 2) * Math.Sin(radAngle) + MidX);
                    int indexW = (int)((j - newSize / 2) * Math.Cos(radAngle) - (i - newSize / 2) * Math.Sin(radAngle) + MidY);

                    // Si la position du pixel n'est pas une coordonnée de la nouvelle image, on le met en blanc
                    if (indexH > 0 && indexH < this.height && indexW > 0 && indexW < this.width)
                    {
                        RotImage[i, j] = imageMatrix[indexH, indexW];
                    }
                    else RotImage[i, j] = new Pixel(255, 255, 255);
                }
            }

            // On modifie la mtrice de l'image et ses dimensions
            this.imageMatrix = RotImage;
            this.height = newSize;
            this.width = newSize;
        }

        #region Filtres
        /// <summary>
        /// Application de filtres grâce à des matrices de convolution
        /// </summary>
        /// <param name="choice">Choix du filtre</param>
        public void Filters(int choice)
        {
            int[,] contourLeger = { { 1, 0, -1 }, { 0, 0, 0 }, { -1, 0, 1 } };
            int[,] contourMoyen = { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } };
            int[,] contourFort = { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
            int[,] renforcement = { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } };
            int[,] flou = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
            int[,] repoussage = { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } };

            switch (choice)
            {
                case 1:
                    Console.Clear();
                    Convolution(contourLeger, false);
                    Console.WriteLine("Le filtre de contour léger a été appliqué");

                    break;
                case 2:
                    Console.Clear();
                    Convolution(contourMoyen, false);
                    Console.WriteLine("Le filtre de contour moyen a été appliqué");
                    break;
                case 3:
                    Console.Clear();
                    Convolution(contourFort, false);
                    Console.WriteLine("Le filtre de contour fort a été appliqué");
                    break;
                case 4:
                    Console.Clear();
                    Convolution(renforcement, false);
                    Console.WriteLine("Le filtre de rendorcement a été appliqué");
                    break;
                case 5:
                    Console.Clear();
                    Convolution(flou, true);
                    Console.WriteLine("Le filtre de flou a été appliqué");
                    break;
                case 6:
                    Console.Clear();
                    Convolution(repoussage, false);
                    Console.WriteLine("Le filtre de repoussage a été appliqué");
                    break;

            }


        }

        /// <summary>
        /// Applique une matrice de convolution à l'image
        /// </summary>
        /// <param name="mat">Matrice de convolution</param>
        public void Convolution(int[,] mat, bool flou)
        {
            // Code très moche mais qui marche (pardon) 
            // Faire un code optimisé avec 4 boucle pour créait un décalage au niveau des index
            // Pas le temps de l'améliorer plus

            // Nouvelles composantes RGB
            int newRed = 0;
            int newGreen = 0;
            int newBlue = 0;

            // Matrice après convolution
            Pixel[,] afterConvo = new Pixel[this.height, this.width];

            // Matrice temporaire pour pallier aux dépassements
            Pixel[,] tmp = new Pixel[this.height + 2, this.width + 2];

            // On duplique les contours dans la matrice temp
            for (int i = 1; i < this.width + 1; i++)
            {
                tmp[0, i] = imageMatrix[0, i - 1];
            }

            for (int i = 1; i < this.height + 1; i++)
            {
                tmp[i, 0] = imageMatrix[i - 1, 0];
            }

            for (int i = 1; i < this.width + 1; i++)
            {
                tmp[this.height + 1, i] = imageMatrix[this.height - 1, i - 1];
            }

            for (int i = 1; i < this.height + 1; i++)
            {
                tmp[i, this.width + 1] = imageMatrix[i - 1, this.width - 1];
            }

            // On place la matrice initiale dans la temp
            for (int i = 1; i < this.height + 1; i++)
            {
                for (int j = 1; j < this.width + 1; j++)
                {
                    tmp[i, j] = this.imageMatrix[i - 1, j - 1];
                }
            }

            // On duplique les coins dans la temp
            tmp[0, 0] = this.imageMatrix[0, 0];
            tmp[0, width + 1] = this.imageMatrix[0, width - 1];
            tmp[height + 1, width + 1] = this.imageMatrix[height - 1, width - 1];
            tmp[height + 1, 0] = this.imageMatrix[height - 1, 0];


            // CONVOLUTION 

            for (int i = 1; i < tmp.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < tmp.GetLength(1) - 1; j++)
                {

                    newRed = tmp[i - 1, j - 1].B * mat[0, 0] + tmp[i - 1, j].B * mat[0, 1] + tmp[i - 1, j + 1].B * mat[0, 2] +
                    tmp[i, j - 1].B * mat[1, 0] + tmp[i, j].B * mat[1, 1] + tmp[i, j + 1].B * mat[1, 2] +
                        tmp[i + 1, j - 1].B * mat[2, 0] + tmp[i + 1, j].B * mat[2, 1] + tmp[i + 1, j + 1].B * mat[2, 2];


                    newGreen = tmp[i - 1, j - 1].G * mat[0, 0] + tmp[i - 1, j].G * mat[0, 1] + tmp[i - 1, j + 1].G * mat[0, 2] +
                                     tmp[i, j - 1].G * mat[1, 0] + tmp[i, j].G * mat[1, 1] + tmp[i, j + 1].G * mat[1, 2] +
                                     tmp[i + 1, j - 1].G * mat[2, 0] + tmp[i + 1, j].G * mat[2, 1] + tmp[i + 1, j + 1].G * mat[2, 2];


                    newBlue = tmp[i - 1, j - 1].R * mat[0, 0] + tmp[i - 1, j].R * mat[0, 1] + tmp[i - 1, j + 1].R * mat[0, 2] +
                                    tmp[i, j - 1].R * mat[1, 0] + tmp[i, j].R * mat[1, 1] + tmp[i, j + 1].R * mat[1, 2] +
                                    tmp[i + 1, j - 1].R * mat[2, 0] + tmp[i + 1, j].R * mat[2, 1] + tmp[i + 1, j + 1].R * mat[2, 2];


                    if (flou)
                    {
                        newRed /= 9;
                        newGreen /= 9;
                        newBlue /= 9;
                    }

                    // On borne les valeurs

                    newRed = newRed > 255 ? 255 : newRed;
                    newRed = newRed < 0 ? 0 : newRed;

                    newGreen = newGreen > 255 ? 255 : newGreen;
                    newGreen = newGreen < 0 ? 0 : newGreen;

                    newBlue = newBlue > 255 ? 255 : newBlue;
                    newBlue = newBlue < 0 ? 0 : newBlue;

                    // Remplissage de la nouvelle matrice
                    afterConvo[i - 1, j - 1] = new Pixel((byte)(newBlue), (byte)(newGreen), (byte)(newRed));

                }
            }

            this.imageMatrix = afterConvo;


        }
        #endregion

        #region Fractale
        public MyImage Fractale(int choixFractale, int choixCouleur)
        {         
            MyImage fractale = new MyImage("tigre.bmp");
            //SETTINGS pour le cadrage de l'image
            double x1;
            double x2;
            double y1;
            double y2;

            if (choixFractale == 1)
            {
                 x1 = -2.1;
                 x2 = 0.6;
                 y1 = -1.2;
                 y2 = 1.2;
            }
            else
            {
                 x1 = -1;
                 x2 = 1;
                 y1 = -1.2;
                 y2 = 1.2;
            }
            int zoom = 1000; //zoom idéal pour l'affichage de l'image

            int iterationMax = 100;

            int image_x = Convert.ToInt32((x2 - x1) * zoom);
            int image_y = Convert.ToInt32((y2 - y1) * zoom);
            while (image_y % 4 != 0 || image_x % 4 != 0)
            {
                if (image_x % 4 != 0)
                {
                    image_x++;
                }
                if (image_y % 4 != 0)
                {
                    image_y++;
                }
            }
            fractale.height = image_x; 
            fractale.width = image_y;
            fractale.size = fractale.height * fractale.width * 3 + 54;

            fractale.imageMatrix = new Pixel[image_x, image_y];
            Pixel noir = new Pixel(0, 0, 0);
            Pixel blanc = new Pixel(255, 255, 255);

            for (int x=0; x<image_x;x++)
            { 
                for (int y=0; y<image_y; y++)
                {
                    double val_x = x;
                    double val_y = y;
                    byte i = 0;

                    if (choixFractale == 1)
                    {
                        Complex c = new Complex(val_x / zoom + x1, val_y / zoom + y1);
                        Complex z = new Complex(0, 0);
                        /*-0.70176; -0.3842; */
                        

                        do
                        {
                            i = (byte)(i + 1);
                            z.Square();
                            z.Add(c);
                        } while (z.Test() < 4 && i < iterationMax);
                    }
                    else if (choixFractale == 2) //JULIA SET 1
                    {
                        Complex c = new Complex(val_x / zoom + x1, val_y / zoom + y1);
                        Complex z = new Complex(-0.4, 0.6);
                        do
                        {
                            i = (byte)(i + 1);
                            c.Square();
                            c.Add(z);
                        } while (c.Test() < 4 && i < iterationMax);
                    }
                    else  //JULIA SET 2
                    {
                        Complex c = new Complex(val_x / zoom + x1, val_y / zoom + y1);
                        Complex z = new Complex(-0.70176, -0.3842);
                        do
                        {
                            i = (byte)(i + 1);
                            c.Square();
                            c.Add(z);
                        } while (c.Test() < 4 && i < iterationMax);
                    }



                    if (i == iterationMax)
                    {
                        fractale.imageMatrix[x, y] = noir; //new Pixel(255,255,255) pour du blanc;
                    }
                    else
                    {
                        //set par défaut
                        Pixel couleur = new Pixel(Convert.ToByte((Math.Sqrt(i * 655))), Convert.ToByte((Math.Sqrt(i * 655))), Convert.ToByte((Math.Sqrt(i * 655))));

                        if (choixCouleur == 2) //blue set
                        {
                            couleur = new Pixel(Convert.ToByte((Math.Sqrt(i * 655))), 0, 0);
                        }
                        if (choixCouleur == 3) //green set
                        {
                            couleur = new Pixel(0, Convert.ToByte((Math.Sqrt(i * 655))), 0);
                        }
                        if (choixCouleur == 4) //red set
                        {
                            couleur = new Pixel(0, 0, Convert.ToByte((Math.Sqrt(i * 655))));
                        }
                        if (choixCouleur==5)
                        {
                            if (i <= 20) //blue set
                            {
                                couleur = new Pixel(Convert.ToByte((Math.Sqrt(i * 655))), 0, 0);
                            }
                            else if (20 <= i && i <= 50) //green set
                            {
                                couleur = new Pixel(0, Convert.ToByte((Math.Sqrt(i * 655))), 0);
                            }
                            else  //red set
                            {
                                couleur = new Pixel(0, 0, Convert.ToByte((Math.Sqrt(i * 655))));
                            }
                        }
                    
                        fractale.imageMatrix[x, y] = couleur;
                    }
                    
                }
                
            }
            return fractale;
        }
        #endregion

        #region Histogramme
        /// <summary>
        /// Creates an histogragram of the picture
        /// </summary>
        public void Histo()
        {
            // Nombre d'occurence de chaque valeur de pixel
            int[] maxR = new int[256];
            int[] maxG = new int[256];
            int[] maxB = new int[256];
            int max=0;

            // Compte le nombre d'occurence de chaque valeur de pixel
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {

                    maxR[this.imageMatrix[i, j].R]++;
                    maxG[this.imageMatrix[i, j].G]++;
                    maxB[this.imageMatrix[i, j].B]++;
                }
            }
            for (int j = 0; j < 256; j++)
            {
                if (maxR[j] > max) max = maxR[j];
                if (maxG[j] > max) max = maxG[j];
                if (maxB[j] > max) max = maxB[j];
            }

            // Remplit l'histogramme de pixels noirs
            Pixel[,] histo = new Pixel[100, 256];
            for (int i = 0; i < histo.GetLength(0); i++)
            {
                for (int j = 0; j < histo.GetLength(1); j++)
                {
                    histo[i, j] = new Pixel(0, 0, 0);
                }
            }

            for (int i = 0; i < 256; i++)
            {

                        for(int j = 0; j < maxR[i]*100/max; j++)
                        {
                            histo[j,i] = new Pixel(255, 0, 0);
                        }

                        for (int j = 0; j < maxG[i] * 100 / max; j++)
                        {
                    if (histo[j, i].R == 0)
                    {
                        histo[j, i] = new Pixel(0, 255, 0);
                    } else
                    {
                        histo[j, i] = new Pixel(255, 100, 0);
                    }
                            
                        }

                        for (int j = 0; j < maxB[i] * 100 / max; j++)
                        {
                    if (histo[j, i].R == 0 & histo[j,i].G==0)
                    {
                        histo[j, i] = new Pixel(0, 0, 255);
                    }
                    else if(histo[j,i].R > 0 & histo[j,i].G == 0)
                    {
                        histo[j, i] = new Pixel(255, 0, 100);
                    } else if (histo[j, i].R == 0 & histo[j, i].G > 0)
                    {
                        histo[j, i] = new Pixel(0, 255, 100);
                    }
                }
               
            }
          
            this.height = histo.GetLength(0);
            this.width = histo.GetLength(1);
            this.imageMatrix = histo;
        }
        #endregion

        #region Hide/Show
        /// <summary>
        /// Hide a picture
        /// </summary>
        /// <param name="toHide"></param>
        public void Hide(MyImage toHide)
        {
            byte tmp;
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    tmp = (byte)((toHide.ImageMatrix[i,j].R & 0b_1111_0000) >> 4);
                    this.imageMatrix[i,j].R= (byte)((this.imageMatrix[i, j].R & 0b_1111_0000) | tmp);
                    tmp = (byte)((toHide.ImageMatrix[i, j].G & 0b_1111_0000) >> 4);
                    this.imageMatrix[i, j].G = (byte)((this.imageMatrix[i, j].G & 0b_1111_0000) | tmp);
                    tmp = (byte)((toHide.ImageMatrix[i, j].B & 0b_1111_0000) >> 4);
                    this.imageMatrix[i, j].B = (byte)((this.imageMatrix[i, j].B & 0b_1111_0000) | tmp);

                }
            }
            
            
        }

        /// <summary>
        /// Shows the hidden picture
        /// </summary>
        public void Show()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    this.imageMatrix[i,j].R = (byte)((this.imageMatrix[i, j].R & 0b_0000_1111) << 4);
                    this.imageMatrix[i, j].G = (byte)((this.imageMatrix[i, j].G & 0b_0000_1111) << 4);
                    this.imageMatrix[i, j].B = (byte)((this.imageMatrix[i, j].B & 0b_0000_1111) << 4);

                }
            }

        }
        #endregion







    }

}
