using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigRW
{
   /*! \mainpage User Documentation
    * \tableofcontents
    *
    * \section sec_intro ConfigRW
    *
    * Tool for easy reading and writing of configuration files.
    * Convenient usage of this tool is at first covered by series of simple explanations with examples, for more theoretically based users.
    * Convenient usage of this tool is also later covered by series of simple examples with explanation, for more practically oriented users.
    * 
    * format of configuration files this tool handles by default can be found online
    * http://d3s.mff.cuni.cz/teaching/programming_practices/ukol2/
    *
    * \section sec_overview Overview
    * 
    * \subsection subsect_overview_hierarach Structure hierarchy
    * 
    * We treat configuration as hierarchical structure. The hieararchy starts with the configuration structure itself. 
    * The configuration structure then contains few sections, where each section contains its options.
    * 
    * \subsection subsect_overview_handling Handling
    * 
    * When user wishes to utilize our config, he/she needs to specifi this hierarchy, for which he/she may utilize directly .NET language features.
    * Once the configuration structure is known, user can proceed with operations on the configuration itself.
    *    The configuration can be obtained as configuration of default values (used before additional processing or as default config creation)
    *    The configuration can be modified from within the code (by natural .NET environment)
    *    The configuration can be imprinted into a stream
    *    The configuration can be stored into a file
    *    The configuration can be loaded from a stream
    *    The configuration can be loaded from a file
    *    
    * \subsection subsect_overview_except Exceptions
    * 
    * In case that ConfigRW occurs some undesired situation, it utilizes Exception system to do so. All (non trully fatal) exceptions are inherited from ConfigRWException,
    * so these are the ones to be handled.
    * 
    * The exception contains userMsg, that is the message designated for user, unles you have your own. 
    * Then it contains logMsg, which is message designated to be blindly flushed into a log file, for later inspection.
    * Messgae a developer is expected to be concerned with the most, is so called developer message. As during development, 
    * exceptions are expected to strike the most, this particular message is provided as Exception's main message,
    * so most IDEs are expected to show it in easy accessible way during development.
    * 
    * Inner exceptions are commonly assigned, to provide developer with more detailed information, considering he/she needs to examine them
    * 
    * Where it is rational, exception laying lower in the hierarchy is thrown, these are:
    * 
    * ParserException:
    *    thrown when the originator is parsing process, so most probably, the problem lays somewhere in the data being 
    *    processed (either loaded from file, or stored manually)
    *    
    * ParserExceptionWithinConfig
    *    even more specific level of ParserException, occurs mostly during parsing of the stream or file, and provides developer with 
    *    recent Line, Section and Option (all that are available when exception occurs)
    * 
    * \section sec_specifyStruct Specification of config hierarchy
    * 
    * \subsection subsect_structure Configuration Structure
    * 
    * Configuration structure is represented by simple .NET public interface inherited from IConfiguration.
    * The public interface is expected to contain multiple properties, one propertie per one section with default getter. The section has the same name as given property.
    * 
    * \code{.cs}
    * // Example: Simple configuration with single section
    * public interface myConfiguration : IConfiguration
    * {
	 *    Section mySection {get;}
    * }
    * \endcode
    * 
    * And that's it. Next paragraph contains more advanced features, which you have to master only when you actually need them.
    * 
    * Additional features are operated by attributes.
    * 
    * May user need to name a section inside a configuration file differently (e.g. for shared configs with pseudo-equivalent sections), he/she can use SectionInfo attribute,
    * with parameter ID set to whatever value he/she wishes to use as the section identifier inside the configuration files.
    * 
    * \code{.cs}
    * // Example: Configuration with single, custom named section
    * public interface myConfiguration : IConfiguration
    * {
    *    [SectionInfo(ID = "customNamedSection")]
	 *    Section mySection {get;}
    * }
    * \endcode
    * 
    * Section attribute also can be utilized to mark section as optional.
    * 
    * \code{.cs}
    * // Example: Configuration with single, optional section
    * public interface myConfiguration : IConfiguration
    * {
    *    [SectionInfo(IsOptional = true)]
	 *    Section mySection {get;}
    * }
    * \endcode
    * 
    * If user wishes to assign default comment with certain section, DefaultComment attribute serves this purpose. It takes desired comment as its parameter.
    * 
    * \code{.cs}
    * // Example: Configuration with single section with comment
    * public interface myConfiguration : IConfiguration
    * {
    *    [DefaultComment("Comment for my section")]
	 *    Section mySection {get;}
    * }
    * \endcode
    * 
    * \subsection subsec_section Section
    * 
    * Section of configuration structure is represented by simple .NET public interface. The section is expected to contain one property per option, with default getter (and setter if modification is desired).
    *    
    * \code{.cs}
    * // Example: Simple section with two options, one is mutable
    * public interface Section
    * {
	 *    int myOption1 {get;}
    *    List<bool> myOption2 {get; set;}
    * }
    * \endcode
    * 
    * And that's it. Next paragraph contains more advanced features, which you have to master only when you actually need their additional features.
    * 
    * If there is need for advanced control of options, OptionInfo attribute serves as the right tool for this job. It recognizes these parameters:
    *    DefaultValue - default value for given option (i.e. value when an option is not set)
    *    ID - sets custom name for given option
    *    IsOptional - sets whether this option is considered to be optional
    * 
    * \code{.cs}
    * // Example: Not so simple example of OptionInfo attribute usage
    * public interface Section
    * {
    *    [OptionInfo(ID = "userOption", IsOptional = true, DefaultValue = "default_value")]
    *    string myOption {get; set;}
    * }
    * \endcode
    * 
    * If user wishes to assign default comment with certain option, DefaultComment attribute can be used. It takes the comment as its parameter
    *    
	 * \code{.cs}
    * // Example: Simple section with two options, one is mutable, both have default comment
    * public interface Section
    * {
    *    [DefaultComment("Comment for read only option")]
	 *    int myOption1 {get;}
    *    [DefaultComment("Comment for mutable option")]
    *    List<float> myOption2 {get; set;}
    * }
    * \endcode
    * 
    * In case of IComparable type (or IEnumerable of IComparable types), it is possible to define lower and/or upper bound, both are inclusive.
    * To do so, Range attribute needs to be present.
    * 
    * \code{.cs}
    * // Example: Simple section with two options, one is mutable, both have default comment
    * public interface Section
    * {
    *    [Range(LowerBound = 0, UpperBound = 10)]
	 *    int remainingFingers {get;}
    *    [Range(LowerBound = 1)]
    *    List<int> positiveNumbers {get; set;}
    * }
    * \endcode
    * 
    * It is also possible to use custom defined enumeration type as option type
    * 
    * \code{.cs}
    * // Custom defined enumeration
    * public enum myEnum {Sat=1, Sun, Mon, Tue, Wed, Thu, Fri};
    * 
    * // Example: Simple section with one option type of custom defined Enum
    * public interface SectionWithEnum
    * {
	 *    myEnum enumTypedOption {get;}
    * }
    * \endcode
    * 
    * \section sec_compatibility Compatibility
    * 
    * The tool is developed under C# .NET 3.5
    * therefore it should be compatible with .NET 3.5 onwards (unless MS ceases backwars compatibility of used features)
    * 
    * Due to .NET being MS proprietary language and third party solutions (MONO) are not with par,
    * the ConfigRW is compilable under MONO, however not all versions are able to run it, due to many functions having
    * throw NotimplementedException() as their implementation. As this task has requirements to be reasonably compilable under wider selection
    * of operating systems (MONO stated explicitly for .NET), we claim that this project follows .NET standards and is compilable on reasonable .NET (3.5 compatible) 
    * implementation. Most problematic methods, are the ones working around reflection. After consultations with lab teacher, we concluded that the solution 
    * is prepared to be working with satisfying implementation of the .NET,
    * therefore satisfies the task as compleetness of the MONO is out of our concern.
    * 
    * \section sec_containers Container types as option
    * 
    * ConfigRW supports among scalar types also container types. However for usage of container types, certain criteria needs to be met.
    * Container type can be an array of scalar types. For example array of integers is perfectly valid scalar type.
    * Container type can be generic type of  IEnumerable<> or ICollection<>.
    * Container type can be any non-abstract type implementing ICollection interface meeting certain criteria.
    *     Type needs to have parameter-less accessible constructor.
    *     Method Add() is used to add new elements into a collection.
    *     IEnumerable.GetEnumerator() is used for elements extraction.
    *     
    * When setting default values for containers, it is set as array of given type. For empty collection, decision needs to be made,
    * whether null or empty collection is desired.
    * 
    * \code{.cs}
    * 
    * // Section structure with numerous container types
    * public interface Section
    * {
    *    [DefaultComment("This is scalar type")]
	 *    int scalar {get;}
    *    
    *    [DefaultComment("This is array of integers")]
    *    int[] arrayInt {get;}
    *    
    *    [DefaultComment("This is list of integers")]
    *    List<int> listInt {get;}
    *    
    *    [DefaultComment("This is list of strings with default values")]
    *    [OptionInfo(DefaultValue = new string[] { "value1", "value2", "valu3" })]
    *    List<string> List { get; }
    * }
    * 
    * \endcode
    * 
    * \section sec_advanced Advanced type in config
    * 
    * NOTE: This section contains tweaks for advanced developers and needs tweaking of the parser code itself.
    * If you wish to utilize any of them, be prepared to modify internal structures of the program.
    * No examples are provided here, as these tweaks are designated for users who trully understand what are they doing 
    * and are familiar with ConfigRW internal structures.
    * 
    * In case that more advanced type is expected to be present in a structure,
    * user needs to implement its serializer/deserializer inherited from IValueConverter and register it in ConfigConverters class
    * 
    * In case that more complex structures are to be parsed from config file, full parser contains of standard duo Parser + Lexer.
    * Both can be found in expected classes inside Parsing namespace. Lexer except its work also resolves internal links and is implemented as state automaton,
    * which is its storn place, as here can be implemented powerfull gadgets if you may need them.
    * 
    * \section usage Usage
    * 
    * Usage of ConfigRW once Connfiguration Structure is specified is quite simple and intuitive.
    * When user needs to access option optA in section SecB inside configuration confC, he/she accessed confC.SecB.optA.
    * The approach is same for reading of the option and for modification of mutable options.
    * 
    * Creating default config is achieved by calling Configuration.CreateFromDefaults<IConfiguration>() factory method, with appropriate type instead of IConfiguration;
    * 
    * Loading config from file utilizes method is Configuration.CreateFromFile<IConfiguration>(string, ParsingMode), where the first argument specifies a config file and 
    * the second one determines parsing mode (strict or relaxed) 
    * 
    * Loading config from stream is very similar, just the utilized method is Configuration.CreateFromStream<IConfiguration>(StreamReader, ParsingMode), and file name
    * is replaced by the input stream
    * 
    * Storing configuration inside file is achieved by .SaveTo(string) method of configuration with output filename as its parameter.
    * 
    * Storing configuration into stream is similar to storing it into file, however utilized method is .WriteTo(StreamWriter) and parameter takes output stream instead of filename
    * 
    * \section sec_examples Examples
    *
    * \subsection sub_exSimple Simple quick to work example
    * 
    * \code{.cs}
    * // Configuration structure
    * public interface myConfiguration : IConfiguration
    * {
	 *    Section mySection {get;}
    * }
    * 
    * // Section structure
    * public interface Section
    * {
	 *    int myOption1 {get;}
    *    List<bool> myOption2 {get; set;}
    * }
    * 
    * // usage
    * void workingMethod()
    * {
    *    // just save config to file
    *    var defaultConfig = Configuration.CreateFromDefaults<myConfiguration>();
    *    defaultConfig.SaveTo("config.cfg");
    *    
    *    // just load config from file (relaxed mode)
    *    var fromFileConfig = Configuration.CreateFromFile<myConfiguration>("config.cfg", ParsingMode.Relaxed);
    * }
    * \endcode
    * 
    * \subsection sub_exComplex More complex example demonstrating wider functionality
    * 
    * \code{.cs}
    * // Configuration structure
    * public interface myConfiguration : IConfiguration
    * {
    *    [DefaultComment("This is the alpha section and is compulsory")]
    *    [SectionInfo(ID = "AlphaSection", IsOptional = false)]
	 *    Section_alpha alphaSect {get;}
    *    
    *    [DefaultComment("This is the bravo section, however user file recognizes beta instead, it is optional")]
    *    [SectionInfo(ID = "BetaSection", IsOptional = true)]
    *    Section_bravo bravoSect {get;}
    *    
    *    [DefaultComment("This is the second alpha section, which is compulsory and named charlie")]
    *    [SectionInfo(ID = "CharlieSection", IsOptional = false)]
	 *    Section_alpha charlieSect {get;}
    * }
    * 
    * // Section structure alpha
    * public interface Section_alpha
    * {
	 *    int myOption1 {get;}
    *    List<bool> myOption2 {get; set;}
    *    customEnum userSelection {get;}
    * }
    * 
    * // Section structure bravo
    * public interface Section_bravo
    * {
    *    [OptionInfo(DefaultValue = new int[] {1, 2, 3, 4, 5})]
    *    [DefaultComment("This readonly option contains 5 elements, 1-5 by default")]
	 *    List<int> intList {get;}
    *    
    *    [Range(LowerBound = 0.3f, UpperBound = 0.9f)]
    *    [OptionInfo(ID = "commonValue", IsOptional = true, DefaultValue = 0.4f)]
    *    float trulySuspiciousValue {get; set;}
    * }
    * 
    * // usage
    * void workingMethod()
    * {
    *    
    *    // just load config from file (strict mode)
    *    try
    *    {
    *       // LoadFrom file
    *       var configData = Configuration.CreateFromFile<myConfiguration>("config.cfg", ParsingMode.Relaxed);
    *       
    *       // retrieve and use value
    *       if (configData.bravoSect.trulySuspiciousValue > 0.5f)
    *       {
    *          doSomething();
    *       }
    *       else
    *       {
    *          doSomethingElse();
    *       }
    *       
    *       // Tinker with value
    *       configData.alphaSect.myOption2 = new List<int>();
    *       foreach (int someInt in someInts)
    *       {
    *          configData.alphaSect.myOption2.add(someInt*2);
    *       }
    * 
    *       // Save back to the file
    *       configData.SaveTo("config.cfg");
    *    }
    *    catch (ConfigRWException ex)
    *    {
    *       // something failed, config may be in wrong format, who knows
    *    }
    *    catch (Exception ex)
    *    {
    *       // trully fatal problem occurred, or the source is not originated from ConfigRW
    *    }
    * }
    * \endcode
    */
}
