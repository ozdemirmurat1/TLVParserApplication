namespace TLVParserApp
{
    // UYGULAMA .NET8 e YÜKSELTİLDİ....
    // GETLENGTH GETTAG METOTLARINA TRY EXCEPTİON EKLENECEK.....
    // GENEL TEST EDİLECEK , TRY EXCEPTİON MEKANİZMALARI

    internal class Program
    {

        static List<TagList> tagListValues = new List<TagList>();
        static void Main(string[] args)
        {
            bool repeat = true;

            while (repeat)
            {
                Console.Write("Enter value: ");
                var readHexadecimalValue = Console.ReadLine()!.Replace(" ", "");

                RecursiveLoopTagValues(readHexadecimalValue);

                foreach (var tag in tagListValues)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Tag: {tag.TagValue}, Length: {tag.HexadecimalTagValue} , TagName: {tag.TagName}, Value: {tag.DecimalTagValue}");
                    Console.ResetColor();
                }

                Console.Write("Do you want to enter another value? (Y/N): ");
                string response = Console.ReadLine()?.Trim().ToUpper();

                if (response != "Y")
                {
                    repeat = false;
                }
                else
                {
                    tagListValues.Clear();
                }
            }



        }

        // LENGTH'I HEXADECİMAL FORMAT TA DA YAZDIR. PROGRAM DECİMAL YAZDIRIYOR
        static void RecursiveLoopTagValues(string h)
        {
            int index = 0;
            while (index < h.Length)
            {
                string tag = GetTag(h, ref index);
                int length = (int)GetLength(h, ref index);
                string hexLengthValue = DecimalToHex(length);
                string value = h.Substring(index, length * 2);
                index += length * 2;

                tagListValues.Add(new TagList
                {
                    TagName = EMVFieldNames.emvTagNames
                            .FirstOrDefault(x => x.Key.Equals(tag))
                            .Value ?? "Unknown Tag",
                    TagValue = tag,
                    TagLentghValue = length.ToString(),
                    HexadecimalTagValue = hexLengthValue,
                    DecimalTagValue = value,
                });

                if (IsConstructedTag(tag))
                {
                    RecursiveLoopTagValues(value);
                }
            }
        }

        // ÖNEMLİ NOTLAR
        // & bit operatörü bit düzeyinde işlem gerçekleştirir. İlk 2 biti karşılaştır. AND Operatörü
        // void RecursiveLoopTagValues metodundaki index değişkeni GetTag metoduna  ref anahtar sözcüğüyle değerini atar.
        static string GetTag(string h, ref int index)
        {
            try
            {
                string tag = h.Substring(index, 2);
                index += 2;


                // 0x1F 00011111 bit decimal 31

                if ((Convert.ToInt32(tag, 16) & 0x1F) == 0x1F)
                {
                    tag += h.Substring(index, 2);
                    index += 2;
                }

                return tag;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }

        }

        // void RecursiveLoopTagValues metodundaki index değişkeni GetLength metoduna  ref anahtar sözcüğüyle değerini atar.
        static object GetLength(string h, ref int index)
        {
            try
            {
                int length = Convert.ToInt32(h.Substring(index, 2), 16);
                index += 2;

                // 0x80 hexadecimal sayısı, ikili tabanda 10000000'a eşittir.(128 decimal) en sol bit i kontrol eder
                // 0x7F  01111111

                if ((length & 0x80) != 0)
                {
                    int numberOfLengthBytes = length & 0x7F;
                    length = Convert.ToInt32(h.Substring(index, numberOfLengthBytes * 2), 16);
                    index += numberOfLengthBytes * 2;
                }

                return length;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }

        }

        static bool IsConstructedTag(string tag)
        {
            try
            {
                // Constructed kontrolü yapılır. Bit 6 
                //0x20(yani 32 ondalık veya 00100000 ikilik)

                int firstByte = Convert.ToInt32(tag.Length > 0 ? tag.Substring(0, 2) : "00", 16);
                bool control = (firstByte & 0x20) == 0x20;
                return control;
            }
            catch (Exception)
            {
                return false;
            }
        }

        static string DecimalToHex(int decimalValue)
        {
            try
            {
                return Convert.ToString(decimalValue, 16).ToUpper().PadLeft(2, '0');

            }
            catch (Exception)
            {
                return "";
            }

        }



    }
}