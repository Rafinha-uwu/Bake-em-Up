using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeBalconyMesh : MonoBehaviour
{
    [SerializeField]
    private MeshFilter _recipeMeshFilter;

    [Serializable]
    public class RecipeMeshAndCount
    {
        public int minCount;
        public Mesh recipeMesh;
    }

    [SerializeField]
    private List<RecipeMeshAndCount> _meshCount = new();

    public void CheckMeshCount(int count)
    {
		RecipeMeshAndCount auxItem = null;
        foreach(RecipeMeshAndCount item in _meshCount)
        {
            if(count >= item.minCount && (auxItem.IsUnityNull() || !auxItem.IsUnityNull() && item.minCount > auxItem.minCount))
            {
                auxItem = item;
            }
        }

        if(!auxItem.IsUnityNull())
		    _recipeMeshFilter.mesh = auxItem.recipeMesh;
	}
}
