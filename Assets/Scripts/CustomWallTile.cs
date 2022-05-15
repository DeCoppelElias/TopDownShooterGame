using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Tilemaps.Tile;

[CreateAssetMenu(fileName = "new customWallTile", menuName = "customWallTile")]
public class CustomWallTile : TileBase
{
    public Sprite EdgeSprite;
    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        for (int yd = -1; yd <= 1; yd++)
        {
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (tilemap.GetTile(position))
                {
                    tilemap.RefreshTile(position);
                }
            }
        }
    }

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = EdgeSprite;
        tileData.colliderType = ColliderType.Sprite;
        if (tilemap.GetTile(location + new Vector3Int(0, 1, 0)) || tilemap.GetTile(location + new Vector3Int(0, -1, 0)))
        {
            Quaternion angle = Quaternion.Euler(0, 0, -90f);
            var m = tileData.transform;
            m.SetTRS(Vector3.zero, angle, Vector3.one);
            tileData.transform = m;
        }
        tileData.flags = TileFlags.LockTransform;
    }
}
