using Newtonsoft.Json;
using System.Collections.Generic;

namespace CriticalHit;

public class CritMessage
{
	[JsonProperty("ÏêÏ¸ÏûÏ¢ÉèÖÃ")]
	public Dictionary<string, int[]> Messages = new Dictionary<string, int[]>();
}
