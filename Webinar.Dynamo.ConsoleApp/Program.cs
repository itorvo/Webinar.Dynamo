using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Webinar.Dynamo.Domain.Entities;
using Webinar.Dynamo.Domain.Repository;
using Webinar.Dynamo.Repository.ValueObject;

namespace Webinar.Dynamo.ConsoleApp
{
    class Program
    {
        protected Program()
        {

        }

        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
               .Build();

            IStateRepository stateRepository = new StateRepository(config);
            ICountryRepository countryRepository = new CountryRepository(config);

            GetStatesByCountryPaginatedControl(stateRepository);

            FillTables(stateRepository, countryRepository);

        }

        private static void FillTables(IStateRepository stateRepository, ICountryRepository countryRepository)
        {
            string json = File.ReadAllText(@"Resources\inputStates.txt");
            List<State> states = JsonConvert.DeserializeObject<List<State>>(json);
            states = states.FindAll(s => !string.IsNullOrEmpty(s.Code) && !string.IsNullOrEmpty(s.Country));
            states = states.FindAll(s => s.Country == "CO");
            Console.WriteLine($"Full Table: {stateRepository.Add(states)}");

            json = File.ReadAllText(@"Resources\inputCountries.txt");
            List<Country> countries = JsonConvert.DeserializeObject<List<Country>>(json);
            countries = countries.FindAll(c => !string.IsNullOrEmpty(c.Code) && (c.Code == "CO" || c.Code == "US"));
            Console.WriteLine($"Full Table: {countryRepository.Add(countries)}");
        }

        private static void GetStatesByCountryPaginatedControl(IStateRepository stateRepository)
        {
            string paginationToken;
            int option;

            string message = "Ingrese el Tamaño de Página";
            int? limit = GetOption(message);

            List<string> pagTokens = new List<string> { "{}" };
            int currentPage = 1;

            do
            {
                paginationToken = pagTokens[currentPage - 1];
                FilterResponse<State> response = stateRepository.GetStateByCountry("CO", limit, paginationToken);
                ListStates(response.Elements);
                Console.WriteLine($"Pagina: {currentPage} - {response.PaginationToken}");

                if (currentPage == 1)
                {
                    message = "1.Avanzar\n3.Salir";
                }
                else if (currentPage > 1 && response.PaginationToken.Equals("{}"))
                {
                    message = "2. Atras\n3. Salir";
                }
                else
                {
                    message = "1. Avanzar\n2. Atras\n3. Salir";
                }

                option = GetOption(message);

                if (option == 2 && currentPage > 1)
                {
                    currentPage -= 1;
                    pagTokens.RemoveAt(pagTokens.Count - 1);
                }
                else if (option == 1 && !response.PaginationToken.Equals("{}"))
                {
                    pagTokens.Add(response.PaginationToken);
                    currentPage++;
                }

            } while (option != 3);
        }

        private static int GetOption(string message)
        {
            do
            {
                try
                {
                    Console.WriteLine(message);
                    return int.Parse(Console.ReadLine());
                }
                catch
                {
                    //Not action
                }
            }
            while (true);
        }

