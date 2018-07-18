namespace Launcher
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// Представляет методы для работы с динамически подключаемыми алгоритмами
	/// </summary>
	public class AlgoActivator
	{
		private const string EntryPointClassName = "Startup";
		private const string EntryPointMethodName = "OnActivated";

		/// <summary>
		/// Запускает алгоритм
		/// </summary>
		/// <param name="rootDirectory">Директория с файлами алгоритма</param>
		/// <param name="parameters">Параметры</param>
		/// <returns></returns>
		public object Activate(string rootDirectory, AlgoParams parameters = null)
		{
			if (string.IsNullOrEmpty(rootDirectory))
				throw new ArgumentNullException("rootDirectory");

			var entryPoint = FindEntryPoint(rootDirectory);
			return CallEntryPointMethod(entryPoint, parameters);
		}

		/// <summary>
		/// Ищет точку входа в алгоритм
		/// </summary>
		/// <param name="rootDirectory">Директория с файлами алгоритма</param>
		/// <returns></returns>
		private MethodInfo FindEntryPoint(string rootDirectory)
		{
			var assemblyFiles = Directory.EnumerateFiles(rootDirectory, "*.dll");

			foreach (var assemblyFile in assemblyFiles)
			{
				var assembly = Assembly.LoadFrom(assemblyFile);

				foreach (var type in assembly.ExportedTypes)
				{
					if (type.Name == EntryPointClassName)
					{
						var methods = type.GetMethods();
						var entryPointMethod = methods.FirstOrDefault(p => p.Name == EntryPointMethodName);

						if (entryPointMethod != null)
							return entryPointMethod;
					}
				}
			}

			throw new Exception("Entry point not found in " + rootDirectory);
		}

		/// <summary>
		/// Вызывает точку входа в алгоритм
		/// </summary>
		/// <param name="entryPoint">Информация о методе</param>
		/// <param name="parameters">Параметры алгоритма</param>
		/// <returns></returns>
		private object CallEntryPointMethod(MethodInfo entryPoint, AlgoParams parameters)
		{
			var assembly = entryPoint.Module.Assembly;
			var typeName = assembly.GetName().Name + "." + EntryPointClassName;
			var type = assembly.GetType(typeName);

			var algoInstance = Activator.CreateInstance(type);
			var algoInputInstance = CreateInputInstance(entryPoint);
			MapInputParams(entryPoint, algoInputInstance, parameters);

			return entryPoint.Invoke(algoInstance, new[] { algoInputInstance });
		}

		/// <summary>
		/// Создает экземпляр параметра для передачи в алгоритм
		/// </summary>
		/// <param name="entryPoint">Информация о методе</param>
		/// <returns></returns>
		private object CreateInputInstance(MethodInfo entryPoint)
		{
			var parameterInfo = entryPoint.GetParameters().FirstOrDefault();

			if (parameterInfo == null)
				return null;

			return Activator.CreateInstance(parameterInfo.ParameterType);
		}

		/// <summary>
		/// Устанавливает свойства для параметра алгоритма
		/// </summary>
		/// <param name="entryPoint">Информация о методе</param>
		/// <param name="instance">Экземпляр параметра</param>
		/// <param name="parameters">Параметры для передачи в алгоритм</param>
		private void MapInputParams(MethodInfo entryPoint, object instance, AlgoParams parameters)
		{
			if (parameters == null)
				return;

			var parameterInfo = entryPoint.GetParameters().FirstOrDefault();
			var properties = parameterInfo.ParameterType.GetProperties();

			foreach (var property in properties)
			{
				if (!property.CanWrite)
					continue;

				var parameterName = parameters.Keys.FirstOrDefault(p => p.ToLower() == property.Name.ToLower());

				if (parameterName != null)
				{
					property.SetValue(instance, Convert.ChangeType(parameters[parameterName], property.PropertyType));
				}
			}
		}
	}
}