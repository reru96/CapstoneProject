using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomGenerator 
{
    private int maxIterations;
    private int roomWidthMin;
    private int roomLengthMin;

    public RoomGenerator(int maxIterations, int roomLengthMin, int roomWidthMin)
    {
        this.maxIterations = maxIterations;
        this.roomLengthMin = roomLengthMin;
        this.roomWidthMin = roomWidthMin;
    }

    public List<RoomNode> GenerateRoomsInGivenSpaces(List<Node> roomSpaces)
    {
        List<RoomNode> listToReturn = new List<RoomNode>();
        foreach (var space in roomSpaces)
        {
            Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween(
                space.BottomLeftAreaCorner, space.TopRightAreaCorner, 0.1f, 1);

            Vector2Int newTopRightPoint = StructureHelper.GenerateTopRightCornerBetween(
                space.BottomLeftAreaCorner, space.TopRightAreaCorner, 0.9f, 1);
            space.BottomLeftAreaCorner = newBottomLeftPoint;
            space.TopRightAreaCorner = newTopRightPoint;
            space.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);
            space.BottomRightAreaCorner = new Vector2Int( newTopRightPoint.x,newBottomLeftPoint.y);
            listToReturn.Add((RoomNode)space);
        }
        return listToReturn;
    }
}
