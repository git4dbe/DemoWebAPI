using ECommerceDemoCommon.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ECommerceDemoCommon.DataProviders
{
    [Obsolete]
    public class FileEntityProvider<T> : IDataProvider<T> where T : IEntity
    {
        private readonly string _dataPath;

        public FileEntityProvider(string dataPath)
        {
            _dataPath = dataPath;
            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }
        }

        public void AddAsync(T entity)
        {
            if (Get(entity.Id) != null)
            {
                throw new ArgumentException($"{typeof(T).Name} with id='{entity.Id}' already exists");
            }

            Save(entity);
        }

        public void Update(T entity)
        {
            if (Get(entity.Id) == null)
            {
                throw new ArgumentException($"{typeof(T).Name} with id='{entity.Id}' does not exist");
            }

            Save(entity);
        }

        public T Get(string id)
        {
            string filePath = GetFilePath(id);
            if (!File.Exists(filePath))
            {
                return default;
            }

            return GetEntityByFilePath(filePath);
        }
        public void Delete(string id)
        {
            File.Delete(GetFilePath(id));
        }

        public IEnumerable<T> Get()
        {
            var jsonFiles = Directory.EnumerateFiles(_dataPath, "*.json", SearchOption.AllDirectories);

            foreach (string jsonFile in jsonFiles)
            {
                yield return GetEntityByFilePath(jsonFile);
            }
        }

        private string GetFilePath(string id) => Path.Combine(_dataPath, $"{id}.json");

        private T GetEntityByFilePath(string filePath)
        {
            string json = File.ReadAllText(filePath);

            T result = JsonConvert.DeserializeObject<T>(json);

            return result;
        }

        private void Save(T entity)
        {
            string json = JsonConvert.SerializeObject(entity, Formatting.Indented);
            File.WriteAllText(GetFilePath(entity.Id), json);
        }


    }
}
