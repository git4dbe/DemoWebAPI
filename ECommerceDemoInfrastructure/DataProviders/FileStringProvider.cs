using ECommerceDemoInfrastructure.Contracts;
using System;
using System.Collections.Generic;
using System.IO;

namespace ECommerceDemoInfrastructure.DataProviders
{
    public class FileStringProvider : IStringDataProvider
    {
        private readonly string _dataPath;
        private readonly string _entityType;

        public FileStringProvider(string entityType)
        {
            _dataPath = $"{entityType}s";
            _entityType = entityType;
            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }
        }

        public void Add(string id, string entity)
        {
            string filePath = GetFilePath(id);
            if (File.Exists(filePath))
            {
                throw new ArgumentException($"{_entityType} with id='{id}' already exists");
            }
            File.WriteAllText(filePath, entity);
        }

        public void Delete(string id)
        {
            File.Delete(GetFilePath(id));
        }

        public string Get(string id)
        {
            string filePath = GetFilePath(id);
            if (!File.Exists(filePath))
            {
                return null;
            }

            return File.ReadAllText(filePath);
        }

        public IEnumerable<string> Get()
        {
            var jsonFiles = Directory.EnumerateFiles(_dataPath, "*.json", SearchOption.AllDirectories);

            foreach (string jsonFile in jsonFiles)
            {
                yield return File.ReadAllText(jsonFile);
            }
        }

        public bool IsNew(string id)
        {
            return !File.Exists(GetFilePath(id));
        }

        public void Update(string id, string entity)
        {
            string filePath = GetFilePath(id);
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"{_entityType} with id='{id}' does not exist");
            }
            File.WriteAllText(filePath, entity);
        }

        private string GetFilePath(string id) => Path.Combine(_dataPath, $"{id}.json");
    }
}
