using LinqToDB.Common;
using Newtonsoft.Json;

namespace CaiBotLite;

public class JsonConverter<T>() : ValueConverter<List<T>, string>(v => JsonConvert.SerializeObject(v),
    s => JsonConvert.DeserializeObject<List<T>>(s) ?? new List<T>(),
    true);