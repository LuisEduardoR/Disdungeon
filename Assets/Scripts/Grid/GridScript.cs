using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{

    public static GridScript instance;

    [SerializeField] private Vector2 originPos = new Vector2(0,2.5f);
    [SerializeField] private Vector2 nodeSize = new Vector2(0.5f,0.5f);   
    private GridObject[,] gridNodes;

    private void Start() {

        if(instance != null) {

            Debug.Log("GridScript: More than one instance of this script was found!");
            return;

        }

        instance = this;

        gridNodes = new GridObject[6,4];

    }

    // Inserts an GridObject at a position on the grid.
    public bool InsertObject(GridObject obj, Vector2Int pos) {

        if(gridNodes[pos.x, pos.y] != null) {

            Debug.Log("GridScript: Position " + pos + " already occupied!");
            return false;

        }

        gridNodes[pos.x, pos.y] = obj;
        PositionObject(obj, pos);        

        return true;

    }

    // Sets the position of an GridObject.transform to a position relative to the grid.
    private void PositionObject(GridObject obj, Vector2Int pos) {

        if(obj == null)
            return;

        obj.gridPosition.x = pos.x;
        obj.gridPosition.y = pos.y;

        Vector3 newPos = new Vector3();

        newPos.x = originPos.x - (2.5f * nodeSize.x) + (pos.x * nodeSize.x);
        newPos.y = originPos.y - (1.5f * nodeSize.x) + (pos.y * nodeSize.y);
        newPos.z = 0;

        obj.transform.position = newPos;

    }

     // Removes an GridObject from a position on the grid.
    public GridObject RemoveObject(Vector2Int pos) {

        if(gridNodes[pos.x, pos.y] != null) {

            GridObject obj = gridNodes[pos.x, pos.y];
            gridNodes[pos.x, pos.y] = null;
            return obj;

        }

        return null;

    }

    public GridObject GetNodeContent(Vector2Int pos) {

        return gridNodes[pos.x, pos.y];

    }

    public void SwapNodes(Vector2Int n1, Vector2Int n2) {

        GridObject temp = gridNodes[n1.x, n1.y];
        gridNodes[n1.x, n1.y] = gridNodes[n2.x, n2.y];
        gridNodes[n2.x, n2.y] = temp;

        if(GetNodeContent(n1) != null) {
            PositionObject(GetNodeContent(n1), n1);
        }
        if(GetNodeContent(n2) != null)
            PositionObject(GetNodeContent(n2), n2);        

    }

    public void ClearGrid() {

        for(int i = 0; i < 4; i++) {

             for(int j = 0; j < 6; j++) {

                if(gridNodes[j, i] != null) {

                    if(gridNodes[j, i].GetType() != typeof(PlayerScript) && gridNodes[j, i].GetType() != typeof(LevelExit))
                        Destroy(gridNodes[j, i].gameObject);

                    gridNodes[j, i] = null;

                }
            
            }

        }

    }

}
