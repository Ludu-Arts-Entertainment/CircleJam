using System.Collections.Generic;

public class PlayFabDataBatcher 
{
    private List<Dictionary<string, string>> _batchedData;
    private int _counter;
    private int _groupSize;

    public PlayFabDataBatcher(int groupSize = 10)
    {
        _batchedData = new List<Dictionary<string, string>>();
        _groupSize = 10;
    }
    
    public List<Dictionary<string,string>> GetBatchedData(Dictionary<string, string> rawDataDictionary, IData data)
    {
        _batchedData.Clear();
        _counter = 0;

        foreach (var rawData in rawDataDictionary)
        {
            if (_counter % 10 == 0)
            {
                _batchedData.Add(new Dictionary<string, string>());
            }
            
            _batchedData[_counter++/_groupSize].Add(rawData.Key, rawData.Value);
        }

        return _batchedData;
    }
}