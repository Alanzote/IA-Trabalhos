using System.Text.RegularExpressions;
using GraphUtils;
using System.Drawing;

Graph GN = new Graph();

Node N0 = new Node<Tuple<int, int>>("N0", Tuple.Create(50, 50));
Node N1 = new Node<Tuple<int, int>>("N1", Tuple.Create(100, 100));
Node N2 = new Node<Tuple<int, int>>("N2", Tuple.Create(200, 100));
Node N3 = new Node<Tuple<int, int>>("N3", Tuple.Create(150, 200));
Node N4 = new Node<Tuple<int, int>>("N4", Tuple.Create(150, 130));
Node N5 = new Node<Tuple<int, int>>("N5", Tuple.Create(250, 180));
Node N6 = new Node<Tuple<int, int>>("N6", Tuple.Create(300, 150));
Node N7 = new Node<Tuple<int, int>>("N7", Tuple.Create(100, 200));

GN.AddNode(N0);
GN.AddNode(N1);
GN.AddNode(N2);
GN.AddNode(N3);
GN.AddNode(N4);
GN.AddNode(N5);
GN.AddNode(N6);
GN.AddNode(N7);

GN.ConnectNode(N0, N1, 1);
GN.ConnectNode(N1, N3, 1);
GN.ConnectNode(N3, N2, 10);
GN.ConnectNode(N2, N1, 1);
GN.ConnectNode(N2, N4, 1);
GN.ConnectNode(N3, N5, 1);
GN.ConnectNode(N5, N6, 1);
GN.ConnectNode(N6, N2, 1);
GN.ConnectNode(N5, N4, 1);
GN.ConnectNode(N4, N7, 1);

GN.BreadthSearch(N7, N5, out List<Node> Path);

GraphExtensions.PrintArrayOfNodes(Path, "Path found");

Path.Clear();

GN.DepthSearch(N7, N5, out Path);

GraphExtensions.PrintArrayOfNodes(Path, "Path found");

GN.Display();

return;

// Const miles_dat location.
const string miles_dat = "./miles_dat.txt";

// Create New Graph;
Graph G = new Graph();

// Read the miles_dat.txt file.
string[] MilesDat = File.ReadAllLines(miles_dat);

// List of all Cities and Current City.
List<Node> Cities = new List<Node>();
Node? City = null;

// For Parsing.
int i = 0;

// For Each Line...
foreach (var Line in MilesDat) {
	// Skip Comments.
	if (Line.StartsWith("*"))
		continue;

	// Check for Distances.
	if (Regex.IsMatch(Line, "^\\d+")) {
		// Split the Distances.
		var Distances = Line.Split(" ").Select(x => int.Parse(x));

		// For Each Distance.
		foreach (var D in Distances) {
			// Connect Nodes.
			G.ConnectNode(City!, Cities[i], D);

			// Add I.
			i += 1;
		}
	} else {
		// Reset I.
		i = 1;

		// Splt.
		var Splt = Line.Split("[");

		// Get Position.
		var Pos = Splt.Last().Split("]").First().Split(",").Select(x => int.Parse(x));

		// Create City.
		City = new Node<Tuple<int, int>>(Splt.First(), Tuple.Create((Pos.First() - 2672) / 7, (Pos.Last() - 7180) / 7));

		// Add City.
		Cities.Insert(0, City);

		// Add Node.
		G.AddNode(City);
	}
}

// Grab All Nodes.
var AllNodes = G.GetAllNodes();

// Create a Random.
Random Rand = new Random(1701);

// Grab a Copy of it.
var CopyNodes = new List<Node>(AllNodes).OrderBy(x => Rand.Next()).Take(10);

// Remove all Copy Nodes.
AllNodes.RemoveAll(x => CopyNodes.Contains(x));

// For Each All Node...
foreach (var Node in AllNodes)
	G.RemoveNode(Node);

// Grab all Connections.
var AllConns = G.GetAllConnections();

// Create new Random.
Rand = new Random(1701);

// Grab a Copy of it.
var CopyConns = new List<Connection>(AllConns).OrderBy(x => Rand.Next()).Take(10);

// Remove all Copy Conns.
AllConns.RemoveAll(x => CopyConns.Contains(x));

// For Each Connection...
foreach (var Conn in AllConns)
	G.DisconnectNode(Conn.NodeA, Conn.NodeB);

// Get Yakima and Stringfield.
var Yakima = G.FindWithLabel("Yakima, WA");
var Springfield = G.FindWithLabel("Springfield, IL");

//

// Display the Graph.
G.Display();