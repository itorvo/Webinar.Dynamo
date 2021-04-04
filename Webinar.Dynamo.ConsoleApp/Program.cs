using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Webinar.Dynamo.ConsoleApp.AccessData;
using Webinar.Dynamo.ConsoleApp.Entities;

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
            State state;
            List<State> states;
            Stopwatch stopwatch;

#if false
            #region Fill Table
            FillTable(stateRepository);
            #endregion
#endif

#if false
            #region Crear Registro
            state = new State
            {
                Country = "CO",
                Code = "18",
                Name = "Caquetá",
                NumberCitizens = 60
            };

            Console.WriteLine("Add a only element: " + stateRepository.Add(state));
            ListStates(stateRepository.GetAll());

            states = new List<State>
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
            #endregion
#endif

#if true
            #region Modificar Registro
            state = new State
            {
                Country = "CO",
                Code = "01",
                Name = "Amazonas",
                NumberCitizens = 70
            };

            Console.WriteLine($"Modify a only element: {stateRepository.Update(state)}");
            ListStates(stateRepository.GetAll());
            #endregion
#endif
#if true
            #region Get All Elements
            stopwatch = new Stopwatch();
            stopwatch.Start();
            states = stateRepository.GetAll();
            stopwatch.Stop();
            Console.WriteLine($"Time Get All Data: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            #endregion
#endif
#if true
            #region Get Elements by Query
            stopwatch = new Stopwatch();
            stopwatch.Start();
            states = stateRepository.GetStateByCountryQuery("US");
            stopwatch.Stop();
            Console.WriteLine($"Time Get Data by Query: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            #endregion
#endif

#if true
            #region Get Elements by Scan
            stopwatch = new Stopwatch();
            stopwatch.Start();
            states = stateRepository.GetStateByCountryScan("US");
            stopwatch.Stop();
            Console.WriteLine($"Time Get Data by Scan: {stopwatch.ElapsedMilliseconds} ms - Total Elements: {states.Count}");
            #endregion  
#endif
        }

        private static void FillTable(IStateRepository stateRepository)
        {
            string json = File.ReadAllText(@"Resources\input.json");
            List<State> states = JsonConvert.DeserializeObject<List<State>>(json);
            Console.WriteLine($"Full Table: {stateRepository.Add(states)}");
        }

        private static void ListStates(List<State> states)
        {
            Console.WriteLine($"\nCountry\t\tCode\t\tName\t\tNumber Citizens");
            states.ForEach(state =>
            {
                Console.WriteLine($"{state}");
            });
            Console.WriteLine();
        }
    }
}
