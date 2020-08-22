using System.Collections.Generic;
using System.Text.Json;

namespace Application.Core.Helpers
{
	public class JsonHelper
	{
		public static IEnumerable<TResult> DeserializeIEnumerable<TResult>(string jsonString)
		{
			IEnumerable<TResult> result = new List<TResult>();

			if (!string.IsNullOrEmpty(jsonString))
			{
				result = JsonSerializer.Deserialize<IEnumerable<TResult>>(jsonString);
			}

			return result;
		}

		public static string SerializeObject(object serializingObject)
		{
			var result = JsonSerializer.Serialize(serializingObject);

			return result;
		}
	}
}
