using System.Collections.Generic;
using System.Linq;
using ChessModel;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    private TileScript[] tileList;
    private BoardManager boardManager;

    private void Awake()
    {
        tileList = gameObject.GetComponentsInChildren<TileScript>();
        boardManager = gameObject.GetComponentInParent<BoardManager>();
    }

    public void clickTile(int tilePlacement)
    {
        boardManager.ClickTile(tilePlacement);
    }


    public GameObject getTile(int position)
    {
        return tileList[position].gameObject;
    }

    public Vector3 getCoordinatesByTilePlacement(int position)
    {
        Vector3 tileCoord = getTile(position).transform.position;
        tileCoord.x += (int) (5 * transform.localScale.x);
        tileCoord.y += (int) (5 * transform.localScale.y);
        tileCoord.z += (int) (5 * transform.localScale.z);
        return tileCoord;
    }

    public void updateLegalMoves(List<Move> moves)
    {
        foreach (var tile in tileList)
        {
            tile.UnHighlightTile();
        }

        foreach (var position in moves.Select(move => move.EndPosition))
        {
            getTile(position).GetComponent<TileScript>().HighlightTile();
        }
    }
}
