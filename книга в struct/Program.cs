using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace книга_в_struct
{
    internal class Program
    {
        struct Book
        {
            string name;
            int pages;
            string author;
            bool illustrations;

            public Book (string name, int pages, string author, bool illustrations)
            {              
                this.name = name;
                this.pages = pages;
                this.author = author;
                this.illustrations = illustrations;
                Info();
            }
            void Info()
            {
                Console.WriteLine($"имя: {name} \nавтор: {author} \nиллюстрации: {illustrations} \nстраницы: {pages}", name, author, illustrations, pages );
            }  
        }
        static void Main(string[] args)
        {
            Console.Write("Введите имя: ");
            String name= Console.ReadLine();
            Console.Clear();
            Console.Write("Введите кол-во страниц: ");
            int pages = int.Parse(Console.ReadLine());
            Console.Clear();
            Console.Write("Введите имя автора: ");
            String author = Console.ReadLine();
            Console.Clear();
            Console.Write("Введите есть илюстрации или нет: ");
            bool illustrations = bool.Parse(Console.ReadLine());
            Console.Clear();

            Book book1 = new Book(name, pages, author, illustrations);
            Console.ReadKey();
        }
    }
}
