using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ConfigRW;

namespace UsageExamples
{

    /// <summary>
    /// Configuration of both players
    /// </summary>
    interface Players : IConfiguration
    {
        Player Player1 { get; }
        Player Player2 { get; }
    }

    /// <summary>
    /// Configuration describing single player
    /// </summary>
    interface Player
    {
        [OptionInfo(DefaultValue = "NoName")]
        string Name { get; }
        int SavedLevel { get; set; }
    }



    /// <summary>
    /// Some examples used in documentation
    /// </summary>
    static class Examples
    {
        /// <summary>
        /// Example usage of Players interface
        /// </summary>
        static void Example1()
        {
            var players = Configuration.CreateFromFile<Players>("players.ini");

            //now we can work with players like with usuall .NET object
            ++players.Player1.SavedLevel;

            //...other work with configuration data...

            //we can save configuration easily
            players.SaveTo("changedConfig.ini");
        }

        /// <summary>
        /// Example usage of Builder based configuration access
        /// </summary>
        static void BuilderExample()
        {
            var structureBuilder = new ConfigBuilder();

            //Structure is hidden in code :-(
            structureBuilder.AddOption<string>("Player1", "Name")
                .SetDefault("NoName").SetReadonly();
            structureBuilder.AddOption<int>("Player1", "SavedLevel");

            structureBuilder.AddOption<string>("Player2", "Name")
                .SetDefault("NoName").SetReadonly();
            structureBuilder.AddOption<int>("Player2", "SavedLevel");

            var configuration = structureBuilder.Build("players.ini");

            //we has to explicitly now what type which value hase
            var savedLevel = configuration.ReadValue<int>("Player1", "SavedLevel");
            ++savedLevel;

            //we need to explicitly write changes into configuration object
            configuration.WriteValue<int>("Player1", "SavedLevel", savedLevel);

            //...other work with configuration data...

            //save results
            configuration.SaveTo("changedConfig.ini");
        }

        /// <summary>
        /// Example of data saving variants
        /// </summary>
        static void ExampleSaveProvider()
        {
            var configObj = new Config();

            //Save from external class
            Config.Save(configObj, "output.ini");

            //SaveTo directly on object
            configObj.SaveTo("output.ini");
        }
    }


    /// <summary>
    /// This prototype is used as documentation example to show, that interface based
    /// configuration handling looks better in code.
    /// </summary>
    #region Configuration via builder interface prototype

    class ConfigBuilder
    {
        /// <summary>
        /// Add definition of an option into ConfigBuilder
        /// </summary>
        /// <typeparam name="T">Type of added option</typeparam>
        /// <param name="section">Section where option will be stored</param>
        /// <param name="option">ID of added option</param>
        /// <returns>Option builder of added option</returns>
        public OptionBuilder AddOption<T>(string section, string option)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Build described configuration object with data from file.
        /// </summary>
        /// <param name="file">File with configuration data</param>
        /// <returns>Configuration object</returns>
        public Config Build(string file)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Builder for single option of configuration file
    /// </summary>
    class OptionBuilder
    {
        /// <summary>
        /// Set default value for option
        /// </summary>
        /// <param name="defaultValue">Value for option</param>
        /// <returns>This option builder (because of chaining methods)</returns>
        public OptionBuilder SetDefault(object defaultValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set option as readonly
        /// </summary>
        /// <returns>This option builder (because of chaining methods)</returns>
        internal OptionBuilder SetReadonly()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Configuration object used for manipulationg data in configuration file
    /// </summary>
    class Config
    {
        /// <summary>
        /// Reads value for specified option
        /// </summary>
        /// <typeparam name="T1">Type of readed value</typeparam>
        /// <param name="section">Section in configuration file</param>
        /// <param name="option">Option in section</param>
        /// <returns>Readed value</returns>
        internal T1 ReadValue<T1>(string section, string option)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write given value for specified option
        /// </summary>
        /// <typeparam name="T1">Type of readed value</typeparam>
        /// <param name="section">Section in configuration file</param>
        /// <param name="option">Option in section</param>
        /// <param name="value">Value to be written</param>
        internal void WriteValue<T1>(string section, string option, T1 value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Direct method for saving configuration into file
        /// </summary>
        /// <param name="file">File where configuration will be saved</param>
        internal void SaveTo(string file)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Indirect method for savinf configuration into file
        /// </summary>
        /// <param name="configObj">Configuration object to be saved</param>
        /// <param name="file">File where configuration will be saved</param>
        internal static void Save(object configObj, string file)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
