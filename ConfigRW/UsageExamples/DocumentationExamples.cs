using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ConfigRW;

namespace UsageExamples
{

interface Players : IConfiguration
{
    Player Player1 { get; }
    Player Player2 { get; }
}

interface Player
{
    [OptionInfo(DefaultValue = "NoName")]
    string Name { get; }
    int SavedLevel { get; set; }
}




    static class Examples
    {
        static void Example1()
        {
var players = Configuration.CreateFromFile<Players>("players.ini");

//now we can work with players like with usuall .NET object
++players.Player1.SavedLevel;

//...other work with configuration...

//we can save configuration easily
players.SaveTo("changedConfig.ini");
        }

        static void BuilderExample()
        {
var structureBuilder = new ConfigBuilder();

//Structure is hidden in code
structureBuilder.AddOption<string>("Player1", "Name").SetDefault("NoName");
structureBuilder.AddOption<int>("Player1", "SavedLevel");

structureBuilder.AddOption<string>("Player2", "Name").SetDefault("NoName");
structureBuilder.AddOption<int>("Player2", "SavedLevel");

var configuration = structureBuilder.Build("players.ini");

//we has to explicitly now what type which value hase
var savedLevel=configuration.ReadValue<int>("Player1", "SavedLevel");
++savedLevel;

//we need to explicitly write changes into configuration object
configuration.WriteValue<int>("Player1", "SavedLevel", savedLevel);

//...other work with configuration...

//save results
configuration.SaveTo("changedConfig.ini");
        }

        static void ExampleSaveProvider()
        {
            var configObj=new Config();
            Config.Save(configObj, "output.ini");

            configObj.SaveTo("output.ini");            
        }
    }


    #region Configuration via builder interface prototype
    class ConfigBuilder
    {
        public OptionBuilder AddOption<T>(string section, string option)
        {
            throw new NotImplementedException();
        }

        public Config Build(string file)
        {
            throw new NotImplementedException();
        }
    }

    class OptionBuilder
    {
        public OptionBuilder SetDefault(object defaultValue)
        {
            throw new NotImplementedException();
        }
    }

    class Config
    {
        internal T1 ReadValue<T1>(string p1, string p2)
        {
            throw new NotImplementedException();
        }

        internal void WriteValue<T1>(string p1, string p2, T1 savedLevel)
        {
            throw new NotImplementedException();
        }

        internal void SaveTo(string p)
        {
            throw new NotImplementedException();
        }

        internal static void Save(object config, string p)
        {
            throw new NotImplementedException();
        }
    }
    #endregion
}
