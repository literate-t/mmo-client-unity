using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor
{

#if UNITY_EDITOR
    // %(Ctrl), #(Shift), &(Alt)
    [MenuItem("Tools/GenerateMap %#&g")]
    static void GenerateMap()
    {
        GenerateByPath("Assets/Resources/Map");
        GenerateByPath("../Common/MapData");
    }

    static void GenerateByPath(string prefixPath)
    {
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");
        if (gameObjects.Length == 0)
            throw new NullReferenceException();

        foreach (GameObject gameObject in gameObjects)
        {
            Tilemap collisionMap = Util.FindChild<Tilemap>(gameObject, "Tilemap_Collision", true);
            Tilemap tileMapBase = Util.FindChild<Tilemap>(gameObject, "Tilemap_Base", true);

            using (var writer = File.CreateText($"{prefixPath}/{gameObject.name}.txt"))
            {
                writer.WriteLine(tileMapBase.cellBounds.xMin);
                writer.WriteLine(tileMapBase.cellBounds.xMax);
                writer.WriteLine(tileMapBase.cellBounds.yMin);
                writer.WriteLine(tileMapBase.cellBounds.yMax);

                for (int y = tileMapBase.cellBounds.yMax; y >= tileMapBase.cellBounds.yMin; --y)
                {
                    for (int x = tileMapBase.cellBounds.xMin; x <= tileMapBase.cellBounds.xMax; ++x)
                    {
                        TileBase tile = collisionMap.GetTile(new Vector3Int(x, y));
                        if (tile != null)
                            writer.Write("1");
                        else
                            writer.Write("0");
                    }
                    writer.WriteLine();
                }
            }
        }
    }
#endif
}
