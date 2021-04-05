using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace TD2_PSI
{
    class Program
    {
        /// <summary>
        /// Menu
        /// </summary>
        static void Menu()
        {
            Console.Clear();
            Console.WriteLine("Informations sur l'image de test :\n\n\n"
                                 + "Remarque : les fonctions 1 à 4 peuvent prendre un certain temps à s'éxécuter en fonction de la taille de votre image\n"
                                 + "0 : Sélectionner l'image avec laquelle vous voulez travailler \n"
                                 + "1 : Toutes les informations contenues dans les métadonnées et les données de l'image \n"
                                 + "2 : Exporter l'image \n"
                                 + "3 : Métadonnées et données de l'image et du fichier de l'image au format brut \n"
                                 + "4 : Affichage de la matrice de l'image \n"
                                 + "5 : Passage d’une photo couleur à une photo en nuances de gris \n"
                                 + "6 : Passage d’une photo couleur à une photo  en noir et blanc  \n"
                                 + "7 : Agrandir une image\n"
                                 + "8 : Rétrécir une image\n"
                                 + "9 : Rotation\n"
                                 + "10 : Effet miroir\n"
                                 + "11 : Filtres \n"
                                 + "12 : Fractale TEST \n"
                                 + "13 : Histogramme \n"
                                 + "14 : Cacher/AFficher une image (morceau de lena) \n"
                                 + "Saisissez le nombre correspondant à l'option que vous désirez ");
        }

        static void MenuFiltres()
        {
            Console.Clear();
            Console.WriteLine("1 : Contour Léger \n"
                + "2 : Contour Moyen \n"
                + "3 : Contour Fort \n"
                + "4 : Renforcement \n" +
                "5 : Flou \n"
                + "6 : Repoussage \n");
        }

        static void LancementImage(string link)
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(@link)
            {
                UseShellExecute = true
            };
            p.Start();
        }


        static void Main(string[] args)
        {
            string nameImage = "coco.bmp";

            do
            {
                try
                {
                    Console.Clear();
                    Menu();


                    MyImage image = new MyImage(nameImage);
                    switch (Int32.Parse(Console.ReadLine()))
                    {
                        case 0:
                            Console.WriteLine("L'image par défaut est coco.bmp");
                            Console.WriteLine("Pour changer en Lena.bmp saisir 1 et pour changer en tigre.bmp tapez 2");
                            Console.WriteLine("Saisir un autre nombre pour rester sur coco.bmp");
                            int rep = Int32.Parse(Console.ReadLine());
                            nameImage = "coco.bmp";

                            if (rep == 1)
                            {
                                nameImage = "lena.bmp";
                            }
                            else if (rep == 2)
                            {
                                nameImage = "tigre.bmp";
                            }

                            break;
                        case 1: // Infos sur l'image 
                            Console.Clear();
                            Console.WriteLine(image.toString());
                            /*image.Histo();
                            image.From_Image_To_File("Histo");
                            LancementImage("Histo.bmp");/*
                            MyImage toHide = new MyImage("lena.bmp");
                            image.Hide(toHide);
                            image.From_Image_To_File("Hide");
                            LancementImage("Hide.bmp");
                            image.Show();
                            image.From_Image_To_File("Show");
                            LancementImage("Show.bmp");*/
                            break;
                        case 2: // Conversion des données en fichier 
                            Console.Clear();
                            Console.WriteLine("Saisissez le nom de votre fichier");
                            image.From_Image_To_File(Console.ReadLine());
                            break;

                        case 3: // Affichage du fichier sous forme d'un tableau de bits
                            Console.Clear();
                            Console.WriteLine("\n HEADER \n");
                            for (int i = 0; i < 14; i++)
                                Console.Write(image.DataImage[i] + " ");
                            Console.WriteLine("\n HEADER INFO \n");
                            for (int i = 14; i < 54; i++)
                                Console.Write(image.DataImage[i] + " ");
                            Console.WriteLine("\n IMAGE \n");
                            for (int i = 54; i < image.DataImage.Length; i = i + 60)
                            {
                                for (int j = i; j < i + 60; j++)
                                {
                                    Console.Write(image.DataImage[j] + " ");
                                }
                                Console.WriteLine();
                            }
                            break;

                        case 4: // Affichage de la matrice de l'image
                            Console.Clear();
                            int count = 0;
                            foreach (Pixel rgb in image.ImageMatrix)
                            {
                                count++;
                                Console.Write(rgb.ToString() + " ");
                                if (count == image.Width)
                                {
                                    Console.Write("\n");
                                    count = 0;
                                }
                            }
                            break;

                        case 5: //Gray scale
                            Console.Clear();
                            MyImage imageEnGris = new MyImage(nameImage);
                            imageEnGris.GrayScaleImage();
                            imageEnGris.From_Image_To_File("Gris");
                            LancementImage("Gris.bmp");
                            break;

                        case 6: //Black and white
                            Console.Clear();
                            MyImage futurNoirEtBlanc = new MyImage(nameImage);
                            futurNoirEtBlanc.Black_And_White();
                            futurNoirEtBlanc.From_Image_To_File("NoirEtBlanc");
                            LancementImage("NoirEtBlanc.bmp");
                            break;

                        case 7:
                            //Agrandir une image
                            Console.Clear();
                            MyImage imageAAgrandir = new MyImage(nameImage);
                            Console.WriteLine("Sélectionner le coefficient d'aggrandissement de l'image souhaité (entier naturel)");
                            int a;
                            try { a = Convert.ToInt32(Console.ReadLine()); }
                            catch { a = 1; }
                            imageAAgrandir.Enlarge(a);
                            imageAAgrandir.From_Image_To_File("Agrandissement");
                            LancementImage("Agrandissement.bmp");
                            break;

                        case 8:
                            //Rétrécir une image
                            Console.Clear();
                            MyImage imageARetrecir = new MyImage(nameImage);
                            Console.WriteLine("Sélectionner le coefficient de rétrécissement de l'image souhaité (max 10)");
                            int b;
                            try { b = Convert.ToInt32(Console.ReadLine()); }
                            catch { b = 1; }
                            if (b <= 10)
                            {
                                imageARetrecir.ReduceSelect(b);
                                imageARetrecir.From_Image_To_File("Reduction");
                                LancementImage("Reduction.bmp");
                            }
                            break;

                        case 9:
                            //Rotation image
                            Console.Clear();
                            MyImage imageRotation = new MyImage(nameImage);
                            Console.WriteLine("Veuillez saisir l'angle de rotation souhaité en degrés");
                            int angle = Convert.ToInt32(Console.ReadLine());
                            if (angle < 361 && angle > 0)
                            {
                                imageRotation.Rotate(angle);
                                imageRotation.From_Image_To_File("Rotation");
                                LancementImage("Rotation.bmp");
                            }
                            break;

                        case 10:
                            Console.Clear();
                            Console.WriteLine("Veuillez saisir le numéro correspondant au type de miroir appliqué : \n"
                                     + "1 : Mirroir Droite-Gauche\n"
                                     + "2 : Mirroir Gauche-Droite\n"
                                     + "3 : Mirroir Haut-Bas\n"
                                     + "4 : Mirroir Bas-Haut\n");

                            int nb = Convert.ToInt32(Console.ReadLine());
                            MyImage imageMiroir = new MyImage(nameImage);
                            switch (nb)
                            {
                                case 1:
                                    imageMiroir.VerticalMirror_RL();
                                    imageMiroir.From_Image_To_File("Mirroir_R_L_");
                                    LancementImage("Mirroir_R_L_.bmp");
                                    break;
                                case 2:
                                    imageMiroir.VerticalMirror_LR();
                                    imageMiroir.From_Image_To_File("Mirroir_L_R_");
                                    LancementImage("Mirroir_L_R_.bmp");
                                    break;
                                case 3:
                                    imageMiroir.HorizontalMirror_UD();
                                    imageMiroir.From_Image_To_File("Mirroir_U_D_");
                                    LancementImage("Mirroir_U_D_.bmp");
                                    break;
                                case 4:
                                    imageMiroir.HorizontalMirror_DU();
                                    imageMiroir.From_Image_To_File("Mirroir_D_U_");
                                    LancementImage("Mirroir_D_U_.bmp");
                                    break;
                                default:
                                    Console.WriteLine("Il n'y a pas d'options correspondant à ce nombre");
                                    break;
                            }
                            break;


                        case 11:
                            // Filtres 
                            MenuFiltres();
                            int answer = Int32.Parse(Console.ReadLine());
                            if (answer >= 1 && answer < 7)
                            {
                                Console.Clear();
                                MyImage filtre = new MyImage(nameImage);
                                filtre.Filters(answer);
                                filtre.From_Image_To_File("Filtre");
                                LancementImage("Filtre.bmp");
                            }
                            else
                            {
                                Console.WriteLine("Saisissez un nombre entre 1 et 6");
                            }
                            break;

                        case 12: //TEST AUTRE FRACTALE
                            Console.Clear();
                            Console.WriteLine("Veuillez saisir le numéro correspondant au type de fractale voulue : \n"
                                    + "1 : fractale de Mandelbrot \n"
                                    + "2 : fractale de Julia V1\n"
                                    + "3 : fractale de Julia V2\n\n"
                                    + "Recommandation : fractale de Julia V1 en mix de couleurs; fractale de Julia V2 en noir et blanc\n");
                            int choix = 1;
                            try
                            {
                                choix = Convert.ToInt32(Console.ReadLine());
                            }
                            catch { Console.WriteLine("votre saisie n'étant pas valide, la fractale de Mandelbrot sera affichée par défaut"); }
                            if (choix != 1 && choix != 2 && choix != 3)
                            {
                                choix = 1;
                                Console.WriteLine("votre saisie n'étant pas valide, la fractale de Mandelbrot sera affichée par défaut");
                            }

                            Console.WriteLine("Veuillez saisir le numéro correspondant à la couleur associée à votre fractale : \n"
                                    + "1 : noir et blanc \n"
                                    + "2 : bleu\n"
                                    + "3 : vert\n"
                                    + "4 : rouge\n"
                                    + "5 : mix des couleurs\n");

                            int choixCouleur = 1;
                            try
                            {
                                choixCouleur = Convert.ToInt32(Console.ReadLine());
                            }
                            catch { Console.WriteLine("votre saisie n'étant pas valide, la couleur noir-blanc sera attribué par défaut"); }
                            if (choixCouleur != 1 && choixCouleur != 2 && choixCouleur != 3 && choixCouleur != 4 && choixCouleur != 5)
                            {
                                choixCouleur = 1;
                                Console.WriteLine("votre saisie n'étant pas valide, la couleur noir-blanc sera attribué par défaut");
                            }

                            MyImage fractale3 = new MyImage("tigre.bmp");
                            MyImage FRACTALE = fractale3.Fractale(choix, choixCouleur);
                            FRACTALE.From_Image_To_File("FractaleTESTTTT");
                            LancementImage("FractaleTESTTTT.bmp");

                            break;

                        case 13:

                            Console.Clear();
                            Console.WriteLine("Voici l'histogramme de " + nameImage);
                            image.Histo();
                            image.From_Image_To_File("Histo");
                            LancementImage("Histo.bmp");
                            break;                               


                        case 14:

                            MyImage toHide = new MyImage("lena.bmp");
                            image.Hide(toHide);
                            Console.WriteLine("l'image" + nameImage + "a été caché dans cette image");
                            image.From_Image_To_File("Hide");                           
                            LancementImage("Hide.bmp");
                            image.Show();
                            Console.WriteLine("à partir de cette image, on va retrouvé " + nameImage);
                            image.From_Image_To_File("Show");
                            LancementImage("Show.bmp"); 


                            break;
                   


                        default:
                            Console.WriteLine("Il n'y a pas d'options correspondant à ce nombre");
                            break;
                    }
                    Console.ReadKey();
                    Console.Clear();
                    Console.WriteLine("Tapez escape pour sortir ou sur une autre touche pour retourner au menu principal");


                }

                // IO Exception lors de la saisie
                catch (Exception e)
                {
                    Console.Clear();
                    Console.WriteLine(" Erreur de saisie, veuillez saisir ce qui vous est demandé ");
                    Console.WriteLine("Détails de l'exception : \n" + e);
                }

            } while (Console.ReadKey().Key != ConsoleKey.Escape);





        }


    }
}





