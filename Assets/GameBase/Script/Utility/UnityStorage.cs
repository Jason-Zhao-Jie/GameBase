using GameBase.Common.PlatformInterface;

public class UnityStorage : IQuickStorage
{
    public void Clear()
    {
        UnityEngine.PlayerPrefs.DeleteAll();
    }

    public T GetItem<T>(string key)
    {
        var json = UnityEngine.PlayerPrefs.GetString(key);
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }
        return UnityEngine.JsonUtility.FromJson<T>(json);
    }

    public void SetItem<T>(string key, T value)
    {
        UnityEngine.PlayerPrefs.SetString(key, UnityEngine.JsonUtility.ToJson(value));
    }
}