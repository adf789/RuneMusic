using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableAssets
{
    private static AddressableAssets instance;
    public static AddressableAssets Instance
    {
        get
        {
            if (instance == null) instance = new AddressableAssets();
            return instance;
        }
    }
    private Dictionary<EnumList.IMAGENAMES, AsyncOperationHandle> loadedAddressables;

    private AddressableAssets()
    {
        loadedAddressables = new Dictionary<EnumList.IMAGENAMES, AsyncOperationHandle>();
    }

    public AsyncOperationHandle LoadSprite(EnumList.IMAGENAMES name)
    {
        AsyncOperationHandle handle;
        if (loadedAddressables.TryGetValue(name, out handle)) return handle;

        handle = Addressables.LoadAssetAsync<Sprite>(name.ToString());

        loadedAddressables.Add(name, handle);
        return handle;
    }

    public void Clear()
    {
        foreach (AsyncOperationHandle handle in loadedAddressables.Values)
        {
            Addressables.Release(handle);
        }

        loadedAddressables.Clear();
    }

    public void Release(EnumList.IMAGENAMES name)
    {
        AsyncOperationHandle handle;
        if (!loadedAddressables.TryGetValue(name, out handle)) return;

        Addressables.Release(handle);
        loadedAddressables.Remove(name);
    }
}
