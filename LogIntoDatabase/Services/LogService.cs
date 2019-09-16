using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LogIntoDatabase.Enums;
using LogIntoDatabase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LogIntoDatabase.Services
{
    public class LogService<T> where T : class
    {
        private readonly ContosoContext _db;
        private readonly string _key;

        public LogService()
        {
            // Uses the configuration file to get the default primary key reference from the table bank.
            var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .Build();

            // It is assumed that your tables use the name column name for primary key.
            _key = config.GetValue<string>("KeyModel");

            // Initialize context
            _db = new ContosoContext();
        }

        // Function for organizing data to be saved in the log and logging model changes
        // Use context to find url
        public async void Save(T values, LogType.Type type, HttpContext context)
        {
            var page = context.Request.GetDisplayUrl();
            var changesToSave = GetValues(values, typeof(T));
            await Save(values, type, changesToSave, page);
        }

        // Function for organizing data to be saved in the log and logging model changes
        // Change url is passed by parameter
        public async void Save(T values, LogType.Type type, string page)
        {
            var changesToSave = GetValues(values, typeof(T));
            await Save(values, type, changesToSave, page);
        }

        // Function that saves log data
        internal async Task Save(T values, LogType.Type type, string newValues, string url)
        {
            // Create the log object
            var log = new Log { Url = url, DateUpdate = DateTime.Now, Type = type.ToString() };

            // Selects which logging method to save
            switch (type)
            {
                case LogType.Type.Create:
                    log.After = newValues;
                    break;
                case LogType.Type.Update:
                    {
                        // When it is update, consult the current values ​​of the table, to register in the before.
                        var currentData = await GetCurrentModelState(values);
                        var oldModel = GetValues(currentData, typeof(T));

                        log.After = newValues;
                        log.Before = oldModel;

                        break;
                    }

                case LogType.Type.Delete:
                    log.Before = newValues;
                    break;

                default:
                    log.After = newValues;
                    break;
            }

            await SaveLog(log);
        }

        // Function to query column values ​​and return a string to reference changes
        internal string GetValues(T values, IReflect model)
        {
            var stringToSave = string.Empty;

            // Specifies flags that control binding and the manner in which search for types and members is performed by reflection.
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            // Query the fields available in the reflection model
            var fieldNames = model
                .GetFields(bindingFlags)
                .Select(field => field.Name)
                .ToList();

            foreach (var field in fieldNames)
            {
                // Selected columns need a treatment to have their name exactly as it is saved in the database
                var start = field.IndexOf("<", StringComparison.Ordinal);
                var end = field.IndexOf(">", StringComparison.Ordinal);

                // Get column name
                var nameField = field.Substring(start, end - start).Replace("<", "").Replace(">", "");

                // Get value column name
                var value = values.GetType().GetProperty(nameField)?.GetValue(values, null);

                // This is the return you will save in the database for future reference.
                stringToSave += $"[Column]: {nameField}, [Value]: {value}" + Environment.NewLine;
            }

            return stringToSave;
        }

        // Function to get the current state of the table before updating
        internal async Task<T> GetCurrentModelState(T model)
        {
            // Get entity primary key
            var id = (int)model.GetType().GetProperty(_key).GetValue(model, null);

            //Make a query and get the current value from the table, this query uses AsNoTracking to not keep the context state.
            var oldData = await _db.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => (int)x.GetType().GetProperty(_key).GetValue(x, null) == id);

            // Returns current values ​​from table
            return oldData;
        }

        // Save data by Entity Framework
        internal async Task SaveLog(Log log)
        {
            await _db.Log.AddAsync(log);
            await _db.SaveChangesAsync();
        }
    }
}
