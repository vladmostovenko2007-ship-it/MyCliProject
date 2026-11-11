using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; // Додано для кодування

namespace CliProject
{
    // ==========================================
    // 1. ШАР CORE (Бізнес-логіка - Патерн Стратегія)
    // ==========================================
    
    public interface ICommandStrategy
    {
        string Name { get; }
        void Execute(string[] args);
    }

    // Заглушка "бази даних" (Data шар)
    public static class FakeDatabase
    {
        public static List<string> Characters = new List<string> { "Артем (Воїн)", "Олена (Маг)" };
    }

    // Конкретна команда 1: Вивести список
    public class ListCharactersStrategy : ICommandStrategy
    {
        public string Name => "list";
        public void Execute(string[] args)
        {
            Console.WriteLine("\n--- Список персонажів ---");
            if (FakeDatabase.Characters.Count == 0)
            {
                Console.WriteLine("Список порожній.");
                return;
            }
            for (int i = 0; i < FakeDatabase.Characters.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {FakeDatabase.Characters[i]}");
            }
            Console.WriteLine("-------------------------\n");
        }
    }

    // Конкретна команда 2: Додати персонажа
    public class AddCharacterStrategy : ICommandStrategy
    {
        public string Name => "add";
        public void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Помилка: Вкажіть ім'я. Приклад: add Гендальф");
                return;
            }
            
            string newCharName = string.Join(" ", args);
            FakeDatabase.Characters.Add(newCharName);
            Console.WriteLine($"Персонажа '{newCharName}' успішно додано!");
        }
    }


    // ==========================================
    // 2. ШАР CLI (Представлення - Фасад)
    // ==========================================

    public class ArgParser
    {
        public (string command, string[] args) Parse(string rawInput)
        {
            var parts = rawInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return ("", new string[0]);
            
            string command = parts[0].ToLower();
            string[] args = parts.Skip(1).ToArray();
            return (command, args);
        }
    }

    public class CliFacade
    {
        private readonly Dictionary<string, ICommandStrategy> _strategies = new();
        private readonly ArgParser _parser = new();

        public void UseStrategy(ICommandStrategy strategy)
        {
            _strategies[strategy.Name] = strategy;
        }

        public void ExecCommand(string rawString)
        {
            var (commandName, args) = _parser.Parse(rawString);

            if (_strategies.ContainsKey(commandName))
            {
                _strategies[commandName].Execute(args);
            }
            else
            {
                Console.WriteLine($"Невідома команда: '{commandName}'. Введіть 'help' для довідки.");
            }
        }

        public void DisplayHelp()
        {
            Console.WriteLine("\nДоступні команди:");
            foreach (var strategy in _strategies.Keys)
            {
                Console.WriteLine($"- {strategy}");
            }
            Console.WriteLine("- help");
            Console.WriteLine("- exit\n");
        }
    }

    // ==========================================
    // 3. ЗАПУСК ПРОГРАМИ
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            // Увімкнення підтримки української мови в консолі Windows
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            CliFacade cli = new CliFacade();
            
            cli.UseStrategy(new ListCharactersStrategy());
            cli.UseStrategy(new AddCharacterStrategy());

            Console.WriteLine("Вітаємо в Каталозі Персонажів (CLI)!");
            Console.WriteLine("Введіть команду (напр. 'list' або 'add Ім'я'). 'help' - довідка, 'exit' - вихід.");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input)) continue;

                if (input.ToLower() == "exit") break;
                if (input.ToLower() == "help") 
                {
                    cli.DisplayHelp();
                    continue;
                }

                cli.ExecCommand(input);
            }
        }
    }
}