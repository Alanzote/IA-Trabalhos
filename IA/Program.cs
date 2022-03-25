using System.Text.RegularExpressions;
using GraphUtils;
using System.Drawing;

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