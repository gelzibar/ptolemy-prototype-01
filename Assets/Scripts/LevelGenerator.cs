using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int width;
    public int length;

    void Start()
    {
        // var prefab = Resources.Load<GameObject>("Prefabs/Chunk");
        // var obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        // var chunkWidth = obj.GetComponent<TerrainChunk>().width;
        var chunkWidth = TerrainChunk.width;
        // Destroy(obj);

        var widthCount = width / chunkWidth;
        var lengthCount = length / chunkWidth;
        // Debug.Log($"Chunk width={TerrainChunk.width}, LG width={width}");
        // Debug.Log($"Chunk length={TerrainChunk.width}, LG width={length}");

        var total = widthCount * lengthCount;

        for (var w = 0; w < widthCount; w++)
        {
            for (var l = 0; l < lengthCount; l++)
            {
                Debug.Log($"{w}, {l}");
                var x = w * chunkWidth;
                var z = l * chunkWidth;
                var pos = new Vector3(x, 0, z);
                var pos2d = new Vector2(w, l);
                var prefab = Resources.Load<GameObject>("Prefabs/Chunk");
                var obj = Instantiate(prefab, pos, Quaternion.identity);
                obj.GetComponent<TerrainChunk>().SetGridPosition(pos2d);
            }
        }


    }

    void Update()
    {

    }
}