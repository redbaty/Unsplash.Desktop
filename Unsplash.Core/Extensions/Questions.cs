using System;
using System.Drawing;
using Console = Colorful.Console;

namespace Unsplash.Core.Extensions
{
    public static class Questions
    {
        public static int AskIntMessage(string message, Color color)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine(message, color);
                    var stringinput = Console.ReadLine();
                    return Convert.ToInt32(stringinput);
                }
                catch
                {
                    Console.WriteLine("> Couldn't convert the input to int (Only Numbers)", Color.Crimson);
                }
            }
        }

        public static string AskQuestion(string question) => AskQuestion(question, Color.Gray);

        public static string AskQuestion(string question, Color color)
        {
            Console.Write(question, color);
            return Console.ReadLine();
        }

        public static bool AskBoolQuestion(string question) => AskBoolQuestion(question, Color.Gray);

        public static bool AskBoolQuestion(string question, Color color)
        {
            while (true)
            {
                Console.Write(question, color);
                var response = Console.ReadLine();
                if (response.ToLower() == "y")
                    return true;
                if (response.ToLower() == "n")
                    return false;
                Console.WriteLine("Only acceptable answers are (Y)es or (N)o");
            }
        }
    }
}