using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager
{
	// TODO : Load 호출이 많을 겨우 딕셔너리 생성하여 관리
	public Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

    public void Init()
    {
	}

	public T Load<T>(string path) where T : Object
	{
		// 찾으려는 타입이 GameObject일 경우 Pool에서 찾음.
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');  // '/' 문자까지의 문자열 개수 반환
            if (index >= 0)
                name.Substring(index + 1);      // name을 index+1 문자열 위치에서 반환

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go.IsNull() == false)
                return go as T;
        }
		else if (typeof(T) == typeof(Sprite))
		{
			if (_sprites.TryGetValue(path, out Sprite sprite))
				return sprite as T;

			Sprite sp = Resources.Load<Sprite>(path);
			_sprites.Add(path, sp);
			return sp as T;
		}

		return Resources.Load<T>(path);
	}

	public GameObject Instantiate(string path, Transform parent = null)
	{
		GameObject prefab = Load<GameObject>($"Prefabs/{path}");
		if (prefab == null)
		{
			Debug.Log($"Failed to load prefab : {path}");
			return null;
		}

		return Instantiate(prefab, parent);
	}

	public GameObject Instantiate(GameObject prefab, Transform parent = null)
	{
		// 풀링이 적용된 객체인지 확인
        if (prefab.GetComponent<Poolable>().IsNull() == false)
            return Managers.Pool.Pop(prefab, parent).gameObject;

		GameObject go = Object.Instantiate(prefab, parent);
		go.name = prefab.name;

		return go;
	}

	public void Destroy(GameObject go)
	{
		if (go == null)
			return;

		// 만약에 풀링이 필요한 아이라면 PoolManager한테 위탁
        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable.IsNull() == false)
        {
            Managers.Pool.Push(poolable);
            return;
        }

		Object.Destroy(go);
	}
}