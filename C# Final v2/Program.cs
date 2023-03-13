using C__Final_v2;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace C__Final
{
    internal class Program
    {
        static string? PROJECT_PATH { get; } = Path.GetDirectoryName(
            System.Reflection.Assembly.GetExecutingAssembly().Location
        );
        static DirectoryInfo Dictionaries { get; } = new DirectoryInfo(PROJECT_PATH + @"\Dictionaries");
        static string DicExtension = ".dic";
        static void DicsCheck()
        {
            if (!Dictionaries.Exists)
            {
                if (new Menu.YesOrNo(
                    "Для работы программы нужна директория " +
                    "\"Dictionaries\".Создать автоматически?"
                ).GetChoice()) Dictionaries.Create();
                else throw new Exception("Работа програмы без \"Dictionaries\" не возможна.");
            }
        }
        static void Main(string[] args)
        {
            //Отрыть папку
            try { DicsCheck(); }
            catch (Exception e)
            {
                new Menu.Msg(e.Message).Print();
                return;
            }

            var MainMenu = new Menu.Dropdown("Главное меню:",
                "Перевести",
                "Редактировать",
                "Выход"
            );

            while (true)
                try
                {
                    switch (MainMenu.GetChoice())
                    {
                        case 0: //Перевести
                            {
                                //Выбор словаря и ввод слова для перевода
                                int choice = ChoiseDic();
                                if (choice == -1) break;

                                string find_word = new Menu.InoutStr(
                                    "Введите слово на языке: "
                                ).GetChoice().Trim().ToLower();

                                //Поиск в файле и вывод
                                var dic = new LanguageDictionary();
                                dic.Load(Dictionaries.GetFiles()[choice].FullName);

                                if (new Menu.YesOrNo(
                                    $"Переводы слова \"{find_word}\":\n\t" +
                                    $"{String.Join("\n\t", dic.Translations[find_word])}" +
                                    $"\nЭкспортировать в текстовый файл?"
                                ).GetChoice())
                                {
                                    dic.Export(find_word, PROJECT_PATH + @"/export" + DicExtension);
                                }
                                break;
                            }

                        case 1: //Редактировать
                            {
                                Edit();
                                break;
                            }
                        case 2: return; //Выход
                    }
                }
                catch (Exception e)
                {
                    new Menu.Msg($"Ошибка: {e.Message}").Print();
                }
        }
        static int ChoiseDic()
        {
            int choice = new Menu.Dropdown("Выберите словарь:",
                "Назад",
                Dictionaries.GetFiles().Select(
                    f => Path.ChangeExtension(f.Name, null)
                ).ToArray()
            ).GetChoice();

            return choice - 1;
        }
        static string? ChoiseWord(LanguageDictionary dictionary)
        {
            List<string> translations = new List<string>(dictionary.Translations.Keys.Select(i => i));

            int choice = new Menu.Dropdown("Выберите слово:",
                "Назад",
                translations.ToArray()
            ).GetChoice() - 1;

            if (choice == -1) return null;
            string test = translations[choice];

            return test;
        }
        static void Edit()
        {
            switch (new Menu.Dropdown("Выберите действие",
                "Создать словарь",
                "Удалить словарь",
                "Редактировать словарь",
                "Назад"
            ).GetChoice())
            {
                case 0: //Создать словарь
                    {
                        string TranslatedLang = new Menu.InoutStr(
                            "Введите название языка который будет переводится: "
                        ).GetChoice().Trim();
                        string TranslationLang = new Menu.InoutStr(
                            "Введите название языка на который будет переводится: "
                        ).GetChoice().Trim();

                        File.Create($"{Dictionaries.FullName}/{TranslatedLang}-{TranslationLang}{DicExtension}").Close();
                        break;
                    }

                case 1: //Удалить словарь
                    {
                        int choice = ChoiseDic();
                        if (choice == -1) break;

                        File.Delete(Dictionaries.GetFiles()[choice].FullName);
                        break;
                    }

                case 2: //Редактировать словарь
                    {
                        EditDictionary();
                        break;
                    }

                case 3: break; //Назад
            }
        }
        static void EditDictionary()
        {
            int choice = ChoiseDic();
            if (choice == -1) return;

            var dic = new LanguageDictionary();
            dic.Load(Dictionaries.GetFiles()[choice].FullName);

            switch (new Menu.Dropdown("Выберите действие",
                "Добавить перевод",
                "Удалить перевод",
                "Редактировать перевод",
                "Назад"
            ).GetChoice())
            {
                case 0: //Добавить перевод
                    {
                        //Ввод слова
                        string word = new Menu.InoutStr(
                            "Введите слово для перевода: "
                        ).GetChoice().Trim();

                        //Его переводы
                        List<string> translations = new List<string>();
                        do
                        {
                            translations.Add(new Menu.InoutStr(
                                "Введите перевод слова: "
                            ).GetChoice().Trim());
                        } while (new Menu.YesOrNo(
                            "Добавить ещё один перевод?"
                        ).GetChoice());

                        //Добавить
                        dic.Translations.Add(word, translations.ToArray());
                        break;
                    }

                case 1: //Удалить перевод
                    {
                        //Ввод слова
                        string? word = ChoiseWord(dic);
                        if (word != null) dic.Translations.Remove(word);
                        break;
                    }

                case 2: //Редактировать перевод
                    {
                        EditTranslation(dic);
                        break;
                    }

                case 3: break; //Назад
            }

            //Сохранить изменения
            dic.Save(Dictionaries.GetFiles()[choice].FullName);
        }
        static void EditTranslation(LanguageDictionary dictionary)
        {
            //Выбор слова для перевода
            string? word = ChoiseWord(dictionary);
            if (word == null) return;

            //Выбор то что нужно изменить
            switch (new Menu.Dropdown("Что нужно изменить?",
                "Слово",
                "Перевод",
                "Назад"
            ).GetChoice())
            {
                case 0: //Изменить слово
                    {
                        string nWord = new Menu.InoutStr("Введите слово: ").GetChoice().Trim().ToLower();
                        string[] temp = dictionary.Translations[word];
                        dictionary.Translations.Remove(word);
                        dictionary.Translations.Add(nWord, temp);
                        break;
                    }

                case 1: //Изменить перевод
                    {
                        int choice = new Menu.Dropdown("Что нужно изменить?",
                            "Назад",
                            dictionary.Translations[word]
                        ).GetChoice() - 1;
                        if (choice == -1) break;

                        string nTranslation = new Menu.InoutStr("Введите перевод: ").GetChoice().Trim().ToLower();
                        dictionary.Translations[word][choice] = nTranslation;
                        break;
                    }

                case 2: break; //Назад
            }
        }
    }
}