using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OGA.AppSettings.Writeable.JSONConfig
{
	public class cJSONFlattener
	{
		private readonly IDictionary<string, string> _data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		private readonly Stack<string> _context = new Stack<string>();
		private string _currentPath;

		private cJSONFlattener() { }

		static public IDictionary<string, string> Parse(object o)
		{
			// Convert the object to a JObject so we can parse it out...
			JObject jo = (JObject)JToken.FromObject(o);

			return new cJSONFlattener().ParseJObject(jo);
		}
		static public IDictionary<string, string> Parse(JObject jo)
			=> new cJSONFlattener().ParseJObject(jo);

		private IDictionary<string, string> ParseJObject(JObject jo)
		{
			_data.Clear();

			VisitJObject(jo);

			return _data;
		}

		private void VisitJObject(JObject jObject)
		{
			foreach (var property in jObject.Properties())
			{
				EnterContext(property.Name);
				VisitProperty(property);
				ExitContext();
			}
		}

		private void VisitProperty(JProperty property)
		{
			VisitToken(property.Value);
		}

		private void VisitToken(JToken token)
		{
			switch (token.Type)
			{
				case JTokenType.Object:
					VisitJObject(token.Value<JObject>());
					break;

				case JTokenType.Array:
					VisitArray(token.Value<JArray>());
					break;

				case JTokenType.String:
				case JTokenType.Integer:
				case JTokenType.Float:
				case JTokenType.Boolean:
				case JTokenType.Bytes:
				case JTokenType.Raw:
				case JTokenType.Guid:
				case JTokenType.Null:
					VisitPrimitive(token.Value<JValue>());
					break;

				case JTokenType.Date:
				case JTokenType.TimeSpan:
				case JTokenType.Uri:
					VisitSerializablePrimitive(token.Value<JValue>());
					break;

				default:
					throw new FormatException(Resources.FormatError_UnsupportedJSONTokenType(token.Type));
			}
		}

		private void VisitSerializablePrimitive(JValue data)
		{
			var key = _currentPath;

			if (_data.ContainsKey(key))
			{
				throw new FormatException(Resources.FormatError_KeyIsDuplicated(key));
			}

			var jsonValue = JsonConvert.SerializeObject(data.Value);
			_data[key] = jsonValue.Replace("\"", "");
		}

		private void VisitArray(JArray array)
		{
			for (int index = 0; index < array.Count; index++)
			{
				EnterContext(index.ToString());
				VisitToken(array[index]);
				ExitContext();
			}
		}

		private void VisitPrimitive(JValue data)
		{
			var key = _currentPath;

			if (_data.ContainsKey(key))
			{
				throw new FormatException(Resources.FormatError_KeyIsDuplicated(key));
			}
			_data[key] = data.ToString(CultureInfo.InvariantCulture);
		}

		private void EnterContext(string context)
		{
			_context.Push(context);
			_currentPath = Microsoft.Extensions.Configuration.ConfigurationPath.Combine(_context.Reverse());
		}

		private void ExitContext()
		{
			_context.Pop();
			_currentPath = Microsoft.Extensions.Configuration.ConfigurationPath.Combine(_context.Reverse());
		}
	}
}