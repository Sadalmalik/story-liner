using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
	public string              id;
	public List<string>        nextNodes;
	public List<NodeBehaviour> behaviours;
}