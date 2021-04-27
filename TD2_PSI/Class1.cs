using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TD2_PSI
{
    class QRCode
    {
        int[] data_alpha;
        List<bool> data_encoded= new List<bool>();

        bool[] b = { true, true, true, false, true, true, false, false, false, false, false, true, false, false, false, true };

        public QRCode(string toEncode)
        {
            this.data_alpha = new int[toEncode.Length];
            // mode
            this.data_encoded.Add(false);
            this.data_encoded.Add(false);
            this.data_encoded.Add(true);
            this.data_encoded.Add(false);
            // longueur chaine
            for (int i = 0; i < 9; i++)
            {
                this.data_encoded.Add(ConvertToBinaryArray(9, toEncode.Length)[i]);
            }
            Encode(toEncode);
        }

        void Encode(string toEncode)
        {
            char[] charToEncode = toEncode.ToCharArray();
            for (int i = 0; i < charToEncode.Length; i++)
            {
                if (IsAlpha(charToEncode[i]))
                {
                    for (int j = 65; j <= 90; j++)
                    {
                        if (charToEncode[i] == j) data_alpha[i] = charToEncode[i] - 55;
                        
                    }

                    for (int j = 30; j <= 39; j++)
                    {
                        if (charToEncode[i] == j) data_alpha[i] = charToEncode[i] - 30;
                    }

                    if (charToEncode[i] == ' ') data_alpha[i] = 36;
                    if (charToEncode[i] == '$') data_alpha[i] = 37;
                    if (charToEncode[i] == '%') data_alpha[i] = 38;
                    if (charToEncode[i] == '*') data_alpha[i] = 39;
                    if (charToEncode[i] == '+') data_alpha[i] = 40;
                    if (charToEncode[i] == '-') data_alpha[i] = 41;
                    if (charToEncode[i] == '.') data_alpha[i] = 42;
                    if (charToEncode[i] == '/') data_alpha[i] = 43;
                    if (charToEncode[i] == ':') data_alpha[i] = 44;

                }
            }


            for (int i=0; i < Convert.ToInt32(toEncode.Length)-1; i=i+2)
            {
                for(int j = 0; j < 11; j++)
                {
                this.data_encoded.Add(ConvertToBinaryArray(11, 45 * data_alpha[i] + data_alpha[i + 1])[j]);
                }              
            }

          /*  List<int> test = new List<int>();
            for (int i = 0; i < Convert.ToInt32(toEncode.Length)-1; i=i+2)
            {
                for (int j = 0; j < 11; j++)
                {
                    test.Add(ConvertToBinaryArraybis(11, 45 * data_alpha[i] + data_alpha[i + 1])[j]);
            }
        }
            foreach (var i in test)
            {
                Console.Write(i);
                Console.Write(" ");
            }*/

            if (toEncode.Length % 2 != 0)
            {
                for (int j = 0; j < 6; j++)
                {
                    this.data_encoded.Add(ConvertToBinaryArray(6, data_alpha[toEncode.Length - 1])[j]);
                }
            }

            // Terminaison
            for (int i = 0; i < 4; i++)
            {
                if (data_encoded.Count < 152)
                {
                    data_encoded.Add(false);
                }
            }


            while (data_encoded.Count%8 != 0)
            {
                data_encoded.Add(false);
            }

            for (int i = data_encoded.Count; i < 152; i++)
            {
                data_encoded.Add(b[i % 16]);
            }


            byte[] byteArrayS = data_encoded.ToArray().Select(b => (byte)(b ? 1 : 0)).ToArray();
            byte[] byteArray = ConvertToByteArray(data_encoded.ToArray());
            // byte[] byteArray = { 32, 91, 11, 120, 209, 114, 220, 77, 67, 64, 236, 17, 236, 17, 236, 17, 236, 17, 236 };
            byte[] result = ReedSolomonAlgorithm.Encode(byteArray, 7, ErrorCorrectionCodeType.QRCode);
            //bool[] boolArray = result.Select(b => (bool)(Convert.To(b) ? true : false)).ToArray();
            string s = string.Join(" ", result.Select(x => Convert.ToString(x, 2).PadLeft(8, '0')));

              foreach(var i in result)
              {
                  Console.Write(i);
                Console.Write(" ");
              }
            /*foreach (var b in s)
            {
                data_encoded.Add(b=='1'?true:false);
            }*/
            Console.WriteLine(s);

            int k=1;
            foreach (var pers in byteArrayS)
            {
                Console.Write(pers);
                //Console.Write(pers ? 1:0);
                if (k == 8)
                {
                    Console.Write(" ");
                    k = 1;
                }else
                {
                    k++;
                }

                

            }

        }


        bool[] ConvertToBinaryArray(int arrayLength, int number)
        {
            bool[] binArray= new bool[arrayLength];
            int count = 1;

            for (int i = 0; number > 0; i++)
            {
                bool b = !(number % 2 == 0);
                binArray[arrayLength - count] = b;
                number /= 2;
                count++;
            }
            
            for(int i = count; i <= arrayLength; i++)
            {
                binArray[arrayLength - i] = false;
            }

            return binArray; 
        }

        int[] ConvertToBinaryArraybis(int arrayLength, int number)
        {
            int[] binArray = new int[arrayLength];
            int count = 1;

            for (int i = 0; number > 0; i++)
            {

                binArray[arrayLength - count] = (number % 2 == 0) ? 0 : 1 ;
                number /= 2;
                count++;
            }

            for (int i = count; i <= arrayLength; i++)
            {
                binArray[arrayLength - i] = 0;
            }

            return binArray;
        }

        byte[] ConvertToByteArray(bool[] boolArray)
        {
            byte[] byteArray = new byte[boolArray.Length / 8];
            string s = "";
            int count = 0;
            int m;
            for (int i = 0; i < boolArray.Length; i=i+8)
            {
                for (int j = 0; j < 8; j++)
                {
                    s = s + (boolArray[i + j] ? "1" : "0");

                }
                byte val = 0;
                //Console.WriteLine(s);
                for (int j = 0; j < 8; j++)
                {
                    m = (int)char.GetNumericValue(s[j]);
                    val += (byte)(m * Convert.ToInt32(Math.Pow(2, 7 - j)));
                }
                byteArray[count] = val;
                Console.Write(" "+val);
                s = "";
                count++;
            }

            Console.WriteLine();
            return byteArray;
        }

        public byte Bite_To_Octet(string tab)
        {
            byte val = 0;

            for (int i = 0; i < 8; i++)
            {
                val += (byte)(tab[i] * Convert.ToInt32(Math.Pow(2, 7 - i)));
            }
            return val;
        }

        bool IsAlpha(char tested_char)
        {
            bool isAlpha = false;

                for (int j = 65; j <= 90; j++)
                {
                    if (tested_char == j) isAlpha=true;
                }

                for (int j = 30; j <= 39; j++)
                {
                    if (tested_char == j) isAlpha = true ;
                }   

                if (tested_char == '$') isAlpha = true;
                if (tested_char == '%') isAlpha = true;
                if (tested_char == '*') isAlpha = true;
                if (tested_char == '+') isAlpha = true;
                if (tested_char == '-') isAlpha = true;
                if (tested_char == '.') isAlpha = true;
                if (tested_char == '/') isAlpha = true;
                if (tested_char == ':') isAlpha = true;

            return isAlpha;
 
        }

        
        /*
        public void AffichageDonne(string toEncode)
        {

            Console.WriteLine("Nous voulons coder le texte : " + toEncode +
                              "\nMode : " + //VERSION 1 OU VERSION 2, JSP SI YA UN TRUC POUR D2TERMINER CA
                              "\nIndicateur du mode sur 4 bits : 0010" + //Code de l'alphanumérique
                              "\nIndicateur du nombre de caractère sur 9 bits" +
                              "\n" +
                              "");
                             

        }*/
    }
}
