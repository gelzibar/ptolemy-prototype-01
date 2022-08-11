using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainChunk : MonoBehaviour
{
    public const int width = 16;
    public const int height = 64;

    // These seems to build the vertices from a corner vertex, VERSUS building the vertics from a central point
    // (top/bot), (front/back), (right/left)
    Vector3 tfl = Vector3.up;
    Vector3 tfr = Vector3.up + Vector3.right;
    Vector3 tbl = Vector3.up + Vector3.forward;
    Vector3 tbr = Vector3.up + Vector3.forward + Vector3.right;

    Vector3 bfl = Vector3.zero;
    Vector3 bfr = Vector3.right;
    Vector3 bbl = Vector3.forward;
    Vector3 bbr = Vector3.forward + Vector3.right;

    Vector3[] topVerts;
    Vector3[] botVerts;
    Vector3[] frontVerts;
    Vector3[] backVerts;
    Vector3[] rightVerts;
    Vector3[] leftVerts;

    public BlockType[,,] blocks = new BlockType[width, height, width];
    [SerializeField]
    private int blockCount;
    [SerializeField]
    private int dirtCount;

    [SerializeField]
    private List<int> heights;

    // Global Position
    Vector2 gridPos;

    Mesh mesh;
    void Start()
    {
        heights = new List<int>();
        // Populate the arrays to represent cube sides.
        // The order of the vertices in the array is important.
        // Changing the order will effect triangles when building the mesh
        topVerts = new Vector3[] { tfl, tbl, tbr, tfr };
        botVerts = new Vector3[] { bfl, bfr, bbr, bbl };
        frontVerts = new Vector3[] { bfl, tfl, tfr, bfr };
        backVerts = new Vector3[] { bbr, tbr, tbl, bbl };
        rightVerts = new Vector3[] { bfr, tfr, tbr, bbr };
        leftVerts = new Vector3[] { bbl, tbl, tfl, bfl };

        mesh = GetComponent<MeshFilter>().mesh;

        SeedBlocks();
        BuildMesh();
    }

    void Update()
    {
        blockCount = blocks.Length;
        dirtCount = 0;
        foreach (var blk in blocks)
        {
            if (blk == BlockType.Dirt)
            {
                dirtCount++;
            }
        }
    }

    public void SetGridPosition(Vector2 pos)
    {
        gridPos = pos;
    }

    void SeedBlocks()
    {

        for (var x = 0; x < width; x++)
        {
            for (var z = 0; z < width; z++)
            {
                // var adjust = Mathf.RoundToInt(Random.Range(-5.0f, 5.0f));
                // var xChunkMultiplier = gridPos.x;
                // var zChunkMultiplier = gridPos.y;
                // var xFactor = (float)(x + xChunkMultiplier) / (float)width * 4;
                // var zFactor = (float)(z + zChunkMultiplier) / (float)width;
                var globalX = ((float)x + (float)gridPos.x * (float)width) / (5 * 16);
                Debug.Log($"x: {gridPos.x} + {width} = {globalX}");
                var globalZ = ((float)z + (float)gridPos.y * (float)width) / (5 * 16);
                Debug.Log($"global: {globalX}, {globalZ}");
                var perlinMultiplier = Mathf.PerlinNoise(globalX, globalZ);
                // Debug.Log($"perlin: {perlinMultiplier}");
                var targetHeight = Mathf.RoundToInt(perlinMultiplier * height);
                heights.Add(targetHeight);
                Debug.Log($"height: {targetHeight}");
                // var baseHeight = height / 2;
                for (var y = 0; y < height; y++)
                {
                    if (y <= targetHeight)
                    {
                        blocks[x, y, z] = BlockType.Dirt;
                    }
                    else
                    {
                        blocks[x, y, z] = BlockType.Air;
                    }
                }
            }
        }
    }

    List<Vector3> BuildSide(Vector3 pos, Vector3[] template)
    {
        var side = new List<Vector3>();
        foreach (var vertex in template)
        {
            side.Add(pos + vertex);
        }
        return side;
    }

    void BuildMesh()
    {
        mesh.Clear();

        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();

        var indexOffset = 0;

        for (var x = 0; x < width; x++)
        {
            for (var z = 0; z < width; z++)
            {
                for (var y = 0; y < height; y++)
                {
                    indexOffset = verts.Count;
                    if (blocks[x, y, z] != BlockType.Air)
                    {
                        // var blockPos = new Vector3(x - 1, y, z - 1);
                        var blockPos = new Vector3(x, y, z);
                        int numFaces = 0;

                        // top
                        if (y < height - 1 && blocks[x, y + 1, z] == BlockType.Air)
                        {
                            verts.AddRange(BuildSide(blockPos, topVerts));
                            numFaces++;

                            uvs.AddRange(GetUVs(0, 0));
                        }
                        // bottom
                        // Render when exposed to air OR
                        // Render when at the bottom of the chunk
                        if ((y > 0 && blocks[x, y - 1, z] == BlockType.Air) ||
                            y == 0)
                        {
                            verts.AddRange(BuildSide(blockPos, botVerts));
                            numFaces++;

                            uvs.AddRange(GetUVs(0, 0));
                        }
                        // front
                        // Render when exposed to air OR
                        // when at chunk boundary
                        if ((z > 0 && blocks[x, y, z - 1] == BlockType.Air) ||
                            z == 0)
                        {
                            verts.AddRange(BuildSide(blockPos, frontVerts));
                            numFaces++;

                            uvs.AddRange(GetUVs(0, 0));
                        }
                        // back
                        // Render when exposed to air OR
                        // when at chunk boundary
                        if (((z + 1 < blocks.GetLength(2)) && blocks[x, y, z + 1] == BlockType.Air) ||
                            z == width - 1)
                        {
                            verts.AddRange(BuildSide(blockPos, backVerts));
                            numFaces++;

                            uvs.AddRange(GetUVs(0, 0));
                        }
                        // left
                        // Render when exposed to air OR
                        // when at chunk boundary
                        if ((x > 0 && blocks[x - 1, y, z] == BlockType.Air) ||
                            x == 0)
                        {
                            verts.AddRange(BuildSide(blockPos, leftVerts));
                            numFaces++;

                            uvs.AddRange(GetUVs(0, 0));
                        }
                        // right
                        // Render when exposed to air OR
                        // when at chunk boundary
                        if ((x + 1 < width && blocks[x + 1, y, z] == BlockType.Air) ||
                            x == width - 1)
                        {
                            verts.AddRange(BuildSide(blockPos, rightVerts));
                            numFaces++;

                            uvs.AddRange(GetUVs(0, 0));
                        }

                        for (var i = 0; i < numFaces; i++)
                        {
                            var newTris = new int[] {
                                indexOffset + i * 4,
                                indexOffset + i * 4 + 1,
                                indexOffset + i * 4 + 2,
                                indexOffset + i * 4,
                                indexOffset + i * 4 + 2,
                                indexOffset + i * 4 + 3
                            };
                            tris.AddRange(newTris);
                        }
                    }
                }
            }
        }
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        var collider = GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }

    Vector2[] GetUVs(float x, float y)
    {
        var textureSize = 64f;
        var toPercent = 1;
        var uvs = new Vector2[]
        {

            new Vector2(x/textureSize + toPercent, y/textureSize + toPercent),
            new Vector2(x/textureSize + toPercent, (y+1)/textureSize - toPercent),
            new Vector2((x+1)/textureSize - toPercent, (y+1)/textureSize - toPercent),
            new Vector2((x+1)/textureSize - toPercent, y/textureSize + toPercent),
        };
        return uvs;
    }

    public void ResolveHit(RaycastHit hit, Transform ship)
    {

        var pointInTargetBlock = hit.point + ship.forward * .01f;
        int bix = Mathf.FloorToInt(pointInTargetBlock.x);// - chunkPosX + 1;
        int biy = Mathf.FloorToInt(pointInTargetBlock.y);
        int biz = Mathf.FloorToInt(pointInTargetBlock.z);// - chunkPosZ + 1;
        Debug.Log($"capture: {bix}, {biy}, {biz}");

        //Convert World Coordinates into local x, y, z
        Debug.Log($"grid: {(int)(gridPos.x)}, {(int)(gridPos.y)}");
        Debug.Log($"width: {width}");
        Debug.Log($"multi: {(int)(gridPos.x * width)}, {(int)(gridPos.y * width)}");
        // Debug.Log($"floortoint: {Mathf.FloorToInt(gridPos.x * width)}, {Mathf.FloorToInt(gridPos.y * width)}");
        bix = bix - (int)(gridPos.x * width);
        biz = biz - (int)(gridPos.y * width);


        Debug.Log($"{bix}, {biy}, {biz}");
        Debug.Log($"{blocks[bix, biy, biz]}");
        blocks[bix, biy, biz] = BlockType.Air;
        var audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioSource.clip, 1.0f);


        BuildMesh();
    }
}
