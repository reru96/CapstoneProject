using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorNode : Node
{
    private Node structure1;
    private Node structure2;
    private int corridorWidth;
    private int modifierDistanceFromWall = 1;

    public CorridorNode(Node node1, Node node2, int corridorWidth) : base(null)
    {
        this.structure1 = node1;
        this.structure2 = node2;
        this.corridorWidth = corridorWidth;
        GenerateCorridor();
    }

    private void GenerateCorridor()
    {
        var relativePositionOfStructure2 = CheckPositionStructure2AgainstStructure();
        switch (relativePositionOfStructure2)
        {
            case RelativePosition.Up:
                ProcessRoomInRelationUpOrDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Down:
                ProcessRoomInRelationRightOrLeft(this.structure2, this.structure1);
                break;
            case RelativePosition.Right:
                ProcessRoomInRelationUpOrDown(this.structure1, this.structure2);
                break;
            case RelativePosition.Left:
                ProcessRoomInRelationRightOrLeft(this.structure2, this.structure1);
                break;
            default:
                break;
        }
    }

    private void ProcessRoomInRelationUpOrDown(Node structure1, Node structure2)
    {
        var bottomLeaves = StructureHelper.TraverseGraphToExtractLowestLeafes(structure1);
        var topLeaves = StructureHelper.TraverseGraphToExtractLowestLeafes(structure2);

        var sortedBottom = bottomLeaves.OrderByDescending(c => c.TopLeftAreaCorner.y).ToList();
        int maxY = sortedBottom[0].TopLeftAreaCorner.y;
        var candidatesBottom = sortedBottom.Where(c => Mathf.Abs(c.TopLeftAreaCorner.y - maxY) < 10).ToList();

        var bottomStructure = candidatesBottom[UnityEngine.Random.Range(0, candidatesBottom.Count)];

        var possibleTop = topLeaves
            .Select(t => new { Node = t, X = GetValidXForNeighbourUpDown(bottomStructure.TopLeftAreaCorner, 
            bottomStructure.TopRightAreaCorner, 
            t.BottomLeftAreaCorner, 
            t.BottomRightAreaCorner) })
            .Where(item => item.X != -1)
            .OrderBy(item => item.Node.BottomRightAreaCorner.y)
            .ToList();

        var topStructure = possibleTop.FirstOrDefault()?.Node ?? structure2;

        int x = GetValidXForNeighbourUpDown(bottomStructure.TopLeftAreaCorner, 
            bottomStructure.TopRightAreaCorner, 
            topStructure.BottomLeftAreaCorner, 
            topStructure.BottomRightAreaCorner);

        BottomLeftAreaCorner = new Vector2Int(x, bottomStructure.TopLeftAreaCorner.y);
        TopRightAreaCorner = new Vector2Int(x + corridorWidth, topStructure.BottomLeftAreaCorner.y);
    }
    private void ProcessRoomInRelationRightOrLeft(Node structure2, Node structure1)
    {
        var leftLeaves = StructureHelper.TraverseGraphToExtractLowestLeafes(structure1);
        var rightLeaves = StructureHelper.TraverseGraphToExtractLowestLeafes(structure2);

        var sortedLeft = leftLeaves.OrderByDescending(c => c.TopRightAreaCorner.x).ToList();
        int maxX = sortedLeft[0].TopRightAreaCorner.x;
        var candidatesLeft = sortedLeft.Where(c => Math.Abs(c.TopRightAreaCorner.x - maxX) < 10).ToList();
        var leftStructure = candidatesLeft[UnityEngine.Random.Range(0, candidatesLeft.Count)];

        var possibleRight = rightLeaves
            .Select(r => new { Node = r, Y = GetValidYForNeighourLeftRight(leftStructure.TopRightAreaCorner, 
            leftStructure.BottomRightAreaCorner, 
            r.TopLeftAreaCorner, 
            r.BottomLeftAreaCorner) })
            .Where(item => item.Y != -1)
            .OrderBy(item => item.Node.BottomRightAreaCorner.x)
            .ToList();

        var rightStructure = possibleRight.FirstOrDefault()?.Node ?? structure2;

        int y = GetValidYForNeighourLeftRight(leftStructure.TopLeftAreaCorner,
            leftStructure.BottomRightAreaCorner,
            rightStructure.TopLeftAreaCorner,
            rightStructure.BottomLeftAreaCorner);

        BottomLeftAreaCorner = new Vector2Int(leftStructure.BottomRightAreaCorner.x, y);
        TopRightAreaCorner = new Vector2Int(rightStructure.TopLeftAreaCorner.x, y + corridorWidth);
    }

    private int GetValidXForNeighbourUpDown(Vector2Int bottomNodeLeft,
        Vector2Int bottomNodeRight, Vector2Int topNodeLeft, Vector2Int topNodeRight)
    {
        if (topNodeLeft.x < bottomNodeLeft.x && bottomNodeRight.x < topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)
                ).x;
        }
        if (topNodeLeft.x >= bottomNodeLeft.x && bottomNodeRight.x >= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)
                ).x;
        }
        if (bottomNodeLeft.x >= (topNodeLeft.x) && bottomNodeLeft.x <= topNodeRight.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                bottomNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                topNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)

                ).x;
        }
        if (bottomNodeRight.x <= topNodeRight.x && bottomNodeRight.x >= topNodeLeft.x)
        {
            return StructureHelper.CalculateMiddlePoint(
                topNodeLeft + new Vector2Int(modifierDistanceFromWall, 0),
                bottomNodeRight - new Vector2Int(this.corridorWidth + modifierDistanceFromWall, 0)

                ).x;
        }
        return -1;
    }


    private int GetValidYForNeighourLeftRight(Vector2Int leftNodeUp, Vector2Int leftNodeDown , Vector2Int rightNodeUp, Vector2Int rightNodeDown)
    {
        if(rightNodeUp.y >= leftNodeUp.y && leftNodeDown.y >= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
             leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
             leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
             ).y;
        }

        if(rightNodeUp.y <= leftNodeDown.y && leftNodeDown.y <= rightNodeDown.y)
        {
            return StructureHelper.CalculateMiddlePoint(
             rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
             rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
             ).y;
        }

        if(leftNodeUp.y >= rightNodeDown.y && leftNodeUp.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
              rightNodeDown + new Vector2Int(0, modifierDistanceFromWall),
              leftNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
              ).y;
        }

        if(leftNodeDown.y >= rightNodeDown.y && leftNodeDown.y <= rightNodeUp.y)
        {
            return StructureHelper.CalculateMiddlePoint(
             leftNodeDown + new Vector2Int(0, modifierDistanceFromWall),
             rightNodeUp - new Vector2Int(0, modifierDistanceFromWall + this.corridorWidth)
             ).y;
        }
        return -1;
    }


    private RelativePosition CheckPositionStructure2AgainstStructure()
    {
        Vector2 middlePointStructure1Temp = ((Vector2)structure1.TopRightAreaCorner + structure1.BottomLeftAreaCorner) / 2;
        Vector2 middlePointStructure2Temp = ((Vector2)structure2.TopRightAreaCorner + structure2.BottomLeftAreaCorner) / 2;
        float angle = CalculateAngle(middlePointStructure1Temp, middlePointStructure2Temp);
        if (angle < 45 && angle > 0 || (angle > -45 && angle < 0))
        {
            return RelativePosition.Right;

        }
        else if (angle > 45 && angle < 135)
        {
            return RelativePosition.Up;

        }
        else if (angle > -135 && angle < -45)
        {
            return RelativePosition.Down;
        }
        else
        {
            return RelativePosition.Left;
        }


    }

    private float CalculateAngle(Vector2 middlePointStructure1Temp, Vector2 middlePointStructure2Temp)
    {
        return Mathf.Atan2(middlePointStructure2Temp.y - middlePointStructure1Temp.y, middlePointStructure2Temp.x - middlePointStructure1Temp.x) * Mathf.Rad2Deg;
    }
}


