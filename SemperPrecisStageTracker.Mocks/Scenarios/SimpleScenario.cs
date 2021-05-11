﻿using System;
using System.IO;
using System.Text.Json;
using SemperPrecisStageTracker.Mocks.Scenarios.Common;

namespace SemperPrecisStageTracker.Mocks.Scenarios
{
    /// <summary>
    /// Single scenario
    /// </summary>
    public class SimpleScenario : SemperPrecisStageTrackerScenarioBase
    {
        /// <summary>
        /// Admin user
        /// </summary>
        public static string AdminUser = "morgan-admin";

        /// <summary>
        /// Operative user with permissions for analysis
        /// </summary>
        public static string OperativeUser = "morgan";

        /// <summary>
        /// Viewer user with permissions for analysis
        /// </summary>
        public static string ViewerUser = "employee";

        /// <summary>
        /// Executes initialization of entities
        /// </summary>
        public override void InitializeEntities()
        {
            //Nome del file
            string fileName = $"{GetType().Name}.json";

            //Percorso del file statico
            string testsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "../../../..", "SemperPrecisStageTracker.Docs", "data", fileName);

            //percorso file su Azure
            string azureFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                   "App_Data", fileName);

            //Se il file di test esiste, uso quello
            string file = File.Exists(testsFile)
                ? testsFile
                : File.Exists(azureFile)
                    ? azureFile
                    : null;

            //Se non ho il file, eccezione
            if (file == null)
                throw new FileNotFoundException($"Source scenario json file '{fileName}' not found");

            //Lettura del contenuto
            var json = File.ReadAllText(file);

            //Deserializzazione e assegnazione
            var scenarioClone = JsonSerializer.Deserialize<SimpleScenario>(json, new JsonSerializerOptions
                {
                    //PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            Groups = scenarioClone.Groups;
            Matches = scenarioClone.Matches;
            ShooterStages = scenarioClone.ShooterStages;
            Stages = scenarioClone.Stages;
            Shooters = scenarioClone.Shooters;
            GroupShooters = scenarioClone.GroupShooters;
        }
    }
}
