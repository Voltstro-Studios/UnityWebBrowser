using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Voltstro.CommandLineParser.TypeReaders;

namespace Voltstro.CommandLineParser
{
	/// <summary>
	///     The main class for parsing command line arguments
	/// </summary>
	public static class CommandLineParser
	{
		private static readonly Dictionary<Type, ITypeReader> TypeReaders = new Dictionary<Type, ITypeReader>
		{
			[typeof(string)] = new StringReader(),
			[typeof(int)] = new IntReader(),
			[typeof(byte)] = new ByteReader(),
			[typeof(float)] = new FloatReader(),
			[typeof(bool)] = new BoolReader()
		};

		/// <summary>
		///     Adds a new, or overrides a TypeReader used for knowing what to set when parsing the arguments
		/// </summary>
		/// <param name="type"></param>
		/// <param name="reader"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public static void AddTypeReader([NotNull] Type type, [NotNull] ITypeReader reader)
		{
			//Make sure our arguments are not null
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));

			//If the type reader already exists, we override the old one with the one we are adding
			if (TypeReaders.ContainsKey(type))
			{
				TypeReaders[type] = reader;
				return;
			}

			TypeReaders.Add(type, reader);
		}

		#region Initialization

		/// <summary>
		///     Initializes and parses the command line arguments
		/// </summary>
		/// <param name="args"></param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public static void Init([NotNull] string[] args)
		{
			//Make sure args are not null
			if (args == null)
				throw new ArgumentNullException(nameof(args));

			Dictionary<string, FieldInfo> argumentProperties = new Dictionary<string, FieldInfo>();

			//Go through all found arguments and add them to argumentProperties
			foreach (KeyValuePair<FieldInfo, CommandLineArgumentAttribute> argument in GetCommandFields())
			{
				if (argumentProperties.ContainsKey(argument.Value.Name))
					throw new ArgumentException(
						$"The argument {argument.Value.Name} has already been defined as an argument!",
						nameof(argument.Value.Name));

				argumentProperties.Add(argument.Value.Name, argument.Key);
			}

			//Now sort through all the arguments and set the corresponding argument
			int i = 0;
			while (i < args.Length)
			{
				string arg = args[i];
				if (!arg.StartsWith("-"))
				{
					i++;
					continue;
				}

				string value = null;
				if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
				{
					value = args[i + 1];
					i++;
				}

				//Handle reading and setting the type
				if (argumentProperties.TryGetValue(arg.Replace("-", ""), out FieldInfo property))
				{
					if (TypeReaders.TryGetValue(property.FieldType, out ITypeReader reader))
						property.SetValue(property, reader.ReadType(value));

					//Handling for enums
					if (property.FieldType.IsEnum)
					{
						if(string.IsNullOrEmpty(value))
							continue;

						Type baseType = Enum.GetUnderlyingType(property.FieldType);
						if(!TypeReaders.TryGetValue(baseType, out reader))
							continue;

						object enumValue = Enum.ToObject(property.FieldType, reader.ReadType(value));

						property.SetValue(property, enumValue);
					}
				}

				i++;
			}
		}

		/// <summary>
		///     Gets all fields with the <see cref="CommandLineArgumentAttribute" /> attached
		/// </summary>
		/// <returns></returns>
		public static Dictionary<FieldInfo, CommandLineArgumentAttribute> GetCommandFields()
		{
			const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			IEnumerable<FieldInfo> fields = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.SelectMany(x => x.GetFields(bindingFlags))
				.Where(x => x.GetCustomAttribute<CommandLineArgumentAttribute>() != null);
			return fields.ToDictionary(fieldInfo => fieldInfo,
				fieldInfo => fieldInfo.GetCustomAttribute<CommandLineArgumentAttribute>());
		}

		#endregion
	}
}