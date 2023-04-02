using System.Collections.Generic;

namespace AllieJoe.Util
{
    //TODO: Add support for a better id generator (hash for example)
    public static class IDMapperGenerator
    {
        private static int _incrementalID;
        private static Dictionary<string, string> _map = new Dictionary<string, string>();

        public static void Reset()
        {
            _incrementalID = 0;
            _map.Clear();
        }
        
        public static string MapValueToIncrementalID(string key)
        {
            if (!_map.ContainsKey(key))
            {
                _incrementalID++;
                _map.Add(key, _incrementalID.ToString());
            } 
            
            return _map[key];
        }
    }
}