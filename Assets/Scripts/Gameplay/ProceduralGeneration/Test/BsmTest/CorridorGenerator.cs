using System;
using System.Collections;
using System.Collections.Generic;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;
using System.Linq;

public class CorridorGenerator 
{
    public List<Node> CreateCorridor(List<RoomNode> allNodesCollection, int corridorWidth)
    {
        List<Node> corridorList = new List<Node>();

        var sortedNodes = allNodesCollection.OrderByDescending(node => node.TreeLayerIndex);

        foreach (var node in sortedNodes)
        {
            if (node.ChildrenNodeList.Count < 2)
                continue;

            CorridorNode corridor = new CorridorNode(node.ChildrenNodeList[0], node.ChildrenNodeList[1], corridorWidth);
            corridorList.Add(corridor);
        }

        return corridorList;
    }
}
