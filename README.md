# SB4ScriptSplitter

SB4ScriptSplitter is a small C# tool that splits a large SannyBuilder main.txt script into separate files and rebuilds it using $INCLUDE directives.

Small desc:

- Detects all script_name blocks.

- Saves each script into scripts/<name>.txt.

- Rewrites main.txt with $INCLUDE references.

Usage

1. Set the path to your main.txt in Program.cs.

2. Run the program.

3. Find the results in the out folder.