        private static void ListStates(List<State> states)
        {
            int maxItem = Math.Max("Item".Length, $"{states.Count}".Length);
            int maxCountry = Math.Max("Country".Length, states.Max(s => s.Country.Length));
            int maxCode = Math.Max("Code".Length, states.Max(s => s.Code.Length));
            int maxName = Math.Max("Name".Length, states.Max(s => s.Name.Length));
            int maxNumberCitizens = Math.Max("Number Citizens".Length, states.Max(s => s.Country.Length));

            Console.Write("| " + "Item".PadRight(maxItem, ' ') + " | ");
            Console.Write("Country".PadRight(maxCountry, ' ') + " | ");
            Console.Write($"Code".PadRight(maxCode, ' ') + " | ");
            Console.Write($"Name".PadRight(maxName, ' ') + " | ");
            Console.WriteLine($"Number Citizens".PadRight(maxNumberCitizens, ' ') + " |");

            int index = 1;
            states.ForEach(state =>
            {
                Console.Write("| " + $"{index++}".PadRight(maxItem, ' ') + " | ");
                Console.Write($"{state.Country}".PadRight(maxCountry, ' ') + " | ");
                Console.Write($"{state.Code}".PadRight(maxCode, ' ') + " | ");
                Console.Write($"{state.Name}".PadRight(maxName, ' ') + " | ");
                Console.WriteLine($"{state.NumberCitizens}".PadRight(maxNumberCitizens, ' ') + " |");
            });
            Console.WriteLine();
        }

#if false
        private static void CreateRecord(IStateRepository stateRepository)
        {
            State state = new State
            {
                Country = "CO",
                Code = "18",
                Name = "Caquetá",
                NumberCitizens = 60
            };

            Console.WriteLine("Add a only element: " + stateRepository.Add(state));
            ListStates(stateRepository.GetAll());

            List<State> states = new List<State>
            {
                new State
                {
                    Country = "CO",
                    Code = "05",
                    Name = "Antioquia",
                    NumberCitizens = 100
                },
                new State
                {
                    Country = "CO",
                    Code = "01",
                    Name = "Cundinamarca",
                    NumberCitizens = 100
                }
            };
            Console.WriteLine("Add many elements: " + stateRepository.Add(states));
            ListStates(stateRepository.GetAll());
        }

        private static void UpdateRecord(IStateRepository stateRepository)
        {
            State state = new State
            {
                Country = "CO",
                Code = "01",
                Name = "Amazonas",
                NumberCitizens = 70
            };

            Console.WriteLine($"Modify a only element: {stateRepository.Update(state)}");
            ListStates(stateRepository.GetAll());
        }

        private static void GetAllElement(IStateRepository stateRepository)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetAll();
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetStatesByCountryPaginated(IStateRepository stateRepository)
        {
            string paginationToken = "{}";
            int total = 0;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int? limit = 10;
            List<State> states = stateRepository.GetStateByCountry("CO", limit, ref paginationToken, ref total);
            stopwatch.Stop();
            ListStates(states);
            Console.WriteLine($"Time Get Data by Query: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
        }

        private static void GetByEqualOperator(IStateRepository stateRepository)
        {
            var nameState = "Texas";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetStateByName(nameState);
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetByNotEqualOperator(IStateRepository stateRepository)
        {
            var nameState = "Texas";
            var country = "US";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetStateByNotName(country, nameState);
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetByLessThanOrEqualOperator(IStateRepository stateRepository)
        {
            int number = 76;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetStateByPoblationLE(number);
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetByLessThanOperator(IStateRepository stateRepository)
        {
            int number = 76;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetStateByPoblationLT(number);
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetByGreaterThanOrEqualOperator(IStateRepository stateRepository)
        {
            int number = 76;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetByGreaterThanOrEqualOperator(number);
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetByGreaterThanOperator(IStateRepository stateRepository)
        {
            int number = 76;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetByGreaterThanOperator(number);
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetByBeginsWithOperator(IStateRepository stateRepository)
        {
            string name = "Ca";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetByBeginsWithOperator(name);
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetByIsNotNullOperator(IStateRepository stateRepository)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetByIsNotNullOperator();
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetByIsNullOperator(IStateRepository stateRepository)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetByIsNullOperator();
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetByContainsOperator(IStateRepository stateRepository)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetByContainsOperator();
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetByNotContainsOperator(IStateRepository stateRepository)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetByNotContainsOperator();
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetByInOperator(IStateRepository stateRepository)
        {
            List<string> names = new List<string> { "Caqueta", "Antioquia", "Bogota" };

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetByInOperator(names);
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        }

        private static void GetByBetweenOperator(IStateRepository stateRepository)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<State> states = stateRepository.GetByBetweenOperator(335, 621);
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            ListStates(states);
        } 
#endif
    }
}
