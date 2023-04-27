using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    public static TileBoard Instance;
    public Tile tilePrefab;
    public TileState[] tileStates;

    public TileGrid grid;
    private List<Tile> tiles;

    private int tileNumberMax = 4096;
    private bool waiting;

    private void Awake()
    {
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(11);
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells) {
            cell.tile = null;
        }

        foreach (var tile in tiles) {
            Destroy(tile.gameObject);
        }

        tiles.Clear();
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[9], 1024);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    public void LoadTile(int number, TileCell tileCell)
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[(int)Math.Log(number, 2) - 1], number);
        tile.Spawn(tileCell);
        tiles.Add(tile);

    }


    private void Update()
    {
        if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                Move(Vector2Int.up, 0, 1, 1, 1);
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                Move(Vector2Int.left, 1, 1, 0, 1);
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                Move(Vector2Int.down, 0, 1, grid.height - 2, -1);
            } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                Move(Vector2Int.right, grid.width - 2, -1, 0, 1);
            }
            PlayerPrefs.Save();
        }
        
    }

    private void Move(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;

        for (int x = startX; x >= 0 && x < grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < grid.height; y += incrementY)
            {
                TileCell cell = grid.GetCell(x, y);

                if (cell.occupied) {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed) {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.occupied)
            {
                if (CanMerge(tile, adjacent.tile))
                {
                    MergeTiles(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        if (b.number == tileNumberMax)
        {
            return false;
        }
        return a.number == b.number && !b.locked && b.number != tileNumberMax;
    }

    private void MergeTiles(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int number = b.number * 2;

        b.SetState(tileStates[index], number);

        GameManager.Instance.IncreaseScore(number);

        if (number == tileNumberMax/2 && GameManager.Instance.isContinue == false)
        {
            GameManager.Instance.KhanhWinTheGame();
        }
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i]) {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;

        yield return new WaitForSeconds(0.1f);

        waiting = false;

        foreach (var tile in tiles) {
            tile.locked = false;
        }

        if (tiles.Count != grid.size) {
            CreateTile();
        }

        if (KhanhIsWin())
        {
            GameManager.Instance.KhanhWinTheGame();
        }
        else
        {
            if (CheckForGameOver())
            {
                GameManager.Instance.KhanhGameOver();
            }
        }

    }

    public bool CheckForGameOver()
    {
        if (tiles.Count != grid.size) {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile)) {
                return false;
            }

            if (down != null && CanMerge(tile, down.tile)) {
                return false;
            }

            if (left != null && CanMerge(tile, left.tile)) {
                return false;
            }

            if (right != null && CanMerge(tile, right.tile)) {
                return false;
            }
        }

        return true;
    }

    public bool KhanhIsWin()
    {
        if (tiles.Count != grid.size)
        {
            return false;
        }

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].number < tileNumberMax)
            {
                return false;
            }
        }

        return true;
    }
}
