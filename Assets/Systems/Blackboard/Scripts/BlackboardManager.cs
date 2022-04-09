using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBlackboardKey
{
    Character_Stat_Energy,
    Character_Stat_Fun,
    Character_FocusObject,


    Household_ObjectsInUse
}

public class Blackboard
{
    Dictionary<EBlackboardKey, int>         IntValues           = new Dictionary<EBlackboardKey, int>();
    Dictionary<EBlackboardKey, float>       FloatValues         = new Dictionary<EBlackboardKey, float>();
    Dictionary<EBlackboardKey, bool>        BoolValues          = new Dictionary<EBlackboardKey, bool>();
    Dictionary<EBlackboardKey, string>      StringValues        = new Dictionary<EBlackboardKey, string>();
    Dictionary<EBlackboardKey, Vector3>     Vector3Values       = new Dictionary<EBlackboardKey, Vector3>();
    Dictionary<EBlackboardKey, GameObject>  GameObjectValues    = new Dictionary<EBlackboardKey, GameObject>();
    Dictionary<EBlackboardKey, object>      GenericValues       = new Dictionary<EBlackboardKey, object>();

    public void SetGeneric<T>(EBlackboardKey key, T value)
    {
        GenericValues[key] = value;
    }

    public T GetGeneric<T>(EBlackboardKey key)
    {
        if (!GenericValues.ContainsKey(key))
            throw new System.ArgumentException($"Could not find value for {key} in GenericValues");

        return (T)GenericValues[key];
    }

    public bool TryGetGeneric<T>(EBlackboardKey key, out T value, T defaultValue)
    {
        if (GenericValues.ContainsKey(key))
        {
            value = (T)GenericValues[key];
            return true;
        }

        value = defaultValue;
        return false;
    }

    public void Set(EBlackboardKey key, int value)
    {
        IntValues[key] = value;
    }

    public int GetInt(EBlackboardKey key)
    {
        if (!IntValues.ContainsKey(key))
            throw new System.ArgumentException($"Could not find value for {key} in IntValues");

        return IntValues[key];
    }

    public bool TryGet(EBlackboardKey key, out int value, int defaultValue = 0)
    {
        if (IntValues.ContainsKey(key))
        {
            value = IntValues[key];
            return true;
        }

        value = defaultValue;
        return false;
    }

    public void Set(EBlackboardKey key, float value)
    {
        FloatValues[key] = value;
    }

    public float GetFloat(EBlackboardKey key)
    {
        if (!FloatValues.ContainsKey(key))
            throw new System.ArgumentException($"Could not find value for {key} in FloatValues");

        return FloatValues[key];
    }

    public bool TryGet(EBlackboardKey key, out float value, float defaultValue = 0)
    {
        if (FloatValues.ContainsKey(key))
        {
            value = FloatValues[key];
            return true;
        }

        value = defaultValue;
        return false;
    }

    public void Set(EBlackboardKey key, bool value)
    {
        BoolValues[key] = value;
    }

    public bool GetBool(EBlackboardKey key)
    {
        if (!BoolValues.ContainsKey(key))
            throw new System.ArgumentException($"Could not find value for {key} in BoolValues");

        return BoolValues[key];
    }

    public bool TryGet(EBlackboardKey key, out bool value, bool defaultValue = false)
    {
        if (BoolValues.ContainsKey(key))
        {
            value = BoolValues[key];
            return true;
        }

        value = defaultValue;
        return false;
    }

    public void Set(EBlackboardKey key, string value)
    {
        StringValues[key] = value;
    }

    public string GetString(EBlackboardKey key)
    {
        if (!StringValues.ContainsKey(key))
            throw new System.ArgumentException($"Could not find value for {key} in StringValues");

        return StringValues[key];
    }

    public bool TryGet(EBlackboardKey key, out string value, string defaultValue = "")
    {
        if (IntValues.ContainsKey(key))
        {
            value = StringValues[key];
            return true;
        }

        value = defaultValue;
        return false;
    }

    public void Set(EBlackboardKey key, Vector3 value)
    {
        Vector3Values[key] = value;
    }

    public Vector3 GetVector3(EBlackboardKey key)
    {
        if (!Vector3Values.ContainsKey(key))
            throw new System.ArgumentException($"Could not find value for {key} in Vector3Values");

        return Vector3Values[key];
    }

    public bool TryGet(EBlackboardKey key, out Vector3 value, Vector3 defaultValue)
    {
        if (Vector3Values.ContainsKey(key))
        {
            value = Vector3Values[key];
            return true;
        }

        value = defaultValue;
        return false;
    }

    public void Set(EBlackboardKey key, GameObject value)
    {
        GameObjectValues[key] = value;
    }

    public GameObject GetGameObject(EBlackboardKey key)
    {
        if (!GameObjectValues.ContainsKey(key))
            throw new System.ArgumentException($"Could not find value for {key} in GameObjectValues");

        return GameObjectValues[key];
    }

    public bool TryGet(EBlackboardKey key, out GameObject value, GameObject defaultValue = null)
    {
        if (GameObjectValues.ContainsKey(key))
        {
            value = GameObjectValues[key];
            return true;
        }

        value = defaultValue;
        return false;
    }
}

public class BlackboardManager : MonoBehaviour
{
    public static BlackboardManager Instance { get; private set; } = null;

    Dictionary<MonoBehaviour, Blackboard> IndividualBlackboards = new Dictionary<MonoBehaviour, Blackboard>();
    Dictionary<int, Blackboard> SharedBlackboards = new Dictionary<int, Blackboard>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Trying to create second BlackboardManager on {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public Blackboard GetIndividualBlackboard(MonoBehaviour requestor)
    {
        if (!IndividualBlackboards.ContainsKey(requestor))
            IndividualBlackboards[requestor] = new Blackboard();

        return IndividualBlackboards[requestor];
    }

    public Blackboard GetSharedBlackboard(int uniqueID)
    {
        if (!SharedBlackboards.ContainsKey(uniqueID))
            SharedBlackboards[uniqueID] = new Blackboard();

        return SharedBlackboards[uniqueID];
    }
}
